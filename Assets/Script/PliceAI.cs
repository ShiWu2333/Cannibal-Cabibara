using UnityEngine;
using System.Collections;

public class PoliceAI : MonoBehaviour
{
    [Header("Ѳ�ߵ�����")]
    public Transform pointA;   // Ѳ�ߵ� A
    public Transform pointB;   // Ѳ�ߵ� B
    private Transform currentTarget; // ��ǰĿ���

    [Header("Ѳ�߲���")]
    public float speed = 3.0f;      // Ѳ���ٶ�
    public float waitTime = 2.0f;   // ͣ���ȴ�ʱ��
    private bool isWaiting = false; // �Ƿ����ڵȴ�
    private bool goingToB = true;   // �Ƿ�����ǰ�� B ��

    [Header("���䷶Χ")]
    public Collider detectionCollider; // ���ڼ����ҵ� Collider
    public Transform respawnPoint;      // ��Ҹ����
    public float caughtWaitTime = 1.0f; // ��ץס��ȴ�ʱ��

    private Rigidbody rb;
    private bool isChasing = false; // �Ƿ���׷�����

    public static event System.Action<Transform> OnPlayerCaught;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // �˶�ѧ Rigidbody
        currentTarget = pointA; // ��ʼѲ��Ŀ��
        StartCoroutine(Patrol());
    }

    private IEnumerator Patrol()
    {
        while (true)
        {
            if (!isChasing && !isWaiting)
            {
                MoveToNextPatrolPoint();
            }
            yield return null;
        }
    }

    private void MoveToNextPatrolPoint()
    {
        Vector3 targetPos = currentTarget.position;
        Vector3 direction = (targetPos - transform.position);
        direction.y = 0f; // ����ˮƽ�ƶ�
        direction.Normalize();

        float distance = Vector3.Distance(transform.position, targetPos);
        if (distance < 0.2f)
        {
            StartCoroutine(WaitAtPatrolPoint());
        }
        else
        {
            rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
            rb.rotation = Quaternion.Slerp(rb.rotation, Quaternion.LookRotation(direction), 5f * Time.fixedDeltaTime);
        }
    }

    private IEnumerator WaitAtPatrolPoint()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);
        isWaiting = false;
        currentTarget = (currentTarget == pointA) ? pointB : pointA;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // ֻ���Tag�Ƿ���"Player"
        {
            if (!isChasing)
            {
                isChasing = true;
                Debug.Log("��ҽ��뾯������");

                OnPlayerCaught?.Invoke(transform); // �����¼���֪ͨCameraManager
                StartCoroutine(HandlePlayerCaught(other.gameObject));
            }
        }
    }


    private IEnumerator HandlePlayerCaught(GameObject player)
    {
        yield return new WaitForSeconds(caughtWaitTime);
        player.transform.position = respawnPoint.position;
        isChasing = false;
    }
}
