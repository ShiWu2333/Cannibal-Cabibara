using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class ItemIDToImage
{
    public int itemID;      // ��UI��Ӧ�ĵ���ID
    public Image imageSlot; // ������ʾ���ɫ��UI Image
}

[System.Serializable]
public class MemoryIDToImage
{
    public int memoryID;    // ����Ƭ��ID����Ӧ BackPackManager.memoryFragments �е� memoryId��
    public Image imageSlot; // ������ʾ���ɫ��UI Image
}

public class BackPackUIManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject backPackPanel; // ����������壬�����л���ʾ
    [Tooltip("�ֶ�ָ��ÿ������ID��Ӧ��UI Image��")]
    public List<ItemIDToImage> itemIDToImages; // �洢������ID -> UI���ӡ��Ķ�Ӧ��ϵ
    [Tooltip("�ֶ�ָ��ÿ������Ƭ��ID��Ӧ��UI Image��")]
    public List<MemoryIDToImage> memoryIDToImages; // �洢������Ƭ��ID -> UI���ӡ��Ķ�Ӧ��ϵ



    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        if (BackPackManager.Instance != null)
        {
            BackPackManager.Instance.OnBackPackChanged -= UpdateBackPackUI;
        }
    }

    private void Start()
    {
        if (BackPackManager.Instance != null)
        {
            BackPackManager.Instance.OnBackPackChanged += UpdateBackPackUI;
            Debug.Log("BackPackUIManager ������ OnBackPackChanged �¼�");
        }
        else
        {
            Debug.Log("BackPackUManager is not found");
        }
        // ����ʱ���ز˵�
        backPackPanel.SetActive(false);
        // ����ʱ����һ�� UI
        UpdateBackPackUI();
    }

    /// <summary>
    /// ���� BackPackManager ���ռ��ĵ����Լ�����Ƭ�ν���״̬������ UI ��ʾ
    /// </summary>
    public void UpdateBackPackUI()
    {
        // ���û�б�����������ֱ�ӷ���
        if (BackPackManager.Instance == null) return;

        // 1. ���µ��߲��֣��Ƚ����е��� UI ����Ϊ��ɫ��δ�ռ���
        foreach (var mapping in itemIDToImages)
        {
            if (mapping.imageSlot != null)
                mapping.imageSlot.color = Color.gray;
        }

        // �ٱ������ռ��ĵ��ߣ�����Ӧ�� UI �����ɫ
        var collectedItems = BackPackManager.Instance.GetCollectedItems();
        foreach (var item in collectedItems)
        {
            var mapping = itemIDToImages.FirstOrDefault(m => m.itemID == item.itemID);
            if (mapping != null && mapping.imageSlot != null)
            {
                mapping.imageSlot.color = Color.green;
            }
        }

        // 2. ���»���Ƭ�β��֣�������ӳ��� UI �����Ƿ����������ɫ
        foreach (var memoryMapping in memoryIDToImages)
        {
            if (memoryMapping.imageSlot != null)
            {
                // ���� BackPackManager �� IsMemoryFragmentUnlocked �ж�ָ������Ƭ���Ƿ����
                bool unlocked = BackPackManager.Instance.IsMemoryFragmentUnlocked(memoryMapping.memoryID);
                memoryMapping.imageSlot.color = unlocked ? Color.green : Color.gray;
            }
        }
    }

    /// <summary>
    /// �л����� UI ��ʾ������
    /// </summary>
    public void ToggleBackPackUI()
    {
        bool isActive = backPackPanel.activeSelf;
        backPackPanel.SetActive(!isActive);
    }
}
