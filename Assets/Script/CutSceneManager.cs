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

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
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

        // 隐藏背包界面，显示过场动画界面
        if (backPackPanel != null && cutScenePanel != null)
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
        Debug.Log("播放视频：" + clip.name);
    }

    public void PlayVideo(int index)
    {
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
        if (BackPackManager.Instance.IsMemoryFragmentUnlocked(memoryId))
        {
            int index = memoryId - 1;
            if (videoClips != null && index >= 0 && index < videoClips.Length)
            {
                StartCoroutine(PlayVideoSequence(videoClips[index]));
                Debug.Log($"播放记忆视频: Memory ID {memoryId}, Video Index {index}");
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

    private IEnumerator PlayVideoSequence(VideoClip clip)
    {
        PlayVideo(clip);
        backButton.SetActive(false);

        bool videoFinished = false;
        videoPlayer.loopPointReached += (VideoPlayer vp) => { videoFinished = true; };

        yield return new WaitUntil(() => videoFinished);

        videoPlayer.Stop();

        if (cutScenePanel != null)
        {
            cutScenePanel.SetActive(false);
        }

        // ✅ **确保背包图标是激活状态**
        if (backpackIcon != null)
        {
            backpackIcon.SetActive(true);
        }

        // ✅ **让背包图标闪烁**
        if (backpackIcon != null)
        {
            BackpackIconBlink blinkScript = backpackIcon.GetComponent<BackpackIconBlink>();
            if (blinkScript != null)
            {
                blinkScript.StartBlinking();
            }
        }
    }

    public void PlayMemoryVideoDelayed(int memoryId)
    {
        Debug.Log($"记忆片段 {memoryId} 即将在 1.5 秒后播放...");
        StartCoroutine(PlayMemoryVideoCoroutine(memoryId));
    }

    private IEnumerator PlayMemoryVideoCoroutine(int memoryId)
    {
        yield return new WaitForSeconds(1.5f);
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
