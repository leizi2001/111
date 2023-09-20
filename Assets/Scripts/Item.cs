using UnityEngine;
//物品抽象类，所有物品类型都需要继承此类
[System.Serializable]
public abstract class Item
{
    public abstract string GiveName();// 获取物品名称
    public virtual int MaxStacks()// 获取每个物品槽的最大堆叠数量
    {
        return 30;// 默认为 30
    }
    public virtual Sprite GiveItemImage()// 获取物品图片
    {
        return Resources.Load<Sprite>("UI/Item Images/No Item Image Icon");// 默认图片
    }
}

