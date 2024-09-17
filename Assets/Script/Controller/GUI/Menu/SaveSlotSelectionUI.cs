using UnityEngine;
using UnityEngine.UI;

public class SaveSlotSelectionUI : MonoBehaviour
{
    private MenuManager _MenuManager;
    public Button slot1Button; // Nút chọn Slot 1
    public Button slot2Button; // Nút chọn Slot 2
    public Button slot3Button; // Nút chọn Slot 3

    public Button continueButton; // Nút "Continue Game"
    public Button deleteButton;   // Nút "Delete"
    public Button newGameButton;  // Nút "New Game"

    private DataManager.SaveSlot currentSelectedSlot; // Biến lưu slot đang được chọn

    private void Start()
    {
        _MenuManager = FindAnyObjectByType<MenuManager>();

        // Đăng ký sự kiện bấm nút
        slot1Button.onClick.AddListener(() => OnSlotSelected(DataManager.SaveSlot.SaveData001));
        slot2Button.onClick.AddListener(() => OnSlotSelected(DataManager.SaveSlot.SaveData002));
        slot3Button.onClick.AddListener(() => OnSlotSelected(DataManager.SaveSlot.SaveData003));

        continueButton.onClick.AddListener(OnClickContinueGame);
        deleteButton.onClick.AddListener(OnClickDeleteGame);
        newGameButton.onClick.AddListener(OnClickNewGame);

        // Ẩn các nút tiếp tục/xóa/mới khi bắt đầu
        continueButton.gameObject.SetActive(false);
        deleteButton.gameObject.SetActive(false);
        newGameButton.gameObject.SetActive(false);
    }

    private void OnSlotSelected(DataManager.SaveSlot slot)
    {
        currentSelectedSlot = slot;
        DataManager.Instance.SetCurrentSlot(slot); // Cập nhật slot hiện tại trong DataManager

        // Kiểm tra xem slot có dữ liệu không
        if (DataManager.Instance.CheckIfSlotHasData(slot))
        {
            continueButton.gameObject.SetActive(true);
            deleteButton.gameObject.SetActive(true);
            newGameButton.gameObject.SetActive(false); // Ẩn nút New Game nếu đã có dữ liệu
        }
        else
        {
            continueButton.gameObject.SetActive(false);
            deleteButton.gameObject.SetActive(false);
            newGameButton.gameObject.SetActive(true); // Hiện nút New Game nếu chưa có dữ liệu
        }
    }

    private void OnClickContinueGame()
    {
        DataManager.Instance.LoadGame(currentSelectedSlot);
        _MenuManager.StartGame();
        Debug.Log("Loading game from slot: " + currentSelectedSlot);
    }

    private void OnClickDeleteGame()
    {
        DataManager.Instance.DeleteSave(currentSelectedSlot);
        // Cập nhật giao diện sau khi xóa dữ liệu
        continueButton.gameObject.SetActive(false);
        deleteButton.gameObject.SetActive(false);
        newGameButton.gameObject.SetActive(true); // Hiện nút New Game sau khi xóa
        Debug.Log("Deleted data in slot: " + currentSelectedSlot);
    }

    private void OnClickNewGame()
    {
        // Xóa dữ liệu cũ nếu có và tạo dữ liệu mới
        DataManager.Instance.DeleteSave(currentSelectedSlot);
        DataManager.Instance.LoadGame(currentSelectedSlot); // Gọi LoadGame để tạo dữ liệu mới nếu không có
        _MenuManager.StartGame();
        Debug.Log("Started new game in slot: " + currentSelectedSlot);
    }
}
