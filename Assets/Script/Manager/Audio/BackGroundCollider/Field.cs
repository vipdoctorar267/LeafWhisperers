using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{
    private BGAudioCtrl _bGAudioCtrl;
    

    private void Start()
    {
        // Tìm và gán giá trị cho _bGAudioCtrl nếu chưa được gán
        if (_bGAudioCtrl == null)
        {
            _bGAudioCtrl = FindObjectOfType<BGAudioCtrl>();
        }

        // Xác định trạng thái của khu vực này
         
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Khi phát hiện người chơi, thay đổi âm thanh nền
            if (_bGAudioCtrl != null)
            {
                _bGAudioCtrl.ChangeBGState(BackGroundAudioManager.BGState.Field);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Kiểm tra trạng thái hiện tại từ Singleton và so sánh với trạng thái của khu vực
            if (_bGAudioCtrl != null && BackGroundAudioManager.Instance._currentState != BackGroundAudioManager.BGState.Field )
            {
                _bGAudioCtrl.ChangeBGState(BackGroundAudioManager.BGState.Field);
            }
        }
    }

}
