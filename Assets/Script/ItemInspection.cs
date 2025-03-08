using UnityEngine;
using UnityEngine.UI;

public class ItemInspection : MonoBehaviour
{
    public GameObject inspectionPanel; // ���Ӵ��ڵ� UI Panel
    public RawImage itemDisplay; // ������ʾ 3D ģ�͵� RawImage
    public Camera itemCamera; // ������Ⱦ 3D ģ�͵����
    public RenderTexture itemRenderTexture; // Render Texture

    private GameObject currentItemModel; // ��ǰѡ�е���Ʒģ��
    private GameObject inspectedModel; // ��ǰ���ӵ�ģ�͸���
    private bool isInspecting = false; // �Ƿ����ڼ���
    private Vector3 lastMousePosition; // ��һ֡�����λ��

    private void Start()
    {
        // ��ʼ���ؼ��Ӵ���
        inspectionPanel.SetActive(false);
        itemCamera.gameObject.SetActive(false);

        // ���� Camera �� Target Texture
        itemCamera.targetTexture = itemRenderTexture;

        // ���� RawImage �� Texture
        itemDisplay.texture = itemRenderTexture;
    }

    private void Update()
    {
        // ������ڼ��ӣ�������תģ��
        if (isInspecting)
        {
            RotateItemModel();
        }

        // ���� Esc ���رռ��Ӵ���
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseInspection();
        }
    }

    // �򿪼��Ӵ���
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

    // �򿪼��Ӵ���
    private void OpenInspection()
    {
        if (currentItemModel == null) return;

        // ���Ƶ�ǰѡ�е���Ʒģ��
        inspectedModel = Instantiate(currentItemModel);

        // ���ø��Ƶ�ģ�͵� Rigidbody
        Rigidbody rb = inspectedModel.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true; // ��������ģ��
        }

        // �ݹ���ø��Ƶ�ģ�ͼ����Ӷ���� Outline ���
        DisableOutlineRecursively(inspectedModel);

        // ����ģ�͵����ĵ�
        Renderer renderer = inspectedModel.GetComponent<Renderer>();
        if (renderer != null)
        {
            Vector3 center = renderer.bounds.center; // ��ȡģ�͵ļ�������
            inspectedModel.transform.position = itemCamera.transform.position + itemCamera.transform.forward * 2f - center + inspectedModel.transform.position;
        }

        // ��ʾ���Ӵ���
        inspectionPanel.SetActive(true);
        itemCamera.gameObject.SetActive(true);

        isInspecting = true;
    }

    // �رռ��Ӵ���
    private void CloseInspection()
    {
        // ���ؼ��Ӵ���
        inspectionPanel.SetActive(false);
        itemCamera.gameObject.SetActive(false);

        // ���ٸ��Ƶ�ģ��
        if (inspectedModel != null)
        {
            Destroy(inspectedModel);
        }

        if (currentItemModel != null)
        {
            Item item = currentItemModel.GetComponent<Item>();
            if (item != null)
            {
                item.Deselect(); // ȡ��ѡ��
                item.RemoveHighlight(); // ȡ���������� Update() ���¼��
            }
        }

        isInspecting = false;
    }

    // ��ת��Ʒģ��
    private void RotateItemModel()
    {
        if (Input.GetMouseButton(0)) // ��ס������
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;

            // ��ȡģ�͵ļ�������
            Renderer renderer = inspectedModel.GetComponent<Renderer>();
            if (renderer != null)
            {
                Vector3 center = renderer.bounds.center; // ��̬���㼸������

                // Χ��ģ�͵ļ���������ת
                inspectedModel.transform.RotateAround(center, Vector3.up, -delta.x * 0.2f); // ˮƽ��ת
                inspectedModel.transform.RotateAround(center, Vector3.right, delta.y * 0.2f); // ��ֱ��ת
            }
        }

        lastMousePosition = Input.mousePosition;
    }

    // �ݹ���� Outline ���
    private void DisableOutlineRecursively(GameObject obj)
    {
        // ���õ�ǰ����� Outline ���
        Outline outline = obj.GetComponent<Outline>();
        if (outline != null)
        {
            outline.enabled = false; // ���� Outline
            // Destroy(outline); // ����ֱ���Ƴ� Outline ���
        }

        // �ݹ���������Ӷ���� Outline ���
        foreach (Transform child in obj.transform)
        {
            DisableOutlineRecursively(child.gameObject);
        }
    }

    // ���õ�ǰѡ�е���Ʒģ��
    public void SetItemModel(GameObject itemModel)
    {
        if (itemModel != null)
        {
            currentItemModel = itemModel; // ֻ������Ʒʱ����
        }
    }
}