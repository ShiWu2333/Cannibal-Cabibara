using UnityEngine;
using System.Collections;

public class VillagerAI : MonoBehaviour
{
    public float moveSpeed = 2f; // ✅ 村民的移动速度
    public float minWalkTime = 2f; // ✅ 最短行走时间
    public float maxWalkTime = 5f; // ✅ 最长行走时间
    public float minIdleTime = 1f; // ✅ 最短停留时间
    public float maxIdleTime = 3f; // ✅ 最长停留时间
    public float changeDirectionInterval = 3f; // ✅ 每 3 秒随机换方向
    public float raycastDistance = 1f; // ✅ 用于检测障碍物的射线长度
    public float rotationSpeed = 5f; // ✅ 旋转速度（越大转向越快）

    private Vector3 moveDirection; // ✅ 村民当前的移动方向
    private Rigidbody rb; // ✅ 村民的刚体组件
    private bool isWalking = false; // ✅ 是否正在行走

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // ✅ 获取 Rigidbody
        StartCoroutine(WalkCycle()); // ✅ 开始“走走停停”循环
    }

    void FixedUpdate()
    {
        if (isWalking)
        {
            MoveVillager();
        }
    }

    private void MoveVillager()
    {
        if (!IsPathBlocked())
        {
            rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
            UpdateRotation(); // ✅ 确保村民朝向移动方向
        }
        else
        {
            ChooseNewDirection();
        }
    }

    private IEnumerator WalkCycle()
    {
        while (true)
        {
            // ✅ **随机选择行走时间**
            float walkTime = Random.Range(minWalkTime, maxWalkTime);
            isWalking = true; // 🚶‍♂️ **开始行走**
            ChooseNewDirection();
            yield return new WaitForSeconds(walkTime);

            // ✅ **随机选择停留时间**
            float idleTime = Random.Range(minIdleTime, maxIdleTime);
            isWalking = false; // ⏸ **停止移动**
            yield return new WaitForSeconds(idleTime);
        }
    }

    private void ChooseNewDirection()
    {
        moveDirection = Random.insideUnitSphere; // ✅ 生成一个随机方向
        moveDirection.y = 0f; // ✅ 让村民不往上飞，只在地面移动
        moveDirection.Normalize(); // ✅ 保持方向一致
    }

    private bool IsPathBlocked()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, moveDirection, out hit, raycastDistance))
        {
            if (hit.collider.CompareTag("Villager") == false)
            {
                return true;
            }
        }
        return false;
    }

    private void UpdateRotation()
    {
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, moveDirection * raycastDistance); // ✅ 画出射线检测
    }

    public bool IsWalking()
    {
        return isWalking;
    }
}
