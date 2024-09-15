using System.Runtime.CompilerServices;
using BymlLibrary;
using Revrs;
using Revrs.Buffers;
using SarcLibrary;
using SarcMerger.Core.Helpers;
using SarcMerger.Core.Models;
using TotkCommon;
using TotkCommon.Components;
using TotkCommon.Extensions;

namespace SarcMerger.Core;

public class SarcChangelogBuilder(TotkChecksums checksums)
{
    private readonly TotkChecksums _checksums = checksums;
    
    /// <summary>
    /// Build changelogs for the input mod and writes them to the provided output folder.
    /// </summary>
    /// <param name="inputModFolder">The absolute path to the input romfs mod folder.</param>
    /// <param name="outputModFolder">The path to the output romfs mod folder.</param>
    public async ValueTask BuildChangelogsAsync(string inputModFolder, string outputModFolder)
    {
        IEnumerable<string> files = Directory.EnumerateFiles(inputModFolder, "*.*", SearchOption.AllDirectories);
        await Parallel.ForEachAsync(files, (file, _) => {
                BuildChangelog(file, inputModFolder, outputModFolder);
                return ValueTask.CompletedTask;
            }
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void BuildChangelog(string inputFile, string romfs, string outputFolder)
    {
        ReadOnlySpan<char> input = inputFile.AsSpan();
        ReadOnlySpan<char> canonical = input.ToCanonical(romfs, out RomfsFileAttributes attributes);
        ReadOnlySpan<char> ext = Path.GetExtension(canonical);

        switch (ext) {
            case ".bfarc" or ".bkres" or ".blarc" or ".genvb" or ".pack" or ".ta":
                ProcessSarc(inputFile, canonical, outputFolder, attributes);
                return;
            case ".byml" or ".bgyml" when canonical.Length > 4 && canonical[..4] is not "RSDB":
                ProcessByml(inputFile, canonical, ext, outputFolder, attributes);
                break;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ProcessSarc(string file, ReadOnlySpan<char> canonical, string outputFolder,
        RomfsFileAttributes attributes)
    {
        if (!RomfsHelper.IsVanillaFile(canonical, attributes, out string path)) {
            CopyContent(file, outputFolder, canonical, attributes);
            return;
        }

        using ArraySegmentOwner<byte> vanillaData = RomfsHelper.GetVanilla(path, out _);
        using ArraySegmentOwner<byte> inputData = GetIo(file, canonical, outputFolder, out Stream output);

        WriteToStream(output, inputData.Segment, vanillaData.Segment, canonical);
        output.Dispose();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ProcessByml(string file, ReadOnlySpan<char> canonical, ReadOnlySpan<char> ext,
        string outputFolder, RomfsFileAttributes attributes)
    {
        if (!RomfsHelper.IsVanillaFile(canonical, attributes, out string path)) {
            CopyContent(file, outputFolder, canonical, attributes);
            return;
        }

        using ArraySegmentOwner<byte> vanillaData = RomfsHelper.GetVanilla(path, out _);
        using ArraySegmentOwner<byte> inputData = GetIo(file, canonical, outputFolder, out Stream output);

        BymlChangeInfo info = RomfsHelper.GetBymlType(canonical, ext);

        Byml changelogByml = BymlChangelogBuilder.LogChanges(ref info, inputData.Segment, vanillaData.Segment,
            out Endianness endianness, out ushort version);

        // Writing into memory is faster
        // than writing to disk directly
        using MemoryStream ms = new();
        changelogByml.WriteBinary(ms, endianness, version);

        ms.Seek(0, SeekOrigin.Begin);
        ms.CopyTo(output);
        output.Dispose();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ArraySegmentOwner<byte> GetIo(string inputFile, ReadOnlySpan<char> canonical, string outputFolder,
        out Stream output)
    {
        string canonicalManaged = canonical.ToString();
        string outputFile = Path.Combine(
            outputFolder,
            Totk.AddressTable?.TryGetValue(canonicalManaged, out string? versionedFilePath) is true
                ? versionedFilePath
                : canonicalManaged
        );

        if (Path.GetDirectoryName(outputFile) is string folder) {
            Directory.CreateDirectory(folder);
        }

        output = File.Create(outputFile);

        using Stream fs = File.OpenRead(inputFile);
        int size = Convert.ToInt32(fs.Length);
        ArraySegmentOwner<byte> input = ArraySegmentOwner<byte>.Allocate(size);
        _ = fs.Read(input.Segment);

        if (!Zstd.IsCompressed(input.Segment)) {
            return input;
        }

        int decompressedSize = Zstd.GetDecompressedSize(input.Segment);
        ArraySegmentOwner<byte> decompressedInput = ArraySegmentOwner<byte>.Allocate(decompressedSize);
        Totk.Zstd.Decompress(input.Segment, decompressedInput.Segment);
        input.Dispose();
        return decompressedInput;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void CopyContent(string inputFile, string outputFolder, ReadOnlySpan<char> canonical,
        RomfsFileAttributes attributes)
    {
        string outputFile = RomfsHelper.GetVersionedOutput(outputFolder, canonical, attributes);
        if (Path.GetDirectoryName(outputFile) is string folder) {
            Directory.CreateDirectory(folder);
        }

        File.Copy(inputFile, outputFile, true);
    }

    private void WriteToStream(Stream output, ArraySegment<byte> input, ArraySegment<byte> vanillaBuffer, ReadOnlySpan<char> canonical)
    {
        Sarc vanilla = Sarc.FromBinary(vanillaBuffer);

        Sarc changelog = [];
        Sarc src = Sarc.FromBinary(input);

        foreach ((string name, ArraySegment<byte> data) in src) {
            if (!vanilla.TryGetValue(name, out ArraySegment<byte> vanillaData)) {
                // Custom file, take default path
                goto MoveContent;
            }

            // TODO: Check non-pack files (include sarc canonical path)
            if (_checksums.IsFileVanilla(name, data, Totk.Config.Version)) {
                // Vanilla file, ignore
                continue;
            }

            if (DataHelper.IsBymlFile(data)) {
                BymlChangeInfo info = RomfsHelper.GetBymlType(name);
                Byml changelogByml = BymlChangelogBuilder.LogChanges(
                    ref info, data, vanillaData,
                    out Endianness endianness, out ushort version
                );

                using MemoryStream ms = new();
                changelogByml.WriteBinary(ms, endianness, version);
                changelog[name] = ms.ToArray();
                continue;
            }

        MoveContent:
            changelog[name] = data;
        }

        changelog.Write(output, changelog.Endianness);
    }
}