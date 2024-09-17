using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BGAudioCtrl : MonoBehaviour
{
    private AudioSource _audioSource;

    [SerializeField]
    private float fadeDuration = 2f; // Thời gian fade in/out

    private Dictionary<BackGroundAudioManager.BGState, AudioClip> _clipDictionary;
    private Dictionary<BackGroundAudioManager.BGState, float> _clipVolumeDictionary;
    private Dictionary<BackGroundAudioManager.BGState, bool> _clipLoopDictionary;

    private void Awake()
    {
        
    }

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        LoadAllAudioClips();
        StartCoroutine(PreloadAllAudioClips());
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (currentSceneName == "MainMenu")
        {
            Debug.Log("----------------Start-----------------");
            StartScene(BackGroundAudioManager.BGState.MainMenu);
        }
        else if (currentSceneName == "MainGame")
        {
            Debug.Log("----------------Start-----------------");
            StartScene(BackGroundAudioManager.BGState.Village);
        }
    }

    // Load tất cả các clip âm thanh và dữ liệu vào từ điển cục bộ
    private void LoadAllAudioClips()
    {
        _clipDictionary = new Dictionary<BackGroundAudioManager.BGState, AudioClip>();
        _clipVolumeDictionary = new Dictionary<BackGroundAudioManager.BGState, float>();
        _clipLoopDictionary = new Dictionary<BackGroundAudioManager.BGState, bool>();

        foreach (BackGroundAudioManager.BGState state in System.Enum.GetValues(typeof(BackGroundAudioManager.BGState)))
        {
            BGAudioClipData clipData = BackGroundAudioManager.Instance.GetBGClip(state);
            if (clipData != null)
            {
                _clipDictionary[state] = clipData.clip;
                _clipVolumeDictionary[state] = clipData.clipVolume;
                _clipLoopDictionary[state] = clipData.loop;
            }
        }
    }

    public void ReloadAllAudioClips()
    {
        StartCoroutine(PreloadAllAudioClips()); // Tải lại tất cả các clip
        StartScene(BackGroundAudioManager.Instance._currentState); // Phát nhạc nền theo trạng thái hiện tại
    }

    private IEnumerator PreloadAllAudioClips()
    {
        foreach (BackGroundAudioManager.BGState state in _clipDictionary.Keys)
        {
            AudioClip clip = _clipDictionary[state];
            if (clip != null)
            {
                _audioSource.clip = clip;
                _audioSource.loop = _clipLoopDictionary[state];
                _audioSource.volume = 0; // Phát với âm lượng 0
                _audioSource.Play();

                // Đợi cho đến khi clip bắt đầu phát để đảm bảo nó đã được nạp vào bộ nhớ
                yield return new WaitForSeconds(clip.length);
            }
        }
    }

    private float GetAdjustedVolume(BackGroundAudioManager.BGState state)
    {
        if (_clipVolumeDictionary.TryGetValue(state, out float clipVolume))
        {
            float globalVolume = BackGroundAudioManager.Instance.GetBgGlobalVolume();
            return clipVolume * globalVolume;
        }
        return 0f; // Nếu không tìm thấy clip, trả về 0
    }

    public void StartScene(BackGroundAudioManager.BGState StnewState)
    {
        BackGroundAudioManager.Instance._currentState = StnewState;

        if (_clipDictionary.TryGetValue(StnewState, out AudioClip initialClip))
        {
            if (initialClip != null)
            {
                _audioSource.clip = initialClip;
                _audioSource.loop = _clipLoopDictionary[StnewState];
                _audioSource.volume = GetAdjustedVolume(StnewState); // Đặt âm lượng với globalVolume
                _audioSource.Play();
            }
        }
    }

    public void ChangeBGState(BackGroundAudioManager.BGState newState)
    {
        if (newState != BackGroundAudioManager.Instance._currentState)
        {
            BackGroundAudioManager.Instance._currentState = newState;
            StartCoroutine(FadeOutAndPlayNewClip(newState));
        }
    }

    private IEnumerator FadeOutAndPlayNewClip(BackGroundAudioManager.BGState newState)
    {
        float startVolume = _audioSource.volume;
        float elapsedTime = 0f;

        // Fade out âm lượng hiện tại
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            _audioSource.volume = Mathf.Lerp(startVolume, 0, elapsedTime / fadeDuration);
            yield return null;
        }
        _audioSource.volume = 0; // Đảm bảo âm lượng về 0 sau khi fade out

        // Đổi clip nếu tồn tại trong từ điển
        if (_clipDictionary.TryGetValue(newState, out AudioClip newClip) && newClip != null)
        {
            if (_clipLoopDictionary.TryGetValue(newState, out bool shouldLoop))
            {
                _audioSource.clip = newClip;
                _audioSource.loop = shouldLoop;
                _audioSource.volume = 0; // Đặt âm lượng bắt đầu từ 0
                _audioSource.Play(); // Bắt đầu phát clip mới
            }
            else
            {
                Debug.LogWarning($"Không tìm thấy thông tin loop cho trạng thái: {newState}");
            }
        }
        else
        {
            Debug.LogWarning($"Không tìm thấy clip cho trạng thái: {newState}");
        }

        // Fade in âm lượng của clip mới
        float targetVolume = GetAdjustedVolume(newState); // Sử dụng _FxGlobalVolume và âm lượng clip
        elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            _audioSource.volume = Mathf.Lerp(0, targetVolume, elapsedTime / fadeDuration);
            yield return null;
        }
        _audioSource.volume = targetVolume; // Đảm bảo âm lượng đạt giá trị mục tiêu
    }
}
