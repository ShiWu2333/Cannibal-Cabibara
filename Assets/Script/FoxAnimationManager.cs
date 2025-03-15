using UnityEngine;

public class FoxAnimationManager : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private PoliceAI policeAI;

    private void Start()
    {

        if (animator == null)
        {
            Debug.LogError("FoxOfficer 没有绑定 Animator 组件！");
        }
        if (policeAI == null)
        {
            Debug.LogError("FoxOfficer 没有绑定 PoliceAI 组件！");
        }
    }

    private void Update()
    {
        if (policeAI == null) return;

        // 判断警察是否在移动
        bool isWalking = policeAI.IsWalking();
        animator.SetBool("isWalking", isWalking);
    }



}
