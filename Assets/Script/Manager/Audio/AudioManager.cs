using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private float _FxGlobalVolume = 1f;
    private float _BgGlobalVolume = 1f; // Thêm biến BGGlobalVolume

    private DataManager _dataManager; // Tham chiếu đến DataManager

    private void Awake()
    {
        // Tìm đối tượng DataManager trong scene
        _dataManager = FindObjectOfType<DataManager>();

        // Tải dữ liệu âm thanh từ DataManager
        LoadAudioSettings();
        // Kiểm tra xem có đối tượng singleton tồn tại không
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Giữ đối tượng không bị hủy khi chuyển scene
        }
        else
        {
            Destroy(gameObject);
        }

        
    }

    private void Start()
    {
        // Áp dụng FXGlobalVolume và BGGlobalVolume từ dữ liệu đã load lên hệ thống âm thanh
        InGameAudioManager.Instance.SetFXGlobalVolume(_FxGlobalVolume);
        BackGroundAudioManager.Instance.SetBgGlobalVolume(_BgGlobalVolume);
        //BackGroundAudioManager.Instance.FirstBgGlobalVolume(_BgGlobalVolume);

    }

    // Phương thức để cài đặt FXGlobalVolume
    public void SetVolume(float volume)
    {
        _FxGlobalVolume = volume;
        InGameAudioManager.Instance.SetFXGlobalVolume(_FxGlobalVolume);
    }

    // Phương thức để cài đặt BGGlobalVolume
    public void SetBGVolume(float volume)
    {
        _BgGlobalVolume = volume;
        BackGroundAudioManager.Instance.SetBgGlobalVolume(_BgGlobalVolume);
    }

    // Lấy FXGlobalVolume
    public float GetVolume()
    {
        return _FxGlobalVolume;
    }

    // Lấy BGGlobalVolume
    public float GetBGVolume()
    {
        return _BgGlobalVolume;
    }

    // Tải dữ liệu âm thanh từ DataManager
    private void LoadAudioSettings()
    {
        if (_dataManager != null)
        {
            // Tải dữ liệu âm thanh từ DataManager
            _dataManager.LoadAudioSettings();
            var audioSettings = _dataManager._audioSetting;

            // Nếu có dữ liệu, cập nhật giá trị âm lượng
            if (audioSettings != null)
            {
                _FxGlobalVolume = audioSettings.FXGlobalVolume;
                _BgGlobalVolume = audioSettings.BGGlobalVolume; // Cập nhật BGGlobalVolume
            }
        }
    }
}
