using UnityEngine;
using System.Collections.Generic;

public class TransparentObjectController : MonoBehaviour
{
    public Transform player; // 角色
    public LayerMask obstacleLayer; // 遮挡物的层级
    public float transparency = 0.3f; // 透明度

    private Dictionary<Renderer, MaterialPropertyBlock> propertyBlocks = new Dictionary<Renderer, MaterialPropertyBlock>(); // 存储 MaterialPropertyBlock
    private List<Renderer> currentTransparentObjects = new List<Renderer>(); // 当前变透明的物体

    void Update()
    {
        RestorePreviousObjects(); // 恢复上次变透明的物体

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
            propertyBlocks[renderer] = new MaterialPropertyBlock(); // 创建 PropertyBlock
        }

        MaterialPropertyBlock block = propertyBlocks[renderer];
        renderer.GetPropertyBlock(block);

        // 读取原始颜色并调整透明度
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

                // 恢复原始颜色
                Color originalColor = renderer.sharedMaterial.color;
                block.SetColor("_Color", originalColor);
                renderer.SetPropertyBlock(block);
            }
        }
        currentTransparentObjects.Clear();
    }
}
