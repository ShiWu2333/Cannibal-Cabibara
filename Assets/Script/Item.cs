using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public bool isPickedUp = false; // �Ƿ񱻼���
    public string itemName; // ��Ʒ����
    public int itemID; // ��ƷΨһID
    public Sprite itemIcon; // ��Ʒͼ�꣨����UI��
    public bool canBePickedUp = true; // �Ƿ���Ա�ʰȡ
}
