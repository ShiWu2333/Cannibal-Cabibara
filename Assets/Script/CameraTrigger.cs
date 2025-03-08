using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    public Transform cameraCentre; // Ŀ���ӽ�
    public float stayDuration = 2.0f; // ͣ��ʱ��
    private bool hasTriggered = false; // �Ƿ��Ѿ�������

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player")) // ֻ����һ��
        {
            hasTriggered = true;
            ThirdPersonCamera cameraController = Camera.main.GetComponent<ThirdPersonCamera>();

            if (cameraController != null)
            {
                cameraController.SetTarget(cameraCentre, stayDuration);
            }
            else
            {
                Debug.LogError("CameraTrigger: �Ҳ��� ThirdPersonCamera������ Main Camera �Ƿ���أ�");
            }
        }
    }
}
