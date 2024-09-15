using SarcMerger.Core;
using TotkCommon;
using TotkCommon.Components;

namespace SarcMergerTools.Scripts;

public static class TestArrayKeysScript
{
    public static async Task Execute()
    {
        SarcMergerModule module = new(TotkChecksums.FromFile(@"C:\Users\ArchLeaders\AppData\Local\totk\checksums.bin"));
        await module.BuildChangelogsAsync(Totk.Config.GamePath, "output");
    }
}