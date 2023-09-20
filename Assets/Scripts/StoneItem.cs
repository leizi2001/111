using UnityEngine;
public class StoneItem : Item
{
    public override string GiveName()// 获取物品名称
    {
        return "Stone";
    }
    public override int MaxStacks()// 获取每个物品槽的最大堆叠数量
    {
        return 5;
    }
    public override Sprite GiveItemImage()// 获取物品图片
    {
        return Resources.Load<Sprite>("UI/Item Images/Stone Icon");
    }
}

