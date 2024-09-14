using UnityEngine;

public class CharAudioCtrl : MonoBehaviour
{
    private AudioSource _audioSource;
    private CharacterStateMachine _character; // Tham chiếu tới đối tượng Character để lấy trạng thái
    private CharacterStateMachine.CharFxState _currentState; // Trạng thái hiện tại của Character

    private void Awake()
    {
        // Lấy AudioSource trên object này
        _audioSource = GetComponent<AudioSource>();

        // Lấy tham chiếu tới CharacterStateMachine
        _character = GetComponentInParent<CharacterStateMachine>();
    }

    private void Update()
    {
        // Kiểm tra nếu trạng thái của Character thay đổi, update âm thanh
        if (_character != null)
        {
            CharacterStateMachine.CharFxState newState = _character.GetCurrentState();
            if (_currentState != newState)
            {
                _currentState = newState;
                UpdateAudioForState(_currentState);
            }
        }
    }

    private void UpdateAudioForState(CharacterStateMachine.CharFxState state)
    {
        // Sử dụng InGameAudioManager để lấy AudioClip tương ứng với trạng thái
        FXAudioClipData clipData = InGameAudioManager.Instance.GetCharacterClip(state);

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
