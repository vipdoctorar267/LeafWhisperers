using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class TutorialData
{
    public bool _1stGame;
    
    // Không cần Vector3 position nữa nếu vị trí không thay đổi
}
public class PlayerTutorial : MonoBehaviour
{
    public GameObject tutorialPanel;
    public GameObject _KnightCap;
    public GameObject _TutKnightCap;
    public GameObject _Block;
    private DataManager _dataManager;
    [Header("Tutorial Data")]
    public TutorialData _tutorialData;
    //-----------------

    [Header("Tutorial Settings")]
    public Image _tutIBoxImage;
    public Sprite _noneImage;
    public Sprite _moveImage;
    public Sprite _jumpImage;
    public Sprite _dashImage;
    public Sprite _attackImage;

   
    public TextMeshProUGUI _tutTxt;
    // Reference đến CharacterStateMachine để quản lý trạng thái tutorial
    private CharacterStateMachine _characterStateMachine;
    private void Awake()
    {
        _dataManager = FindObjectOfType<DataManager>();
        LoadTutorialData();
    }
    private void Start()
    {

        if (_tutorialData._1stGame)
        {
            _KnightCap.SetActive(false);
            _Block.SetActive(true);
            _TutKnightCap.SetActive(true);
        }else
        {
            _KnightCap.SetActive(true);
            _TutKnightCap.SetActive(false);
            _Block.SetActive(false);
        }
        if (_characterStateMachine == null)
        {
            _characterStateMachine = FindObjectOfType<CharacterStateMachine>();
        }
        tutorialPanel.SetActive(false);
        
    }
    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            ShowTutorial();
        }
        TutalInfor();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && _tutorialData._1stGame)
        {
            // Hiển thị panel tutorial khi lần đầu gặp NPC

            
            ShowTutorial();
            SaveTutorialData();
        }
    } 
    void TutalInfor()
    {
        switch (_characterStateMachine.CurrentTutorialState)
        {
            case CharacterStateMachine.TutorialState.Move:           
                    Debug.Log("-------moveTut-----");
                    _tutTxt.text = "Move";
                break;

            case CharacterStateMachine.TutorialState.Jump:

                    Debug.Log("-----JumpTut------");
                    _tutTxt.text = "Jump";
                break;

            case CharacterStateMachine.TutorialState.Dash:
                    Debug.Log("------DashTut-----");
                    _tutTxt.text = "Dash";
                break;

            case CharacterStateMachine.TutorialState.Attack:
                    Debug.Log("------Attacktut-----");
                    _tutTxt.text = "Attack";

                break;
        }
    }
    void ShowTutorial()
    {
        tutorialPanel.SetActive(true);
        // Bắt đầu tutorial bằng cách kích hoạt tutorial từ CharacterStateMachine
        _characterStateMachine.StartTutorial(CharacterStateMachine.TutorialState.Move);
        StartCoroutine(WaitForTutorialCompletion());
    }

    IEnumerator WaitForTutorialCompletion()
    {
        // Đợi cho đến khi tutorial hoàn thành
        yield return new WaitUntil(() => _characterStateMachine.CurrentTutorialState == CharacterStateMachine.TutorialState.None);
        // Kết thúc tutorial
        _tutorialData._1stGame = false;
        Debug.Log("--------------------------------false---------------------------------");
        SaveTutorialData();
        EndTutorial();
        yield return new WaitForSeconds(3f);
        
    }

    void EndTutorial()
    {
        tutorialPanel.SetActive(false);
        Debug.Log("Tutorial ended."); 
    }
    //---------------------------------------------------------------
    public void SaveTutorialData()
    {
        if (_dataManager != null)
        {
            _dataManager._tutorialData = _tutorialData;
            Debug.Log("Saving Tutorial Data: " + _dataManager._tutorialData._1stGame);
            _dataManager.SaveTutorialData();
        }
             
    }

    public void LoadTutorialData()
    {
       
        if (_dataManager != null)
        {
            _dataManager.LoadTutorialData();
            _tutorialData = _dataManager._tutorialData;
        }
        if (_tutorialData == null)
        {
            _tutorialData = new TutorialData
            {
                _1stGame = true
            };
            _dataManager._tutorialData = _tutorialData;
            _dataManager.SaveTutorialData();
        }
    }
}
