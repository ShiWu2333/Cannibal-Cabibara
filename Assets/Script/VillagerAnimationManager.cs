using UnityEngine;

public class VillagerAnimationManager : MonoBehaviour
{
    [SerializeField] private Animator animator; // ����������
    [SerializeField] private VillagerAI villagerAI; // ���� AI �߼�

    private void Start()
    {
        // ȷ�� Animator �� VillagerAI �������
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("VillagerAnimationManager: δ�ҵ� Animator �����");
            }
        }

        if (villagerAI == null)
        {
            villagerAI = GetComponent<VillagerAI>();
            if (villagerAI == null)
            {
                Debug.LogError("VillagerAnimationManager: δ�ҵ� VillagerAI �����");
            }
        }
    }

    private void Update()
    {
        if (villagerAI == null || animator == null) return;

        // ��ȡ Villager ���ƶ�״̬
        bool isWalking = villagerAI.IsWalking();
        animator.SetBool("isWalking", isWalking); // ���� Animator ����

    }
}
