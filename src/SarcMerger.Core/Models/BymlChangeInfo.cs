namespace SarcMerger.Core.Models;

public ref struct BymlChangeInfo(ReadOnlySpan<char> type)
{
    public ReadOnlySpan<char> Type = type;
    public int Level;

    public static implicit operator BymlChangeInfo(ReadOnlySpan<char> type) => new(type);
}