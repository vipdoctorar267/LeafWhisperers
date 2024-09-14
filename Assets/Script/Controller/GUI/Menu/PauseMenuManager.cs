using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class PauseMenuManager : MonoBehaviour
{
    public UIManager _UIManager;
    [Header("Buttons")]
    public Button continueButton;
    public Button settingButton;
    public Button exitButton;

    public Button yesButton;
    public Button noButton;

    [Header("Panels")]
    public GameObject pauseMenuPanel; // Panel PauseMenu
    public GameObject loadPanel;
    public GameObject confirmExitPopup; // Popup xác nhận thoát

    private void Start()
    {
        pauseMenuPanel.SetActive(true);
        confirmExitPopup.SetActive(false);
        loadPanel.SetActive(false);

        // Đăng ký các sự kiện cho các nút
        continueButton.onClick.AddListener(() => OnButtonClick(continueButton));
        settingButton.onClick.AddListener(() => OnButtonClick(settingButton));
        exitButton.onClick.AddListener(() => OnButtonClick(exitButton));

        yesButton.onClick.AddListener(() => OnButtonClick(yesButton));
        noButton.onClick.AddListener(() => OnButtonClick(noButton));
    }

    public void OnButtonClick(Button button)
    {
        if (pauseMenuPanel.activeSelf)
        {
            if (button == continueButton)
            {
                ContinueGame();
            }
            else if (button == settingButton)
            {
                OpenSettings();
            }
            else if (button == exitButton)
            {
                Debug.Log("Ex button clicked");
                ShowConfirmExitPopup();
            }
            else if(button == yesButton)
            {
                OnConfirmExitYes();
            }
            else if (button == noButton)
            {
                OnConfirmExitNo();
            }
        }
    }

    private void ContinueGame()
    {
        // Quay trở lại giao diện InGame UI và tiếp tục game
        Debug.Log("Continue button clicked");
        pauseMenuPanel.SetActive(false);
        _UIManager.SetPanelState(UIManager.PanelState.InGUI);
        // Thêm logic để quay trở lại InGame UI (PanelState.InGUI) nếu cần
    }

    private void OpenSettings()
    {
        // Mở menu cài đặt (chưa thực hiện)
        Debug.Log("Settings button clicked");
    }

    private void ShowConfirmExitPopup()
    {
        // Hiển thị popup xác nhận thoát
        
        confirmExitPopup.SetActive(true);
    }

    public void OnConfirmExitYes()
    {
        // Thoát game khi người dùng chọn "Yes"
        Debug.Log("Yes button clicked");
        
        confirmExitPopup.SetActive(false);
        loadPanel.SetActive(true);
        StartCoroutine(LoadGameScene());
    }

    public void OnConfirmExitNo()
    {
        Debug.Log("No button clicked");
        // Ẩn popup xác nhận thoát và hiện lại panel PauseMenu
        confirmExitPopup.SetActive(false);
        
    }

    private IEnumerator LoadGameScene()
    {
        // Đợi một chút để chắc chắn rằng loadPanel đã được hiển thị
        yield return new WaitForSeconds(0.5f); // Thay đổi thời gian nếu cần

        // Tải cảnh
        SceneManager.LoadScene("MainMenu");
    }
}



