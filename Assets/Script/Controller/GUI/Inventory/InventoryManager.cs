using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class InventoryData
{
    public int _hpCount;
    public int _mpCount;
    public int _stmCount;
    public int _hp1 = 100;
    public int _mp1 = 100;
    public int _stm1 = 100;
}

public class InventoryManager : MonoBehaviour
{
    public DataManager _dataManager;
    public PlayerData _playerData;
    public InventoryData _inventoryData;
    public PlayerManager _playerManager;
    public Button _hpBt; // Button HP
    public Button _mpBt; // Button MP
    public Button _stmBt;
    public Button _useBt;

    public enum PotionType
    {
        None,
        Hp,
        Mp,
        Stm
    }

    [HideInInspector] public PotionType _ptType;
    

    public Image _itemIBoxImage;
    public Sprite _noneImage;
    public Sprite _hpImage;
    public Sprite _mpImage;
    public Sprite _stmImage;

    public Text _itemCoutTxt;
    public Text _itemNoteTxt;

    void Start()
    {
        // Tìm DataManager trong scene
        _dataManager = FindObjectOfType<DataManager>();
        _playerManager = FindObjectOfType<PlayerManager>();
        // Tải dữ liệu kho đồ nếu có
        LoadInventoryData();
        LoadPlayerData();

        // Gán sự kiện onClick cho các button
        _useBt.onClick.AddListener(() => OnButtonClick(_useBt));
        _hpBt.onClick.AddListener(() => OnButtonClick(_hpBt));
        _mpBt.onClick.AddListener(() => OnButtonClick(_mpBt));
        
        
        _itemNoteTxt.text = "none item";
    }

    void Update()
    {
        ItemInfo();
        

    }

    
    public void ItemInfo()
    {
        switch (_ptType)
        {
            case PotionType.None:
                if (_itemIBoxImage != null && _noneImage != null)
                {
                    _itemIBoxImage.sprite = _noneImage;
                    _itemCoutTxt.text = "";
                    _itemNoteTxt.text = " Pick your item ";
                }
                break;
            case PotionType.Hp:
                // Áp dụng logic cho Hp
                if (_itemIBoxImage != null && _hpImage != null)
                {
                    _itemIBoxImage.sprite = _hpImage;
                    _itemCoutTxt.text = _inventoryData._hpCount.ToString();
                    _itemNoteTxt.text = " Add " + _inventoryData._hp1 + " Hp to Player  ";
                }
                break;
            case PotionType.Mp:
                // Áp dụng logic cho Mp
                if (_itemIBoxImage != null && _hpImage != null)
                {
                    _itemIBoxImage.sprite = _mpImage;
                    _itemCoutTxt.text = _inventoryData._mpCount.ToString();
                    _itemNoteTxt.text = " Add " + _inventoryData._mp1 + " Mp to Player  ";
                }
                break;
            case PotionType.Stm:
                // Áp dụng logic cho Stm
                break;
        }

       
    }

     public void UseItem()
    {
        switch (_ptType)
        {
            case PotionType.Hp:
                // Áp dụng logic cho Hp
                LoadPlayerData();
                //---------------------------------------------
                _inventoryData._hpCount -= 1;
                Debug.Log("Remaining HP Count: " + _inventoryData._hpCount);
                _playerData._currentHealth += _inventoryData._hp1;
                //---------------------------------------------
                SavePlayerData();
                SaveInventoryData();
                break;
            case PotionType.Mp:
                // Áp dụng logic cho Mp
                LoadPlayerData();
                //---------------------------------------------
                _inventoryData._mpCount -= 1;
                Debug.Log("Remaining HP Count: " + _inventoryData._mpCount);
                _playerData._currentMana += _inventoryData._mp1;
                //---------------------------------------------
                SavePlayerData();
                SaveInventoryData();
                break;
            case PotionType.Stm:
                // Áp dụng logic cho Stm
                break;
        }
       
    }
    public void OnButtonClick(Button button)
    {
        if(button == _useBt)
        {
            UseItem();
        }
        else if (button == _hpBt)
        {
            Debug.Log("HP Button clicked");
            _ptType = PotionType.Hp;
           
        }
        else if (button == _mpBt)
        {
            Debug.Log("MP Button clicked");
            _ptType = PotionType.Mp;

        }
        else
        {
            _ptType = PotionType.None;
        }
    }


    //--------------------------------------------------------------------------------
    public void LoadPlayerData()
    {
        _dataManager.LoadPlayerData();
        _playerManager._playerData = _dataManager._playerData;
        _playerData = _playerManager._playerData;
    }
    public void SavePlayerData()
    {
        _playerManager._playerData = _playerData;
        _dataManager._playerData = _playerManager._playerData;
        _dataManager.SavePlayerData();
    }
    public void SaveInventoryData()
    {
        if (_dataManager != null)
        {
            _dataManager.SaveInventoryData();
        }
    }

    public void LoadInventoryData()
    {
        if (_dataManager != null)
        {
            _dataManager.LoadInventoryData();
            _inventoryData = _dataManager._inventoryData; // Cập nhật InventoryData với dữ liệu đã tải
        }
        if (_inventoryData == null)
        {
            _inventoryData = new InventoryData
            {
            _hpCount = 6,
            _mpCount = 6,
            _stmCount = 6,
            _hp1 = 100,
            _mp1 = 100,
            _stm1 = 100
        };

            // Lưu dữ liệu lần đầu tiên
            _dataManager._inventoryData = _inventoryData;
            _dataManager.SaveInventoryData();
        }
    }
}
