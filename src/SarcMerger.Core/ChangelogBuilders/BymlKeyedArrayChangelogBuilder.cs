using BymlLibrary;
using BymlLibrary.Nodes.Containers;
using SarcMerger.Core.Models;

namespace SarcMerger.Core.ChangelogBuilders;

public class BymlKeyedArrayChangelogBuilder<T>(string key) : IBymlArrayChangelogBuilder where T : IEquatable<T>
{
    private readonly string _key = key;

    public bool LogChanges(ref BymlTrackingInfo info, ref Byml root, BymlArray src, BymlArray vanilla)
    {
        BymlArrayChangelog changelog = [];

        for (int i = 0; i < src.Count; i++) {
            Byml element = src[i];
            if (!TryGetIndex(vanilla, element.GetMap()[_key].Get<T>(), _key, out int vanillaIndex)) {
                changelog.Add(i, (BymlChangeType.Add, element));
                continue;
            }

            if (BymlChangelogBuilder.LogChangesInline(ref info, ref element, vanilla[vanillaIndex])) {
                src[i] = BymlChangeType.Remove;
                goto UpdateVanilla;
            }

            changelog.Add(vanillaIndex, (BymlChangeType.Edit, element));

        UpdateVanilla:
            vanilla[vanillaIndex] = BymlChangeType.Remove;
        }

        for (int i = 0; i < vanilla.Count; i++) {
            if (vanilla[i].Type is BymlNodeType.Changelog) {
                continue;
            }

            changelog.Add(i, (BymlChangeType.Remove, new Byml()));
        }

        root = changelog;
        return changelog.Count == 0;
    }

    private static bool TryGetIndex(IList<Byml> list, T element, string key, out int index)
    {
        int len = list.Count;
        for (int i = 0; i < len; i++) {
            if (list[i].Value is not BymlMap map || !map[key].Get<T>().Equals(element)) {
                continue;
            }

            index = i;
            return true;
        }

        index = -1;
        return false;
    }
}