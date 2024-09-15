using SarcMerger.Core;
using TotkCommon.Components;

SarcMergerModule module = new(TotkChecksums.FromFile(@"C:\Users\ArchLeaders\AppData\Local\totk\checksums.bin"));
await module.BuildChangelogsAsync(args[0], args[1]);