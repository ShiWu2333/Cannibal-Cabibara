using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TeleportTrigger : MonoBehaviour
{
    [SerializeField] private Transform teleportDestination; // 传送目标点
    [SerializeField] private GameObject loadingScreen; // Loading UI
    [SerializeField] private float loadingTime = 1f; // Loading 屏幕持续时间

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // 仅玩家触发
        {
            StartCoroutine(TeleportPlayer(other.gameObject));
        }
    }

    private IEnumerator TeleportPlayer(GameObject player)
    {
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(true); // 显示 Loading 画面
        }

        yield return new WaitForSeconds(loadingTime); // 等待指定时间

        if (teleportDestination != null)
        {
            player.transform.position = teleportDestination.position; // 传送玩家
        }
        else
        {
            Debug.LogWarning("Teleport destination not set!");
        }

        if (loadingScreen != null)
        {
            loadingScreen.SetActive(false); // 关闭 Loading 画面
        }
    }
}
