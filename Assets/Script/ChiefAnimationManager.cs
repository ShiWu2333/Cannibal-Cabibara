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
        if (chiefAI == null) return;

        // 只更新 isWalking
        animator.SetBool("isWalking", chiefAI.IsWalking());

        if (chiefAI.IsWalking())
        {
            animator.speed = chiefAI.IsEscaping() ? 1.5f : 1.0f; // 逃跑时加速，巡逻时恢复
        }
        else
        {
            animator.speed = 1.0f; // 停止移动时恢复默认
        }

    }
}
