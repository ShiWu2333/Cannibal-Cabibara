using UnityEngine;
using System.Collections;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;  // 目标（默认是玩家）
    public Vector3 offset = new Vector3(0, 2, -4); // 相机偏移
    public float transitionSpeed = 2.0f; // 过渡平滑速度
    private Transform defaultTarget; // 记录默认目标（玩家）
    private Quaternion lockedRotation; // 记录初始相机旋转
    public bool lockRotation = true; // 是否锁定相机旋转
    public float instantTeleportDistance = 500f; // 超过此距离相机瞬移

    void Start()
    {
        defaultTarget = target; // 记录默认的目标（玩家）
        lockedRotation = transform.rotation; // 记录初始相机旋转

        // 监听玩家被抓事件
        PoliceAI.OnPlayerCaught += HandlePlayerCaught;
    }

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;

            // **距离检测：如果相机距离目标超过 `instantTeleportDistance`，直接瞬移**
            if (Vector3.Distance(transform.position, desiredPosition) > instantTeleportDistance)
            {
                transform.position = desiredPosition; // 直接瞬移
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * transitionSpeed);
            }

            if (!lockRotation)
            {
                transform.LookAt(target.position); // 只有当 lockRotation 为 false 时才旋转
            }
            else
            {
                transform.rotation = lockedRotation; // 维持原本的旋转角度
            }
        }
    }

    public void SetTarget(Transform newTarget, float duration)
    {
        StopAllCoroutines(); // 确保不会有多个协程同时运行
        StartCoroutine(SwitchTarget(newTarget, duration));
    }

    private IEnumerator SwitchTarget(Transform newTarget, float duration)
    {
        PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.PauseInput();
        }
        else
        {
            Debug.LogError("PlayerMovement 脚本未找到，请确保 Player 上有此组件！");
        }

        target = newTarget; // 切换目标

        yield return new WaitForSeconds(duration);

        if (playerMovement != null)
        {
            playerMovement.ResumeInput();
        }
        else
        {
            Debug.LogError("PlayerMovement 脚本未找到，请确保 Player 上有此组件！");
        }

        target = defaultTarget; // 恢复为默认目标（玩家）
    }

    private void HandlePlayerCaught(Transform policeTransform)
    {
        Debug.Log("相机切换到警察视角！");
        SetTarget(policeTransform, 2f); // 设定相机目标为警察，并在2秒后切换回玩家
    }

    private void OnDestroy()
    {
        // 取消事件监听，防止错误
        PoliceAI.OnPlayerCaught -= HandlePlayerCaught;
    }
}
