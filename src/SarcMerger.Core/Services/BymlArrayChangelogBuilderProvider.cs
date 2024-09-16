using SarcMerger.Core.ChangelogBuilders;
using SarcMerger.Core.Models;

namespace SarcMerger.Core.Services;

// ReSharper disable StringLiteralTypo

// NOTE: The typos in this file
// are from Nintendo, not TKMM.

public static class BymlArrayChangelogBuilderProvider
{
    public static IBymlArrayChangelogBuilder GetChangelogBuilder(ref BymlTrackingInfo info, ReadOnlySpan<char> key)
    {
        return key switch {
            "Enemy" or "FallFloorInsect" or "Fish" or "GrassCut" or "Insect" or "NotDecayedLargeSwordList"
                or "NotDecayedSmallSwordList" or "NotDecayedSpearList" or "RainBonusMaterial" or "Seafood"
                or "SpObjCapsuleBlockMaster" or "Weapon" or "bow" or "shields" or "weapons"
                or "helmets" => new BymlKeyedArrayChangelogBuilder<ulong>("name"),  
            "Actors" => info.Type switch {
                "bcett" => new BymlKeyedArrayChangelogBuilder<ulong>("Hash"),
                "game__component__ArmyManagerParam" => new BymlKeyedArrayChangelogBuilder<string>("ActorName"),
                _ => BymlArrayChangelogBuilder.Instance
            },
            "Table" => info.Type switch {
                "game__colorvariation__ConversionActorNameToParasailPatternSetTable"
                    => new BymlKeyedArrayChangelogBuilder<string>("ParasailPattern"),
                "game__ecosystem__DecayedWeaponMappingTable"
                    => new BymlKeyedArrayChangelogBuilder<uint>("EquipmentDeathCountGmdHash"),
                _ => BymlArrayChangelogBuilder.Instance
            },
            "BoneInfoArray" => info.Type switch {
                "game__component__DragonParam" => new BymlKeyedArrayChangelogBuilder<uint>("Hash"),
                "phive__LookIKResourceHeaderParam"
                    or "phive__TailBoneResourceHeaderParam" => new BymlKeyedArrayChangelogBuilder<string>("BoneName"),
                _ => BymlArrayChangelogBuilder.Instance
            },
            "Elements" => info.Type switch {
                "game__enemy__DrakeSubModelInfo" => new BymlKeyedArrayChangelogBuilder<string>("BoneName"),
                "game__gamebalance__LevelSensorTargetDefine" => new BymlKeyedArrayChangelogBuilder<uint>("ActorNameHash"),
                _ => BymlArrayChangelogBuilder.Instance
            },
            "Items" => info.Type switch {
                "game__pouchcontent__EnhancementMaterial" => new BymlKeyedArrayChangelogBuilder<string>("Actor"),
                _ => BymlArrayChangelogBuilder.Instance
            },
            "List" => info.Type switch {
                "game__sound__ShrineSpotBgmTypeInfoList" => new BymlKeyedArrayChangelogBuilder<string>("DungeonIndexStr"),
                _ => BymlArrayChangelogBuilder.Instance
            },
            "Contents" => info.Type switch {
                "game__ui__FairyFountainGlobalSetting" => new BymlKeyedArrayChangelogBuilder<string>("Actor"),
                _ => BymlArrayChangelogBuilder.Instance
            },
            "SettingTable" => info.Type switch {
                "game__ui__LargeDungeonFloorDefaultSettingTable" => new BymlKeyedArrayChangelogBuilder<string>("DungeonType"),
                _ => BymlArrayChangelogBuilder.Instance
            },
            "BrainVerbs" => new BymlKeyedArrayChangelogBuilder<string>("ActionSeqContainer"),
            "ActionVerbContainerElements" => new BymlKeyedArrayChangelogBuilder<string>("ActionVerb"),
            "ResidentActors" or "Settings"
                or "ShootableShareActorSettings" or "GoodsList" => new BymlKeyedArrayChangelogBuilder<string>("Actor"),
            "BindActorInfo" => new BymlKeyedArrayChangelogBuilder<string>("ActorHolderKey"),
            "RegisteredActorArray" or "RequirementList"
                or "Rewards" => new BymlKeyedArrayChangelogBuilder<string>("ActorName"),
            "SharpInfoBowList" => new BymlKeyedArrayChangelogBuilder<uint>("ActorNameHash"),
            "PictureBookParamArray" => new BymlKeyedArrayChangelogBuilder<string>("ActorNameShort"),
            "WeakPointActorArray" => new BymlKeyedArrayChangelogBuilder<string>("ActorPath"),
            "NavMeshObjects" => new BymlKeyedArrayChangelogBuilder<string>("Alias"),
            "AliasEntityList" => new BymlKeyedArrayChangelogBuilder<string>("AliasEntity"),
            "AliasSensorList" => new BymlKeyedArrayChangelogBuilder<string>("AliasSensor"),
            "Anchors" => new BymlKeyedArrayChangelogBuilder<string>("AnchorName"),
            "ArmorEffect" => new BymlKeyedArrayChangelogBuilder<string>("ArmorEffectType"),
            "HornTypeAndAttachmentMapping" => new BymlKeyedArrayChangelogBuilder<string>("AttachmentName"),
            "AttackParams" => new BymlKeyedArrayChangelogBuilder<string>("AttackType"),
            "BlackboardParamBoolArray" or "BlackboardParamCustomTypeArray" or "BlackboardParamF32Array"
                or "BlackboardParamMtx33fArray" or "BlackboardParamMtx34fArray" or "BlackboardParamPtrArray"
                or "BlackboardParamQuatfArray" or "BlackboardParamS32Array" or "BlackboardParamS8Array"
                or "BlackboardParamStringArray" or "BlackboardParamU32Array" or "BlackboardParamU64Array"
                or "BlackboardParamU8Array" or "BlackboardParamVec3fArray"
                or "EditBBParams" => new BymlKeyedArrayChangelogBuilder<string>("BBKey"),
            "DragonInfoList" => new BymlKeyedArrayChangelogBuilder<uint>("BindPointRespawnGameDataHash"),
            "OperationAngular" or "OperationLinear" => new BymlKeyedArrayChangelogBuilder<string>("Body"),
            "BindBoneList" or "BoneList" or "BoneModifierSet" or "Bones" or "FruitOffsetTranslation"
                or "ModelBindSettings" or "StickWeaponBone" => new BymlKeyedArrayChangelogBuilder<string>("BoneName"),
            "Categories" => new BymlKeyedArrayChangelogBuilder<string>("CallbackName"),
            "CaveParams" => new BymlKeyedArrayChangelogBuilder<ulong>("CaveInstanceId"),
            "CheckList" => new BymlKeyedArrayChangelogBuilder<string>("CheckType"),
            "Object" => new BymlKeyedArrayChangelogBuilder<string>("ChemicalMaterial"),
            "CharacterComponentPresetCollection" or "LayerHitMaskEntityCollection" or "LayerHitMaskSensorCollection"
                or "MaterialCollection" or "MaterialPresetCollection" or "MotionPropertiesCollection"
                or "PhysicsMaterialMappingInfoCollection" or "SubLayerHitMaskEntityCollection"
                or "SubLayerHitMaskSensorCollection"
                or "UserShapeTagMaskCollection" => new BymlKeyedArrayChangelogBuilder<string>("ComponentName"),
            "HingeArray" or "RangeArray" => new BymlKeyedArrayChangelogBuilder<string>("ConstraintName"),
            "CropYieldTable" => new BymlKeyedArrayChangelogBuilder<string>("CropName"),
            "DungeonBossDifficultyGameData" => new BymlKeyedArrayChangelogBuilder<uint>("DefeatedNumGameDataHash"),
            "DoCondition" or "FinCondition" or "PickConditions"
                or "SuccessCondition" => new BymlKeyedArrayChangelogBuilder<uint>("DefineNameHash"),
            "FallenActorTable" => new BymlKeyedArrayChangelogBuilder<string>("DropActorName"),
            "SmallDungeonLocationList" => new BymlKeyedArrayChangelogBuilder<string>("DungeonIndexStr"),
            "VariationListForArmorDye" => new BymlKeyedArrayChangelogBuilder<string>("DyeColor"),
            "HackEquip" => new BymlKeyedArrayChangelogBuilder<string>("EquipUserBbKey"),
            "AdventureMemorySetArray"
                or "GlobalResidentEventList" => new BymlKeyedArrayChangelogBuilder<string>("EventName"),
            "AutoPlayBoneVisibilities" or "AutoPlayMaterials" => new BymlKeyedArrayChangelogBuilder<string>("FileName"),
            "WinningRateTable" => new BymlKeyedArrayChangelogBuilder<int>("FlintstonesNum"),
            "ModelVariationAnims" => new BymlKeyedArrayChangelogBuilder<string>("Fmab"),
            "OptionParam" => new BymlKeyedArrayChangelogBuilder<string>("FootIKMode"),
            "PartialList" => new BymlKeyedArrayChangelogBuilder<string>("GameData"),
            "PlacementGroups" => new BymlKeyedArrayChangelogBuilder<int>("GroupID"),
            "EffectLimiterGroup" or "HiddenMaterialGroupList" => new BymlKeyedArrayChangelogBuilder<string>("GroupName"),
            "Textures" => new BymlKeyedArrayChangelogBuilder<uint>("guid"),
            "AiGroups" or "Points" => new BymlKeyedArrayChangelogBuilder<ulong>("Hash"),
            "Rails" => info.Level switch {
                0 => new BymlKeyedArrayChangelogBuilder<ulong>("Hash"),
                _ => BymlArrayChangelogBuilder.Instance
            },
            "HeadshotDamageParameters" => new BymlKeyedArrayChangelogBuilder<string>("HeadshotBoneName"),
            "TransitionParam" => new BymlKeyedArrayChangelogBuilder<int>("Index"),
            "OverwriteParam" => new BymlKeyedArrayChangelogBuilder<ulong>("InstanceId"),
            "Interests" => new BymlKeyedArrayChangelogBuilder<string>("InterestType"),
            "StrongInterests" => new BymlKeyedArrayChangelogBuilder<string>("InterestType"),
            "ConditionArray" or "OverrideASEvReactVerbSettings" or "OverrideASEventReactSettings" or "SwitchParam"
                or "TriggerParams" or "Triggers" => new BymlKeyedArrayChangelogBuilder<string>("Key"),
            "OverrideReactionVerbSettings" => new BymlKeyedArrayChangelogBuilder<string>("KeyActionVerb"),
            "ShootableActorSettings" => new BymlKeyedArrayChangelogBuilder<uint>("KeyHash"),
            "AttachmentGroupList" or "EnemyGroupList" or "ShopWeaponGroupList"
                or "WeaponGroupList" => new BymlKeyedArrayChangelogBuilder<string>("Label"),
            "ActionSeqs" => new BymlKeyedArrayChangelogBuilder<uint>("LabelHash"),
            "CaveEntranceNormal" or "CaveEntranceSpecial" or "CaveEntranceWell" or "CheckPoint" or "City"
                or "District" or "DragonTears" or "Dungeon" or "Ground" or "ShopArmor" or "ShopDye" or "ShopGeneral"
                or "ShopInn" or "ShopJewelry" or "Shrine" or "SkyArchipelago" or "SpotBig" or "SpotBigArtifact"
                or "SpotBigMagma" or "SpotBigMountain" or "SpotBigOther" or "SpotBigTimber" or "SpotBigWater"
                or "SpotBigWithNameIcon" or "SpotMiddle" or "SpotMiddleArtifact" or "SpotMiddleMagma"
                or "SpotMiddleMountain" or "SpotMiddleOther" or "SpotMiddleTimber" or "SpotMiddleWater"
                or "SpotSmallArtifact" or "SpotSmallMagma" or "SpotSmallMountain" or "SpotSmallOther"
                or "SpotSmallTimber" or "SpotSmallWater" or "Stable" or "Tower"
                or "Underground" => info.Type switch {
                    "locationarea" => new BymlKeyedArrayChangelogBuilder<string>("LocationName"),
                    _ => BymlArrayChangelogBuilder.Instance
                },
            "MiasmaAreaParam" => new BymlKeyedArrayChangelogBuilder<string>("MiasmaAreaType"),
            "AnimationDrive" or "CColEntityNamePathAry" or "CColSensorNamePathAry" or "CheckPointSetting" or "Cloth"
                or "ClothAdvandecOption" or "ClothList" or "ClothReaction" or "CollectItem" or "CollidableList"
                or "ControllerEntityNamePathAry" or "ControllerSensorNamePathAry" or "ControllerSensorUnitAry"
                or "ExternalShapeNamePathAry" or "HelperBoneList" or "IntData" or "LookIKControllerNamePathAry"
                or "LookingControllerNamePathAry" or "MatterRigidBodyNamePathAry" or "MeshList" or "ParamTable"
                or "RagdollReaction" or "RagdollReactionList" or "RagdollStructure" or "Reaction"
                or "RigidBodyEntityNamePathAry" or "RigidBodySensorNamePathAry" or "ShapeList" or "ShapeNamePathAry"
                or "StringData" or "TailBoneControllerNamePathAry"
                or "Node" => new BymlKeyedArrayChangelogBuilder<string>("Name"),
            "Property" or "TowingHookParams" => new BymlKeyedArrayChangelogBuilder<uint>("NameHash"),
            "ExtraNewsSourceInfo" or "TopNewsSourceInfo" => new BymlKeyedArrayChangelogBuilder<string>("NewsKeyName"),
            "PictureBookPackInfoArray" => new BymlKeyedArrayChangelogBuilder<string>("PackActor"),
            "VariationListForParasail" => new BymlKeyedArrayChangelogBuilder<string>("Pattern"),
            "PhshMesh" => new BymlKeyedArrayChangelogBuilder<string>("PhshMeshPath"),
            "PlgGdTable" => new BymlKeyedArrayChangelogBuilder<ulong>("PlgGuid"),
            "PropertyDefinitions" or "Sources" => new BymlKeyedArrayChangelogBuilder<uint>("PropertyNameHash"),
            "Connections" => new BymlKeyedArrayChangelogBuilder<ulong>("RailHash"),
            "CustomCullInfos" or "SpecialCullInfos" => new BymlKeyedArrayChangelogBuilder<string>("ResourceName"),
            "Seats" => new BymlKeyedArrayChangelogBuilder<string>("RidableType"),
            "VisibleSageOnNonMember" => new BymlKeyedArrayChangelogBuilder<string>("SageType"),
            "SeriesArmorEffectList" => new BymlKeyedArrayChangelogBuilder<string>("SeriesName"),
            "CropActorTable" => new BymlKeyedArrayChangelogBuilder<string>("SrcActorName"),
            "SuspiciosBuffs" => new BymlKeyedArrayChangelogBuilder<string>("SuspiciousBuffType"),
            "BoostOnlyTable" => new BymlKeyedArrayChangelogBuilder<string>("Target"),
            "PartialConfigs" => new BymlKeyedArrayChangelogBuilder<string>("TargetName"),
            "OverwritePropertiesEffect"
                or "OverwritePropertiesSound" => new BymlKeyedArrayChangelogBuilder<uint>("TargetTypeNameHash"),
            "BGParamArray" => new BymlKeyedArrayChangelogBuilder<string>("TexName"),
            "TipsSetArray" => new BymlKeyedArrayChangelogBuilder<string>("TipsType"),
            "TmbMesh" => new BymlKeyedArrayChangelogBuilder<string>("TmbMeshPath"),
            "ActorPositionData" or "EventEntry" => new BymlKeyedArrayChangelogBuilder<string> ("$type"),
            "SB" or "T" or "U" => new BymlKeyedArrayChangelogBuilder<string>("Umii"),
            "AlreadyReadInfo" => new BymlKeyedArrayChangelogBuilder<string>("UpdateGameDataFlag"),
            "ConditionList" => new BymlKeyedArrayChangelogBuilder<string>("WeaponEssence"),
            "WeaponTypeAndSubModelMapping" => new BymlKeyedArrayChangelogBuilder<string>("WeaponType"),
            _ => BymlArrayChangelogBuilder.Instance
        };
    }
}