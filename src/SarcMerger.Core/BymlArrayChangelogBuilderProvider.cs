using SarcMerger.Core.ChangelogBuilders;

namespace SarcMerger.Core;

// ReSharper disable StringLiteralTypo

// NOTE: The typos in this file
// are from Nintendo, not TKMM.

public static class BymlArrayChangelogBuilderProvider
{
    public static IArrayChangelogBuilder GetChangelogBuilder(ReadOnlySpan<char> type, ReadOnlySpan<char> key)
    {
        return key switch {
            "Actors" => type switch {
                "bcett" => new KeyedArrayChangelogBuilder<ulong>("Hash"),
                "game__component__ArmyManagerParam" => new KeyedArrayChangelogBuilder<string>("ActorName"),
                _ => DefaultArrayChangelogBuilder.Instance
            },
            "Table" => type switch {
                "game__colorvariation__ConversionActorNameToParasailPatternSetTable"
                    => new KeyedArrayChangelogBuilder<ulong>("ParasailPattern"),
                "game__ecosystem__DecayedWeaponMappingTable"
                    => new KeyedArrayChangelogBuilder<uint>("EquipmentDeathCountGmdHash"),
                _ => DefaultArrayChangelogBuilder.Instance
            },
            "BoneInfoArray" => type switch {
                "game__component__DragonParam" => new KeyedArrayChangelogBuilder<uint>("Hash"),
                "phive__LookIKResourceHeaderParam"
                    or "phive__TailBoneResourceHeaderParam" => new KeyedArrayChangelogBuilder<string>("BoneName"),
                _ => DefaultArrayChangelogBuilder.Instance
            },
            "Elements" => type switch {
                "game__enemy__DrakeSubModelInfo" => new KeyedArrayChangelogBuilder<string>("BoneName"),
                "game__gamebalance__LevelSensorTargetDefine" => new KeyedArrayChangelogBuilder<uint>("ActorNameHash"),
                _ => DefaultArrayChangelogBuilder.Instance
            },
            "Items" => type switch {
                "game__pouchcontent__EnhancementMaterial" => new KeyedArrayChangelogBuilder<string>("Actor"),
                _ => DefaultArrayChangelogBuilder.Instance
            },
            "List" => type switch {
                "game__sound__ShrineSpotBgmTypeInfoList" => new KeyedArrayChangelogBuilder<string>("DungeonIndexStr"),
                _ => DefaultArrayChangelogBuilder.Instance
            },
            "Contents" => type switch {
                "game__ui__FairyFountainGlobalSetting" => new KeyedArrayChangelogBuilder<string>("Actor"),
                _ => DefaultArrayChangelogBuilder.Instance
            },
            "SettingTable" => type switch {
                "game__ui__LargeDungeonFloorDefaultSettingTable" => new KeyedArrayChangelogBuilder<string>("DungeonType"),
                _ => DefaultArrayChangelogBuilder.Instance
            },
            "BrainVerbs" => new KeyedArrayChangelogBuilder<string>("ActionSeqContainer"),
            "ActionVerbContainerElements" => new KeyedArrayChangelogBuilder<string>("ActionVerb"),
            "ResidentActors" or "Settings"
                or "ShootableShareActorSettings" => new KeyedArrayChangelogBuilder<string>("Actor"),
            "BindActorInfo" => new KeyedArrayChangelogBuilder<string>("ActorHolderKey"),
            "GoodsList" => new KeyedArrayChangelogBuilder<string>("ActorName"),
            "RegisteredActorArray" or "RequirementList"
                or "Rewards" => new KeyedArrayChangelogBuilder<string>("ActorName"),
            "SharpInfoBowList" => new KeyedArrayChangelogBuilder<uint>("ActorNameHash"),
            "PictureBookParamArray" => new KeyedArrayChangelogBuilder<string>("ActorNameShort"),
            "WeakPointActorArray" => new KeyedArrayChangelogBuilder<string>("ActorPath"),
            "NavMeshObjects" => new KeyedArrayChangelogBuilder<string>("Alias"),
            "AliasEntityList" => new KeyedArrayChangelogBuilder<string>("AliasEntity"),
            "AliasSensorList" => new KeyedArrayChangelogBuilder<string>("AliasSensor"),
            "Anchors" => new KeyedArrayChangelogBuilder<string>("AnchorName"),
            "ArmorEffect" => new KeyedArrayChangelogBuilder<string>("ArmorEffectType"),
            "HornTypeAndAttachmentMapping" => new KeyedArrayChangelogBuilder<string>("AttachmentName"),
            "AttackParams" => new KeyedArrayChangelogBuilder<string>("AttackType"),
            "BlackboardParamBoolArray" or "BlackboardParamCustomTypeArray" or "BlackboardParamF32Array"
                or "BlackboardParamMtx33fArray" or "BlackboardParamMtx34fArray" or "BlackboardParamPtrArray"
                or "BlackboardParamQuatfArray" or "BlackboardParamS32Array" or "BlackboardParamS8Array"
                or "BlackboardParamStringArray" or "BlackboardParamU32Array" or "BlackboardParamU64Array"
                or "BlackboardParamU8Array" or "BlackboardParamVec3fArray"
                or "EditBBParams" => new KeyedArrayChangelogBuilder<string>("BBKey"),
            "DragonInfoList" => new KeyedArrayChangelogBuilder<uint>("BindPointRespawnGameDataHash"),
            "OperationAngular" or "OperationLinear" => new KeyedArrayChangelogBuilder<string>("Body"),
            "BindBoneList" or "BoneList" or "BoneModifierSet" or "Bones" or "FruitOffsetTranslation"
                or "ModelBindSettings" or "StickWeaponBone" => new KeyedArrayChangelogBuilder<string>("BoneName"),
            "Categories" => new KeyedArrayChangelogBuilder<string>("CallbackName"),
            "CaveParams" => new KeyedArrayChangelogBuilder<ulong>("CaveInstanceId"),
            "CheckList" => new KeyedArrayChangelogBuilder<string>("CheckType"),
            "Object" => new KeyedArrayChangelogBuilder<string>("ChemicalMaterial"),
            "CharacterComponentPresetCollection" or "LayerHitMaskEntityCollection" or "LayerHitMaskSensorCollection"
                or "MaterialCollection" or "MaterialPresetCollection" or "MotionPropertiesCollection"
                or "PhysicsMaterialMappingInfoCollection" or "SubLayerHitMaskEntityCollection"
                or "SubLayerHitMaskSensorCollection"
                or "UserShapeTagMaskCollection" => new KeyedArrayChangelogBuilder<string>("ComponentName"),
            "HingeArray" or "RangeArray" => new KeyedArrayChangelogBuilder<string>("ConstraintName"),
            "CropYieldTable" => new KeyedArrayChangelogBuilder<string>("CropName"),
            "DungeonBossDifficultyGameData" => new KeyedArrayChangelogBuilder<uint>("DefeatedNumGameDataHash"),
            "DoCondition" or "FinCondition" or "PickConditions"
                or "SuccessCondition" => new KeyedArrayChangelogBuilder<uint>("DefineNameHash"),
            "FallenActorTable" => new KeyedArrayChangelogBuilder<string>("DropActorName"),
            "SmallDungeonLocationList" => new KeyedArrayChangelogBuilder<string>("DungeonIndexStr"),
            "VariationListForArmorDye" => new KeyedArrayChangelogBuilder<string>("DyeColor"),
            "HackEquip" => new KeyedArrayChangelogBuilder<string>("EquipUserBbKey"),
            "AdventureMemorySetArray" or "EventEntry"
                or "GlobalResidentEventList" => new KeyedArrayChangelogBuilder<string>("EventName"),
            "AutoPlayBoneVisibilities" or "AutoPlayMaterials" => new KeyedArrayChangelogBuilder<string>("FileName"),
            "WinningRateTable" => new KeyedArrayChangelogBuilder<int>("FlintstonesNum"),
            "ModelVariationAnims" => new KeyedArrayChangelogBuilder<string>("Fmab"),
            "OptionParam" => new KeyedArrayChangelogBuilder<string>("FootIKMode"),
            "PartialList" => new KeyedArrayChangelogBuilder<string>("GameData"),
            "PlacementGroups" => new KeyedArrayChangelogBuilder<int>("GroupID"),
            "EffectLimiterGroup" or "HiddenMaterialGroupList" => new KeyedArrayChangelogBuilder<string>("GroupName"),
            "Node" => new KeyedArrayChangelogBuilder<byte[]>("GUID"),
            "Textures" => new KeyedArrayChangelogBuilder<uint>("guid"),
            "AiGroups" or "Points" or "Rails" => new KeyedArrayChangelogBuilder<ulong>("Hash"),
            "HeadshotDamageParameters" => new KeyedArrayChangelogBuilder<string>("HeadshotBoneName"),
            "References" => new KeyedArrayChangelogBuilder<string>("Id"),
            "TransitionParam" => new KeyedArrayChangelogBuilder<int>("Index"),
            "OverwriteParam" => new KeyedArrayChangelogBuilder<ulong>("InstanceId"),
            "Interests" => new KeyedArrayChangelogBuilder<string>("InterestType"),
            "StrongInterests" => new KeyedArrayChangelogBuilder<string>("InterestType"),
            "ConditionArray" or "OverrideASEvReactVerbSettings" or "OverrideASEventReactSettings" or "SwitchParam"
                or "TriggerParams" or "Triggers" => new KeyedArrayChangelogBuilder<string>("Key"),
            "OverrideReactionVerbSettings" => new KeyedArrayChangelogBuilder<string>("KeyActionVerb"),
            "ShootableActorSettings" => new KeyedArrayChangelogBuilder<uint>("KeyHash"),
            "AttachmentGroupList" or "EnemyGroupList" or "ShopWeaponGroupList"
                or "WeaponGroupList" => new KeyedArrayChangelogBuilder<string>("Label"),
            "ActionSeqs" => new KeyedArrayChangelogBuilder<uint>("LabelHash"),
            "CaveEntranceNormal" or "CaveEntranceSpecial" or "CaveEntranceWell" or "CheckPoint" or "City"
                or "District" or "DragonTears" or "Dungeon" or "Ground" or "ShopArmor" or "ShopDye" or "ShopGeneral"
                or "ShopInn" or "ShopJewelry" or "Shrine" or "SkyArchipelago" or "SpotBig" or "SpotBigArtifact"
                or "SpotBigMagma" or "SpotBigMountain" or "SpotBigOther" or "SpotBigTimber" or "SpotBigWater"
                or "SpotBigWithNameIcon" or "SpotMiddle" or "SpotMiddleArtifact" or "SpotMiddleMagma"
                or "SpotMiddleMountain" or "SpotMiddleOther" or "SpotMiddleTimber" or "SpotMiddleWater"
                or "SpotSmallArtifact" or "SpotSmallMagma" or "SpotSmallMountain" or "SpotSmallOther"
                or "SpotSmallTimber" or "SpotSmallWater" or "Stable" or "Tower"
                or "Underground" => new KeyedArrayChangelogBuilder<string>("LocationName"),
            "ManeNamePairs" => new KeyedArrayChangelogBuilder<string>("ManeActorName"),
            "MiasmaAreaParam" => new KeyedArrayChangelogBuilder<string>("MiasmaAreaType"),
            "AnimationDrive" or "CColEntityNamePathAry" or "CColSensorNamePathAry" or "CheckPointSetting" or "Cloth"
                or "ClothAdvandecOption" or "ClothList" or "ClothReaction" or "CollectItem" or "CollidableList"
                or "ControllerEntityNamePathAry" or "ControllerSensorNamePathAry" or "ControllerSensorUnitAry"
                or "ExternalShapeNamePathAry" or "HelperBoneList" or "IntData" or "LookIKControllerNamePathAry"
                or "LookingControllerNamePathAry" or "MatterRigidBodyNamePathAry" or "MeshList" or "ParamTable"
                or "RagdollReaction" or "RagdollReactionList" or "RagdollStructure" or "Reaction"
                or "RigidBodyEntityNamePathAry" or "RigidBodySensorNamePathAry" or "ShapeList" or "ShapeNamePathAry"
                or "StringData" or "TailBoneControllerNamePathAry" => new KeyedArrayChangelogBuilder<string>("Name"),
            "Property" or "TowingHookParams" => new KeyedArrayChangelogBuilder<uint>("NameHash"),
            "ExtraNewsSourceInfo" or "TopNewsSourceInfo" => new KeyedArrayChangelogBuilder<string>("NewsKeyName"),
            "PictureBookPackInfoArray" => new KeyedArrayChangelogBuilder<string>("PackActor"),
            "VariationListForParasail" => new KeyedArrayChangelogBuilder<string>("Pattern"),
            "PhshMesh" => new KeyedArrayChangelogBuilder<string>("PhshMeshPath"),
            "PlgGdTable" => new KeyedArrayChangelogBuilder<ulong>("PlgGuid"),
            "PropertyDefinitions" or "Sources" => new KeyedArrayChangelogBuilder<uint>("PropertyNameHash"),
            "Connections" => new KeyedArrayChangelogBuilder<ulong>("RailHash"),
            "CustomCullInfos" or "SpecialCullInfos" => new KeyedArrayChangelogBuilder<string>("ResourceName"),
            "Seats" => new KeyedArrayChangelogBuilder<string>("RidableType"),
            "VisibleSageOnNonMember" => new KeyedArrayChangelogBuilder<string>("SageType"),
            "SeriesArmorEffectList" => new KeyedArrayChangelogBuilder<string>("SeriesName"),
            "CropActorTable" => new KeyedArrayChangelogBuilder<string>("SrcActorName"),
            "SuspiciosBuffs" => new KeyedArrayChangelogBuilder<string>("SuspiciousBuffType"),
            "BoostOnlyTable" => new KeyedArrayChangelogBuilder<string>("Target"),
            "PartialConfigs" => new KeyedArrayChangelogBuilder<string>("TargetName"),
            "OverwritePropertiesEffect"
                or "OverwritePropertiesSound" => new KeyedArrayChangelogBuilder<uint>("TargetTypeNameHash"),
            "BGParamArray" => new KeyedArrayChangelogBuilder<string>("TexName"),
            "TipsSetArray" => new KeyedArrayChangelogBuilder<string>("TipsType"),
            "TmbMesh" => new KeyedArrayChangelogBuilder<string>("TmbMeshPath"),
            "ActorPositionData" => new KeyedArrayChangelogBuilder<string> ("$type"),
            "SB" or "T" or "U" => new KeyedArrayChangelogBuilder<string>("Umii"),
            "AlreadyReadInfo" => new KeyedArrayChangelogBuilder<string>("UpdateGameDataFlag"),
            "ConditionList" => new KeyedArrayChangelogBuilder<string>("WeaponEssence"),
            "WeaponTypeAndSubModelMapping" => new KeyedArrayChangelogBuilder<string>("WeaponType"),
            _ => DefaultArrayChangelogBuilder.Instance
        };
    }
}