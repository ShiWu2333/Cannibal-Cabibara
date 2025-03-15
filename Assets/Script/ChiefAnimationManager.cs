using UnityEngine;

public class ChiefAnimationManager : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private ChiefAI chiefAI;

    private void Start()
    {

        if (animator == null)
        {
            Debug.LogError("Animator ���δ�ҵ���");
        }
        if (chiefAI == null)
        {
            Debug.LogError("ChiefAI ���δ�ҵ���");
        }
    }

    private void Update()
    {
        if (chiefAI == null) return;

        // ֻ���� isWalking
        animator.SetBool("isWalking", chiefAI.IsWalking());

        if (chiefAI.IsWalking())
        {
            animator.speed = chiefAI.IsEscaping() ? 1.5f : 1.0f; // ����ʱ���٣�Ѳ��ʱ�ָ�
        }
        else
        {
            animator.speed = 1.0f; // ֹͣ�ƶ�ʱ�ָ�Ĭ��
        }

    }
}
