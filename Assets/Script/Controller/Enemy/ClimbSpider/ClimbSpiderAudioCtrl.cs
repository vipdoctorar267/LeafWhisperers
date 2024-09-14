using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbSpiderAudioCtrl : MonoBehaviour
{
    private AudioSource _audioSource;
    private ClimbSpider _climbSpider; // Tham chiếu tới đối tượng ClimbSpider để lấy trạng thái
    private ClimbSpider.ClimbSpiderState _currentState; // Trạng thái hiện tại của ClimbSpider

    private void Awake()
    {
        // Lấy AudioSource trên object này
        _audioSource = GetComponent<AudioSource>();

        // Lấy tham chiếu tới ClimbSpider
        _climbSpider = GetComponentInParent<ClimbSpider>();
    }

    private void Update()
    {
        // Kiểm tra nếu trạng thái của ClimbSpider thay đổi, update âm thanh
        if (_climbSpider != null && _currentState != _climbSpider.GetCurrentState())
        {
            _currentState = _climbSpider.GetCurrentState();
            UpdateAudioForState(_currentState);
        }
    }

    private void UpdateAudioForState(ClimbSpider.ClimbSpiderState state)
    {
        // Sử dụng InGameAudioManager để lấy AudioClip tương ứng với trạng thái
        FXAudioClipData clipData = InGameAudioManager.Instance.GetClimbSpiderClip(state);

        if (clipData != null)
        {
            _audioSource.clip = clipData.clip;
            _audioSource.volume = clipData.clipVolume * InGameAudioManager.Instance.FXGlobalVolume; // Sử dụng FXGlobalVolume
            _audioSource.loop = clipData.loop;
            _audioSource.Play();
        }
        else
        {
            _audioSource.Stop(); // Nếu không có clip, dừng phát âm thanh
        }
    }
}
