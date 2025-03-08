using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    public Transform cameraCentre; // 目标视角
    public float stayDuration = 2.0f; // 停留时间
    private bool hasTriggered = false; // 是否已经触发过

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag("Player")) // 只触发一次
        {
            hasTriggered = true;
            ThirdPersonCamera cameraController = Camera.main.GetComponent<ThirdPersonCamera>();

            if (cameraController != null)
            {
                cameraController.SetTarget(cameraCentre, stayDuration);
            }
            else
            {
                Debug.LogError("CameraTrigger: 找不到 ThirdPersonCamera，请检查 Main Camera 是否挂载！");
            }
        }
    }
}
