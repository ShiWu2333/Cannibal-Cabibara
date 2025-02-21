using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public bool isPickedUp = false; // 是否被捡起
    public string itemName; // 物品名称
    public int itemID; // 物品唯一ID
    public Sprite itemIcon; // 物品图标（用于UI）
    public bool canBePickedUp = true; // 是否可以被拾取
}
