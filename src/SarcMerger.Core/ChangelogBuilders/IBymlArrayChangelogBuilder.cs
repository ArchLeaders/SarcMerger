using BymlLibrary;
using BymlLibrary.Nodes.Containers;
using SarcMerger.Core.Models;

namespace SarcMerger.Core.ChangelogBuilders;

public interface IBymlArrayChangelogBuilder
{
    bool LogChanges(ref BymlTrackingInfo info, ref Byml root, BymlArray src, BymlArray vanilla);
}