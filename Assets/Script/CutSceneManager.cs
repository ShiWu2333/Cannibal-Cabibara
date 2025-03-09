using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Collections;

public class CutSceneManager : MonoBehaviour
{
    // 单例模式：全局唯一实例
    public static CutSceneManager Instance { get; private set; }

    public VideoPlayer videoPlayer;      // VideoPlayer 组件引用
    public VideoClip[] videoClips;         // 存储多个视频片段，可在 Inspector 中配置

    [SerializeField] private GameObject backPackPanel;       // 背包界面（BackPackPanel）的引用
    [SerializeField] private GameObject backpackIcon; //背包icon
    [SerializeField] private GameObject cutScenePanel;
    [SerializeField] private GameObject backButton;  // ✅ 拖入 BackButton

    [Header("黑屏 UI")]
    [SerializeField] private Image fadeImage;  // ✅ 直接拖入 `Canvas` 里的 `FadeImage`

    private void Awake()
    {

        // 如果当前实例为空，则设置为本对象，否则销毁重复对象
        if (Instance == null)
        {
            Instance = this;
            // 如果希望管理器在场景切换时保留，可以调用下面这一行：
            // DontDestroyOnLoad(gameObject);
            Debug.Log("CutSceneManager 实例已创建");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayVideo(VideoClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("PlayVideo: 指定的视频片段为 null");
            return;
        }

        // 隐藏背包界面
        if (backPackPanel != null & cutScenePanel != null)
        {
            backPackPanel.SetActive(false);
            cutScenePanel.SetActive(true);
        }

        // 隐藏背包图标
        if (backpackIcon != null)
        {
            backpackIcon.SetActive(false); 
        }

        videoPlayer.clip = clip;
        videoPlayer.Play();
        Debug.Log("PlayVideo(clip) has been played");
    }

    public void PlayVideo(int index)
    {
        Debug.Log("PlayVideo 被调用，索引: " + index);
        if (videoClips == null || index < 0 || index >= videoClips.Length)
        {
            Debug.LogWarning("PlayVideo: 索引越界或 videoClips 数组未设置");
            return;
        }
        Debug.Log("播放视频: " + videoClips[index].name);
        PlayVideo(videoClips[index]);
    }

    public void PlayMemoryVideo(int memoryId)
    {
        // 检查记忆片段是否解锁
        if (BackPackManager.Instance.IsMemoryFragmentUnlocked(memoryId))
        {
            // 假设 memoryId 从1开始，与 videoClips 数组索引对应需要减1
            int index = memoryId - 1;
            if (videoClips != null && index >= 0 && index < videoClips.Length)
            {
                StartCoroutine(PlayVideoWithFade(videoClips[index]));
                Debug.Log("播放记忆视频，memoryId: " + memoryId + ", videoIndex: " + index);
            }
            else
            {
                Debug.LogWarning("视频索引越界或 videoClips 数组未设置");
            }
        }
        else
        {
            Debug.Log("记忆片段 " + memoryId + " 未解锁，无法播放视频。");
        }
    }

    private IEnumerator PlayVideoWithFade(VideoClip clip)
    {
        // 1. 淡入黑屏
        yield return StartCoroutine(FadeToBlack(0.5f));  // ✅ 0.5 秒黑屏

        // 2. 播放视频
        PlayVideo(clip);
        backButton.SetActive(false);

        // 3️⃣ 立刻变透明，让视频可见
        yield return StartCoroutine(InstantFadeFromBlack());

        // 4. 等待视频播放完成
        // 使用 `loopPointReached` 事件等待视频播放完
        bool videoFinished = false;
        videoPlayer.loopPointReached += (VideoPlayer vp) => { videoFinished = true; };

        yield return new WaitUntil(() => videoFinished);

        // 5 确保 StopVideo() 只在视频播放完后调用
        videoPlayer.Stop();

        // 6. 淡出黑屏
        yield return StartCoroutine(FadeFromBlack(0.5f));  // ✅ 0.5 秒淡出
    }

    private IEnumerator FadeToBlack(float duration)
    {
        Debug.Log("调用fadetoblack");
        float elapsedTime = 0f;
        Color color = fadeImage.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(0f, 1f, elapsedTime / duration);
            fadeImage.color = color;
            yield return null;
        }
    }

    private IEnumerator InstantFadeFromBlack()
    {
        if (fadeImage == null) yield break;  // 避免空引用错误

        Color color = fadeImage.color;
        color.a = 0f;
        fadeImage.color = color;

        yield return null;  // 立刻执行
    }

    private IEnumerator FadeFromBlack(float duration)
    {
        Debug.Log("调用fadefromblack");
        float elapsedTime = 0f;
        Color color = fadeImage.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(1f, 0f, elapsedTime / duration);
            fadeImage.color = color;
            yield return null;
        }
    }

    public void PlayMemoryVideoDelayed(int memoryId)
    {
            Debug.Log($"记忆片段 {memoryId} 即将在 1.5 秒后播放...");
            StartCoroutine(PlayMemoryVideoCoroutine(memoryId));
    }

    private IEnumerator PlayMemoryVideoCoroutine(int memoryId)
    {
        yield return new WaitForSeconds(1.5f); // 适当的延迟，避免突兀
        PlayMemoryVideo(memoryId);
    }


    public void PauseVideo()
    {
        videoPlayer.Pause();
    }

    public void ResumeVideo()
    {
        videoPlayer.Play();
    }

    public void StopVideo()
    {
        videoPlayer.Stop();

        // 恢复显示背包界面
        if (backPackPanel != null)
        {
            cutScenePanel.SetActive(false);
            backPackPanel.SetActive(true);
        }
    }
}
