using BymlLibrary;
using BymlLibrary.Nodes.Containers;
using SarcMerger.Core.Helpers;

namespace SarcMerger.Core.ChangelogBuilders;

public class DefaultArrayChangelogBuilder : IArrayChangelogBuilder
{
    public static readonly DefaultArrayChangelogBuilder Instance = new();
    
    public bool LogChanges(ReadOnlySpan<char> type, ref Byml root, BymlArray src, BymlArray vanilla)
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

            changelog.Add(i, (BymlChangeType.Remove, new Byml()));
        }

        for (int i = 0; i < src.Count; i++) {
            Byml element = src[i];
            if (element.Type is BymlNodeType.Changelog) {
                continue;
            }

            if (editedVanillaIndices.Count > 0) {
                int vanillaIndex = editedVanillaIndices[0];
                BymlChangelogBuilder.LogChangesInline(type, ref element, vanilla[vanillaIndex]);
                changelog.Add(vanillaIndex, (BymlChangeType.Edit, element));
                editedVanillaIndices.RemoveAt(0);
                continue;
            }

            changelog.Add(i, (BymlChangeType.Add, element));
        }

        root = changelog;
        return changelog.Count == 0;
    }
}