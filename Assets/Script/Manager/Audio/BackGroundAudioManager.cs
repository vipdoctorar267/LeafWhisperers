using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BGAudioClipData
{
    public string name;   // Tên của clip
    public AudioClip clip; // AudioClip thực tế
    public bool loop;      // Có lặp lại hay không
    public float clipVolume = 1f;  // Volume mặc định của clip
}

public class BackGroundAudioManager : MonoBehaviour
{
    public static BackGroundAudioManager Instance { get; private set; }

    [SerializeField] private List<BGAudioClipData> _bgAudioClips;  // Danh sách các clip nền
    [SerializeField] private float _globalVolume = 1f; // Âm lượng tổng quát (0-1)

    public enum BGState { MainMenu, Village, Field, /*Cave,*/ Fight } // Trạng thái âm thanh nền

    private Dictionary<BGState, BGAudioClipData> _bgAudioClipDict = new Dictionary<BGState, BGAudioClipData>();

    public BGState _currentState;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        InitializeAudioClipDict();
    }

    // Khởi tạo dictionary với các clip tương ứng với BGState
    private void InitializeAudioClipDict()
    {
        foreach (var clipData in _bgAudioClips)
        {
            // Phân loại clip dữ liệu dựa trên tên
            switch (clipData.name)
            {
                case "MainMenuClip":
                    _bgAudioClipDict[BGState.MainMenu] = clipData;
                    break;
                case "VillageClip":
                    _bgAudioClipDict[BGState.Village] = clipData;
                    break;
                case "FieldClip":
                    _bgAudioClipDict[BGState.Field] = clipData;
                    break;
                //case "CaveClip":
                //    _bgAudioClipDict[BGState.Cave] = clipData;
                //    break;
                case "FightClip":
                    _bgAudioClipDict[BGState.Fight] = clipData;
                    break;
            }
        }
    }

    // Trả về dữ liệu clip dựa trên trạng thái BG
    public BGAudioClipData GetBGClip(BGState state)
    {
        if (_bgAudioClipDict.TryGetValue(state, out BGAudioClipData clipData))
        {
            return clipData;
        }
        return null;
    }

    // Trả về âm lượng tổng hợp (clip volume * global volume)
    public float GetAdjustedVolume(BGAudioClipData clipData)
    {
        return clipData.clipVolume * _globalVolume;
    }
    public void RequestReloadAudioClips()
    {
        // Yêu cầu BGAudioCtrl tải lại tất cả các clip
        BGAudioCtrl audioCtrl = FindObjectOfType<BGAudioCtrl>();
        if (audioCtrl != null)
        {
            audioCtrl.ReloadAllAudioClips();
        }
    }


    // Đặt giá trị âm lượng tổng quát
    public void FirstBgGlobalVolume(float volume)
    {
        _globalVolume = Mathf.Clamp01(volume);
        
    }
    public void SetBgGlobalVolume(float volume)
    {
        _globalVolume = Mathf.Clamp01(volume); 
        RequestReloadAudioClips();
    }


    // Lấy giá trị âm lượng tổng quát
    public float GetBgGlobalVolume()
    {
        return _globalVolume;
    }

    public BGState GetBgCurrentState()
    {
        return _currentState;
    }
}
