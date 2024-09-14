using BymlLibrary;
using BymlLibrary.Nodes.Containers;
using Revrs;
using SarcMerger.Core.Helpers;

namespace SarcMerger.Core;

public static class BymlChangelogBuilder
{
    public static Byml LogChanges(ReadOnlySpan<char> type, Span<byte> data, Span<byte> vanillaData, out Endianness endianness, out ushort version)
    {
        Byml vanillaByml = Byml.FromBinary(vanillaData);
        Byml srcByml = Byml.FromBinary(data, out endianness, out version);
        LogChangesInline(type, ref srcByml, vanillaByml);
        return srcByml;
    }

    internal static bool LogChangesInline(ReadOnlySpan<char> type, ref Byml src, Byml vanilla)
    {
        if (src.Type != vanilla.Type) {
            return false;
        }

        return src.Type switch {
            BymlNodeType.HashMap32 => LogMapChanges(src.GetHashMap32(), vanilla.GetHashMap32()),
            BymlNodeType.HashMap64 => LogMapChanges(src.GetHashMap64(), vanilla.GetHashMap64()),
            BymlNodeType.Array => LogArrayChanges(ref src, src.GetArray(), vanilla.GetArray()),
            BymlNodeType.Map => LogMapChanges(src.GetMap(), vanilla.GetMap()),
            BymlNodeType.String or
            BymlNodeType.Binary or
            BymlNodeType.BinaryAligned or
            BymlNodeType.Bool or
            BymlNodeType.Int or
            BymlNodeType.Float or
            BymlNodeType.UInt32 or
            BymlNodeType.Int64 or
            BymlNodeType.UInt64 or
            BymlNodeType.Double or
            BymlNodeType.Null => Byml.ValueEqualityComparer.Default.Equals(src, vanilla),
            _ => throw new NotSupportedException(
                $"Merging '{src.Type}' is not supported")
        };
    }

    private static bool LogMapChanges<T>(ReadOnlySpan<char> type, IDictionary<T, Byml> src, IDictionary<T, Byml> vanilla)
    {
        foreach (T key in src.Keys.Concat(vanilla.Keys).Distinct().ToArray()) { // TODO: Avoid copying keys
            if (!src.TryGetValue(key, out Byml? srcValue)) {
                src[key] = BymlChangeType.Remove;
                continue;
            }

            if (vanilla.TryGetValue(key, out Byml? vanillaNode) && LogChangesInline(ref srcValue, vanillaNode)) {
                src.Remove(key);
                continue;
            }

            // CreateChangelog can mutate
            // srcValue, so reassign the key
            src[key] = srcValue;
        }

        return src.Count == 0;
    }
}