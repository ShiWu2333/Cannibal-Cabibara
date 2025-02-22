using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    public float rotationSpeed;
    public float selectDistance = 0.5f; // ѡ��Χ�İ�߳�
    private Vector2 move;

    [SerializeField] private Transform holdPosition;
    private Item carriedItem;

    public ItemInspection itemInspection; // ���ӹ��ܽű�

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

        // ���� Q ��������Ʒ
        if (Input.GetKeyDown(KeyCode.Q))
        {
            InspectSelectedItem();
        }
    }

    private void InspectSelectedItem()
    {
        // ʹ��������������
        Item item = DetectItemInBox();
        if (item != null)
        {
            itemInspection.SetItemModel(item.gameObject);
            itemInspection.ToggleInspection();
        }
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
        // ʹ��������������
        Item item = DetectItemInBox();
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

    private Item DetectItemInBox()
    {
        // �������������ĵ�ƫ�ƣ����ǰ�� 0.5f ����
        Vector3 boxCenter = transform.position + transform.forward * 0.5f;

        // ��������������ڵ���Ʒ
        Collider[] hitColliders = Physics.OverlapBox(boxCenter, new Vector3(selectDistance, selectDistance, 0.5f), transform.rotation);
        foreach (var hitCollider in hitColliders)
        {
            Item item = hitCollider.GetComponent<Item>();
            if (item != null)
            {
                return item; // ���ص�һ����⵽����Ʒ
            }
        }
        return null; // ���û�м�⵽��Ʒ������ null
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

    // �ڳ����л��������������Ա����
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 boxCenter = transform.position + transform.forward * 0.5f; // ������е�ƫ��һ��
        Gizmos.matrix = Matrix4x4.TRS(boxCenter, transform.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(selectDistance * 2, selectDistance * 2, 1f)); // �� 1f ��������
    }
}