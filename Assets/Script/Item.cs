using System.Collections;
using UnityEngine;

public class Item : MonoBehaviour
{
    public bool isPickedUp = false; // �Ƿ񱻼���
    public string itemName; // ��Ʒ����
    public int itemID; // ��ƷΨһID
    public Sprite itemIcon; // ��Ʒͼ�꣨����UI��
    public bool canBePickedUp = true; // �Ƿ���Ա�ʰȡ

    // ����ѡ�񲿷ֵĲ���
    [SerializeField] private float fadeDuration = 0.5f; // �������ʱ��
    [SerializeField] private Color highlightColor = Color.blue; // ������ɫ
    [SerializeField] private Color selectedColor = Color.yellow; // ѡ����ɫ
    [SerializeField] private float highlightDistance = 5f; // ��������
    [SerializeField] private float selectDistance = 0.5f; // ѡ�о���
    [SerializeField] private float highLightWith = 2f; // �������
    [SerializeField] private float selectedWith = 5f; // ѡ�п��

    // ��������
    [SerializeField] private float breathSpeed = 1f; // �����ٶ�
    [SerializeField] private float minAlpha = 0.2f; // ��С���
    [SerializeField] private float maxAlpha = 0.8f; // �����

    [SerializeField] private Outline outline;
    private bool isHighlighted = false; // �Ƿ����
    private bool isSelected = false; // �Ƿ�ѡ��
    private GameObject player; // ������Ҷ���

    private void Start()
    {
        if (outline != null)
        {
            outline.OutlineMode = Outline.Mode.OutlineVisible;
            outline.OutlineColor = highlightColor; // ���ó�ʼ��ɫ
            outline.OutlineWidth = highLightWith; // ���ó�ʼ���
            outline.enabled = false; // ��ʼ�����ⷢ��
        }
        else
        {
            Debug.LogError("Failed to find outline");
        }

        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("Failed to find player");
        }
    }

    private void Update()
    {
        if (isPickedUp) return; // �����Ʒ�ѱ�����������

        // ��ȡ���λ��
        if (player == null) return;

        // �������������ľ���
        float distance = Vector3.Distance(player.transform.position, transform.position);

        // �ж��Ƿ����
        if (distance <= highlightDistance)
        {
            if (!isHighlighted)
            {
                Highlight(); // ��������
            }

            // �ж��Ƿ�ѡ��
            if (IsInSelectionBox())
            {
                if (!isSelected)
                {
                    Select(); // ѡ������
                }
            }
            else
            {
                if (isSelected)
                {
                    Deselect(); // ȡ��ѡ��
                }
            }

            // ����ʱ���ú���Ч����ѡ��ʱֹͣ����Ч��
            if (outline != null && outline.enabled)
            {
                if (isHighlighted && !isSelected) // ���ڸ�����δѡ��ʱ���ú���Ч��
                {
                    float breath = (Mathf.Sin(Time.time * breathSpeed) + 1) * 0.5f; // ����Χ�� [-1, 1] ӳ�䵽 [0, 1]
                    float alpha = Mathf.Lerp(minAlpha, maxAlpha, breath); // ����Сֵ�����ֵ֮���ֵ

                    // ���� OutlineColor �� Alpha ֵ
                    Color currentColor = outline.OutlineColor;
                    currentColor.a = alpha; // �޸� Alpha ֵ
                    outline.OutlineColor = currentColor;
                }
                else if (isSelected) // ѡ��ʱ�̶� Alpha ֵΪ���ֵ
                {
                    Color currentColor = outline.OutlineColor;
                    currentColor.a = maxAlpha; // �̶� Alpha ֵ
                    outline.OutlineColor = currentColor;
                }
            }
        }
        else
        {
            if (isHighlighted)
            {
                RemoveHighlight(); // ȡ������
            }
        }
    }

    private bool IsInSelectionBox()
    {
        if (player == null) return false;

        // ��ȡ PlayerMovement �ű�
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement == null) return false;

        // �������������ĵ㣨�� PlayerMovement һ�£�
        Vector3 boxCenter = player.transform.position + player.transform.forward * 0.5f;

        // ��������Ƿ���������������
        Collider[] hitColliders = Physics.OverlapBox(boxCenter, new Vector3(playerMovement.selectDistance, playerMovement.selectDistance, 0.5f), player.transform.rotation);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject == gameObject)
            {
                return true;
            }
        }
        return false;
    }

    private void Select()
    {
        if (outline != null)
        {
            outline.OutlineColor = selectedColor; // ����ѡ����ɫ
            outline.OutlineWidth = selectedWith; // ����ѡ�п��

            // ѡ��ʱ�̶� Alpha ֵΪ���ֵ
            Color currentColor = outline.OutlineColor;
            currentColor.a = maxAlpha; // �̶� Alpha ֵ
            outline.OutlineColor = currentColor;

            isSelected = true;
        }
    }

    private void Deselect()
    {
        if (outline != null)
        {
            outline.OutlineColor = highlightColor; // �ָ�������ɫ
            outline.OutlineWidth = highLightWith; // �ָ��������

            // ȡ��ѡ��ʱ�ָ�����Ч��
            Color currentColor = outline.OutlineColor;
            currentColor.a = maxAlpha; // ��ʼ����Ϊ���ֵ
            outline.OutlineColor = currentColor;

            isSelected = false;
        }
    }

    private void Highlight()
    {
        if (outline != null)
        {
            outline.enabled = true; // �����ⷢ��
            outline.OutlineColor = highlightColor; // ���ø�����ɫ
            outline.OutlineWidth = highLightWith; // ���ø������
            isHighlighted = true;

            // �����������Ч��
            StartCoroutine(FadeOutline(true));
            Debug.Log("Highlight enabled.");
        }
    }

    private void RemoveHighlight()
    {
        if (outline != null)
        {
            isHighlighted = false;
            isSelected = false;

            // ��������䰵Ч��
            StartCoroutine(FadeOutline(false));
            Debug.Log("Highlight disabled.");
        }
    }

    public void OnPickedUp()
    {
        RemoveHighlight();
    }

    private IEnumerator FadeOutline(bool fadeIn)
    {
        float startAlpha = outline.OutlineColor.a; // ��ǰ Alpha ֵ
        float targetAlpha = fadeIn ? maxAlpha : 0f; // Ŀ�� Alpha ֵ
        float elapsedTime = 0f; // ����ʱ��

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / fadeDuration); // �����ֵ����

            // ���� Alpha ֵ
            Color currentColor = outline.OutlineColor;
            currentColor.a = Mathf.Lerp(startAlpha, targetAlpha, t);
            outline.OutlineColor = currentColor;

            yield return null; // �ȴ���һ֡
        }

        // ȷ������ Alpha ֵ��ȷ
        Color finalColor = outline.OutlineColor;
        finalColor.a = targetAlpha;
        outline.OutlineColor = finalColor;

        // �������������ǹرղ���������� Outline
        if (!fadeIn)
        {
            outline.enabled = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // ���Ƹ�����Χ
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, highlightDistance);

        // ����ѡ�з�Χ���� PlayerMovement һ�£�
        if (player != null)
        {
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                Vector3 boxCenter = player.transform.position + player.transform.forward * 0.5f;
                Gizmos.color = Color.yellow;
                Gizmos.matrix = Matrix4x4.TRS(boxCenter, player.transform.rotation, Vector3.one);
                Gizmos.DrawWireCube(Vector3.zero, new Vector3(playerMovement.selectDistance * 2, playerMovement.selectDistance * 2, 1f)); // �� 1f ��������
            }
        }
    }
}