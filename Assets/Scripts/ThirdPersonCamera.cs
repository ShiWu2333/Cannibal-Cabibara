using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target; 
    public Vector3 offset = new Vector3(0, 2, -4); 

    void LateUpdate()
    {
        transform.position = target.position + offset;
        transform.LookAt(target.position);
    }
}