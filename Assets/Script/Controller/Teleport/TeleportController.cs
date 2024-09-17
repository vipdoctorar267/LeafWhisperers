using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class TeleportPointData
{
    public string id; // Dùng để định danh điểm dịch chuyển
    public bool isActive;
    // Không cần Vector3 position nữa nếu vị trí không thay đổi
}

[System.Serializable]
public class TeleportData
{
    public List<TeleportPointData> teleportPoints = new List<TeleportPointData>();
    public string lastUsedPointId; // Lưu ID của điểm dịch chuyển cuối cùng
    public int lastSaveCoin;
    public int lastHpCount;
    public int lastMpCount;
    public int lastStmCount;

}

public class TeleportController : MonoBehaviour
{
    public UIManager _UIManager;
    private GameObject Player;
    public bool isActive;
    [SerializeField] private Button teleButton;
    [SerializeField] private GameObject teleBtnMenu;
    private bool isFirstTouch = true;
    [SerializeField] private string teleportPointId; // ID của điểm dịch chuyển này
    private Animator animator;
    private DataManager _dataManager;

    void Start()
    {
        animator = GetComponent<Animator>();
        Player = GameObject.FindGameObjectWithTag("Player");
        _dataManager = FindObjectOfType<DataManager>(); // Tìm đối tượng DataManager trong scene

        // Load trạng thái của điểm dịch chuyển này
        LoadTeleportPointData();
    }

    private void Update()
    {
        teleButton.interactable = isActive;
        if (isActive) animator.SetBool("Active", true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null && collision.gameObject.CompareTag("Player"))
        {
            if (isFirstTouch)
            {
                animator.SetTrigger("OnActive");
                isFirstTouch = false;
            }
            isActive = true;
            animator.SetBool("Active", true);
            SaveTeleportPointData(); // Lưu trạng thái khi điểm dịch chuyển được kích hoạt
        }
    }

    public void TeleportToPoint()
    {
        if (Vector2.Distance(Player.transform.position, transform.position) > 1f)
        {
            Debug.Log("---------------Tele-----------------");
            
            _UIManager.SetPanelState(UIManager.PanelState.InGUI);
            Player.transform.position = new Vector3(transform.position.x + 3f, transform.position.y + 1f);
            SaveTeleportPointData(); // Lưu dữ liệu khi dịch chuyển
        }
    }

    public void TeleportToLastPoint()
    {
        // Lấy ID của điểm dịch chuyển cuối cùng từ DataManager
        string lastPointId = _dataManager._teleportData.lastUsedPointId;
        _dataManager._coinData._coin = _dataManager._teleportData.lastSaveCoin;
        _dataManager._inventoryData._hpCount = _dataManager._teleportData.lastHpCount;
        _dataManager._inventoryData._mpCount = _dataManager._teleportData.lastMpCount;
        _dataManager._inventoryData._stmCount = _dataManager._teleportData.lastStmCount;

        // Tìm đối tượng TeleportController tương ứng với ID đó
        TeleportController lastTeleportPoint = null;
        TeleportController[] teleportPoints = FindObjectsOfType<TeleportController>();

        foreach (TeleportController point in teleportPoints)
        {
            if (point.teleportPointId == lastPointId)
            {
                lastTeleportPoint = point;
                break;
            }
        }

        if (lastTeleportPoint != null)
        {
            _UIManager.SetPanelState(UIManager.PanelState.InGUI);
            // Dịch chuyển player đến vị trí của điểm dịch chuyển đó
            Player.transform.position = lastTeleportPoint.transform.position;

            // Lưu trạng thái sau khi dịch chuyển
            lastTeleportPoint.SaveTeleportPointData();

            Debug.Log("Teleported to the last used point: " + lastPointId);
        }
        else
        {
            Debug.LogWarning("No teleport point found with ID: " + lastPointId);
        }
    }
    //---------------------------------------------------------------------------------------
    
    private void SaveTeleportPointData()
    {
        TeleportPointData pointData = new TeleportPointData
        {
            id = teleportPointId,
            isActive = isActive,
            // Không cần lưu position nữa
        };

        TeleportData data = _dataManager._teleportData;
        TeleportPointData existingPoint = data.teleportPoints.Find(p => p.id == teleportPointId);
        if (existingPoint != null)
        {
            existingPoint.isActive = isActive;
            // Không cần cập nhật position
        }
        else
        {
            data.teleportPoints.Add(pointData);
        }
        _dataManager._teleportData.lastSaveCoin = _dataManager._coinData._coin;
        _dataManager._teleportData.lastHpCount = _dataManager._inventoryData._hpCount;
        _dataManager._teleportData.lastMpCount = _dataManager._inventoryData._mpCount;
        _dataManager._teleportData.lastStmCount = _dataManager._inventoryData._stmCount;
        data.lastUsedPointId = teleportPointId;
        _dataManager.SaveTeleportData(); // Lưu vào DataManager
    }

    private void LoadTeleportPointData()
    {
        TeleportData data = _dataManager._teleportData;
        TeleportPointData pointData = data.teleportPoints.Find(p => p.id == teleportPointId);
        if (pointData != null)
        {
            isActive = pointData.isActive;
            // Không cần khôi phục position nữa
        }
    }
}
