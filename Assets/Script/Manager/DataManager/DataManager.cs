using Newtonsoft.Json;
using System.IO;
using UnityEngine;

[System.Serializable]
public class DataSlotState
{
    public DataManager.SaveSlot currentSlot;
}

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; } // Singleton instance
    [Header("Data Slot")]
    [SerializeField] private DataSlotState _dataSlotState;
    [SerializeField] private SaveSlot currentSlot; // Slot hiện tại để lưu và tải dữ liệu
    [Header("Data")]
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
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Đảm bảo DataManager không bị hủy khi chuyển cảnh
        }
        else
        {
            Destroy(gameObject); // Xóa đi instance khác nếu đã có một instance tồn tại
        }

        Debug.Log("Current slot in Awake: " + currentSlot); // Thêm log kiểm tra

        // Tải dữ liệu khi game bắt đầu
        LoadSlotState();
        if (!CheckIfSlotHasData(currentSlot))
        {
            CreateNewGame(currentSlot); // Tạo dữ liệu mới nếu slot trống
        }
        else
        {
            LoadGame(currentSlot); // Tải dữ liệu nếu slot có dữ liệu
        }
        LoadAudioSettings();
    }

    //------------------------------------------------------------


    public enum SaveSlot
    {
        None,//menu
        SaveData001,
        SaveData002,
        SaveData003
    }
    // Cập nhật slot hiện tại
    private void LoadSlotState()
    {
        _dataSlotState = LoadData<DataSlotState>("SlotState.json");

        if (_dataSlotState != null)
        {
            currentSlot = _dataSlotState.currentSlot;
        }
    }
    private void SaveSlotState()
    {
        _dataSlotState = new DataSlotState { currentSlot = currentSlot };
        SaveData(_dataSlotState, "SlotState.json");
    }

    public void SetCurrentSlot(SaveSlot saveSlot)
    {
        currentSlot = saveSlot;
        SaveSlotState(); // Lưu trạng thái slot mỗi khi cập nhật
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
    private void SaveData<T>(T data, string fileName, SaveSlot saveSlot)
    {
        string folderPath = Application.persistentDataPath + "/" + saveSlot.ToString();
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(folderPath + "/" + fileName, json);
        Debug.Log("Data saved to " + folderPath + "/" + fileName);
    }

    private T LoadData<T>(string fileName, SaveSlot saveSlot)
    {
        string folderPath = Application.persistentDataPath + "/" + saveSlot.ToString();
        string path = folderPath + "/" + fileName;
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
    public void SaveGame(SaveSlot saveSlot)
    {
        SaveData(_playerData, "_playerData.json", saveSlot);
        SaveData(_inventoryData, "_inventoryData.json", saveSlot);
        SaveData(_shopData, "_shopData.json", saveSlot);
        SaveData(_coinData, "_coinData.json", saveSlot);
        SaveData(_tutorialData, "_tutorialData.json", saveSlot);
        SaveData(_teleportData, "_teleportData.json", saveSlot);
        
    }
    
    public void LoadGame(SaveSlot saveSlot)
    {
        _playerData = LoadData<PlayerData>("_playerData.json", saveSlot);
        _inventoryData = LoadData<InventoryData>("_inventoryData.json", saveSlot);
        _shopData = LoadData<ShopData>("_shopData.json", saveSlot);
        _coinData = LoadData<CoinData>("_coinData.json", saveSlot);
        _tutorialData = LoadData<TutorialData>("_tutorialData.json", saveSlot);
        _teleportData = LoadData<TeleportData>("_teleportData.json", saveSlot);
    }

    public void DeleteSave(SaveSlot saveSlot)
    {
        string folderPath = Application.persistentDataPath + "/" + saveSlot.ToString();
        if (Directory.Exists(folderPath))
        {
            Directory.Delete(folderPath, true); // Xóa thư mục và tất cả các file trong đó
            Debug.Log("Deleted save data in slot: " + saveSlot);
        }
        else
        {
            Debug.LogWarning("Save data not found for slot: " + saveSlot);
        }
    }

    //-------------------------------------------------------------
    public bool CheckIfSlotHasData(SaveSlot slot)
    {
        // Danh sách các file cần kiểm tra sự tồn tại
        string[] fileNames = {
        "_playerData.json",
        "_inventoryData.json",
        "_shopData.json",
        "_coinData.json",
        "_tutorialData.json",
        "_teleportData.json"
    };

        // Kiểm tra sự tồn tại của từng file
        foreach (var fileName in fileNames)
        {
            string path = Application.persistentDataPath + "/" + slot.ToString() + "/" + fileName;
            if (File.Exists(path))
            {
                return true; // Nếu bất kỳ file nào tồn tại, trả về true
            }
        }
        return false; // Nếu không có file nào tồn tại, trả về false
    }

    public void CreateNewGame(SaveSlot saveSlot)
    {
        if (!CheckIfSlotHasData(saveSlot))
        {
            // Tạo dữ liệu mới
            _playerData = new PlayerData
            {
                _maxHealth = 1000,
                _currentHealth = 1000,
                _maxMana = 500,
                _currentMana = 500,
                _maxstamina = 100,
                _currentstamina = 100,
                _attack = 200,
                _defense = 150,
                _maxEquipmentWeight = 50.0f,
                _currentEquipmentWeight = 0.0f,
                _positionX = 144,
                _positionY = 120
            };
            _inventoryData = new InventoryData();
            _shopData = new ShopData
            {
                _hpPrice = 100,
                _mpPrice = 100,
                _stmPrice = 100,
                _strPrice = 100,
                _vitPrice = 100
            };
            _coinData = new CoinData { _coin = 10000 }; // Ví dụ: khởi tạo coin với 10000
            _tutorialData = new TutorialData { _1stGame=true  };
            _teleportData = new TeleportData();

            // Lưu dữ liệu mới vào slot
            SaveGame(saveSlot);
        }
    }

    //-------------------------------------------------------------
    public void SavePlayerData() { SaveData(_playerData, "_playerData.json", currentSlot); }
    public void LoadPlayerData() { _playerData = LoadData<PlayerData>("_playerData.json", currentSlot); }

    public void SaveInventoryData() { SaveData(_inventoryData, "_inventoryData.json", currentSlot); }
    public void LoadInventoryData() { _inventoryData = LoadData<InventoryData>("_inventoryData.json", currentSlot); }

    public void SaveShopData() { SaveData(_shopData, "_shopData.json", currentSlot); }
    public void LoadShopData() { _shopData = LoadData<ShopData>("_shopData.json", currentSlot); }

    public void SaveCoinData() { SaveData(_coinData, "_coinData.json", currentSlot); }
    public void LoadCoinData() { _coinData = LoadData<CoinData>("_coinData.json", currentSlot); }

    public void SaveTeleportData() { SaveData(_teleportData, "_teleportData.json", currentSlot); }
    public void LoadTeleportData() { _teleportData = LoadData<TeleportData>("_teleportData.json", currentSlot); }

    public void SaveTutorialData() { SaveData(_tutorialData, "_tutorialData.json", currentSlot); }
    public void LoadTutorialData() 
    { 
        _tutorialData = LoadData<TutorialData>("_tutorialData.json", currentSlot);
        if (_tutorialData == null)
        {
            _tutorialData = new TutorialData
            {
                _1stGame = true
            };
           
        }
    }


    // Phương thức lưu và tải dữ liệu âm thanh
    public void SaveAudioSettings() { SaveData(_audioSetting, "_audioSettings.json"); }
    public void LoadAudioSettings() { _audioSetting = LoadData<AudioSettings>("_audioSettings.json"); }
}
