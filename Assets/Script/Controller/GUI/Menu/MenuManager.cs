using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    [Header("Buttons")]
    public Button startButton;
    public Button settingButton;
    public Button exitButton;
    public Button yesButton;
    public Button noButton;

    [Header("Panels")]
    public GameObject mainMenuPanel; // Panel MainMenu
    public GameObject confirmExitPanel; // Panel xác nhận thoát
    public GameObject settingPanel;
    public GameObject loadPanel; // Panel tải cảnh

    // Enum để quản lý các trạng thái của menu
    public enum MenuState
    {
        MainMenu,
        ConfirmExit,
        Settings,
        Loading
    }

    private MenuState currentState;

    private void Start()
    {
        SetMenuState(MenuState.MainMenu);
        mainMenuPanel.SetActive(true);
        confirmExitPanel.SetActive(false);
        settingPanel.SetActive(false);
        loadPanel.SetActive(false);
        // Đăng ký các sự kiện cho các nút
        startButton.onClick.AddListener(() => OnButtonClick(startButton));
        settingButton.onClick.AddListener(() => OnButtonClick(settingButton));
        exitButton.onClick.AddListener(() => OnButtonClick(exitButton));

        yesButton.onClick.AddListener(() => OnButtonClick(yesButton));
        noButton.onClick.AddListener(() => OnButtonClick(noButton));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentState == MenuState.MainMenu)
            {
                ShowConfirmExitPanel();
            }
            else
            {
                SetMenuState(MenuState.MainMenu);
            }
        }
    }

    public void OnButtonClick(Button button)
    {
        if (currentState == MenuState.MainMenu)
        {
            if (button == startButton)
            {
                StartGame();
            }
            else if (button == settingButton)
            {
                OpenSettings();
            }
            else if (button == exitButton)
            {
                ShowConfirmExitPanel();
            }
        }
        else if (currentState == MenuState.ConfirmExit)
        {
            if (button == yesButton)
            {
                OnConfirmExitYes();
            }
            else if (button == noButton)
            {
                OnConfirmExitNo();
            }
        }
    }

    private void StartGame()
    {
        SetMenuState(MenuState.Loading);
        StartCoroutine(LoadGameScene());
    }

    private IEnumerator LoadGameScene()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("MainGame");
    }

    private void OpenSettings()
    {
        SetMenuState(MenuState.Settings);
    }

    private void ShowConfirmExitPanel()
    {
        SetMenuState(MenuState.ConfirmExit);
    }

    public void OnConfirmExitYes()
    {
        Application.Quit();
    }

    public void OnConfirmExitNo()
    {
        SetMenuState(MenuState.MainMenu);
    }

    private void SetMenuState(MenuState newState)
    {
        if (currentState == newState) return;

        currentState = newState;

        // Ẩn tất cả các panel
        mainMenuPanel.SetActive(false);
        confirmExitPanel.SetActive(false);
        settingPanel.SetActive(false);
        loadPanel.SetActive(false);

        // Hiển thị panel theo trạng thái hiện tại
        switch (newState)
        {
            case MenuState.MainMenu:
                mainMenuPanel.SetActive(true);
                break;
            case MenuState.ConfirmExit:
                confirmExitPanel.SetActive(true);
                break;
            case MenuState.Settings:
                settingPanel.SetActive(true);
                break;
            case MenuState.Loading:
                loadPanel.SetActive(true);
                break;
        }
    }
}
