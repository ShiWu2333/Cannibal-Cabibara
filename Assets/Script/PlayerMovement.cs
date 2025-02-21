using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    public float speed;
    public float rotationSpeed;
    private Vector2 move;

    [SerializeField] Transform holdPosition;
    private Item carriedItem;

    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        movePlayer();
        CheckInteractInput();
    }

    private void movePlayer()
    {
        Vector3 movement = new Vector3(move.x, 0f, move.y);

        if (movement.magnitude > 0.1f)
        {
            // 计算目标朝向
            Quaternion targetRotation = Quaternion.LookRotation(movement);

            // 根据 rotationSpeed 平滑旋转到目标朝向
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        transform.Translate(movement * speed * Time.deltaTime, Space.World);

    }

    private void CheckInteractInput()
    {
        // 检测按下 E 键
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (carriedItem == null)
            {
                TryPickUpItem(); // 尝试捡起道具
            }
            else
            {
                DropItem(); // 放下道具
            }
        }
    }

    private void TryPickUpItem()
    {
        // 使用射线检测前方的道具
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 2f))
        {
            Debug.Log("Raycast hit: " + hit.collider.name); // 添加调试日志
            Item item = hit.collider.GetComponent<Item>();
            if (item != null && !item.isPickedUp)
            {
                carriedItem = item;
                item.isPickedUp = true;
                PickUpItem(item); // 捡起道具

                // 通知物体取消高亮
                Item highlightItem = item.GetComponent<Item>();
                if (highlightItem != null)
                {
                    highlightItem.OnPickedUp();
                }
            }
        }
        else
        {
            Debug.Log("Raycast did not hit anything."); // 添加调试日志
        }
    }

    private void PickUpItem(Item item)
    {
        // 将道具设置为玩家的子对象，并固定在手上
        item.transform.SetParent(holdPosition);
        item.transform.localPosition = Vector3.zero; // 固定在手上
        item.transform.localRotation = Quaternion.identity; // 重置旋转

        // 禁用物理（如果需要）
        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.detectCollisions = false; // 启用碰撞检测
        }
    }

    private void DropItem()
    {
        if (carriedItem != null)
        {
            carriedItem.isPickedUp = false;
            carriedItem.transform.SetParent(null); // 恢复为独立对象

            // 启用物理（如果需要）
            Rigidbody rb = carriedItem.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.detectCollisions = true; // 启用碰撞检测
            }

            carriedItem = null; // 清空当前拿起的道具
        }
    }
}
