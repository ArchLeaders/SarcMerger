using SarcMergerTools.Scripts;

#region Notes

// TravelerInfo needs a custom merger
// GameDataList needs a custom merger

// AnimationResources: !string ModelProjectName # Custom merger, sort entries by mod priority
// SubModels: !string ModelProjectName # Custom merger, sort entries by mod priority

// DropTable: !string DropActorName # Needs 100% merger
// DropTableElement: !string DropActorName # Needs 100% merger
// LotteryTableElement: !string DropActorName # Needs 100% merger

#endregion

await TestArrayKeysScript.Execute();