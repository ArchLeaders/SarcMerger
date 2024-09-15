namespace SarcMerger.Core.Models;

public ref struct BymlTrackingInfo(ReadOnlySpan<char> type)
{
    public ReadOnlySpan<char> Type = type;
    public int Level;

    public static implicit operator BymlTrackingInfo(ReadOnlySpan<char> type) => new(type);
}