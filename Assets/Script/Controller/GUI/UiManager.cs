using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject GUI;
    public GameObject IngameUIPanel;
    public GameObject PauseGamePanel;
    public GameObject InventoryPanel;
    public GameObject ShopPanel; 
    public GameObject MapPanel;  
    public GameObject DeadPanel;
    public GameObject Setting;

    // Enum để quản lý các trạng thái của UI
    public enum PanelState
    {
        InGUI,
        Inven,
        Pause,
        Shop,
        Map,
        Dead,
        Setting
    }

    private PanelState currentState;

    private void Start()
    {
        // Khởi động game, hiển thị IngameUIPanel mà không thực hiện bất kỳ logic dừng nào
        GUI.SetActive(true);
        IngameUIPanel.SetActive(true);
        InventoryPanel.SetActive(false);
        PauseGamePanel.SetActive(false);
        ShopPanel.SetActive(false);
        MapPanel.SetActive(false);
        DeadPanel.SetActive(false);
        Setting.SetActive(false);
        currentState = PanelState.InGUI;
    }
    private void Update()
    {
        // Kiểm tra các phím nhấn để chuyển đổi giữa các panel
        if (Input.GetKeyDown(KeyCode.Escape) && currentState != PanelState.Dead)
        {
            // Nếu đang ở các panel khác, quay về IngameUIPanel
            if (currentState != PanelState.InGUI )
            {
                if (currentState == PanelState.Setting)
                {
                    SetPanelState(PanelState.Pause);
                }
                else  SetPanelState(PanelState.InGUI);
            }
            else
            {
                // Hiện PauseGamePanel nếu IngameUIPanel đang hiển thị
                SetPanelState(PanelState.Pause);
            }
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            //Debug.Log("an B");
            SetPanelState(PanelState.Inven);
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            //Debug.Log("an M");
            SetPanelState(PanelState.Map);
        }
        // Thêm các phím nhấn khác để hiển thị ShopPanel và MapPanel nếu cần
    }

    // Hàm để thay đổi trạng thái của các panel
    public void SetPanelState(PanelState state)
    {
        // Nếu trạng thái không thay đổi, không cần làm gì
        if (state == currentState) return;

        // Cập nhật trạng thái hiện tại
        currentState = state;
        GUI.SetActive(true);

        // Ẩn tất cả các panel
        IngameUIPanel.SetActive(false);
        InventoryPanel.SetActive(false);
        PauseGamePanel.SetActive(false);
        ShopPanel.SetActive(false);
        MapPanel.SetActive(false);
        DeadPanel.SetActive(false);
        Setting.SetActive(false);

        // Hiển thị panel theo trạng thái hiện tại
        switch (state)
        {
            case PanelState.InGUI:
                IngameUIPanel.SetActive(true);
                break;
            case PanelState.Inven:
                InventoryPanel.SetActive(true);
                break;
            case PanelState.Pause:
                PauseGamePanel.SetActive(true);
                break;
            case PanelState.Shop:
                ShopPanel.SetActive(true);
                break;
            case PanelState.Map:
                MapPanel.SetActive(true);
                break;
            case PanelState.Dead:
                DeadPanel.SetActive(true);
                break;
            case PanelState.Setting:
                Setting.SetActive(true);
                break;
        }

        // Chỉ thực hiện logic dừng đối tượng khi không phải là InGUI
        SetGameObjectsActive(state == PanelState.InGUI, "Enemy");
        SetGameObjectsActive(state == PanelState.InGUI, "Player");
    }
    //---------------------------------------------------------------------
    public void OpenShopPanel()
    {
        SetPanelState(PanelState.Shop);
    }
    public void OpenSettingPanel()
    {
        SetPanelState(PanelState.Setting);
    }
    //----------------------------------------------------------------------
    private void SetGameObjectsActive(bool isActive, string tag)
    {
        //Debug.Log("Setting active state to: " + isActive + " for tag: " + tag);
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject go in objects)
        {
            //Debug.Log("Processing object: " + go.name);
            // Kích hoạt hoặc tạm dừng tất cả các MonoBehaviour
            MonoBehaviour[] monoBehaviours = go.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour mb in monoBehaviours)
            {
                // Chỉ bật lại các MonoBehaviour nếu trạng thái là InGUI
                if (isActive)
                {
                    mb.enabled = true;
                    //Debug.Log("Setting " + mb.GetType().Name + " to " + isActive);
                }
                else
                {
                    mb.enabled = false;
                }
            }

            // Tạm dừng Rigidbody nếu có
            Rigidbody rb = go.GetComponent<Rigidbody>();
            if (rb != null && rb.isKinematic != !isActive)
            {
                rb.isKinematic = !isActive;
                //Debug.Log("Setting Rigidbody isKinematic to " + rb.isKinematic);
            }
        }
    }

    

}
