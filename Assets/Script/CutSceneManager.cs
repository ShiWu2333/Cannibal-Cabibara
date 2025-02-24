using UnityEngine;
using UnityEngine.Video;

public class CutSceneManager : MonoBehaviour
{
    // 单例模式：全局唯一实例
    public static CutSceneManager Instance { get; private set; }

    public VideoPlayer videoPlayer;      // VideoPlayer 组件引用
    public VideoClip[] videoClips;         // 存储多个视频片段，可在 Inspector 中配置

    [SerializeField] private GameObject backPackPanel;       // 背包界面（BackPackPanel）的引用
    [SerializeField] private GameObject cutScenePanel;

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
                PlayVideo(videoClips[index]);
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
