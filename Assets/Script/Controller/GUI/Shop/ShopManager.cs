using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ShopData
{
    public int _hpPrice;
    public int _mpPrice;
    public int _stmPrice;
    public int _strPrice;
    public int _vitPrice;
}

[System.Serializable]
public class CoinData
{
    public int _coin;
}

public class ShopManager : MonoBehaviour
{
    public DataManager _dataManager;
    public InventoryManager _inventoryManager;
    public InventoryData _inventoryData;
    public CoinData _coinData;
    public ShopData _shopData;

    public Button _hpBt;
    public Button _mpBt;
    public Button _stmBt;
    public Button _buyBt;
    public Button _cancelBt;
    public Button _addBt;
    public InputField _quantityField;

    public bool allowClick;

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
    public Text _itemPriceTxt; // Hiển thị giá của từng loại item
    public Text _totalPriceTxt; // Hiển thị tổng giá sau khi bấm Add
    public Text _coinTxt; // Hiển thị số tiền hiện có
    
    void Start()
    {
        _dataManager = FindObjectOfType<DataManager>();
        _inventoryManager = FindObjectOfType<InventoryManager>();
        LoadShopData();
        LoadCoinData();
        LoadInventoryData();
        _quantityField.onValueChanged.AddListener(OnValueChanged);

        // Gán sự kiện onClick cho các button
        _buyBt.onClick.AddListener(() => OnButtonClick(_buyBt));
        _cancelBt.onClick.AddListener(() => OnButtonClick(_cancelBt));
        _addBt.onClick.AddListener(() => OnButtonClick(_addBt));
        _hpBt.onClick.AddListener(() => OnButtonClick(_hpBt));
        _mpBt.onClick.AddListener(() => OnButtonClick(_mpBt));
        _stmBt.onClick.AddListener(() => OnButtonClick(_stmBt));

        _itemNoteTxt.text = "none item";
        _coinTxt.text = $" {_coinData._coin}"; // Hiển thị số tiền hiện có
    }

    void Update()
    {
        
    }

    void OnValueChanged(string value)
    {
        if (!int.TryParse(value, out _))
        {
            _quantityField.text = string.Empty;
        }
    }

    public int GetIntValue()
    {
        if (int.TryParse(_quantityField.text, out int intValue))
        {
            return intValue;
        }
        else
        {
            Debug.LogError("InputField không chứa giá trị hợp lệ.");
            return 0;
        }
    }

    public void ItemInfo()
    {
        switch (_ptType)
        {
            case PotionType.None:
                if (_itemIBoxImage != null && _noneImage != null)
                {
                    _itemIBoxImage.sprite = _noneImage;
                    _itemNoteTxt.text = "Pick your item";
                    _itemPriceTxt.text = "";
                }
                break;
            case PotionType.Hp:
                if (_itemIBoxImage != null && _hpImage != null)
                {
                    _itemIBoxImage.sprite = _hpImage;
                    _itemNoteTxt.text = "Add " + _inventoryData._hp1 + " Hp to Player";
                    _itemPriceTxt.text = $"{_shopData._hpPrice}";
                }
                break;
            case PotionType.Mp:
                if (_itemIBoxImage != null && _mpImage != null)
                {
                    _itemIBoxImage.sprite = _mpImage;
                    _itemNoteTxt.text = "Add " + _inventoryData._mp1 + " Mp to Player";
                    _itemPriceTxt.text = $" {_shopData._mpPrice}";
                }
                break;
            case PotionType.Stm:
                if (_itemIBoxImage != null && _stmImage != null)
                {
                    _itemIBoxImage.sprite = _stmImage;
                    _itemNoteTxt.text = "Add Stamina to Player";
                    _itemPriceTxt.text = $" {_shopData._stmPrice} ";
                }
                break;
        }
    }

    public void BuyItemItem()
    {
        int quantity = GetIntValue();
        switch (_ptType)
        {
            case PotionType.Hp:
                _inventoryData._hpCount += quantity;
                Debug.Log("Remaining HP Count: " + _inventoryData._hpCount);
                _coinData._coin -= _shopData._hpPrice * quantity;
                break;
            case PotionType.Mp:
                _inventoryData._mpCount += quantity;
                Debug.Log("Remaining MP Count: " + _inventoryData._mpCount);
                _coinData._coin -= _shopData._mpPrice * quantity;
                break;
            case PotionType.Stm:
                _inventoryData._stmCount += quantity;
                Debug.Log("Remaining Stamina Count: " + _inventoryData._stmCount);
                _coinData._coin -= _shopData._stmPrice * quantity;
                break;
        }
        SaveCoinData();
        SaveInventoryData();
        _coinTxt.text = $"{_coinData._coin}"; // Cập nhật số tiền sau khi mua
    }

    public void OnButtonClick(Button button)
    {
        if (button == _buyBt)
        {
            BuyItemItem();
        }
        else if (button == _cancelBt)
        {
            _ptType = PotionType.None;
            _itemCoutTxt.text = "";
        }
        else if (button == _addBt)
        {
            int quantity = GetIntValue();
            _itemCoutTxt.text = _quantityField.text;
            UpdateTotalPrice(quantity);
        }
        else if (button == _hpBt)
        {
            Debug.Log("HP Button clicked");
            _ptType = PotionType.Hp;
            ItemInfo();
        }
        else if (button == _mpBt)
        {
            Debug.Log("MP Button clicked");
            _ptType = PotionType.Mp;
            ItemInfo();
        }
        else if (button == _stmBt)
        {
            Debug.Log("Stamina Button clicked");
            _ptType = PotionType.Stm;
            ItemInfo();
        }
    }

    void UpdateTotalPrice(int quantity)
    {
        int totalPrice = 0;
        switch (_ptType)
        {
            case PotionType.Hp:
                totalPrice = _shopData._hpPrice * quantity;
                break;
            case PotionType.Mp:
                totalPrice = _shopData._mpPrice * quantity;
                break;
            case PotionType.Stm:
                totalPrice = _shopData._stmPrice * quantity;
                break;
        }
        _totalPriceTxt.text = $"{totalPrice}";
    }

    public void LoadInventoryData()
    {
        _dataManager.LoadInventoryData();
        _inventoryData = _dataManager._inventoryData;
    }

    public void SaveInventoryData()
    {
        _dataManager._inventoryData = _inventoryData;
        _dataManager.SaveInventoryData();
    }

    public void SaveShopData()
    {
        if (_dataManager != null)
        {
            _dataManager.SaveShopData();
        }
    }

    public void LoadShopData()
    {
        if (_dataManager != null)
        {
            _dataManager.LoadShopData();
            _shopData = _dataManager._shopData;
        }
        if (_shopData == null)
        {
            _shopData = new ShopData
            {
                _hpPrice = 100,
                _mpPrice = 100,
                _stmPrice = 100,
                _strPrice = 100,
                _vitPrice = 100
            };
            _dataManager._shopData = _shopData;
            _dataManager.SaveShopData();
        }
    }

    public void SaveCoinData()
    {
        if (_dataManager != null)
        {
            _dataManager.SaveCoinData();
        }
    }

    public void LoadCoinData()
    {
        if (_dataManager != null)
        {
            _dataManager.LoadCoinData();
            _coinData = _dataManager._coinData;
        }
        if (_coinData == null)
        {
            _coinData = new CoinData
            {
                _coin = 10000
            };
            _dataManager._coinData = _coinData;
            _dataManager.SaveCoinData();
        }
    }
}
