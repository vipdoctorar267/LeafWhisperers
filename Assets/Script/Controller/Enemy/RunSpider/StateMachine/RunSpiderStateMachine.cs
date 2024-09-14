using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunSpiderStateMachine : MonoBehaviour
{
    public enum SpiderState
    {
        Wander,
        Chase,
        DashAttack,
        AffterAttack
    }
    //biến Statemachine
    public SpiderState currentStateType;
    private RunSpiderState currentState;

    //unity2d 
     public Transform Player;
    [HideInInspector] public Rigidbody2D rb;

    public CustomTrigger BodyTrigger;
    public CustomTrigger attackRangeTrigger;
    public CustomTrigger detectRangeTrigger;


    //Controller
    public float WanderSpeed = 2f, ChaseSpeed=5;
    public float PatrolStartPos = 0f;
    public float PatrolEndPos = 5f;
    public bool IsMovingRight = true;
    public float RunSpiderDashStrength = 10f, RunSpiderDashDuration = 0.5f, RunSpiderDashCD = 2f, RunSpiderDashTimer, RunSpiderDashCDTimer;

    
    public bool RunSpiderAllowDash = true; 
    public bool RunSpiderIsDash = false; 
    
    public int RunSpiderDirection; 

 

    private void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>(); // Tự động tìm và gán Rigidbody2D từ đối tượng hiện tại
            if (rb == null)
            {
                Debug.LogError("Rigidbody2D component not found on the object. Please assign it manually.");
            }
        }
    }
    void Start()
    {


        

        if (Player == null)
        {
            Debug.LogError("Player transform could not be found.");
            return;
        }

        PatrolStartPos = transform.position.x; // Thiết lập vị trí tuần tra bắt đầu
        PatrolEndPos = PatrolStartPos + 5f; // Thiết lập vị trí tuần tra kết thúc


        // Đăng ký các sự kiện với CustomTrigger
        BodyTrigger.EnteredTrigger += HandleBodyEnter;
        BodyTrigger.ExitedTrigger += HandleBodyExit;
        attackRangeTrigger.EnteredTrigger += HandleAttackRangeEnter;
        attackRangeTrigger.ExitedTrigger += HandleAttackRangeExit;
        detectRangeTrigger.EnteredTrigger += HandleDetectRangeEnter;
        detectRangeTrigger.ExitedTrigger += HandleDetectRangeExit;





        SetState(new RunSpiderWanderState(this));
    }

    void Update()
    {
        currentState.Update();

        UpdateDirection();
        // Logic cập nhật Dash và Cooldown
        //UpdateDash();
    }
    public void UpdateDash()
    {
        // Thiết lập thời gian và trạng thái dash

        RunSpiderDashTimer = RunSpiderDashDuration;
        RunSpiderDashCDTimer = RunSpiderDashCD;

        if (RunSpiderIsDash)
        {
            RunSpiderDashTimer -= Time.deltaTime;

            if (RunSpiderDashTimer <= 0)
            {
                RunSpiderIsDash = false;
                rb.velocity = new Vector2(0, rb.velocity.y);
                rb.gravityScale = 1;
                SetState(new RunSpiderChaseState(this));
            }
        }

        if (!RunSpiderAllowDash)
        {
            RunSpiderDashCDTimer -= Time.deltaTime;

            if (RunSpiderDashCDTimer <= 0)
            {
                RunSpiderAllowDash = true;
            }
        }
    }
    public void SetState(RunSpiderState newState)
    {
        if (currentState != null)
            currentState.Exit();

        currentState = newState;
        currentState.Enter();

        // Truyền Transform của Player cho trạng thái mới
        if (currentState is RunSpiderChaseState chaseState)
        {
            chaseState.SetPlayerTransform(Player);
        }
        else if (currentState is RunSpiderDashState dashState)
        {
            dashState.SetPlayerTransform(Player);
        }
    }


    public int direction = 1; // 1: Hướng phải, -1: Hướng trái

    // Hàm này cập nhật hướng di chuyển của enemy
    public void UpdateDirection()
    {
        // Kiểm tra hướng Dash nếu đang Dash
        if (RunSpiderIsDash)
        {
            direction = RunSpiderDirection;
        }
        else if (currentState is RunSpiderChaseState && Player != null)
        {
            // Nếu trong trạng thái Chase và Player không null
            direction = Player.position.x < transform.position.x ? -1 : 1;
        }
        else
        {
            // Nếu không phải đang Dash hoặc Chase, xác định hướng dựa trên trạng thái di chuyển
            if (IsMovingRight && direction != 1)
            {
                direction = 1;
            }
            else if (!IsMovingRight && direction != -1)
            {
                direction = -1;
            }
        }


        // Lật hướng của enemy bằng cách thay đổi scale theo trục X
        transform.localScale = new Vector3(direction, transform.localScale.y, transform.localScale.z);
    }

    public IEnumerator DashCoroutine()
    {
        // Thiết lập thời gian và trạng thái dash
        RunSpiderIsDash = true;
        RunSpiderDashTimer = RunSpiderDashDuration;
        RunSpiderDashCDTimer = RunSpiderDashCD;

        // Vô hiệu hóa trọng lực và thiết lập vận tốc ban đầu
        rb.gravityScale = 0;
        rb.velocity = new Vector2(RunSpiderDashStrength * RunSpiderDirection, rb.velocity.y);
        Debug.Log("Dash started with velocity: " + rb.velocity);
        // Chờ trong khoảng thời gian của dash
        yield return new WaitForSeconds(RunSpiderDashDuration);

        // Kết thúc Dash
        RunSpiderIsDash = false;
        rb.velocity = new Vector2(0, rb.velocity.y);
        rb.gravityScale = 1;
        SetState(new RunSpiderChaseState(this));

        // Bắt đầu thời gian hồi chiêu Dash
        yield return new WaitForSeconds(RunSpiderDashCD);

        // Cho phép Dash trở lại sau khi hồi chiêu
        RunSpiderAllowDash = true;
    }

    //------------------------------------------------------------------------------------------------------

    void HandleBodyEnter(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            Debug.Log("Player entered BodyTrigger, Player Get Dmg");
            

        }
    }
    void HandleBodyExit(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            Debug.Log("Player exi BodyTrigger, Player Get Dmg");
            

        }
    }

    void HandleDetectRangeEnter(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            // Khi Player vào phạm vi detectRangeTrigger, chuyển sang trạng thái Chase
            SetState(new RunSpiderChaseState(this));
        }
    }

    void HandleDetectRangeExit(Collider2D collider)
    {
        if (currentStateType == SpiderState.AffterAttack) return;
        if (collider.CompareTag("Player"))
        {
            if (currentStateType == SpiderState.Chase)
            {
                SetState(new RunSpiderWanderState(this));
            }
        }
    }

    void HandleAttackRangeEnter(Collider2D collider)
    {
        //if (collider.CompareTag("Player"))
        //{
        //    // Khi Player vào phạm vi attackRangeTrigger, kiểm tra cooldown Dash và chuyển sang Dash 

        //    if (RunSpiderAllowDash)
        //    {
        //        var dashState = new RunSpiderDashState(this);
        //        dashState.SetPlayerTransform(Player); // Truyền Player vào DashState
        //        SetState(dashState);
        //    }         
        //}
    }

    void HandleAttackRangeExit(Collider2D collider)
    {
        //if (collider.CompareTag("Player"))
        //{
        //    // Khi Player ra khỏi phạm vi attackRangeTrigger, chuyển về trạng thái Chase
        //    //rb.gravityScale = 1;
        //    var chaseState = new RunSpiderChaseState(this);
        //    chaseState.SetPlayerTransform(Player);
        //    SetState(new RunSpiderChaseState(this));
        //}
    }

    void OnDestroy()
    {
        // Hủy đăng ký các sự kiện khi đối tượng bị hủy
        attackRangeTrigger.EnteredTrigger -= HandleAttackRangeEnter;
        attackRangeTrigger.ExitedTrigger -= HandleAttackRangeExit;
        detectRangeTrigger.EnteredTrigger -= HandleDetectRangeEnter;
        detectRangeTrigger.ExitedTrigger -= HandleDetectRangeExit;
    }
}
