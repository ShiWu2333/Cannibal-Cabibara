using UnityEngine;

public class PlayerStateManager : MonoBehaviour
{
    public enum PlayerState
    {
        Idle,        // �޲���
        Running,     // �˶���
        PickUp,      // ������Ʒ
        HoldAndRun   // ������Ʒ�ܲ�
    }

    public PlayerState currentState = PlayerState.Idle;
    private Animator animator;
    private PlayerMovement playerMovement;
    private bool isPickingUp = false; // �Ƿ��� PickUp ����

    private void Start()
    {
        animator = GetComponent<Animator>();  // ��ȡ Animator ���
        playerMovement = GetComponent<PlayerMovement>();  // ��ȡ PlayerMovement ���
    }

    private void Update()
    {
        if (!isPickingUp) // �����ǰû��ִ�� PickUp �����������״̬
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
                isPickingUp = true; // ������ڽ��� PickUp ����
                Invoke(nameof(ResetPickUpState), 0.8f); // ���� PickUp ����ʱ�� 0.8s
                break;
            case PlayerState.HoldAndRun:
                animator.SetTrigger("HoldRun");
                break;
        }
    }

    private void ResetPickUpState()
    {
        isPickingUp = false;
        UpdatePlayerState(); // ���¼��״̬
    }

    public void TriggerPickUpAnimation()
    {
        ChangeState(PlayerState.PickUp);
    }
}
