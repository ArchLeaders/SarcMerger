using System.Runtime.CompilerServices;
using Revrs;
using Revrs.Extensions;

namespace SarcMerger.Core.Helpers;

public static class DataHelper
{
    internal const ushort BYML_MAGIC = 16985;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBymlFile(ReadOnlySpan<byte> data)
    {
        return data[..2].Read<ushort>() is BYML_MAGIC;
    }
}