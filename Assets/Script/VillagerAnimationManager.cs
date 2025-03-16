using UnityEngine;

public class VillagerAnimationManager : MonoBehaviour
{
    [SerializeField] private Animator animator; // 动画控制器
    [SerializeField] private VillagerAI villagerAI; // 村民 AI 逻辑

    private void Start()
    {
        // 确保 Animator 和 VillagerAI 组件存在
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("VillagerAnimationManager: 未找到 Animator 组件！");
            }
        }

        if (villagerAI == null)
        {
            villagerAI = GetComponent<VillagerAI>();
            if (villagerAI == null)
            {
                Debug.LogError("VillagerAnimationManager: 未找到 VillagerAI 组件！");
            }
        }
    }

    private void Update()
    {
        if (villagerAI == null || animator == null) return;

        // 获取 Villager 的移动状态
        bool isWalking = villagerAI.IsWalking();
        animator.SetBool("isWalking", isWalking); // 更新 Animator 参数

    }
}
