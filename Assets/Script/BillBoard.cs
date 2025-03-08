using UnityEngine;

public class BillBoard : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main; // 获取主摄像机
    }

    void Update()
    {
        if (mainCamera != null)
        {
            transform.LookAt(transform.position + mainCamera.transform.forward); // 始终面向摄像机
            Vector3 cameraDirection = mainCamera.transform.position - transform.position;
            cameraDirection.z = 0; // 只在 Y 轴旋转，保持水平

        }
        else
        {
            Debug.Log( "maincamera is not found ");
        }
    }
}
