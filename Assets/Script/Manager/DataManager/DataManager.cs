using Newtonsoft.Json;
using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; } // Singleton instance

    public PlayerData _playerData;
    public InventoryData _inventoryData;
    public ShopData _shopData;
    public CoinData _coinData;
    public TutorialData _tutorialData;
    public TeleportData _teleportData;
    
    //----------------------------------------
    public AudioSettings _audioSetting; // Thêm biến này để chứa dữ liệu âm thanh

    private void Awake()
    {
        // Nếu Instance đã được thiết lập và khác null, kiểm tra xem có đang tồn tại một instance khác không
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Đảm bảo DataManager không bị hủy khi chuyển cảnh
        }
        else
        {
            Destroy(gameObject); // Xóa đi instance khác nếu đã có một instance tồn tại
        }

        // Tải dữ liệu khi game bắt đầu
        LoadPlayerData();
        LoadInventoryData();
        LoadShopData();
        LoadCoinData();
        LoadTeleportData();
        LoadTutorialData();
        LoadAudioSettings(); // Đảm bảo gọi phương thức này để tải dữ liệu âm thanh
    }

    //------------------------------------------------------------

    private void SaveData<T>(T data, string fileName)
    {
        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(Application.persistentDataPath + "/" + fileName, json);
        Debug.Log("Data saved to " + Application.persistentDataPath + "/" + fileName);
    }

    private T LoadData<T>(string fileName)
    {
        string path = Application.persistentDataPath + "/" + fileName;
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            Debug.Log("Data loaded from " + path);
            return JsonConvert.DeserializeObject<T>(json);
        }
        else
        {
            Debug.LogWarning("No data found at " + path);
            return default(T);
        }
    }

    //-------------------------------------------------------------

    public void SavePlayerData() { SaveData(_playerData, "_playerManager.json"); }
    public void LoadPlayerData() { _playerData = LoadData<PlayerData>("_playerManager.json"); }

    public void SaveInventoryData() { SaveData(_inventoryData, "_inventoryData.json"); }
    public void LoadInventoryData() { _inventoryData = LoadData<InventoryData>("_inventoryData.json"); }

    public void SaveShopData() { SaveData(_shopData, "_shopData.json"); }
    public void LoadShopData() { _shopData = LoadData<ShopData>("_shopData.json"); }

    public void SaveCoinData() { SaveData(_coinData, "_coinData.json"); }
    public void LoadCoinData() { _coinData = LoadData<CoinData>("_coinData.json"); }

    public void SaveTeleportData() { SaveData(_teleportData, "_teleportData.json"); }
    public void LoadTeleportData() { _teleportData = LoadData<TeleportData>("_teleportData.json"); }

    public void SaveTutorialData() { SaveData(_tutorialData, "_tutorialData.json"); }
    public void LoadTutorialData() { _tutorialData = LoadData<TutorialData>("_tutorialData.json"); }

    // Phương thức lưu và tải dữ liệu âm thanh
    public void SaveAudioSettings() { SaveData(_audioSetting, "_audioSettings.json"); }
    public void LoadAudioSettings() { _audioSetting = LoadData<AudioSettings>("_audioSettings.json"); }
}
