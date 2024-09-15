using BymlLibrary;
using BymlLibrary.Nodes.Containers;
using Revrs;
using SarcMerger.Core.ChangelogBuilders;
using SarcMerger.Core.Models;

namespace SarcMerger.Core;

public static class BymlChangelogBuilder
{   
    public static Byml LogChanges(ref BymlChangeInfo info, Span<byte> data, Span<byte> vanillaData, out Endianness endianness, out ushort version)
    {
        Byml vanillaByml = Byml.FromBinary(vanillaData);
        Byml srcByml = Byml.FromBinary(data, out endianness, out version);
        LogChangesInline(ref info, ref srcByml, vanillaByml);
        return srcByml;
    }

    internal static bool LogChangesInline(ref BymlChangeInfo info, ref Byml src, Byml vanilla)
    {
        if (src.Type != vanilla.Type) {
            return false;
        }

        return src.Type switch {
            BymlNodeType.HashMap32 => LogMapChanges(ref info, src.GetHashMap32(), vanilla.GetHashMap32()),
            BymlNodeType.HashMap64 => LogMapChanges(ref info, src.GetHashMap64(), vanilla.GetHashMap64()),
            BymlNodeType.Array => info switch {
                { Type: "ecocat", Level: 0 } => new KeyedArrayChangelogBuilder<string>("AreaNumber")
                    .LogChanges(ref info, ref src, src.GetArray(), vanilla.GetArray()),
                _ => DefaultArrayChangelogBuilder.Instance.LogChanges(ref info, ref src, src.GetArray(), vanilla.GetArray())
            },
            BymlNodeType.Map => LogMapChanges(ref info, src.GetMap(), vanilla.GetMap()),
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

    private static bool LogMapChanges<T>(ref BymlChangeInfo info, IDictionary<T, Byml> src, IDictionary<T, Byml> vanilla)
    {
        info.Level++;
        foreach (T key in src.Keys.Concat(vanilla.Keys).Distinct().ToArray()) { // TODO: Avoid copying keys
            if (!src.TryGetValue(key, out Byml? srcValue)) {
                src[key] = BymlChangeType.Remove;
                continue;
            }

            if (!vanilla.TryGetValue(key, out Byml? vanillaNode)) {
                continue;
            }

            if (key is string keyStr && srcValue.Value is BymlArray array && vanillaNode.Value is BymlArray vanillaArray) {
                BymlArrayChangelogBuilderProvider
                    .GetChangelogBuilder(ref info, keyStr)
                    .LogChanges(ref info, ref srcValue, array, vanillaArray);
                goto Default;
            }
            
            if (LogChangesInline(ref info, ref srcValue, vanillaNode)) {
                src.Remove(key);
                continue;
            }

        Default:
            // CreateChangelog can mutate
            // srcValue, so reassign the key
            src[key] = srcValue;
        }

        info.Level--;
        return src.Count == 0;
    }
}