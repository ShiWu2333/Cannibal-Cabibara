using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BubbleController : MonoBehaviour
{
    public GameObject bubbleCanvas; // UI ���� Canvas
    public Image bubbleIcon; // ��ʾ�� Icon
    public Sprite defaultIcon; // Ĭ�� Icon

    public Image bubbleBackground1; // ��һ�㱳��
    public Image bubbleBackground2; // �ڶ��㱳��
    public Image bubbleBackground3; // �����㱳��

    private Coroutine showBubbleCoroutine; // ��¼������ Coroutine����ֹ�ظ�����
    private Coroutine hideCoroutine; // ��¼���ص� Coroutine

    private float fadeDuration = 0.2f;
    private float waitSeconds = 0.1f;

    void Start()
    {
        if (bubbleCanvas == null || bubbleIcon == null || bubbleBackground1 == null || bubbleBackground2 == null || bubbleBackground3 == null)
        {
            Debug.LogError("BubbleController: �������δ��ֵ��");
            return;
        }

        HideBubbleInstantly(); // **��ʼ������������Ԫ��**
    }

    public void ShowBubble(Sprite newIcon)
    {
        if (showBubbleCoroutine != null)
        {
            StopCoroutine(showBubbleCoroutine);
        }

        showBubbleCoroutine = StartCoroutine(AnimateBubble(newIcon));

        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }
        hideCoroutine = StartCoroutine(HideBubbleWithDelay(5f)); // 3 �������
    }

    private IEnumerator AnimateBubble(Sprite newIcon)
    {
        bubbleCanvas.SetActive(true);
        SetAlpha(bubbleBackground1, 0);
        SetAlpha(bubbleBackground2, 0);
        SetAlpha(bubbleBackground3, 0);
        SetAlpha(bubbleIcon, 0);

        yield return StartCoroutine(FadeIn(bubbleBackground1, fadeDuration));
        yield return new WaitForSeconds(waitSeconds);

        yield return StartCoroutine(FadeIn(bubbleBackground2, fadeDuration));
        yield return new WaitForSeconds(waitSeconds);

        bubbleIcon.sprite = newIcon; // **�ȸ�ֵ**
        StartCoroutine(FadeIn(bubbleIcon, fadeDuration)); // **���ȴ�**
        yield return StartCoroutine(FadeIn(bubbleBackground3, fadeDuration));
    }

    private IEnumerator HideBubbleWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        yield return StartCoroutine(FadeOut(bubbleBackground1, fadeDuration));
        yield return new WaitForSeconds(waitSeconds);

        yield return StartCoroutine(FadeOut(bubbleBackground2, fadeDuration));
        yield return new WaitForSeconds(waitSeconds);

        StartCoroutine(FadeOut(bubbleIcon, fadeDuration)); // **���ȴ�**
        yield return StartCoroutine(FadeOut(bubbleBackground3, fadeDuration));

        bubbleCanvas.SetActive(false);
    }

    public void HideBubbleInstantly()
    {
        bubbleCanvas.SetActive(false);
        SetAlpha(bubbleBackground1, 0);
        SetAlpha(bubbleBackground2, 0);
        SetAlpha(bubbleBackground3, 0);
        SetAlpha(bubbleIcon, 0);
    }

    private IEnumerator FadeIn(Image image, float duration)
    {
        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            SetAlpha(image, time / duration);
            yield return null;
        }
        SetAlpha(image, 1);
    }

    private IEnumerator FadeOut(Image image, float duration)
    {
        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            SetAlpha(image, 1 - (time / duration));
            yield return null;
        }
        SetAlpha(image, 0);
    }

    private void SetAlpha(Image image, float alpha)
    {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }
}
