using BymlLibrary;
using BymlLibrary.Nodes.Containers;
using Revrs;
using SarcMerger.Core.Helpers;

namespace SarcMerger.Core;

public static class BymlChangelogBuilder
{
    public static Byml LogChanges(Span<byte> data, Span<byte> vanillaData, out Endianness endianness, out ushort version)
    {
        Byml vanillaByml = Byml.FromBinary(vanillaData);
        Byml srcByml = Byml.FromBinary(data, out endianness, out version);
        LogChangesInline(ref srcByml, vanillaByml);
        return srcByml;
    }

    public static bool LogChangesInline(ref Byml src, Byml vanilla)
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

    public static bool LogArrayChanges(ref Byml root, BymlArray src, BymlArray vanilla)
    {
        BymlArrayChangelog changelog = [];
        List<int> editedVanillaIndices = [];

        for (int i = 0; i < src.Count; i++) {
            Byml element = src[i];
            if (!vanilla.TryGetIndex(element, Byml.ValueEqualityComparer.Default, out int vanillaIndex)) {
                continue;
            }

            src[i] = BymlChangeType.Remove;
            vanilla[vanillaIndex] = BymlChangeType.Remove;
        }

        for (int i = 0; i < vanilla.Count; i++) {
            if (vanilla[i].Type is BymlNodeType.Changelog) {
                continue;
            }

            if (i < src.Count) {
                editedVanillaIndices.Add(i);
                continue;
            }

            changelog.Add(i, (BymlChangeType.Remove, new()));
        }

        for (int i = 0; i < src.Count; i++) {
            Byml element = src[i];
            if (element.Type is BymlNodeType.Changelog) {
                continue;
            }

            if (editedVanillaIndices.Count > 0) {
                changelog.Add(editedVanillaIndices[0], (BymlChangeType.Edit, element));
                editedVanillaIndices.RemoveAt(0);
                continue;
            }

            changelog.Add(i, (BymlChangeType.Add, element));
        }

        root = changelog;
        return changelog.Count == 0;
    }

    private static bool LogMapChanges<T>(IDictionary<T, Byml> src, IDictionary<T, Byml> vanilla)
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