using UnityEngine;

public class RunSpiderAudioCtrl : MonoBehaviour
{
    private AudioSource _audioSource;
    private RunSpider _runSpider; // Tham chiếu tới đối tượng RunSpider để lấy trạng thái
    private RunSpider.RunSpiderState _currentState; // Trạng thái hiện tại của RunSpider

    private void Awake()
    {
        // Lấy AudioSource trên object này
        _audioSource = GetComponent<AudioSource>();

        // Lấy tham chiếu tới RunSpider
        _runSpider = GetComponentInParent<RunSpider>();
    }

    private void Update()
    {
        // Kiểm tra nếu trạng thái của RunSpider thay đổi, update âm thanh
        if (_runSpider != null)
        {
            RunSpider.RunSpiderState newState = _runSpider.GetCurrentState();
            if (_currentState != newState)
            {
                _currentState = newState;
                UpdateAudioForState(_currentState);
            }
        }
    }

    private void UpdateAudioForState(RunSpider.RunSpiderState state)
    {
        // Sử dụng InGameAudioManager để lấy AudioClip tương ứng với trạng thái
        FXAudioClipData clipData = InGameAudioManager.Instance.GetRunSpiderClip(state);

        if (clipData != null && clipData.clip != null)
        {
            // Đặt thuộc tính âm thanh của AudioSource
            _audioSource.clip = clipData.clip;
            _audioSource.volume = clipData.clipVolume * InGameAudioManager.Instance.FXGlobalVolume;
            _audioSource.loop = clipData.loop;

            // Phát âm thanh nếu nó không đang phát
            if (!_audioSource.isPlaying)
            {
                _audioSource.Play();
            }
        }
        else
        {
            // Nếu không có clip, dừng phát âm thanh nếu đang phát
            if (_audioSource.isPlaying)
            {
                _audioSource.Stop();
            }
        }
    }
}
