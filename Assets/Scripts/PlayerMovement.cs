using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;  
    public float rotationSpeed = 10f; 
    public float jumpForce = 5f; 
    public bool isGrounded = true; 

    private Rigidbody rb;
    private Vector3 moveDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float horizontal = 0f;
        float vertical = 0f;

        if (Input.GetKey(KeyCode.A)) horizontal = -1f; 
        if (Input.GetKey(KeyCode.D)) horizontal = 1f;  
        if (Input.GetKey(KeyCode.W)) vertical = 1f;    
        if (Input.GetKey(KeyCode.S)) vertical = -1f;   

        moveDirection = new Vector3(horizontal, 0, vertical).normalized; //normalized 让向量归一化，确保角色斜着走不会比直走快


        if (Input.GetKeyDown(KeyCode.Space) && isGrounded) //is Grounded 用来防止二段跳
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
         
        }
    }

    void FixedUpdate()
    {
        if (moveDirection.magnitude > 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true; 
        }
    }
}