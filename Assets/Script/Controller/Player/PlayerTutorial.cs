using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerTutorial : MonoBehaviour
{
    public GameObject tutorialPanel;
    public bool _1stGame = true; // Kiểm tra lần đầu gặp NPC
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
            _1stGame = false;
            ShowTutorial();
        }
        TutalInfor();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("NPC") && _1stGame)
        {
            // Hiển thị panel tutorial khi lần đầu gặp NPC
            _1stGame = false;
            ShowTutorial();
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
        EndTutorial();
    }

    void EndTutorial()
    {
        tutorialPanel.SetActive(false);
        Debug.Log("Tutorial ended.");
    }
}
