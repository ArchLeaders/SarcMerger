using SarcMerger.Core;
using TotkCommon;
using TotkCommon.Components;

namespace SarcMergerTools.Scripts;

public static class TestArrayKeysScript
{
    public static async Task Execute()
    {
        SarcChangelogBuilder builder = new(TotkChecksums.FromFile(@"C:\Users\ArchLeaders\AppData\Local\totk\checksums.bin"));
        await builder.BuildChangelogsAsync(Totk.Config.GamePath, "output");
    }
}