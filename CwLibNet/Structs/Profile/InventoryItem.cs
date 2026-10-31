using CwLibNet.Enums;
using CwLibNet.EX;
using CwLibNet.IO;
using CwLibNet.IO.Serializer;
using CwLibNet.Structs.Inventory;
using CwLibNet.Types.Data;

namespace CwLibNet.Structs.Profile;

public class InventoryItem: ISerializable
{
    public const int BaseAllocationSize = 0x40;

    /// <summary>
    /// The reference to the actual plan data used by this item.
    /// </summary>
    public ResourceDescriptor? Plan;

    /// <summary>
    /// Cache for inventory details of this item.
    /// </summary>
    public InventoryItemDetails Details = new();

    /// <summary>
    /// Cache of GUID for this item.
    /// </summary>
    
    public GUID? Guid;

    /// <summary>
    /// Unique ID of this item in your inventory(?)
    /// </summary>
    public int Uid;

    /// <summary>
    /// Tutorial related data, see ETutorialLevels
    /// </summary>
    
    public TutorialLevel TutorialLevel = TutorialLevel.UNKNOWN;
    
    public TutorialLevel TutorialVideo = TutorialLevel.UNKNOWN;

    /// <summary>
    /// State of the item in your inventory,
    /// whether it's hearted, uploaded, etc
    /// </summary>
    public int Flags = InventoryItemFlags.NONE;

    /// <summary>
    /// Index of user defined category in the string table.
    /// </summary>
    
    public int UserCategoryIndex;


    public InventoryItem() { }

    public InventoryItem(int uid, ResourceDescriptor? descriptor, InventoryItemDetails? details)
    {
        details ??= new InventoryItemDetails();

        details.DateAdded = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();

        Uid = uid;
        Details = details;
        Plan = descriptor;
    }

    
    public void Serialize(Serializer serializer)
    {
        Plan = serializer.Resource(Plan, ResourceType.Plan, true);
        if (Plan != null)
            serializer.AddDependency(Plan);

        if (serializer.GetRevision().GetSubVersion() >= (int)Revisions.ITEM_GUID)
            Guid = serializer.Guid(Guid);

        Details = serializer.Struct(Details);

        var version = serializer.GetRevision().GetVersion();

        if (version >= (int)Revisions.ITEM_FLAGS)
        {
            Uid = serializer.I32(Uid, true);
            if (version < (int)Revisions.REMOVE_LBP1_TUTORIALS)
            {
                if (serializer.IsWriting())
                {
                    serializer.GetOutput().I32((int)TutorialLevel, true);
                    serializer.GetOutput().I32((int)TutorialVideo, true);
                }
                else
                {
                    TutorialLevel =
                        (TutorialLevel)serializer.GetInput().I32(true);
                    TutorialVideo =
                        (TutorialLevel)serializer.GetInput().I32(true);
                }
            }
            Flags = serializer.I32(Flags, true);
            if (version >= (int)Revisions.USER_CATEGORIES)
                UserCategoryIndex = serializer.I32(UserCategoryIndex, true);
        }
        else throw new SerializationException("InventoryItem's below r565 are not supported!");
    }

    
    public int GetAllocatedSize()
    {
        return BaseAllocationSize + Details.GetAllocatedSize();
    }


}
