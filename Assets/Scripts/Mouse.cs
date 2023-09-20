using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Mouse : MonoBehaviour
{
    public GameObject mouseItemUI; // 鼠标上的物品UI
    public Image mouseCursor; // 鼠标光标
    public ItemSlotInfo itemSlot; // 物品槽信息
    public Image itemImage; // 物品图像
    public TextMeshProUGUI stacksText; // 叠加数量文本

    public ItemPanel sourceItemPanel;// 源物品面板对象
    public int splitSize;// 拆分数量

    void Update()
    {
        // 将鼠标位置设置为当前鼠标位置
        transform.position = Input.mousePosition;

        // 如果鼠标被锁定
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            mouseCursor.enabled = false; // 隐藏鼠标光标
            mouseItemUI.SetActive(false); // 隐藏鼠标上的物品UI
        }
        else
        {
            mouseCursor.enabled = true; // 显示鼠标光标

            // 如果物品槽中有物品
            if (itemSlot.item != null)
            {
                mouseItemUI.SetActive(true); // 显示鼠标上的物品UI
            }
            else
            {
                mouseItemUI.SetActive(false); // 隐藏鼠标上的物品UI
            }
            if (itemSlot.item != null)// 如果物品槽中有物品
            {
                if (Input.GetAxis("Mouse ScrollWheel") > 0 && splitSize < itemSlot.stacks)
                {
                    // 当鼠标向上滚动并且拆分数量小于物品槽剩余堆叠数量时
                    splitSize++;// 增加拆分数量
                }
                if (Input.GetAxis("Mouse ScrollWheel") < 0 && splitSize > 1)
                {
                    // 当鼠标向下滚动并且拆分数量大于1时
                    splitSize--;// 减少拆分数量
                }
                stacksText.text = "" + splitSize;// 在UI中显示拆分数量
                if (splitSize == itemSlot.stacks)// 如果拆分数量等于物品的堆叠数量
                {
                    // 将源物品面板的堆叠数量文本组件设置为不可见
                    sourceItemPanel.stacksText.gameObject.SetActive(false);
                }
                else
                {
                    sourceItemPanel.stacksText.gameObject.SetActive(true);
                    // 在文本组件中显示物品的剩余堆叠数量
                    sourceItemPanel.stacksText.text = "" + (itemSlot.stacks - splitSize);
                }
            }

        }
    }
    public void SetUI()
    {
        // stacksText.text = "" + itemSlot.stacks; // 设置叠加数量文本
        stacksText.text = "" + splitSize;// 在UI中显示拆分数量
        itemImage.sprite = itemSlot.item.GiveItemImage(); // 设置物品图像
    }
    public void EmptySlot()
    {
        itemSlot = new ItemSlotInfo(null, 0);// 清空物品槽
    }
}
