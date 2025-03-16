using UnityEngine;
using System.Collections;

public class ChiefAI : MonoBehaviour
{
    [Header("Ѳ�߲���")]
    public float patrolRange = 5f;   // ��ʼѲ�߷�Χ
    public float patrolSpeed = 2f;   // Ѳ���ƶ��ٶ�
    public float patrolWaitTime = 2f; // ÿ�ε�����ȴ�ʱ��

    [Header("���ܲ���")]
    public Transform safePoint;  // ���ܵĹ̶�λ��
    public float escapeSpeed = 5f; // �����ٶ�
    public float shockedDuration = 1f; // �ܾ�ʱ��
    private bool isEscaping = false; // �Ƿ���������

    [Header("��ⷶΧ")]
    public Collider detectionCollider; // ��������Χ
    public BubbleController bubbleController; // ������ʾ���ݵ����
    public Sprite shockedBubbleIcon; // �ܾ�ͼ��

    private Rigidbody rb;
    private Vector3 initialPosition; // ��¼��ʼλ��
    private bool isPatrolling = true; // �Ƿ������Ѳ��

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        initialPosition = transform.position; // ��¼ NPC �ĳ�ʼλ��
        StartCoroutine(Patrol());
    }

    private IEnumerator Patrol()
    {
        while (isPatrolling) // ֻ����δ���ŵ�ʱ��Ѳ��
        {
            Vector3 targetPoint = GetRandomPatrolPoint();
            while (Vector3.Distance(transform.position, targetPoint) > 0.5f)
            {
                if (!isPatrolling) yield break; // ��� NPC ��������״̬����ֹͣѲ��
                MoveTo(targetPoint, patrolSpeed);
                yield return null;
            }
            yield return new WaitForSeconds(patrolWaitTime);
        }
    }

    private Vector3 GetRandomPatrolPoint()
    {
        // �ڳ�ʼλ�ø������ѡ��
        return initialPosition + new Vector3(
            Random.Range(-patrolRange, patrolRange),
            0f,
            Random.Range(-patrolRange, patrolRange)
        );
    }

    private void MoveTo(Vector3 destination, float speed)
    {
        Vector3 direction = (destination - transform.position).normalized;

        if (direction != Vector3.zero) // ȷ�������� (0,0,0)
        {
            // ֻ�޸� Y �����ת������ NPC ����
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }

        rb.MovePosition(transform.position + direction * speed * Time.fixedDeltaTime);
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isEscaping)
        {
            StartCoroutine(HandlePlayerDetected());
        }
    }

    private IEnumerator HandlePlayerDetected()
{
    isPatrolling = false; // ֹͣѲ��
    isEscaping = true; // ��������ģʽ


    // ��ʾ�ܾ�����
    if (bubbleController != null && shockedBubbleIcon != null)
    {
        bubbleController.ShowBubble(shockedBubbleIcon);
    }

    yield return new WaitForSeconds(shockedDuration); // �ܾ�ͣ��

    // ���ܵ���ȫ��
    while (Vector3.Distance(transform.position, safePoint.position) > 0.5f)
    {
        MoveTo(safePoint.position, escapeSpeed); // ȷ��ʹ�� MoveTo()��������ȷ����
        yield return new WaitForFixedUpdate(); // ȷ���������ͬ��
    }

    yield return new WaitForSeconds(1f); // ���ܺ�����ͣ��

    // �ڰ�ȫ�㸽�����Ѳ��
    initialPosition = safePoint.position;
    isEscaping = false;
    isPatrolling = true;
    StartCoroutine(Patrol()); // ���½���Ѳ��ģʽ
}

    public bool IsWalking()
    {
        return isPatrolling || isEscaping; // ֻҪ��Ѳ�߻����ܣ��Ͳ�����·����
    }

    public bool IsMoving()
    {
        // ֻ�е� NPC ��Ѳ��/���ܲ��� `Rigidbody` �ٶȲ�Ϊ 0 ʱ�ŷ��� `true`
        return IsWalking() && rb.velocity.magnitude > 0.1f;
    }


    public bool IsEscaping()
    {
        return isEscaping;
    }


}

