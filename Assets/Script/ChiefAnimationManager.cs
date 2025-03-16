using UnityEngine;

public class ChiefAnimationManager : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private ChiefAI chiefAI;

    private void Start()
    {
        if (animator == null)
        {
            Debug.LogError("Animator 组件未找到！");
        }
        if (chiefAI == null)
        {
            Debug.LogError("ChiefAI 组件未找到！");
        }
    }

    private void Update()
    {
        if (chiefAI == null || animator == null) return;

        bool isMoving = chiefAI.IsMoving();   // 现在使用 IsMoving()
        animator.SetBool("isWalking", isMoving);

        if (isMoving)
        {
            animator.speed = chiefAI.IsEscaping() ? 1.5f : 1.0f;
        }
        else
        {
            animator.speed = 1.0f;
        }
    }
}
