using UnityEngine;
using System.Collections;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;  // Ŀ�꣨Ĭ������ң�
    public Vector3 offset = new Vector3(0, 2, -4); // ���ƫ��
    public float transitionSpeed = 2.0f; // ����ƽ���ٶ�
    private Transform defaultTarget; // ��¼Ĭ��Ŀ�꣨��ң�
    private Quaternion lockedRotation; // ��¼��ʼ�����ת
    public bool lockRotation = true; // �Ƿ����������ת
    public float instantTeleportDistance = 500f; // �����˾������˲��

    void Start()
    {
        defaultTarget = target; // ��¼Ĭ�ϵ�Ŀ�꣨��ң�
        lockedRotation = transform.rotation; // ��¼��ʼ�����ת

        // ������ұ�ץ�¼�
        PoliceAI.OnPlayerCaught += HandlePlayerCaught;
    }

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;

            // **�����⣺����������Ŀ�곬�� `instantTeleportDistance`��ֱ��˲��**
            if (Vector3.Distance(transform.position, desiredPosition) > instantTeleportDistance)
            {
                transform.position = desiredPosition; // ֱ��˲��
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * transitionSpeed);
            }

            if (!lockRotation)
            {
                transform.LookAt(target.position); // ֻ�е� lockRotation Ϊ false ʱ����ת
            }
            else
            {
                transform.rotation = lockedRotation; // ά��ԭ������ת�Ƕ�
            }
        }
    }

    public void SetTarget(Transform newTarget, float duration)
    {
        StopAllCoroutines(); // ȷ�������ж��Э��ͬʱ����
        StartCoroutine(SwitchTarget(newTarget, duration));
    }

    private IEnumerator SwitchTarget(Transform newTarget, float duration)
    {
        PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.PauseInput();
        }
        else
        {
            Debug.LogError("PlayerMovement �ű�δ�ҵ�����ȷ�� Player ���д������");
        }

        target = newTarget; // �л�Ŀ��

        yield return new WaitForSeconds(duration);

        if (playerMovement != null)
        {
            playerMovement.ResumeInput();
        }
        else
        {
            Debug.LogError("PlayerMovement �ű�δ�ҵ�����ȷ�� Player ���д������");
        }

        target = defaultTarget; // �ָ�ΪĬ��Ŀ�꣨��ң�
    }

    private void HandlePlayerCaught(Transform policeTransform)
    {
        Debug.Log("����л��������ӽǣ�");
        SetTarget(policeTransform, 2f); // �趨���Ŀ��Ϊ���죬����2����л������
    }

    private void OnDestroy()
    {
        // ȡ���¼���������ֹ����
        PoliceAI.OnPlayerCaught -= HandlePlayerCaught;
    }
}
