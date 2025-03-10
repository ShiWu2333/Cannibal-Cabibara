using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 500f;
    public float selectDistance = 0.5f; // 选择范围的半边长
    public Vector2 move;
    public bool pauseInput;

    [SerializeField] private Transform holdPosition;
    [SerializeField] private BackPackUIManager backPackUIManager;
    [SerializeField] private NoticeBoardManager noticeBoardManager;
    public Item carriedItem;

    public ItemInspection itemInspection; // 检视功能脚本
    private Rigidbody rb; // 添加 Rigidbody

    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>(); // 获取 Rigidbody 组件
        rb.freezeRotation = true; // 防止角色旋转
        pauseInput = false;
    }

    private void Update()
    {
        if (!pauseInput)
        {
            movePlayer();
            CheckInteractInput();

            // 按下 Q 键检视物品
            if (Input.GetKeyDown(KeyCode.Q) && itemInspection != null && itemInspection.canToggle)
            {
                    InspectSelectedItem();
                   
            }

            // 检测 B 键按下，切换背包UI显示
            if (Input.GetKeyDown(KeyCode.B))
            {
                if (backPackUIManager != null)
                {
                    backPackUIManager.ToggleBackPackUI();
                }
            }

            // 检测 N 键按下，切换公告栏 UI
            if (Input.GetKeyDown(KeyCode.N))
            {
                if (noticeBoardManager!= null)
                {
                    noticeBoardManager.ToggleNoticeBoardUI();
                }
            }
        }
    }

    private void InspectSelectedItem()
    {
        // 使用立方体区域检测
        Item item = DetectItemInBox();
        if (item != null)
        {
            itemInspection.SetItemModel(item.gameObject);
            itemInspection.ToggleInspection();
            BackPackManager.Instance.AddItem(item);
        }
    }

    private void movePlayer()
    {
        Vector3 movement = new Vector3(move.x, 0f, move.y).normalized;

        if (movement.magnitude > 0.1f)
        {
            // 计算目标朝向
            Quaternion targetRotation = Quaternion.LookRotation(movement);

            // 根据 rotationSpeed 平滑旋转到目标朝向
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        //  修改为使用 Rigidbody 移动，确保物理碰撞生效
        Vector3 newPosition = rb.position + movement * speed * Time.deltaTime;
        rb.MovePosition(newPosition);
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
        // 使用立方体区域检测
        Item item = DetectItemInBox();
        if (item != null && !item.isPickedUp)
        {
            carriedItem = item;
            item.isPickedUp = true;

            // 触发 PickUp 动画
            GetComponent<PlayerStateManager>().TriggerPickUpAnimation();

            PickUpItem(item); // 执行拾取操作

            // 通知物体取消高亮
            Item highlightItem = item.GetComponent<Item>();
            if (highlightItem != null)
            {
                highlightItem.OnPickedUp();
            }
        }
    }


    private Item DetectItemInBox()
    {
        // 定义立方体中心的偏移（玩家前方 0.5f 处）
        Vector3 boxCenter = transform.position + transform.forward * 0.5f;

        // 检测立方体区域内的物品
        Collider[] hitColliders = Physics.OverlapBox(boxCenter, new Vector3(selectDistance, selectDistance, 0.5f), transform.rotation);
        foreach (var hitCollider in hitColliders)
        {
            Item item = hitCollider.GetComponent<Item>();
            if (item != null)
            {
                return item; // 返回第一个检测到的物品
            }
        }
        return null; // 如果没有检测到物品，返回 null
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
            carriedItem.Deselect();
            carriedItem.UpdateHighlightState(); // 重新检查白色高亮状态


            carriedItem = null; // 清空当前拿起的道具
        }
    }

    // 在场景中绘制立方体区域以便调试
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 boxCenter = transform.position + transform.forward * 0.5f; // 与代码中的偏移一致
        Gizmos.matrix = Matrix4x4.TRS(boxCenter, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(selectDistance * 2, selectDistance * 2, 1f)); // 长 1f 的立方体
    }

    public void PauseInput()
    {
        pauseInput = true;
    }

    public void ResumeInput()
    {
        pauseInput = false;
    }
}
