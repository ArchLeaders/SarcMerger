using BymlLibrary;
using BymlLibrary.Extensions;
using BymlLibrary.Nodes.Containers;
using BymlLibrary.Nodes.Containers.HashMap;
using CommunityToolkit.HighPerformance.Buffers;
using Revrs.Buffers;
using SarcLibrary;
using TotkCommon;
using TotkCommon.Extensions;

HashSet<string> types = []; 

string romfs = Totk.Config.GamePath;
romfs = romfs[^1] switch {
    '/' or '\\' => romfs,
    _ => romfs + "\\"
};

const string bymlOutputFolder = "output\\byml"; 
Directory.CreateDirectory(bymlOutputFolder);

const string bgymlOutputFolder = "output\\bgyml";
Directory.CreateDirectory(bgymlOutputFolder);

foreach (string file in Directory.EnumerateFiles(romfs, "*.*", SearchOption.AllDirectories)) {
    ReadOnlySpan<char> canonical = file.ToCanonical(Totk.Config.GamePath);
    ReadOnlySpan<char> ext = Path.GetExtension(canonical);

    string typeStr = "";
    switch (ext) {
        case ".bkres" or ".blarc" or ".genvb" or ".pack" or ".ta":
            CollectSarc(file);
            break;
        case ".bgyml" when canonical.Length > 7 && canonical[..4] is not "RSDB" && Path.GetExtension(canonical[..^6])[1..].ToString() is var type && !types.Contains(type):
            CollectByml(file, type, bgymlOutputFolder);
            break;
        case ".byml" when canonical.Length > 7 && canonical[..4] is not "RSDB" && Path.GetExtension(canonical[..^5]) is var type && (type is "" or "Product" || (type[1..].ToString() is string str && !types.Contains(typeStr = str))):
            CollectByml(file, typeStr, bymlOutputFolder);
            break;
    }
}

return;

void CollectSarc(string file)
{
    using FileStream fs = File.OpenRead(file);
    int size = (int)fs.Length;
    using ArraySegmentOwner<byte> sarcData = ArraySegmentOwner<byte>.Allocate(size);
    _ = fs.Read(sarcData.Segment);

    if (Zstd.IsCompressed(sarcData.Segment)) {
        try {
            int decompressedSize = Zstd.GetDecompressedSize(sarcData.Segment);
            using ArraySegmentOwner<byte> decompressed = ArraySegmentOwner<byte>.Allocate(decompressedSize);
            Totk.Zstd.Decompress(sarcData.Segment, decompressed.Segment);
            sarcData.Dispose();
            CollectSarcFromData(file, decompressed.Segment);
        }
        catch (Exception ex) {
            Console.Write(file);
            Console.Write(": ");
            Console.WriteLine(ex.Message);
        }
        
        return;
    }

    CollectSarcFromData(file, sarcData.Segment);
}

void CollectSarcFromData(string file, ArraySegment<byte> sarcData)
{
    Sarc sarc = Sarc.FromBinary(sarcData);
    foreach ((string name, ArraySegment<byte> data) in sarc) {
        ReadOnlySpan<char> ext = Path.GetExtension(name.AsSpan());

        string? rootOutputFolder = ext switch {
            ".byml" => bymlOutputFolder,
            ".bgyml" => bgymlOutputFolder,
            _ => null
        };

        if (rootOutputFolder is null) {
            continue;
        }

        string output = Path.Combine(rootOutputFolder, file[romfs.Length..], name + ".yml");
        
        ReadOnlySpan<char> type = Path.GetExtension(name.AsSpan()[..^ext.Length]);
        string typeStr = "";
        if (type is not "" && (type[1..].ToString() is not string str || types.Contains(typeStr = str))) {
            continue;
        }

        CollectBymlFromData(output, typeStr, data);
    }
}

void CollectByml(string file, string type, string rootOutputFolder)
{
    string output = Path.Combine(rootOutputFolder, file[romfs.Length..] + ".yml");
    
    using FileStream fs = File.OpenRead(file);
    using SpanOwner<byte> buffer = SpanOwner<byte>.Allocate((int)fs.Length);
    _ = fs.Read(buffer.Span);
    
    if (Zstd.IsCompressed(buffer.Span)) {
        try {
            int decompressedSize = Zstd.GetDecompressedSize(buffer.Span);
            using SpanOwner<byte> decompressed = SpanOwner<byte>.Allocate(decompressedSize);
            Totk.Zstd.Decompress(buffer.Span, decompressed.Span);
            buffer.Dispose();
            CollectBymlFromData(output, type, decompressed.Span);
        }
        catch (Exception ex) {
            Console.Write(file);
            Console.Write(": ");
            Console.WriteLine(ex.Message);
        }
        
        return;
    }
    
    CollectBymlFromData(output, type, buffer.Span);
}

void CollectBymlFromData(string output, string type, Span<byte> data)
{
    Byml byml = Byml.FromBinary(data);

    if (!BymlHasArray(byml)) {
        return;
    }

    if (type is not "") {
        types.Add(type);
    }
    
    if (Path.GetDirectoryName(output) is { } folder) {
        Directory.CreateDirectory(folder);
    }
    
    File.WriteAllText(output, byml.ToYaml());
}

static bool BymlHasArray(Byml src)
{
    return src.Value switch {
        BymlMap map => map.Any(CheckBymlKvp),
        BymlHashMap32 map => map.Values.Any(BymlHasArray),
        BymlHashMap64 map => map.Values.Any(BymlHasArray),
        BymlArray array => array.Count > 0 && array.Any(x => x.Type.IsContainerType()),
        _ => false
    };
}

static bool CheckBymlKvp(KeyValuePair<string, Byml> kvp)
{
    // TravelerInfo needs a custom merger
    // GameDataList needs a custom merger
    
    return BymlHasArray(kvp.Value);
}