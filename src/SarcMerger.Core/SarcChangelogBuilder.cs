using BymlLibrary;
using Revrs;
using SarcLibrary;
using SarcMerger.Core.Helpers;
using TotkCommon;
using TotkCommon.Components;

namespace SarcMerger.Core;

public class SarcChangelogBuilder(TotkChecksums checksums)
{
    private readonly TotkChecksums _checksums = checksums;

    /// <summary>
    /// Writes a <see cref="SarcChangelogBuilder"/> created from the input <paramref name="input"/> into the provided <paramref name="output"></paramref> stream.
    /// </summary>
    /// <param name="output">The output to write the changelog to.</param>
    /// <param name="input">The changed archive data.</param>
    /// <param name="vanillaBuffer"></param>
    /// <param name="canonical">The canonical path of the archive.</param>
    /// <returns><see langword="true"/> if a vanilla sarc was found and a changelog was generated, otherwise <see langword="false"/></returns>
    public void WriteToStream(Stream output, ArraySegment<byte> input, ArraySegment<byte> vanillaBuffer, ReadOnlySpan<char> canonical)
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
                Byml changelogByml = BymlChangelogBuilder.LogChanges(
                    RomfsHelper.GetBymlType(name), data, vanillaData,
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