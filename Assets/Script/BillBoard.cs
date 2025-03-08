using UnityEngine;

public class BillBoard : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main; // ��ȡ�������
    }

    void Update()
    {
        if (mainCamera != null)
        {
            transform.LookAt(transform.position + mainCamera.transform.forward); // ʼ�����������
            Vector3 cameraDirection = mainCamera.transform.position - transform.position;
            cameraDirection.z = 0; // ֻ�� Y ����ת������ˮƽ

        }
        else
        {
            Debug.Log( "maincamera is not found ");
        }
    }
}
