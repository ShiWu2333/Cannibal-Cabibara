using UnityEngine;
using System.Collections;

public class PoliceAI : MonoBehaviour
{
    [Header("巡逻点设置")]
    public Transform pointA;   // 巡逻点 A
    public Transform pointB;   // 巡逻点 B
    private Transform currentTarget; // 当前目标点

    [Header("巡逻参数")]
    public float speed = 3.0f;      // 巡逻速度
    public float waitTime = 2.0f;   // 停留等待时间
    private bool isWaiting = false; // 是否正在等待
    private bool goingToB = true;   // 是否正在前往 B 点

    [Header("警戒范围")]
    public Collider detectionCollider; // 用于检测玩家的 Collider
    public Transform respawnPoint;      // 玩家复活点
    public float caughtWaitTime = 1.0f; // 被抓住后等待时间

    private Rigidbody rb;
    private bool isChasing = false; // 是否在追捕玩家

    public static event System.Action<Transform> OnPlayerCaught;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true; // 运动学 Rigidbody
        currentTarget = pointA; // 初始巡逻目标
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
        direction.y = 0f; // 保持水平移动
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
        if (other.CompareTag("Player")) // 只检查Tag是否是"Player"
        {
            if (!isChasing)
            {
                isChasing = true;
                Debug.Log("玩家进入警戒区域！");

                OnPlayerCaught?.Invoke(transform); // 触发事件，通知CameraManager
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
