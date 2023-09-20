using UnityEngine;
using TMPro;

//物品拾取脚本
public class ItemPickup : MonoBehaviour
{
    public string itemToDrop;   // 需要拾取的物品名称
    public int amount = 1;      // 物品数量，默认为1
    private TextMeshPro text;   //物品文本
    Inventory playerInventory;  //玩家的背包组件

    void Start()
    {
        text = transform.GetComponentInChildren<TextMeshPro>();
        text.text = amount.ToString();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            PickUpItem();
        }
    }

    // 当碰撞体进入触发器时调用
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            // 获取玩家的背包组件
            playerInventory = other.GetComponent<Inventory>();
        }
    }

    //当碰撞体离开触发器时调用
    private void OnTriggerExit2D(Collider2D other){
        if (other.tag == "Player")
        {
            // 获取玩家的背包组件
            playerInventory = null;
        }
    }

    // 拾取物品的方法
    public void PickUpItem()
    {
        // 如果玩家背包组件存在，则拾取物品
        if (playerInventory == null) return;

        // 将物品添加到背包，并返回剩余的物品数量
        amount = playerInventory.AddItem(itemToDrop, amount);

        // 如果数量小于1，销毁该拾取物品的游戏对象
        if (amount < 1)
        {
            Destroy(this.gameObject);
        }
        else
        {
            //更新text
            text.text = amount.ToString();
        }
    }

}
