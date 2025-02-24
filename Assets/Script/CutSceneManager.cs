using UnityEngine;
using UnityEngine.Video;

public class CutSceneManager : MonoBehaviour
{
    // ����ģʽ��ȫ��Ψһʵ��
    public static CutSceneManager Instance { get; private set; }

    public VideoPlayer videoPlayer;      // VideoPlayer �������
    public VideoClip[] videoClips;         // �洢�����ƵƬ�Σ����� Inspector ������

    [SerializeField] private GameObject backPackPanel;       // �������棨BackPackPanel��������
    [SerializeField] private GameObject cutScenePanel;

    private void Awake()
    {

        // �����ǰʵ��Ϊ�գ�������Ϊ�����󣬷��������ظ�����
        if (Instance == null)
        {
            Instance = this;
            // ���ϣ���������ڳ����л�ʱ���������Ե���������һ�У�
            // DontDestroyOnLoad(gameObject);
            Debug.Log("CutSceneManager ʵ���Ѵ���");
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
            Debug.LogWarning("PlayVideo: ָ������ƵƬ��Ϊ null");
            return;
        }

        // ���ر�������
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
        Debug.Log("PlayVideo �����ã�����: " + index);
        if (videoClips == null || index < 0 || index >= videoClips.Length)
        {
            Debug.LogWarning("PlayVideo: ����Խ��� videoClips ����δ����");
            return;
        }
        Debug.Log("������Ƶ: " + videoClips[index].name);
        PlayVideo(videoClips[index]);
    }

    public void PlayMemoryVideo(int memoryId)
    {
        // ������Ƭ���Ƿ����
        if (BackPackManager.Instance.IsMemoryFragmentUnlocked(memoryId))
        {
            // ���� memoryId ��1��ʼ���� videoClips ����������Ӧ��Ҫ��1
            int index = memoryId - 1;
            if (videoClips != null && index >= 0 && index < videoClips.Length)
            {
                PlayVideo(videoClips[index]);
                Debug.Log("���ż�����Ƶ��memoryId: " + memoryId + ", videoIndex: " + index);
            }
            else
            {
                Debug.LogWarning("��Ƶ����Խ��� videoClips ����δ����");
            }
        }
        else
        {
            Debug.Log("����Ƭ�� " + memoryId + " δ�������޷�������Ƶ��");
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

        // �ָ���ʾ��������
        if (backPackPanel != null)
        {
            cutScenePanel.SetActive(false);
            backPackPanel.SetActive(true);
        }
    }
}
