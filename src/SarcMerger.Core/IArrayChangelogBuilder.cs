using BymlLibrary;
using BymlLibrary.Nodes.Containers;

namespace SarcMerger.Core;

public interface IArrayChangelogBuilder
{
    bool LogChanges(ReadOnlySpan<char> type, ref Byml root, BymlArray src, BymlArray vanilla);
}