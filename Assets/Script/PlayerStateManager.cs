using UnityEngine;

public class PlayerStateManager : MonoBehaviour
{
    public enum PlayerState
    {
        Idle,        // 无操作
        Running,     // 运动中
        PickUp,      // 拿起物品
        HoldAndRun   // 拿着物品跑步
    }

    public PlayerState currentState = PlayerState.Idle;
    private Animator animator;
    private PlayerMovement playerMovement;
    private bool isPickingUp = false; // 是否处于 PickUp 动画

    private void Start()
    {
        animator = GetComponent<Animator>();  // 获取 Animator 组件
        playerMovement = GetComponent<PlayerMovement>();  // 获取 PlayerMovement 组件
    }

    private void Update()
    {
        if (!isPickingUp) // 如果当前没有执行 PickUp 动作，则更新状态
        {
            UpdatePlayerState();
        }
    }

    private void UpdatePlayerState()
    {
        bool isMoving = playerMovement.move.magnitude > 0.1f;
        bool isHoldingItem = playerMovement.carriedItem != null;

        if (isHoldingItem)
        {
            if (isMoving)
                ChangeState(PlayerState.HoldAndRun);
            else
                ChangeState(PlayerState.Idle);
        }
        else
        {
            if (isMoving)
                ChangeState(PlayerState.Running);
            else
                ChangeState(PlayerState.Idle);
        }
    }

    private void ChangeState(PlayerState newState)
    {
        if (currentState == newState) return;

        currentState = newState;
        PlayAnimation();
    }

    private void PlayAnimation()
    {
        animator.ResetTrigger("Idle");
        animator.ResetTrigger("Run");
        animator.ResetTrigger("PickUp");
        animator.ResetTrigger("HoldRun");

        switch (currentState)
        {
            case PlayerState.Idle:
                animator.SetTrigger("Idle");
                break;
            case PlayerState.Running:
                animator.SetTrigger("Run");
                break;
            case PlayerState.PickUp:
                animator.SetTrigger("PickUp");
                isPickingUp = true; // 标记正在进行 PickUp 动作
                Invoke(nameof(ResetPickUpState), 0.8f); // 假设 PickUp 动画时长 0.8s
                break;
            case PlayerState.HoldAndRun:
                animator.SetTrigger("HoldRun");
                break;
        }
    }

    private void ResetPickUpState()
    {
        isPickingUp = false;
        UpdatePlayerState(); // 重新检查状态
    }

    public void TriggerPickUpAnimation()
    {
        ChangeState(PlayerState.PickUp);
    }
}
