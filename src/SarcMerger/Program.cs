using SarcMerger.Core;
using TotkCommon.Components;

SarcChangelogBuilder builder = new(TotkChecksums.FromFile(@"C:\Users\ArchLeaders\AppData\Local\totk\checksums.bin"));
await builder.BuildChangelogsAsync(args[0], args[1]);