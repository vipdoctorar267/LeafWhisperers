using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStateMachine : MonoBehaviour
{
    private CharacterState _currentState;

    public CharIdleState IdleState { get; private set; }
    public CharMoveState MoveState { get; private set; }
    public CharJumpState JumpState { get; private set; }
    public CharClimbState ClimbState { get; private set; }
    public CharDashState DashState { get; private set; }
    public CharFallState FallState { get; private set; }
    public CharAttackState AttackState { get; private set; }
    public CharOnDMGState OnDMGState { get; private set; }
    public CharDeadState DeadState { get; private set; }


    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask leftWallLayer;
    [SerializeField] private LayerMask rightWallLayer;
    [SerializeField] private LayerMask enemyLayer;

    public Rigidbody2D rb { get; private set; }
    public CapsuleCollider2D PlayerCollider { get; private set; }
    public Collider2D touchingCheck;
    public Collider2D wallCheck;
    public Collider2D attack01Area;
    public Transform _bodyShadow;
    public SpriteRenderer spriteRenderer { get; private set; }

    public CharAnimManager animManager;
    private Transform playerAvatarTransform;
    //---------------------------------------------------------------------

    public float attackMoveDistance = 1f;
    //---------------------------------------------------------------------
    public int jumpCount = 0; // Biến đếm số lần nhảy
    private int maxJumpCount = 3;
    public float customGravityScale = 2.0f;
    public float moveSpeed = 5f;
    public float spaceMoveSpeed = 2.5f;
    public float runSpeedMultiplier = 2f;
    public float runThreshold = 0.5f;
    public float jumpForce = 15f;
    public float dashStrength = 25f;
    public float dashDuration = 0.3f;
    public float dashCooldown = 1f;
    public float maxSlopeAngle = 15f;
    public float dashCD = 1f;
    public float dashTimer = 0f;
    public float dashCDTimer = 0f; 
    public float direction =1;

    public bool allowDash = true;
    public bool allowClimb = true;
    public bool isIdle;
    public bool isWalk;
    public bool isRunning;
    public bool isJump;
    public bool isSpace;
    public bool isDash;
    public bool isAttack;
    public bool isGround;
    public bool isClimbing;
    public bool isTouchingLeftWall;
    public bool isTouchingRightWall;
    public bool canDoubleJump;
    public bool canTripleJump;
    public bool onDMG = false;
    public bool isDead=false;
    //-------------------------------------------------------------------------

    public enum CharFxState
    {
        Idle,
        Walk,
        Run,
        Jump,
        Attack01,
        Climb,
        Dash,
        Fall,
        OnDMG,
        Dead
    }
    public CharFxState GetCurrentState()
    {
        return CurrentFxState; // Trả về trạng thái hiện tại của RunSpider
    }
    public CharFxState CurrentFxState { get; private set; }

    public void SetAnimState(CharFxState newAnimState)
    {
        CurrentFxState = newAnimState;
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = customGravityScale; // Đặt trọng lực tùy chỉnh cho Rigidbody2D
        PlayerCollider = GetComponent<CapsuleCollider2D>();
        //touchingCheck = GetComponentInChildren<Collider2D>();
        //wallCheck = GetComponentInChildren<Collider2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        attack01Area.enabled = false;

        animManager = GetComponent<CharAnimManager>();

        IdleState = new CharIdleState(this);
        MoveState = new CharMoveState(this);
        JumpState = new CharJumpState(this);
        ClimbState = new CharClimbState(this);
        DashState = new CharDashState(this);
        FallState = new CharFallState(this);
        AttackState = new CharAttackState(this);
        OnDMGState = new CharOnDMGState(this);
        DeadState = new CharDeadState(this);
        SetState(IdleState);

        
    }
    private void Update()
    {
       if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            // Bắt đầu tutorial từ trạng thái Move
            StartTutorial(TutorialState.Move);
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
            CompleteTutorial();

        
        JumpCoutDown();

    }

    private void FixedUpdate()
    {
        _currentState?.Update();
        
        FallCheck();
        GroundCheck();
        WallCheck();
        DashTimer();
        Direction();
        JumpAnim();

        NextTutorialStep();
        if (CurrentTutorialState != TutorialState.None)
        {
            
            HandleTutorial();
            return;
        }
        MoveInput();
        JumpInput();
        DashInput();
        Attack01Input();
       
        
        GetDmg();
    }
    public void SetState(CharacterState newState)
    {
        _currentState?.Exit();
        _currentState = newState;
        _currentState.Enter();
    }
    public void GetDmg()
    {
        if (onDMG)
        {
            SetState(OnDMGState);
        } 
        if(isDead) SetState(DeadState);
    }
    private void EnableWallCheck()
    {
        wallCheck.enabled = true; // Bật lại Collider sau khi nhảy khỏi tường
    }
    private void GroundCheck()
    {
        isGround = touchingCheck.IsTouchingLayers(groundLayer);
        if (isGround)
        {
            ResetJump(); // Đặt lại số lần nhảy khi chạm đất
        }

    }
    private void WallCheck()
    {
        isTouchingLeftWall = wallCheck.IsTouchingLayers(leftWallLayer) && Input.GetKey(KeyCode.D);
        isTouchingRightWall = wallCheck.IsTouchingLayers(rightWallLayer) && Input.GetKey(KeyCode.A);
        isClimbing = isTouchingLeftWall || isTouchingRightWall;
        //Debug.Log("isClimbing" + isClimbing);
        if (isClimbing)
        {
            ResetJump(); // Đặt lại số lần nhảy khi chạm tg
        }
    }
    private void FallCheck()
    {
        if (isJump) return;
        if (rb.velocity.y < 0 && !isGround)
        {
            if (_currentState != FallState)
            {
                SetState(FallState);             
            }
        }
       

    }
    private void JumpAnim()
    {
        if (isJump)
        {
            SetAnimState(CharFxState.Jump);
        }
    }

    private float moveInput;  // Biến toàn cục để lưu giá trị đầu vào di chuyển
    
    public void MoveInput()
    {
        if(isGround)
        {
            bool moveLeft = Input.GetKey(KeyCode.A);
            bool moveRight = Input.GetKey(KeyCode.D);
            isRunning = Input.GetKey(KeyCode.LeftShift) && Mathf.Abs(Input.GetAxis("Horizontal")) > runThreshold;
            moveInput = moveLeft ? -1 : (moveRight ? 1 : 0);
        }
        if (!isClimbing && !isDash && !isAttack  )
        {
            if (isGround && !isRunning)
                isWalk = moveInput != 0;
            else if (!isGround || isRunning)
                isWalk = false;
            // Cập nhật `direction` khi có input từ người dùng
            if (moveInput != 0)
            {
                direction = moveInput;
                Move();
            }
        }
    }

    public void Direction()
    {
        if (direction != 0) spriteRenderer.flipX = direction < 0;
        if (isTouchingLeftWall) direction = -1;
        if (isTouchingRightWall) direction = 1;
    }

    public void Move()
    {
        if (isDash || isClimbing ||isJump || isAttack || onDMG ) return;

        if (isGround)
        {
            Vector2 moveVelocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
            rb.velocity = moveVelocity;

            if (isRunning)
            {
                rb.velocity = new Vector2(moveVelocity.x * runSpeedMultiplier, rb.velocity.y);
            }
        }
    }


    public void JumpCoutDown()
    {
        if (Input.GetKeyDown(KeyCode.Space) && (isGround || (jumpCount < maxJumpCount)))
        {
            jumpCount++;
        }
    }

    // Hàm nhận đầu vào từ người chơi
    public void JumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isDash && !isDead) // Nhảy không phụ thuộc tường
        {
            Jump();
        }
    }

    public void Jump()
    {
        if (isDash) return;
        if (!isWalk && !isRunning && isJump)
        {
            // Chuyển động theo trục ngang khi nhảy
            moveInput = Input.GetKey(KeyCode.A) ? -1 : (Input.GetKey(KeyCode.D) ? 1 : 0);
            Vector2 moveVelocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
            rb.velocity = moveVelocity;
            Debug.Log("SpaceMove");
        }

        if (isClimbing)
        {
            if (Input.GetKey(KeyCode.A) &&  isTouchingLeftWall ) // Nhảy từ tường trái
            {
                wallCheck.enabled = false;
                Invoke("EnableWallCheck", 0.2f); // Tạm thời tắt Collider để tránh va chạm không mong muốn
                rb.gravityScale = customGravityScale; // Đặt trọng lực tùy chỉnh khi không leo tường
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                isJump = true;

                // Đặt trạng thái nhảy thủ công
                if (_currentState != JumpState)
                {
                    SetState(JumpState);
                }
            }
            else if (Input.GetKey(KeyCode.D) && isTouchingRightWall) // Nhảy từ tường phải
            {
                wallCheck.enabled = false;
                Invoke("EnableWallCheck", 0.2f);
                rb.gravityScale = customGravityScale;
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);

                // Đặt trạng thái nhảy thủ công
                if (_currentState != JumpState)
                {
                    SetState(JumpState);
                }
            }
        }
        else if (isGround || jumpCount < maxJumpCount) // Nhảy khi ở trên mặt đất hoặc còn số lần nhảy
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isJump = true;

            // Đặt trạng thái nhảy thủ công
            if (_currentState != JumpState)
            {
                SetState(JumpState);
            }
        }
    }


    public void ResetJump()
    {
        jumpCount = 0; // Đặt lại số lần nhảy khi chạm đất
    }
    public void DashInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) && allowDash)
        {
            SetState(DashState);
        }
    }
    public void DashTimer()
    {
        if (isClimbing) { isDash = false; return; }
        if (isDash)
        {
            dashTimer -= Time.fixedDeltaTime; // Giảm thời gian Dash

            if (dashTimer <= 0)
            {
                isDash = false; // Kết thúc trạng thái Dash khi hết thời gian
                rb.velocity = new Vector2(0, 0); // Đặt lại vận tốc sau khi Dash
                rb.gravityScale = customGravityScale; //Đặt lại trọng lực sau khi Dash 
            }
        }

        if (!allowDash)
        {
            dashCDTimer -= Time.fixedDeltaTime; // Giảm thời gian hồi chiêu Dash

            if (dashCDTimer <= 0 && (isGround || isClimbing))
            {
                allowDash = true; // Cho phép Dash tiếp theo khi thời gian hồi chiêu kết thúc
            }
        }
        
    }
    //----------------------------------------------------------------------------------
    public void ShadowUpdate()
    {
        _bodyShadow.transform.localScale = new Vector3(direction, 1, 1);
        _bodyShadow.transform.localPosition =
        new Vector2(Mathf.Abs(_bodyShadow.transform.localPosition.x) * direction, _bodyShadow.transform.localPosition.y);
    }

    //-----------------------------------------------------------------------------------
    public void Attack01Input()
    {
        if (isAttack || isDash || isClimbing ||onDMG) return; 
        // Gọi hàm Attack01() khi nhấn chuột trái
        if (Input.GetMouseButtonDown(0)&&isGround)
        {
            SetState(AttackState);
        }
    }
    public void Attack01()
    {
        // Di chuyển nhân vật tiến lên trước
        rb.velocity = new Vector2(attackMoveDistance * direction, rb.velocity.y);
       

        // Bật Collider của Attack01Area

        attack01Area.transform.localScale = new Vector3(direction, 1, 1);
        attack01Area.transform.localPosition = new Vector2(Mathf.Abs(attack01Area.transform.localPosition.x) * direction, attack01Area.transform.localPosition.y);
        attack01Area.enabled = true;
        Debug.Log("Attack01");

        // Collider sẽ va chạm với Enemy và Enemy sẽ tự xử lý việc nhận damage dựa trên tag của collider
        // Không cần xử lý gì thêm ở đây, chỉ bật collider lên và để OnTriggerEnter2D của Enemy xử lý

        // Tắt Collider sau khi tấn công
        Invoke("DisableAttackCollider", 15f/60f); // Đợi một chút trước khi tắt để đảm bảo va chạm xảy ra
    }
    

    void DisableAttackCollider()
    {
        attack01Area.enabled = false;
        rb.velocity = new Vector2(0, rb.velocity.y);
       
    }


    //-----------------------------------------------------------------------------------------------
    public enum TutorialState
    {
        None,        // Không trong tutorial
        Move,        // Hướng dẫn di chuyển (A và D)
        Jump,        // Hướng dẫn nhảy (Space)
        Dash,        // Hướng dẫn Dash (Left Control)
        Attack       // Hướng dẫn Attack (Chuột trái)
    }
    public TutorialState _currentTutorialState;// inspector check
    public TutorialState CurrentTutorialState { get; private set; } = TutorialState.None;

    private bool isNextAllowed = false;
    private bool isClick = false;// Khóa chuyển bước
    public void StartTutorial(TutorialState initialStep)
    {
        CurrentTutorialState = initialStep;
        Debug.Log("Tutorial started: " + CurrentTutorialState);
    }

    // Biến trạng thái để theo dõi việc mở khóa từng tính năng
    private bool isMoveUnlocked = false;
    private bool isJumpUnlocked = false;
    private bool isDashUnlocked = false;
    private bool isAttackUnlocked = false;

    private void HandleTutorial()
    {
        // Các hành động đã mở khóa vẫn được phép thực hiện
        if (isMoveUnlocked) MoveInput();
        if (isJumpUnlocked) JumpInput();
        if (isDashUnlocked) DashInput();
        if (isAttackUnlocked) Attack01Input();

        // Switch case để xử lý các bước hướng dẫn hiện tại
        switch (CurrentTutorialState)
        {
            case TutorialState.Move:
                if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
                {
                    Debug.Log("Move step completed");
                    isMoveUnlocked = true;
                    isNextAllowed = true;
                }
                break;

            case TutorialState.Jump:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Debug.Log("Jump step completed");
                    isJumpUnlocked = true;
                    isNextAllowed = true;
                }
                break;

            case TutorialState.Dash:
                if (Input.GetKeyDown(KeyCode.LeftControl))
                {
                    Debug.Log("Dash step completed");
                    isDashUnlocked = true;
                    isNextAllowed = true;
                }
                break;

            case TutorialState.Attack:
                if (Input.GetMouseButtonDown(0))
                {
                    Debug.Log("Attack step completed");
                    isAttackUnlocked = true;
                    isNextAllowed = true;
                }
                break;
        }
    }


    public void NextTutorialStep()
    {
        if (isNextAllowed && isClick) // Nếu chưa được phép hoặc chưa nhấn nút, không chuyển bước
        {
            if (CurrentTutorialState == TutorialState.Move)
            {
                CurrentTutorialState = TutorialState.Jump;

            }
            else if (CurrentTutorialState == TutorialState.Jump)
            {
                CurrentTutorialState = TutorialState.Dash;

            }
            else if (CurrentTutorialState == TutorialState.Dash)
            {
                CurrentTutorialState = TutorialState.Attack;

            }
            isNextAllowed = false;  // Khóa lại sau khi chuyển bước
            isClick = false;
            // Đặt lại trạng thái nhấn nút
            Debug.Log("Next tutorial step: " + CurrentTutorialState);
        }
        else return;
    }

    private void CompleteTutorial()
    {
        CurrentTutorialState = TutorialState.None;
        Debug.Log("Tutorial completed!");
    }

    private IEnumerator SetClickFalse()
    {
        yield return null;  // Đợi một frame để tránh sự kiện bị trùng lặp
        isClick = false;
    }

    public void OnButtonClickNextTut()  // Phương thức gọi khi nút được nhấn
    {
        isClick = true;  // Đặt trạng thái nhấn nút
        StartCoroutine(SetClickFalse());  // Đặt lại trạng thái sau một frame
    }

}
