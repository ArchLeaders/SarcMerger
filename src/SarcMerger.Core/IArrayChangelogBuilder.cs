using BymlLibrary;
using BymlLibrary.Nodes.Containers;
using SarcMerger.Core.Models;

namespace SarcMerger.Core;

public interface IArrayChangelogBuilder
{
    bool LogChanges(ref BymlChangeInfo info, ref Byml root, BymlArray src, BymlArray vanilla);
}