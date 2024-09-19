using UnityEngine;
using UnityEngine.UI;

public class SaveSlotSelectionUI : MonoBehaviour
{
    [Header(" Slot")]
    private MenuManager _MenuManager;
    public Button slot1Button; // Nút chọn Slot 1
    public Button slot2Button; // Nút chọn Slot 2
    public Button slot3Button; // Nút chọn Slot 3
    [Header(" Button")]
    public Button continueButton; // Nút "Continue Game"
    public Button deleteButton;   // Nút "Delete"
    public Button newGameButton;  // Nút "New Game"
    [Header (" Image")]
    public Sprite _DataImage;
    public Sprite _none;
    public Image _slot1BtImg;
    public Image _slot2BtImg;
    public Image _slot3BtImg;
    [Header(" Text")]
    public Text _slot1BtTxt;
    public Text _slot2BtTxt;
    public Text _slot3BtTxt;


    private DataManager.SaveSlot currentSelectedSlot; // Biến lưu slot đang được chọn

    private void Start()
    {
        _MenuManager = FindAnyObjectByType<MenuManager>();
        SaveSlotUIHandle();
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
    private void SaveSlotUIHandle()
    {
        // Cập nhật Slot 1
        if (DataManager.Instance.CheckIfSlotHasData(DataManager.SaveSlot.SaveData001))
        {
            _slot1BtImg.sprite = _DataImage;
            // Thay đổi alpha của màu để làm sprite 
            _slot1BtImg.color = new Color(_slot1BtImg.color.r, _slot1BtImg.color.g, _slot1BtImg.color.b, 255f);
            _slot1BtTxt.enabled = false;
        }
        else
        {
            _slot1BtImg.sprite = _none;
            // Thay đổi alpha của màu để làm sprite trong suốt
            _slot1BtImg.color = new Color(_slot1BtImg.color.r, _slot1BtImg.color.g, _slot1BtImg.color.b, 0f);
            _slot1BtTxt.enabled = true;
        }

        // Cập nhật Slot 2
        if (DataManager.Instance.CheckIfSlotHasData(DataManager.SaveSlot.SaveData002))
        {
            _slot2BtImg.sprite = _DataImage;
            // Thay đổi alpha của màu để làm sprite 
            _slot2BtImg.color = new Color(_slot2BtImg.color.r, _slot2BtImg.color.g, _slot2BtImg.color.b, 255f);
            _slot2BtTxt.enabled = false;
        }
        else
        {
            _slot2BtImg.sprite = _none;
            // Thay đổi alpha của màu để làm sprite trong suốt
            _slot2BtImg.color = new Color(_slot2BtImg.color.r, _slot2BtImg.color.g, _slot2BtImg.color.b, 0f);
            _slot2BtTxt.enabled = true;
        }

        // Cập nhật Slot 3
        if (DataManager.Instance.CheckIfSlotHasData(DataManager.SaveSlot.SaveData003))
        {
            _slot3BtImg.sprite = _DataImage;
            // Thay đổi alpha của màu để làm sprite 
            _slot3BtImg.color = new Color(_slot3BtImg.color.r, _slot3BtImg.color.g, _slot3BtImg.color.b, 255f);
            _slot3BtTxt.enabled = false;
        }
        else
        {
            _slot3BtImg.sprite = _none;
            // Thay đổi alpha của màu để làm sprite trong suốt
            _slot3BtImg.color = new Color(_slot3BtImg.color.r, _slot3BtImg.color.g, _slot3BtImg.color.b, 0f);
            _slot3BtTxt.enabled = true;
        }
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
