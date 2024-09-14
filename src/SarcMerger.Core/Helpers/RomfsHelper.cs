using Revrs.Buffers;
using TotkCommon;
using TotkCommon.Extensions;

namespace SarcMerger.Core.Helpers;

public static class LocationHelper
{
    public static string GetVersionedOutput(string outputRomfs, ReadOnlySpan<char> canonical, RomfsFileAttributes attributes)
    {
        string canonicalPath = canonical.ToString();
        return Path.Combine(
            outputRomfs,
            Totk.AddressTable?.TryGetValue(canonicalPath, out string? versionedPath) is true
                ? versionedPath
                : canonicalPath + (attributes.HasFlag(RomfsFileAttributes.HasZsExtension) ? ".zs" : string.Empty)
        );
    }

    public static bool IsVanillaFile(ReadOnlySpan<char> canonical, RomfsFileAttributes attributes, out string path)
    {
        path = GetVersionedOutput(Totk.Config.GamePath, canonical, attributes);
        return File.Exists(path);
    }

    public static ArraySegmentOwner<byte> GetVanilla(ReadOnlySpan<char> canonical, RomfsFileAttributes attributes, out bool isVanillaFile)
    {
        return GetVanilla(
            GetVersionedOutput(Totk.Config.GamePath, canonical, attributes),
            out isVanillaFile
        );
    }

    public static ArraySegmentOwner<byte> GetVanilla(string path, out bool isVanillaFile)
    {
        if (!File.Exists(path)) {
            isVanillaFile = false;
            return default;
        }

        using Stream fs = File.OpenRead(path);
        int size = Convert.ToInt32(fs.Length);
        ArraySegmentOwner<byte> buffer = ArraySegmentOwner<byte>.Allocate(size);
        _ = fs.Read(buffer.Segment);

        isVanillaFile = true;

        if (!Zstd.IsCompressed(buffer.Segment)) {
            return buffer;
        }

        int decompressedSize = Zstd.GetDecompressedSize(buffer.Segment);
        ArraySegmentOwner<byte> decompressedBuffer = ArraySegmentOwner<byte>.Allocate(decompressedSize);
        Totk.Zstd.Decompress(buffer.Segment, decompressedBuffer.Segment);
        buffer.Dispose();
        return decompressedBuffer;
    }
}