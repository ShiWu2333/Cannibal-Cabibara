using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ItemInspection : MonoBehaviour
{
    public GameObject inspectionPanel; // 检视窗口的 UI Panel
    public RawImage itemDisplay; // 用于显示 3D 模型的 RawImage
    public Camera itemCamera; // 用于渲染 3D 模型的相机
    public RenderTexture itemRenderTexture; // Render Texture
    public Button closeButton;

    private GameObject currentItemModel; // 当前选中的物品模型
    private GameObject inspectedModel; // 当前检视的模型副本
    public bool isInspecting = false; // 是否正在检视
    public bool canToggle = true; // **添加变量来防止同一帧触发两次**
    private Vector3 lastMousePosition; // 上一帧的鼠标位置

    public GameObject backpackIcon; //背包icon
    public GameObject inspectionIcon; //检视icon
    public GameObject interactionIcon; //抓取icon

    private Item currentInspectItem; // ✅ 当前正在检视的物品



    private void Start()
    {
        // 初始隐藏检视窗口
        inspectionPanel.SetActive(false);
        itemCamera.gameObject.SetActive(false);

        // 设置 Camera 的 Target Texture
        itemCamera.targetTexture = itemRenderTexture;

        // 设置 RawImage 的 Texture
        itemDisplay.texture = itemRenderTexture;

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseInspection);
        }
    }

    private void Update()
    {
        // 如果正在检视，允许旋转模型
        if (isInspecting)
        {
            RotateItemModel();
        }

        if (isInspecting && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseInspection();
            Debug.Log("Q has been pressed in ItemInspection - Closing");
        }

        // 按下 Q 键关闭检视窗口
        /*if (isInspecting && Input.GetKeyDown(KeyCode.Q))
         {
             CloseInspection();
             Debug.Log("Q has been pressed in ItemInspection - Closing");
         } */
    }

    // 打开检视窗口
    public void ToggleInspection()
    {
        if (!canToggle) return; // **如果刚刚触发过，不允许再次触发**
        canToggle = false; // **短暂禁用 Q，防止同时触发打开 & 关闭**

        //isInspecting = !isInspecting;

        Debug.Log("🔄 ToggleInspection 被调用 | isInspecting: " + isInspecting);

        if (isInspecting)
        {
            Debug.Log("❌ 关闭检视模式 - 调用 CloseInspection()");
            CloseInspection();
        }
        else
        {
            Debug.Log("✅ 打开检视模式 - 调用 OpenInspection()");
            OpenInspection();
        }

        StartCoroutine(ResetToggleCooldown()); // **启动冷却时间**
    }

    private IEnumerator ResetToggleCooldown()
    {
        yield return new WaitForSeconds(0.1f); // **等待 0.1 秒，防止同一帧再次触发**
        canToggle = true;
    }

    // 打开检视窗口
    private void OpenInspection()
    {
        Debug.Log("打开面板了");
        isInspecting = true;
        if (currentItemModel == null) return;

        // 复制当前选中的物品模型
        inspectedModel = Instantiate(currentItemModel);

        // 禁用复制的模型的 Rigidbody
        Rigidbody rb = inspectedModel.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // 禁用物理模拟
        }

        // 递归禁用复制的模型及其子对象的 Outline 组件
        DisableOutlineRecursively(inspectedModel);

        // 计算模型的中心点
        Renderer renderer = inspectedModel.GetComponent<Renderer>();
        if (renderer != null)
        {
            Vector3 center = renderer.bounds.center; // 获取模型的几何中心
            inspectedModel.transform.position = itemCamera.transform.position + itemCamera.transform.forward * 2f - center + inspectedModel.transform.position;
        }

        // **🔹 隐藏 UI**
        backpackIcon.SetActive(false);
        inspectionIcon.SetActive(false);
        interactionIcon.SetActive(false);

        // 显示检视窗口
        inspectionPanel.SetActive(true);
        itemCamera.gameObject.SetActive(true);

        
    }

    // 关闭检视窗口
    public void CloseInspection()
    {
        Debug.Log("面板关闭");
        isInspecting = false; // **先设置 isInspecting，确保 UI 逻辑正确**

        // 隐藏检视窗口
        inspectionPanel.SetActive(false);
        itemCamera.gameObject.SetActive(false);

        // ✅ 重新显示 UI
        backpackIcon.SetActive(true);
        inspectionIcon.SetActive(true);
        interactionIcon.SetActive(true);

        // ✅ **销毁 `inspectedModel`**
        if (inspectedModel != null)
        {
            Debug.Log("销毁检视模型: " + inspectedModel.name);
            Destroy(inspectedModel);
            inspectedModel = null; // **防止引用残留**
        }
        else
        {
            Debug.LogWarning("❌ 没有需要销毁的 `inspectedModel`");
        }

        // ✅ **恢复物品交互状态**
        if (currentItemModel != null)
        {
            Item item = currentItemModel.GetComponent<Item>();
            if (item != null)
            {
                int itemID = item.itemID;

                // ✅ **确保 `isPickedUp` 没有被错误修改**
                item.isPickedUp = false;
                Debug.Log($"🔄 物品 {item.itemName} 仍然可拾取 isPickedUp: {item.isPickedUp}");

                // ✅ **确保 `Collider` 仍然启用**
                Collider itemCollider = item.GetComponent<Collider>();
                if (itemCollider != null && !itemCollider.enabled)
                {
                    itemCollider.enabled = true;
                    Debug.Log($"✅ `Collider` 已重新启用: {item.itemName}");
                }

                // **只有收集到的物品才播放音效 & 背包动画**
                if (BackPackManager.Instance != null && BackPackManager.Instance.IsCollectedItem(itemID))
                {
                    Debug.Log($"🎯 物品 {itemID} 在背包中，播放解锁音效 & 背包动画！");

                    BackpackIconBlink blinkScript = backpackIcon.GetComponent<BackpackIconBlink>();
                    if (blinkScript != null)
                    {
                        blinkScript.StartBlinking();
                    }
                }
                else
                {
                    Debug.Log($"🚫 物品 {itemID} 不在背包中，不触发音效 & 背包动画");
                }

                // ✅ **取消选中 & 恢复高亮状态**
                item.Deselect();
                item.UpdateHighlightState();
            }
        }
    }

    // 旋转物品模型
    private void RotateItemModel()
    {
        if (Input.GetMouseButton(0)) // 按住鼠标左键
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;

            // 获取模型的几何中心
            Renderer renderer = inspectedModel.GetComponent<Renderer>();
            if (renderer != null)
            {
                Vector3 center = renderer.bounds.center; // 动态计算几何中心

                // 围绕模型的几何中心旋转
                inspectedModel.transform.RotateAround(center, Vector3.up, -delta.x * 0.2f); // 水平旋转
                inspectedModel.transform.RotateAround(center, Vector3.right, delta.y * 0.2f); // 垂直旋转
            }
        }

        lastMousePosition = Input.mousePosition;
    }

    // 递归禁用 Outline 组件
    private void DisableOutlineRecursively(GameObject obj)
    {
        // 禁用当前对象的 Outline 组件
        Outline outline = obj.GetComponent<Outline>();
        if (outline != null)
        {
            outline.enabled = false; // 禁用 Outline
            // Destroy(outline); // 或者直接移除 Outline 组件
        }

        // 递归禁用所有子对象的 Outline 组件
        foreach (Transform child in obj.transform)
        {
            DisableOutlineRecursively(child.gameObject);
        }
    }

    // 设置当前选中的物品模型
    public void SetItemModel(GameObject itemModel)
    {
        if (itemModel != null)
        {
            currentItemModel = itemModel; // ✅ 记录 3D 物体
            currentInspectItem = itemModel.GetComponent<Item>(); // ✅ 获取 `Item` 组件
        }
    }
}