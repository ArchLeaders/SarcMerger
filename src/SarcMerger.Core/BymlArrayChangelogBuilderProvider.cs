namespace SarcMerger.Core.Components;

public partial class BymlArrayChangelogBuilderProvider
{
    public partial IBymlArrayChangelogBuilder GetChangelogBuilder(ReadOnlySpan<char> type, ReadOnlySpan<char> key);
}