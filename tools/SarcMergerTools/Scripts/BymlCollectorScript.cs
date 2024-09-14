using System.Diagnostics.CodeAnalysis;
using BymlLibrary;
using BymlLibrary.Extensions;
using BymlLibrary.Nodes.Containers;
using BymlLibrary.Nodes.Containers.HashMap;
using CommunityToolkit.HighPerformance.Buffers;
using Revrs.Buffers;
using SarcLibrary;
using SarcMergerTools.Extensions;
using TotkCommon;
using TotkCommon.Extensions;

namespace SarcMergerTools.Scripts;

[SuppressMessage("ReSharper", "UnusedType.Global")]
public static class BymlCollectorScript
{
    private const string BymlOutputFolder = "output\\byml";
    private const string BgymlOutputFolder = "output\\bgyml";
    
    private static readonly Dictionary<string, HashSet<string>> TypeArrayKeys = [];
    private static readonly string RomFs = Totk.Config.GamePath;

    static BymlCollectorScript()
    {
        Directory.CreateDirectory(BymlOutputFolder);
        Directory.CreateDirectory(BgymlOutputFolder);

        RomFs = RomFs[^1] switch {
            '/' or '\\' => RomFs,
            _ => RomFs + "\\"
        };
    }

    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public static void Execute()
    {
        foreach (string file in Directory.EnumerateFiles(RomFs, "*.*", SearchOption.AllDirectories)) {
            ReadOnlySpan<char> canonical = file.ToCanonical(Totk.Config.GamePath);
            ReadOnlySpan<char> ext = Path.GetExtension(canonical);

            string typeStr = "";
            switch (ext) {
                case ".bkres" or ".blarc" or ".genvb" or ".pack" or ".ta":
                    CollectSarc(file);
                    break;
                case ".bgyml" when canonical.Length > 7 && canonical[..4] is not "RSDB" && Path.GetExtension(canonical[..^6])[1..].ToString() is var type:
                    CollectByml(file, type, BgymlOutputFolder);
                    break;
                case ".byml" when canonical.Length > 7 && canonical[..4] is not "RSDB" && Path.GetExtension(canonical[..^5]) is var type && (type is "" or "Product" || (typeStr = type[1..].ToString()) is not null):
                    CollectByml(file, typeStr, BymlOutputFolder);
                    break;
            }
        }
    }

    private static void CollectSarc(string file)
    {
        using FileStream fs = File.OpenRead(file);
        int size = (int)fs.Length;
        using ArraySegmentOwner<byte> sarcData = ArraySegmentOwner<byte>.Allocate(size);
        _ = fs.Read(sarcData.Segment);

        if (Zstd.IsCompressed(sarcData.Segment)) {
            int decompressedSize = Zstd.GetDecompressedSize(sarcData.Segment);
            using ArraySegmentOwner<byte> decompressed = ArraySegmentOwner<byte>.Allocate(decompressedSize);
            Totk.Zstd.Decompress(sarcData.Segment, decompressed.Segment);
            CollectSarcFromData(file, decompressed.Segment);
            return;
        }

        CollectSarcFromData(file, sarcData.Segment);
    }

    private static void CollectSarcFromData(string file, ArraySegment<byte> sarcData)
    {
        Sarc sarc = Sarc.FromBinary(sarcData);
        foreach ((string name, ArraySegment<byte> data) in sarc) {
            ReadOnlySpan<char> ext = Path.GetExtension(name.AsSpan());

            string? rootOutputFolder = ext switch {
                ".byml" => BymlOutputFolder,
                ".bgyml" => BgymlOutputFolder,
                _ => null
            };

            if (rootOutputFolder is null) {
                continue;
            }

            string output = Path.Combine(rootOutputFolder, file[RomFs.Length..], name + ".yml");

            ReadOnlySpan<char> type = Path.GetExtension(name.AsSpan()[..^ext.Length]);
            string typeStr = "";
            if (type is not "" && (typeStr = type[1..].ToString()) is not string) {
                continue;
            }

            CollectBymlFromData(output, typeStr, data);
        }
    }

    private static void CollectByml(string file, string type, string rootOutputFolder)
    {
        string output = Path.Combine(rootOutputFolder, file[RomFs.Length..] + ".yml");

        using FileStream fs = File.OpenRead(file);
        using SpanOwner<byte> buffer = SpanOwner<byte>.Allocate((int)fs.Length);
        _ = fs.Read(buffer.Span);

        if (Zstd.IsCompressed(buffer.Span)) {
            int decompressedSize = Zstd.GetDecompressedSize(buffer.Span);
            using SpanOwner<byte> decompressed = SpanOwner<byte>.Allocate(decompressedSize);
            Totk.Zstd.Decompress(buffer.Span, decompressed.Span);
            CollectBymlFromData(output, type, decompressed.Span);
            return;
        }

        CollectBymlFromData(output, type, buffer.Span);
    }

    private static void CollectBymlFromData(string output, string type, Span<byte> data)
    {
        Byml byml = Byml.FromBinary(data);

        if (!TypeArrayKeys.TryGetValue(type, out HashSet<string>? arrayKeys)) {
            TypeArrayKeys[type] = arrayKeys = [];
        }

        if (!BymlHasArray(byml, arrayKeys)) {
            return;
        }

        if (Path.GetDirectoryName(output) is { } folder) {
            Directory.CreateDirectory(folder);
        }

        File.WriteAllText(output, byml.ToYaml());
    }

    private static bool BymlHasArray(Byml src, HashSet<string> arrayKeys)
    {
        bool result = src.Value switch {
            BymlMap map => map.EvalAny(x => CheckBymlKvp(x, arrayKeys)),
            BymlHashMap32 map => map.Values.Any(x => BymlHasArray(x, arrayKeys)),
            BymlHashMap64 map => map.Values.Any(x => BymlHasArray(x, arrayKeys)),
            BymlArray array => array.Count > 0 && array.Any(x => x.Type.IsContainerType()),
            _ => false
        };

        return result;
    }

    private static bool CheckBymlKvp(KeyValuePair<string, Byml> kvp, HashSet<string> arrayKeys)
    {
        return BymlHasArray(kvp.Value, arrayKeys) && arrayKeys.Add(kvp.Key);
    }
}