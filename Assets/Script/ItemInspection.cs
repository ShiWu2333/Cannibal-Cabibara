using UnityEngine;
using UnityEngine.UI;

public class ItemInspection : MonoBehaviour
{
    public GameObject inspectionPanel; // 检视窗口的 UI Panel
    public RawImage itemDisplay; // 用于显示 3D 模型的 RawImage
    public Camera itemCamera; // 用于渲染 3D 模型的相机
    public RenderTexture itemRenderTexture; // Render Texture

    private GameObject currentItemModel; // 当前选中的物品模型
    private GameObject inspectedModel; // 当前检视的模型副本
    private bool isInspecting = false; // 是否正在检视
    private Vector3 lastMousePosition; // 上一帧的鼠标位置

    private void Start()
    {
        // 初始隐藏检视窗口
        inspectionPanel.SetActive(false);
        itemCamera.gameObject.SetActive(false);

        // 设置 Camera 的 Target Texture
        itemCamera.targetTexture = itemRenderTexture;

        // 设置 RawImage 的 Texture
        itemDisplay.texture = itemRenderTexture;
    }

    private void Update()
    {
        // 如果正在检视，允许旋转模型
        if (isInspecting)
        {
            RotateItemModel();
        }

        // 按下 Esc 键关闭检视窗口
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseInspection();
        }
    }

    // 打开检视窗口
    public void ToggleInspection()
    {
        if (isInspecting)
        {
            CloseInspection();
        }
        else
        {
            OpenInspection();
        }
    }

    // 打开检视窗口
    private void OpenInspection()
    {
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

        // 显示检视窗口
        inspectionPanel.SetActive(true);
        itemCamera.gameObject.SetActive(true);

        isInspecting = true;
    }

    // 关闭检视窗口
    private void CloseInspection()
    {
        // 隐藏检视窗口
        inspectionPanel.SetActive(false);
        itemCamera.gameObject.SetActive(false);

        // 销毁复制的模型
        if (inspectedModel != null)
        {
            Destroy(inspectedModel);
        }

        if (currentItemModel != null)
        {
            Item item = currentItemModel.GetComponent<Item>();
            if (item != null)
            {
                item.Deselect(); // 取消选中
                item.RemoveHighlight(); // 取消高亮，让 Update() 重新检测
            }
        }

        isInspecting = false;
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
            currentItemModel = itemModel; // 只在有物品时更新
        }
    }
}