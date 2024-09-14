using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Village : MonoBehaviour
{
    private BGAudioCtrl _bGAudioCtrl;
    

    private void Start()
    {
        // Tìm và gán giá trị cho _bGAudioCtrl nếu chưa được gán
        if (_bGAudioCtrl == null)
        {
            _bGAudioCtrl = FindObjectOfType<BGAudioCtrl>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Khi phát hiện người chơi, thay đổi âm thanh nền
            if (_bGAudioCtrl != null)
            {
                _bGAudioCtrl.ChangeBGState(BackGroundAudioManager.BGState.Village);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Kiểm tra trạng thái hiện tại từ Singleton và so sánh với trạng thái của khu vực
            if (_bGAudioCtrl != null && BackGroundAudioManager.Instance._currentState != BackGroundAudioManager.BGState.Village)
            {  
                _bGAudioCtrl.ChangeBGState(BackGroundAudioManager.BGState.Village);
            }
        }
    }

 
}
