using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TeleportTrigger : MonoBehaviour
{
    [SerializeField] private Transform teleportDestination; // ����Ŀ���
    [SerializeField] private GameObject loadingScreen; // Loading UI
    [SerializeField] private float loadingTime = 1f; // Loading ��Ļ����ʱ��

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // ����Ҵ���
        {
            StartCoroutine(TeleportPlayer(other.gameObject));
        }
    }

    private IEnumerator TeleportPlayer(GameObject player)
    {
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(true); // ��ʾ Loading ����
        }

        yield return new WaitForSeconds(loadingTime); // �ȴ�ָ��ʱ��

        if (teleportDestination != null)
        {
            player.transform.position = teleportDestination.position; // �������
        }
        else
        {
            Debug.LogWarning("Teleport destination not set!");
        }

        if (loadingScreen != null)
        {
            loadingScreen.SetActive(false); // �ر� Loading ����
        }
    }
}
