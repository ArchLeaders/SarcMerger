using BymlLibrary;
using BymlLibrary.Nodes.Containers;

namespace SarcMerger.Core.ChangelogBuilders;

public class StringKeyArrayChangelogBuilder(string key) : IArrayChangelogBuilder
{
    private readonly string _key = key;

    public bool LogArrayChanges(ReadOnlySpan<char> type, ref Byml root, BymlArray src, BymlArray vanilla)
    {
        throw new NotImplementedException();
    }
}