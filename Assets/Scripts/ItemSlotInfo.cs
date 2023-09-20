
// 物品槽信息类，用于存储每个物品槽的信息
[System.Serializable]
public class ItemSlotInfo
{
    public Item item;// 物品对象
    public string name;// 物品名称
    public int stacks;// 堆叠数量
    public ItemSlotInfo(Item newItem, int newstacks)
    {
        item = newItem;
        stacks = newstacks;
    }
}

