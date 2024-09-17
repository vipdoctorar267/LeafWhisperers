using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[System.Serializable]
public class PlayerData
{
    public int _maxHealth;           // Máu tối đa
    public int _currentHealth;       // Máu hiện tại
    public int _maxMana;             // Mana tối đa
    public int _currentMana;         // Mana hiện tại
    public int _maxstamina;          // Thể lực tối đa
    public int _currentstamina;      // Thể lực
    public int _attack;              // Sức tấn công
    public int _defense;             // Phòng thủ
    public float _maxEquipmentWeight;// Sức nặng trang bị tối đa
    public float _currentEquipmentWeight; // Sức nặng trang bị hiện tại

}

public class PlayerManager : MonoBehaviour
{
    private CharacterStateMachine _charStateMachine;
    private UIManager _UIManager;
    private DataManager _dataManager;
    public PlayerData _playerData;
    public CoinData _coinData;
    public Slider _healthSlider;
    public Slider _manaSlider;
    public Slider _staminaSlider;
    public Slider _invenHealthSlider;
    public Slider _invenManaSlider;
    public Slider _invenStaminaSlider;
    public Text _invenHpTxt;
    public Text _invenMnTxt;
    public Text _invenStminTxt;

    public Text _coinTxt;
    //-------------------------         
    private float holdTimer = 0f;
    private bool isKeyHeld = false;

    private void Awake()
    {
        _UIManager = FindObjectOfType<UIManager>();
        _charStateMachine = FindObjectOfType<CharacterStateMachine>();
        // Tìm DataManager trong scene
        _dataManager = FindObjectOfType<DataManager>();

        // Tải dữ liệu người chơi nếu có
        LoadCoinData();
        LoadPlayerData();
    }
    void Start()
    {
        _coinTxt.text = $" {_coinData._coin}";

        // Khởi tạo các thông số UI
        _healthSlider.interactable = false;
        _invenHealthSlider.interactable = false;
        _healthSlider.maxValue = _playerData._maxHealth;
        _invenHealthSlider.maxValue = _playerData._maxHealth;
        _healthSlider.value = _playerData._currentHealth;
        _invenHealthSlider.value = _playerData._currentHealth;
        _invenHpTxt.text = _playerData._currentHealth + "/" + _playerData._maxHealth;

        //--------------------------------
        _manaSlider.interactable = false;
        _invenManaSlider.interactable = false;
        _manaSlider.maxValue = _playerData._maxMana;
        _invenManaSlider.maxValue = _playerData._maxMana;
        _manaSlider.value = _playerData._currentMana;
        _invenManaSlider.value = _playerData._currentMana;
        _invenMnTxt.text = _playerData._currentMana + "/" + _playerData._maxMana;

        //--------------------------------
        _staminaSlider.interactable = false;
        _invenStaminaSlider.interactable = false;
        _staminaSlider.maxValue = _playerData._maxstamina;
        _invenStaminaSlider.maxValue = _playerData._maxstamina;
        _staminaSlider.value = _playerData._currentstamina;
        _invenStaminaSlider.value = _playerData._currentstamina;
        _invenStminTxt.text = _playerData._currentstamina + "/" + _playerData._maxstamina;

    }

    private bool hasDrainedStamina = false;

    void FixedUpdate()
    {
        
        HpValueCtrl();
        MpValueCtrl();
        StaminaValueCtrl();


        if(_UIManager.currentState == UIManager.PanelState.InGUI)
        {
            if (_charStateMachine.isIdle || _charStateMachine.isWalk || _charStateMachine.isFall) RegenerateStamina(5f);
            if (_charStateMachine.isClimbing) DrainStaminaOverTime(2f);
            if (_charStateMachine.isRunning) DrainStaminaOverTime(10f);
            if (_charStateMachine.isDash)
            {
                if (!hasDrainedStamina) { DrainStaminaOnce(15); hasDrainedStamina = true; }
            }
            else hasDrainedStamina = false;
        }
        


        HoldCtrl(KeyCode.R,1.5f, HealWihtMp);

        if (Input.GetKeyDown(KeyCode.P))
        {
            PlayerTakeDamage(70);
            Debug.Log("CurrentHealth: " + _playerData._currentHealth);
            SavePlayerData(); // Lưu dữ liệu khi người chơi nhận sát thương
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            _playerData._currentHealth = _playerData._maxHealth; 
            Debug.Log("CurrentHealth: " + _playerData._currentHealth);
            SavePlayerData(); // Lưu dữ liệu khi người chơi hồi máu
        }

        // Ví dụ: Kiểm tra nếu nhân vật bị thương
        if (_playerData._currentHealth <= 0)
        {
            Debug.Log("Player is dead");
            // Xử lý nhân vật bị chết
        }

        // Các logic khác liên quan đến cập nhật thông tin người chơi
    }

    void HoldCtrl(KeyCode key, float requiredHoldTime, System.Action onHoldComplete)
    {
        if (Input.GetKey(key))
        {
            if (!isKeyHeld)
            {
                isKeyHeld = true;
                holdTimer = 0f;
            }

            holdTimer += Time.deltaTime;

            if (holdTimer >= requiredHoldTime)
            {
                onHoldComplete?.Invoke(); // Gọi hàm callback khi giữ đủ thời gian
                holdTimer = 0f;
                isKeyHeld = false;
            }
        }
        else
        {
            isKeyHeld = false;
            holdTimer = 0f;
        }
    }
    

    public void HealWihtMp()
    {
        _playerData._currentMana -= 50;
        float HealInput = _playerData._attack * 0.5f;
        _playerData._currentHealth += (int)HealInput;
        SavePlayerData();
    }
    // Hàm để nhận sát thương
    //--------------------------------------------------------------------------------------------
    public void PlayerOnAttack(Func<int> calculateDamageMethod)
    {
        // Tính toán sát thương từ ngưòn 
        int damage = calculateDamageMethod();
     
        // Gọi phương thức nhận sát thương
        PlayerTakeDamage(damage);
    }

    //---------------------------------------------------------------------------------------------
    public void PlayerTakeDamage(int damage)
    {
        if (_charStateMachine.isAT2) return;
        if (!_charStateMachine.isDead)
        {
            // Giảm lượng máu của Player dựa trên sát thương nhận được
            _playerData._currentHealth -= Mathf.Max(CalculateReducedDamage(_playerData._defense, damage), 0);
            Debug.Log("Player -" + CalculateReducedDamage(_playerData._defense, damage) + " Hp");
            SavePlayerData();
            // Kiểm tra nếu máu của RunSpider giảm xuống 0 hoặc thấp hơn
            if (_playerData._currentHealth <= 0)
            {
                // Gọi phương thức để xử lý khi RunSpider chết
                _playerData._currentHealth = 0;
                Die();
            }
            else
            {
                // Xử lý khi RunSpider bị tấn công nhưng chưa chết (ví dụ: phát âm thanh đau đớn, chuyển sang trạng thái phòng thủ, v.v.)
                OnDamageTaken();
            }
        }    
    }

    private void Die()
    {
        // Xử lý logic khi RunSpider chết, như phá hủy object, cập nhật UI, v.v.
        Debug.Log("Player đã bị tiêu diệt!");
        _charStateMachine.isDead = true;
        StartCoroutine(WaitDeadAnimEnd());
        

    }
    private IEnumerator WaitDeadAnimEnd()
    {
        if (!_charStateMachine.isDead) yield break;
        yield return new WaitForSeconds(19f / 60f +1f); // Thời gian hoạt ảnh knockback
        _UIManager.SetPanelState(UIManager.PanelState.Dead); 
    }
    private void OnDamageTaken()
    {

        // Xử lý logic khi player nhận sát thương nhưng chưa chết
        Debug.Log("player bị tấn công, còn " + _playerData._currentHealth + " HP");
        _charStateMachine.onDMG = true;
    }
    
    //----------------------------------------------------------------------------------------------------------
    public int CalculateReducedDamage(int defense, int damage)
    {
        float initialDefenseReduction = 0.5f; // 50%
        float reductionDecayPer100 = 0.1f; // 10%

        // Khởi tạo biến
        float reductionFactor = initialDefenseReduction;
        float totalReducedDamage = 0;
        int remainingDamage = damage;

        // Tính số vòng lặp
        int n = Mathf.CeilToInt((float)Mathf.Min(defense, damage) / 100);

        for (int i = 0; i < n; i++)
        {
            int defenseForThisStep = Mathf.Min(100, defense);
            float reducedDamageThisStep = defenseForThisStep * reductionFactor;

            // Cập nhật tổng sát thương giảm
            totalReducedDamage += reducedDamageThisStep;
            remainingDamage -= defenseForThisStep;

            // Cập nhật điểm phòng thủ và hệ số giảm
            defense -= defenseForThisStep;
            reductionFactor -= reductionFactor * reductionDecayPer100;
            reductionFactor = Mathf.Max(reductionFactor, 0.0f); // Đảm bảo không âm

            // Nếu sát thương còn lại nhỏ hơn 0, thoát vòng lặp
            if (remainingDamage <= 0) break;
        }

        // Tính toán phần sát thương còn lại chưa được phòng thủ
        int finalDamage = Mathf.Max(damage - (int)totalReducedDamage, 0);
        
        return finalDamage;
    }
    //---------------------------------------------------------------------------------------
    public void AddCoin(int coinAmount)
    {
        _coinData._coin += coinAmount;
        _coinTxt.text = $" {_coinData._coin}";
        SaveCoinData();
    }
    //---------------------------------------------------------------------------------------
    public struct AttackResult
    {
        public int Damage;
        public float KnockbackForce;
        public Vector2 KnockbackDirection;

        public AttackResult(int damage, float knockbackForce, Vector2 knockbackDirection)
        {
            Damage = damage;
            KnockbackForce = knockbackForce;
            KnockbackDirection = knockbackDirection;
        }
    }

    public AttackResult CalculatePlayerDamage(string attackType, Vector2 playerPosition, Vector2 enemyPosition)
    {
        int damage = 0;
        float knockbackForce = 0f;
        Vector2 knockbackDirection = Vector2.zero;

        switch (attackType)
        {
            case "Attack01":
                // Sát thương của Attack01 là 30% của tấn công người chơi
                damage = Mathf.FloorToInt(_playerData._attack * 0.4f);
                knockbackForce = 5f; // Giá trị này bạn có thể thay đổi tùy theo logic game
                knockbackDirection = (enemyPosition - playerPosition).normalized;
                break;

            case "Attack02":
                damage = Mathf.FloorToInt(_playerData._attack * 0.5f);
                knockbackForce = 15f; // Giá trị knockbackForce tương ứng
                knockbackDirection = (enemyPosition - playerPosition).normalized;
                break;
            case "Attack03":
                damage = Mathf.FloorToInt(_playerData._attack * 0.3f);
                knockbackForce = 10f; // Giá trị knockbackForce tương ứng
                knockbackDirection = (enemyPosition - playerPosition).normalized;
                break;
            // case "Attack04":
            //     damage = Mathf.FloorToInt(_playerManager._attack * 0.5f);
            //     knockbackForce = 10f; // Giá trị knockbackForce tương ứng
            //     knockbackDirection = (enemyPosition - playerPosition).normalized;
            //     break;

            default:
                Debug.LogWarning("Attack type không xác định: " + attackType);
                break;
        }

        return new AttackResult(damage, knockbackForce, knockbackDirection);
    }
    //------------------------------------------------------------------------------
    public void HpValueCtrl()
    {
        if (_playerData._currentHealth >= _playerData._maxHealth) _playerData._currentHealth = _playerData._maxHealth;

        //update value
        _healthSlider.value = _playerData._currentHealth;
        _invenHealthSlider.value = _playerData._currentHealth;
        _invenHpTxt.text = _playerData._currentHealth + "/" + _playerData._maxHealth;

        if (_playerData._currentHealth > 0) _charStateMachine.isDead = false;
    }
    public void MpValueCtrl()
    {
        if (_playerData._currentMana >= _playerData._maxMana) _playerData._currentMana = _playerData._maxMana;

        //update value
        _manaSlider.value = _playerData._currentMana;
        _invenManaSlider.value = _playerData._currentMana;
        _invenMnTxt.text = _playerData._currentMana + "/" + _playerData._maxMana;
    }

    public void StaminaValueCtrl()
    {
        if (_playerData._currentstamina >= _playerData._maxstamina)
            _playerData._currentstamina = _playerData._maxstamina;

        // Cập nhật giá trị thanh Stamina
        _staminaSlider.value = _playerData._currentstamina;
        _invenStaminaSlider.value = _playerData._currentstamina;
        _invenStminTxt.text = _playerData._currentstamina + "/" + _playerData._maxstamina;
    }
    //------------------------------------------------------------------------------
    private float staminaRegenBuffer = 0f;
   

    public void RegenerateStamina(float regenRate)
    {
        // Hồi Stamina nếu chưa đạt đến tối đa
        if (_playerData._currentstamina < _playerData._maxstamina)
        {
            staminaRegenBuffer += regenRate * Time.deltaTime;

            // Khi buffer đạt ít nhất 1, cộng số nguyên vào stamina
            if (staminaRegenBuffer >= 1f)
            {
                int staminaToAdd = Mathf.FloorToInt(staminaRegenBuffer);
                _playerData._currentstamina += staminaToAdd;
                staminaRegenBuffer -= staminaToAdd;

                // Đảm bảo không vượt quá giới hạn
                _playerData._currentstamina = Mathf.Min(_playerData._currentstamina, _playerData._maxstamina);
            }
        }
    }

    private float staminaDrainBuffer = 0f;
    public void DrainStaminaOverTime(float drainRate)
    {
        // Trừ Stamina nếu vẫn còn
        if (_playerData._currentstamina > 0)
        {
            staminaDrainBuffer += drainRate * Time.deltaTime;

            // Khi buffer đạt ít nhất 1, trừ số nguyên khỏi stamina
            if (staminaDrainBuffer >= 1f)
            {
                int staminaToDrain = Mathf.FloorToInt(staminaDrainBuffer);
                _playerData._currentstamina -= staminaToDrain;
                staminaDrainBuffer -= staminaToDrain;

                // Đảm bảo không xuống dưới 0
                _playerData._currentstamina = Mathf.Max(_playerData._currentstamina, 0);
   
            }
        }
    }


    public void DrainStaminaOnce(int staminaCost)
    {
        // Trừ Stamina ngay lập tức
        _playerData._currentstamina -= staminaCost;
        _playerData._currentstamina = Mathf.Max(_playerData._currentstamina, 0);
       
    }

    //-------------------------------------------------------------------------------
    public void SaveCoinData()
    {
        _dataManager._coinData = _coinData;
        _dataManager.SaveCoinData();
    }

    public void LoadCoinData()
    {
        _dataManager.LoadCoinData();
        _coinData = _dataManager._coinData;
    }

    public void SavePlayerData()
    {
        if (_dataManager != null)
        {
            _dataManager.SavePlayerData();
        }
    }

    public void LoadPlayerData()
    {
        if (_dataManager != null)
        {
            _dataManager.LoadPlayerData();
            _playerData = _dataManager._playerData; // Cập nhật _playerManager với dữ liệu đã tải
        }
        if (_playerData == null)
        {
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
                _currentEquipmentWeight = 0.0f
            };

            // Lưu dữ liệu lần đầu tiên
            _dataManager._playerData = _playerData;
            _dataManager.SavePlayerData();
        }

    }
}
