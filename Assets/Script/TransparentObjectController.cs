using UnityEngine;
using System.Collections.Generic;

public class TransparentObjectController : MonoBehaviour
{
    public Transform player; // ��ɫ
    public LayerMask obstacleLayer; // �ڵ���Ĳ㼶
    public float transparency = 0.3f; // ͸����

    private Dictionary<Renderer, MaterialPropertyBlock> propertyBlocks = new Dictionary<Renderer, MaterialPropertyBlock>(); // �洢 MaterialPropertyBlock
    private List<Renderer> currentTransparentObjects = new List<Renderer>(); // ��ǰ��͸��������

    void Update()
    {
        RestorePreviousObjects(); // �ָ��ϴα�͸��������

        Vector3 direction = player.position - transform.position;
        float distance = direction.magnitude;

        RaycastHit[] hits = Physics.RaycastAll(transform.position, direction.normalized, distance, obstacleLayer);
        foreach (RaycastHit hit in hits)
        {
            Renderer renderer = hit.collider.GetComponent<Renderer>();
            if (renderer != null)
            {
                MakeObjectTransparent(renderer);
                currentTransparentObjects.Add(renderer);
            }
        }
    }

    void MakeObjectTransparent(Renderer renderer)
    {
        if (!propertyBlocks.ContainsKey(renderer))
        {
            propertyBlocks[renderer] = new MaterialPropertyBlock(); // ���� PropertyBlock
        }

        MaterialPropertyBlock block = propertyBlocks[renderer];
        renderer.GetPropertyBlock(block);

        // ��ȡԭʼ��ɫ������͸����
        Color originalColor = renderer.sharedMaterial.color;
        Color transparentColor = new Color(originalColor.r, originalColor.g, originalColor.b, transparency);
        block.SetColor("_Color", transparentColor);

        renderer.SetPropertyBlock(block);
    }

    void RestorePreviousObjects()
    {
        foreach (Renderer renderer in currentTransparentObjects)
        {
            if (propertyBlocks.ContainsKey(renderer))
            {
                MaterialPropertyBlock block = propertyBlocks[renderer];
                renderer.GetPropertyBlock(block);

                // �ָ�ԭʼ��ɫ
                Color originalColor = renderer.sharedMaterial.color;
                block.SetColor("_Color", originalColor);
                renderer.SetPropertyBlock(block);
            }
        }
        currentTransparentObjects.Clear();
    }
}
