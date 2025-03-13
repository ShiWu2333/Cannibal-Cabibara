using UnityEngine;

public class CabbibaraAnimationManager : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerMovement playerMovement;

    private void Start()
    {
    }

    private void Update()
    {
        if (playerMovement == null)
        {
            Debug.LogError("PlayerMovement 组件未找到！");
            return;
        }

        // 监听玩家是否在移动
        bool isWalking = playerMovement.move.magnitude > 0.1f;
        animator.SetBool("IsWalking", isWalking);

        // 监听玩家是否拿着物品
        bool isCarrying = playerMovement.carriedItem != null;
        animator.SetBool("IsCarrying", isCarrying);

        // 监听玩家是否按下 E 键进行拾取
        if (Input.GetKeyDown(KeyCode.E) && playerMovement.carriedItem == null)
        {
            TriggerPickUpAnimation();
        }
    }

    private void TriggerPickUpAnimation()
    {
        animator.SetTrigger("IsPickingUp"); // 触发拾取动画
    }
}
