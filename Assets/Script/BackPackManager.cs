using System.Collections.Generic;
using UnityEngine;

// ���ڶ���ÿ������Ƭ�ε����ݽṹ
[System.Serializable]
public class MemoryFragment
{
    public int memoryId;                  // ����Ƭ��ID�����磺1 ~ 7��
    public List<int> requiredItemIDs;     // �����ü���Ƭ������ĵ���ID�б�
    public bool isUnlocked;               // �Ƿ��ѽ���
}

public class BackPackManager : MonoBehaviour
{
    public static BackPackManager Instance { get; private set; }

    // �洢������ռ������������ߣ����� Item �ű�ʵ���洢
    public List<Item> collectedItems = new List<Item>();

    // ���� 7 ������Ƭ�Σ����� Inspector �б༭ÿ��Ƭ����Ҫ�ĵ���ID
    public List<MemoryFragment> memoryFragments = new List<MemoryFragment>();

    // ����֪ͨ UI ���µ��¼�
    public delegate void OnBackPackUpdated();
    public event OnBackPackUpdated OnBackPackChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// ���һ�����ߵ�������������Ƿ��м���Ƭ�ο��Խ���
    /// </summary>
    public void AddItem(Item item)
    {
        // �����ظ��ռ�������Ƿ��Ѿ�����ͬ���� itemID
        if (collectedItems.Exists(i => i.itemID == item.itemID))
        {
            Debug.Log($"���� {item.itemName} �Ѿ����ռ�����");
            return;
        }

        collectedItems.Add(item);
        item.isPickedUp = true;
        Debug.Log($"�ռ���������{item.itemName}");

        // ���ÿ������Ƭ���Ƿ������������
        CheckMemoryFragmentsUnlock();

        OnBackPackChanged?.Invoke(); // ֪ͨ UI ����
    }

    /// <summary>
    /// ������м���Ƭ�εĽ�������
    /// </summary>
    private void CheckMemoryFragmentsUnlock()
    {
        foreach (MemoryFragment fragment in memoryFragments)
        {
            // ����ü���Ƭ���Ѿ�������ֱ������
            if (fragment.isUnlocked)
                continue;

            bool allCollected = true;
            // ���ü���Ƭ�������ÿ�������Ƿ����ռ�
            foreach (int requiredId in fragment.requiredItemIDs)
            {
                if (!collectedItems.Exists(i => i.itemID == requiredId))
                {
                    allCollected = false;
                    break;
                }
            }

            // ������������߶��ռ��󣬱�Ǽ���Ƭ�ν���
            if (allCollected)
            {
                fragment.isUnlocked = true;
                Debug.Log($"����Ƭ�� {fragment.memoryId} �ѽ�����");
                // �˴����Ե��ô��������Ľӿڣ����� TriggerMemoryAnimation(fragment.memoryId);
            }
        }
    }

    /// <summary>
    /// �ж�ָ������Ƭ���Ƿ����
    /// </summary>
    public bool IsMemoryFragmentUnlocked(int memoryId)
    {
        MemoryFragment fragment = memoryFragments.Find(f => f.memoryId == memoryId);
        return fragment != null && fragment.isUnlocked;
    }

    /// <summary>
    /// ��ȡ��ǰ�ռ������е���
    /// </summary>
    public List<Item> GetCollectedItems()
    {
        return new List<Item>(collectedItems);
    }
}
