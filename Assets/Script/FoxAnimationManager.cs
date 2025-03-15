using UnityEngine;

public class FoxAnimationManager : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private PoliceAI policeAI;

    private void Start()
    {

        if (animator == null)
        {
            Debug.LogError("FoxOfficer û�а� Animator �����");
        }
        if (policeAI == null)
        {
            Debug.LogError("FoxOfficer û�а� PoliceAI �����");
        }
    }

    private void Update()
    {
        if (policeAI == null) return;

        // �жϾ����Ƿ����ƶ�
        bool isWalking = policeAI.IsWalking();
        animator.SetBool("isWalking", isWalking);
    }



}
