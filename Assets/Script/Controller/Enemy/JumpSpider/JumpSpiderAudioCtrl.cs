using UnityEngine;

public class JumpSpiderAudioCtrl : MonoBehaviour
{
    private AudioSource _audioSource;
    private JumpSpider _jumpSpider; // Tham chiếu tới đối tượng JumpSpider để lấy trạng thái
    private JumpSpider.JumpSpiderState _currentState; // Trạng thái hiện tại của JumpSpider

    private void Awake()
    {
        // Lấy AudioSource trên object này
        _audioSource = GetComponent<AudioSource>();

        // Lấy tham chiếu tới JumpSpider
        _jumpSpider = GetComponentInParent<JumpSpider>();
    }

    private void Update()
    {
        // Kiểm tra nếu trạng thái của JumpSpider thay đổi, update âm thanh
        if (_jumpSpider != null)
        {
            JumpSpider.JumpSpiderState newState = _jumpSpider.GetCurrentState();
            if (_currentState != newState)
            {
                _currentState = newState;
                UpdateAudioForState(_currentState);
            }
        }
    }

    private void UpdateAudioForState(JumpSpider.JumpSpiderState state)
    {
        // Sử dụng InGameAudioManager để lấy AudioClip tương ứng với trạng thái
        FXAudioClipData clipData = InGameAudioManager.Instance.GetJumpSpiderClip(state);

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
