using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ItemPanel : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler, IDropHandler
{
    public Inventory inventory; // 背包脚本引用
    public ItemSlotInfo itemSlot; // 物品槽信息
    public Image itemImage; // 物品图像组件
    public TextMeshProUGUI stacksText; // 堆叠数量文本组件
    private bool click; // 当前是否点击
    private Mouse mouse; // 鼠标

    // 当鼠标进入时调用的方法
    public void OnPointerEnter(PointerEventData eventData)
    {
        eventData.pointerPress = this.gameObject;
    }

    // 当鼠标按下时调用的方法
    public void OnPointerDown(PointerEventData eventData)
    {
        click = true;
    }

    // 当鼠标抬起时调用的方法
    public void OnPointerUp(PointerEventData eventData)
    {
        if (click)
        {
            OnClick();
            click = false;
        }
    }

    // 在拖拽结束时调用
    public void OnDrop(PointerEventData eventData)
    {
        OnClick();
        click = false;
    }
    // 在拖拽过程中连续调用
    public void OnDrag(PointerEventData eventData)
    {
        if (click)
        {
            OnClick();
            click = false;
            OnDrop(eventData);
        }
    }

    // 将物品拾取到鼠标槽位
    public void PickupItem()
    {
        mouse.itemSlot = itemSlot;
        mouse.sourceItemPanel = this;
        if (Input.GetKey(KeyCode.LeftShift) && itemSlot.stacks > 1)
        {
            mouse.splitSize = itemSlot.stacks / 2;
        }
        else
        {
            mouse.splitSize = itemSlot.stacks;
        }
        mouse.SetUI();
    }

    // 物品面板淡出效果
    public void Fadeout()
    {
        itemImage.CrossFadeAlpha(0.3f, 0.05f, true);//0.05 秒itemImage透明度渐变到0.3
    }

    // 将物品放下到当前物品面板上
    public void DropItem()
    {
        itemSlot.item = mouse.itemSlot.item;
        if (mouse.splitSize < mouse.itemSlot.stacks)
        {
            itemSlot.stacks = mouse.splitSize;
            mouse.itemSlot.stacks -= mouse.splitSize;
            mouse.EmptySlot();
        }
        else
        {
            itemSlot.stacks = mouse.itemSlot.stacks;
            inventory.ClearSlot(mouse.itemSlot);
        }
    }

    // 交换两个物品槽位的物品
    public void SwapItem(ItemSlotInfo slotA, ItemSlotInfo slotB)
    {
        // 暂存槽位A的物品信息
        ItemSlotInfo tempItem = new ItemSlotInfo(slotA.item, slotA.stacks);
        // 将槽位B的物品信息赋值给槽位A
        slotA.item = slotB.item;
        slotA.stacks = slotB.stacks;
        // 将暂存的物品信息赋值给槽位B
        slotB.item = tempItem.item;
        slotB.stacks = tempItem.stacks;
    }

    //物品堆叠
    public void StackItem(ItemSlotInfo source, ItemSlotInfo destination, int amount)
    {
        // 计算目标物品槽中可用的堆叠空间
        int slotsAvailable = destination.item.MaxStacks() - destination.stacks;
        
        // 如果目标物品槽没有可用的堆叠空间，则直接返回
        if (slotsAvailable == 0) return;

        if (amount > slotsAvailable)
        {
            // 堆叠数量超过可用空间时，从源物品槽中减去可用空间
            source.stacks -= slotsAvailable;
            // 目标物品槽的堆叠数量设置为最大堆叠数
            destination.stacks = destination.item.MaxStacks();
        }
        if (amount <= slotsAvailable)
        {
            // 堆叠数量小于可用空间时，将堆叠数量加到目标物品槽中
            destination.stacks += amount;
            // 如果源物品槽中剩余的堆叠数量等于堆叠数量（即所有物品都被堆叠完），则清空源物品槽
            if (source.stacks == amount) inventory.ClearSlot(source);
            // 否则，从源物品槽中减去堆叠数量
            else source.stacks -= amount;  
        }
    }


    // 当物品面板被点击时调用的方法
    void OnClick()
    {
        if (inventory != null)
        {
            mouse = inventory.mouse;
            // 如果鼠标槽位为空，将物品拾取到鼠标槽位
            if (mouse.itemSlot.item == null)
            {
                if (itemSlot.item != null)
                {
                    PickupItem();
                    Fadeout();
                }
            }
            else
            {
                // 点击的是原始槽位
                if (itemSlot == mouse.itemSlot)
                {
                    inventory.RefreshInventory();
                }
                // 点击的是空槽位
                else if (itemSlot.item == null)
                {
                    DropItem();
                    inventory.RefreshInventory();
                }
                // 点击的是不同物品类型的已占用槽位
                else if (itemSlot.item.GiveName() != mouse.itemSlot.item.GiveName())
                {
                    SwapItem(itemSlot, mouse.itemSlot);
                    inventory.RefreshInventory();
                }
                // 点击的是同物品类型的已占用槽位
                else if (itemSlot.stacks < itemSlot.item.MaxStacks()){
                    StackItem(mouse.itemSlot, itemSlot, mouse.splitSize);
                    inventory.RefreshInventory();
                }
            }
        }
    }
}
