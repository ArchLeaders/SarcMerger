using System.Runtime.CompilerServices;
using BymlLibrary;
using Revrs;
using Revrs.Buffers;
using SarcMerger.Core.Helpers;
using TotkCommon;
using TotkCommon.Components;
using TotkCommon.Extensions;

namespace SarcMerger.Core;

public class SarcMergerModule(TotkChecksums checksums)
{
    private readonly SarcChangelogBuilder _sarcChangelogBuilder = new(checksums);

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
            case ".byml" or ".bgyml" when canonical.Length > 4 && canonical[0..4] is not "RSDB":
                ProcessByml(inputFile, canonical, outputFolder, attributes);
                break;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ProcessSarc(string file, ReadOnlySpan<char> canonical, string outputFolder, RomfsFileAttributes attributes)
    {
        if (!LocationHelper.IsVanillaFile(canonical, attributes, out string path)) {
            string outputFile = LocationHelper.GetVersionedOutput(outputFolder, canonical, attributes);
            File.Copy(file, outputFile, true);
            return;
        }

        using ArraySegmentOwner<byte> vanillaData = LocationHelper.GetVanilla(path, out _);
        using ArraySegmentOwner<byte> inputData = GetIo(file, canonical, outputFolder, attributes, out Stream output);

        _sarcChangelogBuilder.WriteToStream(output, inputData.Segment, vanillaData.Segment, canonical);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ProcessByml(string file, ReadOnlySpan<char> canonical, string outputFolder, RomfsFileAttributes attributes)
    {
        if (!LocationHelper.IsVanillaFile(canonical, attributes, out string path)) {
            string outputFile = LocationHelper.GetVersionedOutput(outputFolder, canonical, attributes);
            File.Copy(file, outputFile, true);
            return;
        }

        using ArraySegmentOwner<byte> vanillaData = LocationHelper.GetVanilla(path, out _);
        using ArraySegmentOwner<byte> inputData = GetIo(file, canonical, outputFolder, attributes, out Stream output);

        Byml changelogByml = BymlChangelogBuilder.LogChanges(inputData.Segment, vanillaData.Segment, out Endianness endianness, out ushort version);

        // Writing into memory is faster
        // than writing to disk directly
        using MemoryStream ms = new();
        changelogByml.WriteBinary(ms, endianness, version);

        ms.Seek(0, SeekOrigin.Begin);
        ms.CopyTo(output);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ArraySegmentOwner<byte> GetIo(string inputFile, ReadOnlySpan<char> canonical, string outputFolder, RomfsFileAttributes attributes, out Stream output)
    {
        string canonicalManaged = canonical.ToString();
        output = File.Create(
            Path.Combine(
                outputFolder,
                Totk.AddressTable?.TryGetValue(canonicalManaged, out string? versionedFilePath) is true ? versionedFilePath : canonicalManaged
            )
        );

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
}