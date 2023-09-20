using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Reflection;
using UnityEngine.EventSystems;

public class Inventory : MonoBehaviour
{
    [SerializeReference] public List<ItemSlotInfo> items = new List<ItemSlotInfo>();// 物品列表
    [Space]
    [Header("库存菜单组件")]
    public GameObject inventoryMenu;// 背包菜单
    public GameObject itemPanel;// 物品列表容器
    public GameObject itemPanelGrid;// 物品列表网格布局
    public Mouse mouse;
    public GameObject dropObject;//丢弃物品预制体

    private List<ItemPanel> existingPanels = new List<ItemPanel>();//物品容器列表
    // 创建一个用于存储所有物品的字典，其中键为物品名称，值为对应的物品对象
    Dictionary<string, Item> allItemsDictionary = new Dictionary<string, Item>();

    [Space]
    public int inventorySize = 60;// 背包容量大小
    void Start()
    {
        // 初始化物品列表并赋值为 null
        for (int i = 0; i < inventorySize; i++)
        {
            items.Add(new ItemSlotInfo(null, 0));
        }

        // 获取所有可用的物品，并将它们添加到物品字典中
        List<Item> allItems = GetAllItems().ToList();
        string itemsInDictionary = "字典条目:";
        foreach (Item i in allItems)
        {
            if (!allItemsDictionary.ContainsKey(i.GiveName()))
            {

                allItemsDictionary.Add(i.GiveName(), i);
                itemsInDictionary += "," + i.GiveName();
            }
            else
            {
                // 如果字典中已存在同名的物品，则输出调试信息
                Debug.Log("" + i + "已存在于与之共享名称的字典中 " + allItemsDictionary[i.GiveName()]);
            }

        }

        itemsInDictionary += ".";
        Debug.Log(itemsInDictionary);

        // 添加一些初始物品
        AddItem("Wood", 40);
        AddItem("Stone", 40);
    }

    // 获取所有可用的物品
    // IEnumerable<Item> GetAllItems()
    // {
    //     return System.AppDomain.CurrentDomain.GetAssemblies()
    //         .SelectMany(assembly => assembly.GetTypes()).Where(type => type.IsSubclassOf(typeof(Item)))
    //         .Select(type => System.Activator.CreateInstance(type) as Item);
    // }
    IEnumerable<Item> GetAllItems()
    {
        List<Item> allItems = new List<Item>();

        // 获取当前应用程序域中的所有程序集
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

        // 遍历每个程序集
        foreach (Assembly assembly in assemblies)
        {
            // 获取程序集中定义的所有类型
            Type[] types = assembly.GetTypes();

            // 遍历每个类型
            foreach (Type type in types)
            {
                // 检查类型是否是 Item 类的子类
                if (type.IsSubclassOf(typeof(Item)))
                {
                    // 创建该类型的实例，并将其转换为 Item 对象
                    Item item = Activator.CreateInstance(type) as Item;

                    // 将物品添加到列表中
                    allItems.Add(item);
                }
            }
        }

        return allItems;
    }


    void Update()
    {
        // 按下 Tab 键显示或隐藏背包界面
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (inventoryMenu.activeSelf)
            {
                inventoryMenu.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;// 隐藏光标并锁定在屏幕中心
                mouse.EmptySlot();
            }
            else
            {
                inventoryMenu.SetActive(true);
                Cursor.lockState = CursorLockMode.Confined;// 显示光标并限制在屏幕内部移动
                RefreshInventory();
            }
        }
        //拖拽物品时按鼠标右键还原物品
        if (Input.GetKeyDown(KeyCode.Mouse1) && mouse.itemSlot.item != null)
        {
            RefreshInventory();
        }
        //控制丢弃物品 EventSystem.current.IsPointerOverGameObject()：该条件判断鼠标当前是否位于UI元素之上
        if (Input.GetKeyDown(KeyCode.Mouse0) && mouse.itemSlot.item != null && !EventSystem.current.IsPointerOverGameObject())
        {
            DropItem(mouse.itemSlot.item.GiveName());
        }
    }
    //刷新背包
    public void RefreshInventory()
    {
        //物品容器列表
        existingPanels = itemPanelGrid.GetComponentsInChildren<ItemPanel>().ToList();
        //如果物品列表容器不足，创建物品面板
        if (existingPanels.Count < inventorySize)
        {
            int amountToCreate = inventorySize - existingPanels.Count;
            for (int i = 0; i < amountToCreate; i++)
            {
                GameObject newPanel = Instantiate(itemPanel, itemPanelGrid.transform);
                existingPanels.Add(newPanel.GetComponent<ItemPanel>());
            }
        }
        int index = 0;
        foreach (ItemSlotInfo i in items)
        {
            //给物品列表元素命名
            i.name = "" + (index + 1);
            if (i.item != null) i.name += ":" + i.item.GiveName();
            else i.name += ":-";
            //更新物品面板
            ItemPanel panel = existingPanels[index];
            panel.name = i.name + " Panel";
            if (panel != null)
            {
                panel.inventory = this;
                panel.itemSlot = i;
                if (i.item != null)
                {
                    panel.itemImage.gameObject.SetActive(true);  // 显示物品图标
                    panel.itemImage.sprite = i.item.GiveItemImage();  // 设置物品图标的精灵
                    panel.itemImage.CrossFadeAlpha(1, 0.05f, true);//0.05 秒itemImage透明度渐变到1完全不透明
                    panel.stacksText.gameObject.SetActive(true);  // 显示物品叠加数量
                    panel.stacksText.text = "" + i.stacks;  // 设置物品叠加数量的文本
                }
                else
                {
                    panel.itemImage.gameObject.SetActive(false);  // 隐藏物品图标
                    panel.stacksText.gameObject.SetActive(false);  // 隐藏物品叠加数量
                }
            }
            index++;
        }
        mouse.EmptySlot();
    }

    //添加物品
    public int AddItem(string itemName, int amount)
    {
        //查找要添加的项目
        Item item = null;
        allItemsDictionary.TryGetValue(itemName, out item);
        //如果未找到任何项，则退出方法
        if (item == null)
        {
            Debug.Log("无法在字典中找到要添加到库存中的物品");
            return amount;
        }

        // 检查已有物品槽中是否有空余位置
        foreach (ItemSlotInfo i in items)
        {
            if (i.item != null)
            {
                if (i.item.GiveName() == item.GiveName())
                {
                    if (amount > i.item.MaxStacks() - i.stacks)
                    {
                        amount -= i.item.MaxStacks() - i.stacks;
                        i.stacks = i.item.MaxStacks();
                    }

                    else
                    {
                        i.stacks += amount;
                        //如果背包菜单处于激活状态，刷新背包显示
                        if (inventoryMenu.activeSelf) RefreshInventory();
                        return 0;
                    }

                }
            }

        }

        //将剩余的物品放入空的物品槽中
        foreach (ItemSlotInfo i in items)
        {
            if (i.item == null)
            {
                if (amount > item.MaxStacks())
                {
                    i.item = item;
                    i.stacks = item.MaxStacks();
                    amount -= item.MaxStacks();
                }
                else
                {
                    i.item = item;
                    i.stacks = amount;
                    //如果背包菜单处于激活状态，刷新背包显示
                    if (inventoryMenu.activeSelf) RefreshInventory();
                    return 0;
                }

            }

        }
        Debug.Log("库存中没有空间:" + item.GiveName());
        //如果背包菜单处于激活状态，刷新背包显示
        if (inventoryMenu.activeSelf) RefreshInventory();
        return amount;
    }

    //清空指定物品槽中的物品和叠加数量
    public void ClearSlot(ItemSlotInfo slot)
    {
        slot.item = null;
        slot.stacks = 0;
    }

    //丢弃物品
    public void DropItem(string itemName)
    {
        Item item = null;
        // 从字典中查找物品
        allItemsDictionary.TryGetValue(itemName, out item);
        if (item == null)
        {
            Debug.Log("在字典中找不到要添加到掉落的物品");
            return;
        }

        // 在当前位置实例化一个掉落物体
        GameObject droppedItem = Instantiate(dropObject, transform.position, Quaternion.identity);

        //修改图片
        droppedItem.GetComponent<SpriteRenderer>().sprite = item.GiveItemImage();

        ItemPickup ip = droppedItem.GetComponent<ItemPickup>();

        if (ip != null)
        {
            // 设置掉落物品的属性
            ip.itemToDrop = itemName;
            ip.amount = mouse.splitSize;
            mouse.itemSlot.stacks -= mouse.splitSize;//更新物品槽中该物品的剩余数量，及减去将要丢弃的物品数量
        }

        if (mouse.itemSlot.stacks < 1) ClearSlot(mouse.itemSlot);// 清空物品槽
        
        mouse.EmptySlot();// 清空鼠标上的物品
        RefreshInventory();// 刷新背包显示
    }

}
