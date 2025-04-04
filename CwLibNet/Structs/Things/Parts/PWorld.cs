using System.Numerics;
using CwLibNet.Enums;
using CwLibNet.EX;
using CwLibNet.IO;
using CwLibNet.IO.Serializer;
using CwLibNet.Structs.Slot;
using CwLibNet.Structs.Streaming;
using CwLibNet.Structs.Things.Components;
using CwLibNet.Structs.Things.Components.World;
using CwLibNet.Types;
using CwLibNet.Types.Data;
using CwLibNet.Types.Things;

namespace CwLibNet.Structs.Things.Parts;

public class PWorld: ISerializable
{
    public ResourceDescriptor[] Materials;
    public float BackdropOffsetX, BackdropOffsetY, BackdropOffsetZ;
    public bool BackdropOffsetZAuto;
    public String OverrideBackdropAmbience;
    
    public StreamingManager StreamingManager;
    public List<Thing> Things = [];
    public float MaxVel = 100.0f, MaxAVel = 1.0f;
    public int Frame;
    public int SimFrame;
    public int FrameLevelStarted;
    public int Randy;
    public int ThingUidCounter = 2;
    public EditorSelection[] Selections;
    public Thing Backdrop, BackdropNew;
    public float BackdropTimer;
    public int Lbp2NightDaySwapped = 1;
    public bool IsPaused;
    public float LightingFactor = 1.0f, ColorCorrectionFactor;
    public float FogFactor, FogTintFactor, DarknessFactor;
    public EggLink[] CompleteRewards, CollectRewards, AceRewards;
    public bool AreRewardsShareable = true;
    public SlotID ScoreboardLevelLinkSlot;
    public KeyLink[] CompleteUnlocks, CollectUnlocks, AceUnlocks;
    public int DeathCount;
    public int MaxNumPlayers = 1;
    public Thing[] DissolvingThings, OldDissolvingThings;
    public bool IsTutorialLevel;
    public bool EverSpawned;
    public int SpawnFailureCount;
    public GlobalSettings TargetGlobalSettings = new(), FromGlobalSettings = new();
    public float GlobalSettingsBlendFactor;
    public bool HasLevelLightingBeenSetup;
    public int GlobalSettingsThingUid;
    public CameraSettings CameraSettings = new CameraSettings();
    public float WaterLevel, TargetWaterLevel;
    
    
    public float FromWaterLevel;
    public float WaterWaveMagnitude, FromWaterWaveMagnitude, TargetWaterWaveMagnitude;
    public float Gravity = 1.0f, FromGravity = 1.0f, TargetGravity = 1.0f;
    public float[] CurrGlobalSettingsBlendFactors;
    public int[] GlobalSettingsThingUiDs;
    public int[] GlobalSettingsThingPriority;
    
    
    
    public float WaterTint, TargetWaterTint;

    
    
    public Vector4? FromWaterTintColor = new Vector4(0.47451f, 0.898039f, 0.909804f, 0.125f);

    
    
    public float WaterMurkiness, FromWaterMurkiness, TargetWaterMurkiness;

    
    
    public float WaterBits = 1.0f, FromWaterBits = 1.0f, TargetWaterBits = 1.0f;

    public bool WaterDrainSoundsEnabled = true, CurrWaterDrainSoundsEnabled = true;
    public bool WaterCausticsEnabled = true, CurrWaterCausticsEnabled = true;
    
    
    public int WaterMainColor = -1090453761, FromWaterMainColor = -1090453761, TargetWaterMainColor = -1090453761;
    
    public int WaterHintColorOne = -33554177, FromWaterHintColorOne = -33554177, TargetWaterHintColorOne = -33554177;
    
    public int WaterHintColorTwo = 16662783, FromWaterHintColorTwo = 16662783, TargetWaterHintColorTwo = 16662783;

    public bool BackdropEnabled = true, CurrBackdropEnabled = true;

    
    
    public float CurrWavePos = 0.0186706f;


    public int GameMode = 0;
    public int GameModeRequested = 0;

    public int NextSackbotPlayerNumber = -2;
    public CutsceneCameraManager CutsceneCameraManager = new CutsceneCameraManager();
    public GlobalAudioSettings GlobalAudioSettings = new GlobalAudioSettings();
    public ResourceDescriptor BackdropPlan, BackdropNewPlan;
    public bool SubLevel;
    public bool ScoreLocked;
    public int DebugTimeInLevel;
    public bool UseEvenNewerCheckpointCode;
    public MoveCursor[] MoveCursors;
    
    public bool SinglePlayer;
    public int MinPlayers = 1, MaxPlayers = 4;
    public bool MoveRecommended;
    public bool FixInvalidInOutMoverContacts;
    public bool ContinueMusic;

    public BroadcastMicrochipEntry[] BroadcastMicroChipEntries;
    public int ManualJumpDown;
    public Thing[] DeferredDestroys;
    public float GlobalDofFront, GlobalDofBack, GlobalDofSackTrack;
    public bool EnableSackpocket;
    public bool ShowQuestLog;
    public SlotID ScoreboardUnlockLevelSlot;
    public String ProgressBoardLevelLinkStartPoint;
    public bool IsLbp3World;

    /* Vita Fields */

    public bool NonLinearFog;
    public EggLink[] BronzeRewards, SilverRewards, GoldRewards;
    public KeyLink[] BronzeUnlocks, SilverUnlocks, GoldUnlocks;
    public byte BronzeTrophyConditionType, SilverTrophyConditionType, GoldTrophyConditionType;
    public int ScoreRequiredForBronzeTrophy, ScoreRequiredForSilverTrophy, ScoreRequiredForGoldTrophy;
    public float TimeRequiredForBronzeTrophy, TimeRequiredForSilverTrophy, TimeRequiredForGoldTrophy;
    public int LivesLostRequiredForBronzeTrophy, LivesLostRequiredForSilverTrophy, LivesLostRequiredForGoldTrophy;

    public bool EnableAcing, EnableGoldTrophy, EnableSilverTrophy, EnableBronzeTrophy;
    public bool EnforceMinMaxPlayers;
    
    public int WaterColor;
    public float WaterBrightness;

    public int GlobalTouchCursor;


    public bool PortraitMode; 
    public bool SharedScreen;
    public bool DisableHud;
    public bool DisableShadows;
    public bool FlipBackground;
    public bool MpSeparateScreen;
    
    public PWorld() {
        this.CurrGlobalSettingsBlendFactors = new float[12];
        this.GlobalSettingsThingUiDs = new int[12];
        this.GlobalSettingsThingPriority = new int[12];
        for (int i = 0; i < 12; ++i)
            this.CurrGlobalSettingsBlendFactors[i] = 1.0f;
    }

    public void Serialize(Serializer serializer) {
        Revision revision = serializer.GetRevision();
        int version = revision.GetVersion();
        int subVersion = revision.GetSubVersion();

        if (subVersion >= 0x6d) {
            BackdropOffsetX = serializer.F32(BackdropOffsetX);
            BackdropOffsetY = serializer.F32(BackdropOffsetY);
            BackdropOffsetZ = serializer.F32(BackdropOffsetZ);
        }

        if (subVersion >= 0x72 && subVersion <= 0x73) {
            serializer.F32(0);
            serializer.F32(0);
            serializer.F32(0);
            serializer.U8(0);
            serializer.U8(0);
        }

        if (subVersion >= 0x70)
            BackdropOffsetZAuto = serializer.Bool(BackdropOffsetZAuto);
        if (subVersion >= 0xe2)
            OverrideBackdropAmbience = serializer.Str(OverrideBackdropAmbience);
        
        if (version < 0x14b) {
            if (serializer.IsWriting()) serializer.GetOutput().I32(0);
            else {
                Materials = new ResourceDescriptor[serializer.GetInput().I32()];
                for (int i = 0; i < Materials.Length; ++i)
                    Materials[i] = serializer.Resource(null, ResourceType.Material);
            }
        }

        if (subVersion >= 0x3f)
            StreamingManager = serializer.Reference(StreamingManager);
        
        if (!revision.IsToolkit() || revision.Before(Branch.Mizuki, (int)Revisions.MzSceneGraph)) {
            Things = serializer.Arraylist(Things, true);
            serializer.Log("END OF WORLD THINGS");

            MaxVel = serializer.F32(MaxVel);
            MaxAVel = serializer.F32(MaxAVel);
    
            Frame = serializer.S32(Frame);
            if (version >= 0x2e2) SimFrame = serializer.S32(SimFrame);
            if (version >= 0x377) FrameLevelStarted = serializer.I32(FrameLevelStarted);

            ThingUidCounter = serializer.I32(ThingUidCounter);
        }

        
        if (version < 0x32d)
            Randy = serializer.I32(Randy);

        if (version < 0x1a4) {
            serializer.F32(0);
            serializer.F32(0);
            serializer.F32(0);
            serializer.F32(0);
            serializer.F32(0);
            serializer.F32(0);
            serializer.I32(0);
            if (version < 0x14b)
                serializer.Array<Thing>(null);
        }

        if (!revision.IsToolkit() || revision.Before(Branch.Mizuki, (int)Revisions.MzSceneGraph)) {
            Selections = serializer.Array(Selections, true);

            Backdrop = serializer.Reference(Backdrop);
            BackdropNew = serializer.Reference(BackdropNew);
            BackdropTimer = serializer.F32(BackdropTimer);
        }

        if (version >= 0x3a3)
            Lbp2NightDaySwapped = serializer.S32(Lbp2NightDaySwapped);

        if (version >= 0x14a)
            IsPaused = serializer.Bool(IsPaused);

        if (version >= 0x152) {
            LightingFactor = serializer.F32(LightingFactor);
            ColorCorrectionFactor = serializer.F32(ColorCorrectionFactor);
            if (version >= 0x196) {
                FogFactor = serializer.F32(FogFactor);
                FogTintFactor = serializer.F32(FogTintFactor);
                DarknessFactor = serializer.F32(DarknessFactor);
                if (revision.Has(Branch.Double11, 0x78))
                    NonLinearFog = serializer.Bool(NonLinearFog);
            }
        }

        if (version >= 0x16f) {
            CompleteRewards = serializer.Array(CompleteRewards);
            CollectRewards = serializer.Array(CollectRewards);
            AceRewards = serializer.Array(AceRewards);
            if (version >= 0x208) 
                AreRewardsShareable = serializer.Bool(AreRewardsShareable);
            if (version >= 0x35e)
                ScoreboardLevelLinkSlot = serializer.Struct(ScoreboardLevelLinkSlot);
            CompleteUnlocks = serializer.Array(CompleteUnlocks);
            CollectUnlocks = serializer.Array(CollectUnlocks);
            AceUnlocks = serializer.Array(AceUnlocks);
        }

        if (revision.IsVita()) {

            if (revision.Has(Branch.Double11, 0x22) && revision.Before(Branch.Double11, 0x25)) {
                serializer.Array<EggLink>(null); // unlocks
                serializer.Array<EggLink>(null);  // rewards
                serializer.S32(0); // timeRequired
            }

            if (revision.Has(Branch.Double11, 0x25)) {
                GoldRewards = serializer.Array(GoldRewards);
                GoldUnlocks = serializer.Array(GoldUnlocks);
                SilverRewards = serializer.Array(SilverRewards);
                SilverUnlocks = serializer.Array(SilverUnlocks);
                BronzeRewards = serializer.Array(BronzeRewards);
                BronzeUnlocks = serializer.Array(BronzeUnlocks);

                GoldTrophyConditionType = serializer.I8(GoldTrophyConditionType);
                SilverTrophyConditionType = serializer.I8(SilverTrophyConditionType);
                BronzeTrophyConditionType = serializer.I8(BronzeTrophyConditionType);

                ScoreRequiredForGoldTrophy = serializer.S32(ScoreRequiredForGoldTrophy);
                TimeRequiredForGoldTrophy = serializer.F32(TimeRequiredForGoldTrophy);
                LivesLostRequiredForGoldTrophy = serializer.S32(LivesLostRequiredForGoldTrophy);

                ScoreRequiredForSilverTrophy = serializer.S32(ScoreRequiredForSilverTrophy);
                TimeRequiredForSilverTrophy = serializer.F32(TimeRequiredForSilverTrophy);
                LivesLostRequiredForSilverTrophy = serializer.S32(LivesLostRequiredForSilverTrophy);

                ScoreRequiredForBronzeTrophy = serializer.S32(ScoreRequiredForBronzeTrophy);
                TimeRequiredForBronzeTrophy = serializer.F32(TimeRequiredForBronzeTrophy);
                LivesLostRequiredForBronzeTrophy = serializer.S32(LivesLostRequiredForBronzeTrophy);
            }

            if (revision.Has(Branch.Double11, 0x3d)) {
                EnableAcing = serializer.Bool(EnableAcing);
                EnableGoldTrophy = serializer.Bool(EnableGoldTrophy);
                EnableSilverTrophy = serializer.Bool(EnableSilverTrophy);
                EnableBronzeTrophy = serializer.Bool(EnableBronzeTrophy);
            }

            if (revision.Has(Branch.Double11, 0x4c))
                EnforceMinMaxPlayers = serializer.Bool(EnforceMinMaxPlayers);
        }

        if (0x16e < version && version < 0x1bf) {
            serializer.I32(0);
            serializer.I32(0);
            serializer.I32(0);
        }

        if (0x1a3 < version && version < 0x1d1)
            throw new SerializationException("CGameCamera serialization unsupported!");

        if (0x1bd < version && version < 0x213)
            serializer.I32(0);

        if (version >= 0x1c2 && version < 0x36e)
            DeathCount = serializer.I32(DeathCount);
        if (version >= 0x1c4)
            MaxNumPlayers = serializer.I32(MaxNumPlayers);

        if (version >= 0x1db) {
            DissolvingThings = serializer.Array(DissolvingThings, true);
            OldDissolvingThings = serializer.Array(OldDissolvingThings, true);
        }

        if (version >= 0x1de && version < 0x345)
            IsTutorialLevel = serializer.Bool(IsTutorialLevel);

        if (version >= 0x22e) {
            EverSpawned = serializer.Bool(EverSpawned);
            SpawnFailureCount = serializer.I32(SpawnFailureCount);
        }

        if (version >= 0x25a) {
            TargetGlobalSettings = serializer.Struct(TargetGlobalSettings);
            FromGlobalSettings = serializer.Struct(FromGlobalSettings);
            GlobalSettingsBlendFactor = serializer.F32(GlobalSettingsBlendFactor);
            HasLevelLightingBeenSetup = serializer.Bool(HasLevelLightingBeenSetup);
            GlobalSettingsThingUid = serializer.I32(GlobalSettingsThingUid);
        }

        if (version >= 0x370)
            CameraSettings = serializer.Struct(CameraSettings);

        if (version >= 0x26f) {
            WaterLevel = serializer.F32(WaterLevel);
            if (version >= 0x278 || revision.Has(Branch.Leerdammer, (int)Revisions.LdWaterWave))
                FromWaterLevel = serializer.F32(FromWaterLevel);
            TargetWaterLevel = serializer.F32(TargetWaterLevel);
        }

        if (version >= 0x270) {
            WaterWaveMagnitude = serializer.F32(WaterWaveMagnitude);
            FromWaterWaveMagnitude = serializer.F32(FromWaterWaveMagnitude);
            TargetWaterWaveMagnitude = serializer.F32(TargetWaterWaveMagnitude);
        }

        if (version >= 0x26f) {
            Gravity = serializer.F32(Gravity);
            FromGravity = serializer.F32(FromGravity);
            TargetGravity = serializer.F32(TargetGravity);

            CurrGlobalSettingsBlendFactors = serializer.Floatarray(CurrGlobalSettingsBlendFactors);
            GlobalSettingsThingUiDs = serializer.Intarray(GlobalSettingsThingUiDs);
            if (version >= 0x270)
                GlobalSettingsThingPriority = serializer.Intarray(GlobalSettingsThingPriority);
        }

        if (revision.Has(Branch.Double11, 0x2d)) {
            WaterColor = serializer.S32(WaterColor);
            WaterBrightness = serializer.F32(WaterBrightness);
        } else if (version >= 0x289 || revision.IsLeerdammer()) {
            WaterTint = serializer.F32(WaterTint);
            FromWaterTintColor = serializer.V4(FromWaterTintColor);
            TargetWaterTint = serializer.F32(TargetWaterTint);
        }
        
        if (version >= 0x289 || revision.IsLeerdammer()) {
            WaterMurkiness = serializer.F32(WaterMurkiness);
            FromWaterMurkiness = serializer.F32(FromWaterMurkiness);
            TargetWaterMurkiness = serializer.F32(TargetWaterMurkiness);
        }

        if (version >= 0x2b4 || revision.Has(Branch.Leerdammer, (int)Revisions.LdWaterBits)) {
            WaterBits = serializer.F32(WaterBits);
            FromWaterBits = serializer.F32(FromWaterBits);
            TargetWaterBits = serializer.F32(TargetWaterBits);
        }

        if (version >= 0x34e) {
            WaterDrainSoundsEnabled = serializer.Bool(WaterDrainSoundsEnabled);
            CurrWaterDrainSoundsEnabled = serializer.Bool(CurrWaterDrainSoundsEnabled);
        }

        if (subVersion >= 0xe8) {
            WaterCausticsEnabled = serializer.Bool(WaterCausticsEnabled);
            CurrWaterCausticsEnabled = serializer.Bool(WaterCausticsEnabled);
        }

        if (subVersion >= 0xf8) {
            WaterMainColor = serializer.I32(WaterMainColor);
            FromWaterMainColor = serializer.I32(FromWaterMainColor);
            TargetWaterMainColor = serializer.I32(TargetWaterMainColor);

            WaterHintColorOne = serializer.I32(WaterHintColorOne);
            FromWaterHintColorOne = serializer.I32(FromWaterHintColorOne);
            TargetWaterHintColorOne = serializer.I32(TargetWaterHintColorOne);

            WaterHintColorTwo = serializer.I32(WaterHintColorTwo);
            FromWaterHintColorTwo = serializer.I32(FromWaterHintColorTwo);
            TargetWaterHintColorTwo = serializer.I32(TargetWaterHintColorTwo);
        }

        if (subVersion >= 0xf8 && subVersion < 0x189)
            serializer.Bool(false);

        if (subVersion >= 0x182) {
            BackdropEnabled = serializer.Bool(BackdropEnabled);
            CurrBackdropEnabled = serializer.Bool(CurrBackdropEnabled);
        }

        if (version >= 0x29c || revision.Has(Branch.Leerdammer, (int)Revisions.LdWaterWave))
            CurrWavePos = serializer.F32(CurrWavePos);
        

        if ((version > 0x288 && version < 0x29c) || (revision.IsLeerdammer() && revision.Before(Branch.Leerdammer, (int)Revisions.LdWaterWave))) {
            serializer.F32(0);
            serializer.Bool(false);
        }

        // CBreadLoaf
        if (0x281 < version && version < 0x287) {
            if (serializer.I32(0) != 0)
                throw new SerializationException("CBreadLoaf serialization not supported!");

            // serializer.I32(0); // loafAlloc.numHandles
            // serializer.I32(0); // loafAlloc.freehead
            // serializer.I32(0); // loafAlloc.numUsed
            // // handles?

            // // reflect array 2, loafMin floats
            // serializer.I32(0); // loafSize
            // serializer.u16(0); // maxDepth
            // serializer.u16(0); // first

            // // reflect array, x via numHandles float
            // // reflect array, y via numHandles float
            // // reflect 4 LoafHandle's short?
            // // firstcrumb short
            // // depth, bytearray?
        }

        if (version >= 0x2a3) GameMode = serializer.I32(GameMode);
    
        if (subVersion >= 0x218) 
            GameModeRequested = serializer.I32(GameModeRequested);

        if (version >= 0x2b0) 
            NextSackbotPlayerNumber = serializer.S32(NextSackbotPlayerNumber);

        if (0x2d3 < version && version < 0x2f3)
            throw new SerializationException("Unsupported structure in serialization");

        if (version >= 0x2ee)
            CutsceneCameraManager = serializer.Struct(CutsceneCameraManager);

        if (version >= 0x30c)
            GlobalAudioSettings = serializer.Struct(GlobalAudioSettings);
        
        if ((version >= 0x321 && !revision.IsToolkit()) || revision.Before(Branch.Mizuki, (int)Revisions.MzSceneGraph)) {
            BackdropPlan = serializer.Resource(BackdropPlan, ResourceType.Plan, true);
            BackdropNewPlan = serializer.Resource(BackdropNewPlan, ResourceType.Plan, true);
        }

        if (version >= 0x352)
            SubLevel = serializer.Bool(SubLevel);

        if (version >= 0x38a)
            ScoreLocked = serializer.Bool(ScoreLocked);

        if (version >= 0x38b)
            DebugTimeInLevel = serializer.S32(DebugTimeInLevel);

        if (version >= 0x3ac)
            UseEvenNewerCheckpointCode = serializer.Bool(UseEvenNewerCheckpointCode);
        
        if (version >= 0x3bd && subVersion <= 0x117)
            MoveCursors = serializer.Array(MoveCursors);

        // version > 0x3c0, rather than 0x3e1 for some reason
        if (revision.Has(Branch.Double11, 0x8))
            GlobalTouchCursor = serializer.I32(GlobalTouchCursor);
        if (revision.Has(Branch.Double11, 0xa) && revision.Before(Branch.Double11, 0x28)) 
            SharedScreen = serializer.Bool(SharedScreen);

        if (subVersion > 0x215)
            SinglePlayer = serializer.Bool(SinglePlayer);
        
        if (version >= 0x3d0) {
            MinPlayers = serializer.U8(MinPlayers);
            MaxPlayers = serializer.U8(MaxPlayers);
            MoveRecommended = serializer.Bool(MoveRecommended);
        }

        if (revision.IsVita()) {
            int vita = revision.GetBranchRevision();

            // version > 0x3d4, rathern than 0x3e1 for some reason
            if (vita >= 0x18) 
                PortraitMode = serializer.Bool(PortraitMode);

            if (vita >= 0x28 && vita < 0x47) {
                if (serializer.U8(0) > 1 && !serializer.IsWriting())
                    SharedScreen = true;
            }

            if (version >= 0x3dd)
                FixInvalidInOutMoverContacts = serializer.Bool(FixInvalidInOutMoverContacts);
            
            if (vita >= 0x47) SharedScreen = serializer.Bool(SharedScreen);
            if (vita >= 0x39) DisableHud = serializer.Bool(DisableHud);
            if (vita >= 0x52) DisableShadows = serializer.Bool(DisableShadows);
            if (vita >= 0x3b) FlipBackground = serializer.Bool(FlipBackground); 
            if (vita >= 0x4f) MpSeparateScreen = serializer.Bool(MpSeparateScreen);

        }

        if (version >= 0x3dd)
            FixInvalidInOutMoverContacts = serializer.Bool(FixInvalidInOutMoverContacts);

        if (version >= 0x3f1)
            ContinueMusic = serializer.Bool(ContinueMusic);

        if (subVersion >= 0x2f)
            BroadcastMicroChipEntries = serializer.Array(BroadcastMicroChipEntries);

        if (subVersion >= 0x5d)
            ManualJumpDown = serializer.I32(ManualJumpDown);

        if (subVersion >= 0x98 && subVersion < 0xe5)
            serializer.Thingarray(null);
        if (subVersion >= 0xc3 && subVersion < 0xe5)
            serializer.Thingarray(null);
        if (subVersion >= 0x98 && subVersion < 0xe5)
            serializer.Thingarray(null);

        if (subVersion >= 0xe5)
            DeferredDestroys = serializer.Array(DeferredDestroys, true);

        if (subVersion >= 0xcf) {
            GlobalDofFront = serializer.F32(GlobalDofFront);
            GlobalDofBack = serializer.F32(GlobalDofBack);
            GlobalDofSackTrack = serializer.F32(GlobalDofSackTrack);
        }

        if (subVersion >= 0xda)
            EnableSackpocket = serializer.Bool(EnableSackpocket);

        if (subVersion >= 0x170)
            ShowQuestLog = serializer.Bool(ShowQuestLog);

        if (subVersion >= 0x11d)
            ScoreboardUnlockLevelSlot = serializer.Struct(ScoreboardUnlockLevelSlot);

        if (subVersion >= 0x154)
            ProgressBoardLevelLinkStartPoint = serializer.Wstr(ProgressBoardLevelLinkStartPoint);

        if (subVersion >= 0x15e)
            IsLbp3World = serializer.Bool(IsLbp3World);
    }

    public int GetAllocatedSize() { return 0; }
    
}