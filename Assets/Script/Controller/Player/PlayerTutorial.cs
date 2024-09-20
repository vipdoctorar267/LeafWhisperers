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
   
    private void Start()
    {
        _dataManager = FindObjectOfType<DataManager>();
        LoadTutorialData();
        if (_tutorialData._1stGame)
        {
            _KnightCap.SetActive(false);
            _Block.SetActive(true);
            _TutKnightCap.SetActive(true);
        }
        else
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
                    _tutTxt.text = "Movement: Press A to move left, and D to move right. While moving with A or D, you can press Left Shift to switch to running mode. Keep in mind that running consumes a lot of stamina, so pay attention to the stamina bar. When your stamina is too low, you won't be able to run.";
                break;

            case CharacterStateMachine.TutorialState.Jump:

                    Debug.Log("-----JumpTut------");
                    _tutTxt.text = "Jumping: Press Space to make the character jump. While in the air, you can press Space again to perform a Double Jump or Triple Jump. After the character has jumped three times, they cannot jump again until they touch the ground or a wall.";
                break;

            case CharacterStateMachine.TutorialState.Dash:
                    Debug.Log("------DashTut-----");
                    _tutTxt.text = "Dash: Press Left Ctrl to make the character perform a Dash, which consumes a large amount of stamina. During a Dash, the character cannot be stopped or controlled. A Dash is considered complete once the player presses Left Ctrl and the character finishes dashing and touches the ground.";
                break;
            case CharacterStateMachine.TutorialState.Climb:
                Debug.Log("------ClimbTut-----");
                _tutTxt.text = "Wall Climbing: When moving through the map and encountering steep vertical terrain, the character can cling to and hang on walls by pressing A (if the wall is on the left) or D (if the wall is on the right). Performing this action will continuously drain stamina.";
                break;       
            case CharacterStateMachine.TutorialState.Attack:
                    Debug.Log("------Attacktut-----");
                    _tutTxt.text = "Attack: When you left-click, the character will immediately perform an attack. After each attack, there is a brief window of time between attacks. If you click again during this time, the character can perform a combo attack. If you don't continue clicking, the character will exit the attack state.";

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
        _KnightCap.SetActive(true);
        _TutKnightCap.SetActive(false);
        _Block.SetActive(false);
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
        
    }
}
