using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    public float speed;
    public float rotationSpeed;
    private Vector2 move;

    [SerializeField] Transform holdPosition;
    private Item carriedItem;

    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        movePlayer();
        CheckInteractInput();
    }

    private void movePlayer()
    {
        Vector3 movement = new Vector3(move.x, 0f, move.y);

        if (movement.magnitude > 0.1f)
        {
            // ����Ŀ�곯��
            Quaternion targetRotation = Quaternion.LookRotation(movement);

            // ���� rotationSpeed ƽ����ת��Ŀ�곯��
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        transform.Translate(movement * speed * Time.deltaTime, Space.World);

    }

    private void CheckInteractInput()
    {
        // ��ⰴ�� E ��
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (carriedItem == null)
            {
                TryPickUpItem(); // ���Լ������
            }
            else
            {
                DropItem(); // ���µ���
            }
        }
    }

    private void TryPickUpItem()
    {
        // ʹ�����߼��ǰ���ĵ���
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 2f))
        {
            Debug.Log("Raycast hit: " + hit.collider.name); // ��ӵ�����־
            Item item = hit.collider.GetComponent<Item>();
            if (item != null && !item.isPickedUp)
            {
                carriedItem = item;
                item.isPickedUp = true;
                PickUpItem(item); // �������

                // ֪ͨ����ȡ������
                Item highlightItem = item.GetComponent<Item>();
                if (highlightItem != null)
                {
                    highlightItem.OnPickedUp();
                }
            }
        }
        else
        {
            Debug.Log("Raycast did not hit anything."); // ��ӵ�����־
        }
    }

    private void PickUpItem(Item item)
    {
        // ����������Ϊ��ҵ��Ӷ��󣬲��̶�������
        item.transform.SetParent(holdPosition);
        item.transform.localPosition = Vector3.zero; // �̶�������
        item.transform.localRotation = Quaternion.identity; // ������ת

        // �������������Ҫ��
        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.detectCollisions = false; // ������ײ���
        }
    }

    private void DropItem()
    {
        if (carriedItem != null)
        {
            carriedItem.isPickedUp = false;
            carriedItem.transform.SetParent(null); // �ָ�Ϊ��������

            // �������������Ҫ��
            Rigidbody rb = carriedItem.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.detectCollisions = true; // ������ײ���
            }

            carriedItem = null; // ��յ�ǰ����ĵ���
        }
    }
}
