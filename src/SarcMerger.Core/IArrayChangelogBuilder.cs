using BymlLibrary;
using BymlLibrary.Nodes.Containers;

namespace SarcMerger.Core;

public interface IBymlArrayChangelogBuilder
{
    bool LogArrayChanges(ref Byml root, BymlArray src, BymlArray vanilla);
}