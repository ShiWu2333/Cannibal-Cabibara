using UnityEngine;
using System.Collections;

public class PoliceAI : MonoBehaviour
{

    [Header("Ѳ������")]
    public bool isGuard = false; // �Ƿ�Ϊվ��ģʽ��true = ԭ�ز���, false = Ѳ�ߣ�
    public Transform pointA; // Ѳ�ߵ� A
    public Transform pointB; // Ѳ�ߵ� B
    private Transform currentTarget; // ��ǰĿ���

    [Header("Ѳ�߲���")]
    public float speed = 3.0f; // Ѳ���ٶ�
    public float waitTime = 2.0f; // ��Ѳ�ߵ��ͣ��ʱ��
    private bool isWaiting = false; // �Ƿ����ڵȴ�
    private bool goingToB = true; // �Ƿ�����ǰ�� B ��

    [Header("���䷶Χ")]
    public Collider detectionCollider; // �����ҵ� Collider
    public Transform respawnPoint; // ��Ҹ����
    public float caughtWaitTime = 1.0f; // ��ץס��ȴ�ʱ��

    [Header("��������")]
    private BubbleController bubbleController;
    public Sprite npcBubbleIcon; // NPC ����������ͼ��

    private Rigidbody rb;
    private bool isChasing = false; // �Ƿ���׷�����

    public static event System.Action<Transform> OnPlayerCaught; // �¼�����ұ�����

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // �˶�ѧ Rigidbody

        if (!isGuard) // ֻ�з�վ��ģʽʱ��������Ѳ��Ŀ��
        {
            currentTarget = pointA;
            StartCoroutine(Patrol());
        }

        bubbleController = GetComponent<BubbleController>();

        if (bubbleController == null)
        {
            Debug.LogError("BubbleController ���δ������ NPC �ϣ�");
        }
        if (npcBubbleIcon == null)
        {
            Debug.LogError("NPC û��ָ������ͼ�꣡");
        }
        if (detectionCollider == null)
        {
            Debug.LogError("NPC û�м�ⷶΧ Collider��");
        }
    }

    private IEnumerator Patrol()
    {
        while (!isGuard) // վ��ģʽ�²�ִ��Ѳ��
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
        if (isGuard) return; // �����վ��ģʽ����Ѳ��

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
            bubbleController.ShowBubble(npcBubbleIcon);
            if (!isChasing)
            {
                isChasing = true;
                Debug.Log("��ҽ��뾯������");

                OnPlayerCaught?.Invoke(transform); // �����¼���֪ͨ CameraManager
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
