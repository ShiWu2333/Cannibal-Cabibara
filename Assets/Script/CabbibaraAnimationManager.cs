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
            Debug.LogError("PlayerMovement ���δ�ҵ���");
            return;
        }

        // ��������Ƿ����ƶ�
        bool isWalking = playerMovement.move.magnitude > 0.1f;
        animator.SetBool("IsWalking", isWalking);

        // ��������Ƿ�������Ʒ
        bool isCarrying = playerMovement.carriedItem != null;
        animator.SetBool("IsCarrying", isCarrying);

        // ��������Ƿ��� E ������ʰȡ
        if (Input.GetKeyDown(KeyCode.E) && playerMovement.carriedItem == null)
        {
            TriggerPickUpAnimation();
        }
    }

    private void TriggerPickUpAnimation()
    {
        animator.SetTrigger("IsPickingUp"); // ����ʰȡ����
    }
}
