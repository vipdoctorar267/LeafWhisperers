using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController2D : MonoBehaviour
{
    // LayerMask
    [SerializeField] private LayerMask groundLayer; // Lớp mặt đất
    [SerializeField] private LayerMask leftWallLayer; // Lớp tường bên trái
    [SerializeField] private LayerMask rightWallLayer; // Lớp tường bên phải

    // Thông số 
    //Trọng lục tùy chỉnh 
    public float customGravityScale = 2.0f; // Giá trị trọng lực tùy chỉnh
    // di chuyển 
    [SerializeField] private float moveSpeed = 5f; // Tốc độ di chuyển của nhân vật

    // chạy
    [SerializeField] private float runSpeedMultiplier = 2f; // Hệ số nhân tốc độ khi chạy
    [SerializeField] private float runThreshold = 0.5f; // Ngưỡng để xác định khi nào nhân vật đang chạy

    // nhảy 
    [SerializeField] private float jumpForce = 15; // Lực nhảy

    // dash
    
    /*[SerializeField] private float dashCooldown = 1f;*/ // Thời gian hồi chiêu của Dash
    
    [SerializeField] private float dashStrength = 20f; // Lực đẩy khi Dash
    [SerializeField] private float dashDuration = 0.2f; // Thời gian kéo dài của Dash
    [SerializeField] private float dashCD = 1f; // Thời gian hồi chiêu của Dash (thời gian chờ để thực hiện Dash lại)
    private float dashTimer = 0f; // Thời gian còn lại của Dash
    private float dashCDTimer = 0f; // Thời gian hồi chiêu còn lại của Dash
    private float direction = 1; // Hướng của nhân vật (1: phải, -1: trái)

    //  Biến Điều kiện 
    private bool allowDash = true; // Cho phép Dash hay không
    private bool allowClimb =true;
    private bool isIdle; // Xác định nhân vật có đang đứng yên không 
    private bool isWalk; // Xác định nhân vật có đang đi hay không
    private bool isRunning; // Xác định nhân vật có đang chạy hay không
    private bool isJump; // Xác định nhân vật có đang nhảy hay không
    private bool isFall; // Xác định nhân vật có đang rơi không
    private bool isDash; // Xác định nhân vật có đang Dash hay không
    private bool isGround; // Xác định nhân vật có đang đứng trên mặt đất hay không
    private bool isClimbing; // Xác định nhân vật có đang leo tường hay không
    private bool isTouchingLeftWall; // Xác định nhân vật có chạm vào tường bên trái hay không
    private bool isTouchingRightWall; // Xác định nhân vật có chạm vào tường bên phải hay không
    private bool canDoubleJump; // Cho phép nhân vật nhảy lần hai (nhảy kép)
    private bool canTripleJump; // Cho phép nhân vật nhảy lần ba (nhảy ba)

    // Biến điều khiển
    private bool moveLeft, moveRight, jump, spaceA, spaceD, dash;

    // Thành phần Unity 
    [SerializeField] private Rigidbody2D rb; // Thành phần Rigidbody2D của nhân vật
    [SerializeField] private CapsuleCollider2D PlayerCollider; // Thành phần Collider của nhân vật
    [SerializeField] private Collider2D touchingCheck; // Collider của TouchingCheck
    [SerializeField] private Collider2D wallCheck; // Collider của wallCheck
    [SerializeField] private SpriteRenderer spriteRenderer;

    // Biến từ class khác 
    private CharacterAnimationManager animManager; // Quản lý hoạt ảnh của nhân vật
   
    //
    private Transform playerAvatarTransform; // Transform của nhân vật chính để điều chỉnh vị trí và hướng

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Lấy thành phần Rigidbody2D từ nhân vật
        rb.gravityScale = customGravityScale; // Đặt trọng lực tùy chỉnh cho Rigidbody2D
        PlayerCollider = GetComponent<CapsuleCollider2D>(); // Lấy thành phần Collider từ nhân vật
        animManager = GetComponentInChildren<CharacterAnimationManager>(); // Lấy thành phần quản lý hoạt ảnh từ nhân vật con
        playerAvatarTransform = animManager.transform; // Lấy Transform từ quản lý hoạt ảnh để điều chỉnh hình ảnh
        direction = 1; // Khởi tạo hướng di chuyển mặc định là bên phải
    }

    private void Update()
    {
        //input 
        MoveInput(); // Gọi hàm xử lý input di chuyển
        JumpInput(); // Gọi hàm xử lý input nhảy
        SpaceInput(); // Gọi hàm xử lý input leo tường
        DashInput(); // Gọi hàm xử lý input Dash


        //logic
        Move(); // Gọi hàm di chuyển
        SpaceMove();//Gọi hàm di chuyển trên không
        Jump(); // Gọi hàm nhảy
        Fall(); //Gọi hàm Fall 
        Climb(); // Gọi hàm leo tường
    }

    private void FixedUpdate()
    {

        Dash(); // Gọi hàm Dash trong FixedUpdate để đảm bảo tính đồng nhất
    }

    private void EnableWallCheck()
    {
        wallCheck.enabled = true; // Bật lại Collider sau khi nhảy khỏi tường
    }

    // Check 
    private void Groundcheck()
    {
        if (isJump) return;
        // Kiểm tra va chạm với các collider thuộc groundLayer
        isGround = touchingCheck.IsTouchingLayers(groundLayer);

        if (isGround && isFall)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);//vận tốc y luôn băng 0 khi chạm sàn 
            animManager.TriggerFallToIdle(); // Trigger hoạt ảnh FallToIdle
            isFall = false;
        }

        // In giá trị của isGround ra console
        // Debug.Log("Is Ground: " + isGround);
    }


    private void WallCheck()
    {
        isClimbing = isTouchingLeftWall || isTouchingRightWall; // Kiểm tra xem nhân vật có đang leo tường hay không
        isTouchingLeftWall = wallCheck.IsTouchingLayers(leftWallLayer); // Kiểm tra xem nhân vật có chạm vào tường bên trái hay không
        isTouchingRightWall = wallCheck.IsTouchingLayers(rightWallLayer); // Kiểm tra xem nhân vật có chạm vào tường bên phải hay không

        //Debug.Log("Is Climb : " + isClimbing);
    }

    private void Direction()
    {
        if (direction > 0)
        {
            playerAvatarTransform.localScale = new Vector3(1, 1, 1); // Đặt hướng nhân vật về bên phải
        }
        else if (direction < 0)
        {
            playerAvatarTransform.localScale = new Vector3(-1, 1, 1); // Đặt hướng nhân vật về bên trái
        }
    }
    
    //Điều khiển 
    private void MoveInput()
    {
        moveLeft = Input.GetKey(KeyCode.A);
        moveRight = Input.GetKey(KeyCode.D);
        isRunning = Input.GetKey(KeyCode.LeftShift) && Mathf.Abs(Input.GetAxis("Horizontal")) > runThreshold;
    }

    
    private void Move()
    {
        Groundcheck();
        Direction();
        if (!isGround || isDash|| isClimbing ) return; // Nếu đang Dash,Climb thì không thực hiện di chuyển bình thường
        

        float moveInput = moveLeft ? -1 : (moveRight ? 1 : 0); // Lấy giá trị điều khiển ngang
        Vector2 moveVelocity = new Vector2(moveInput * moveSpeed, rb.velocity.y); // Tạo vận tốc mới cho nhân vật
        rb.velocity = moveVelocity; // Cập nhật vận tốc của nhân vật

        if (moveInput != 0) isWalk = true;
        else
        {
            isWalk = false;
        }

        if (isRunning)
        {
            rb.velocity = new Vector2(moveVelocity.x * runSpeedMultiplier, rb.velocity.y); // Tăng tốc độ khi đang chạy
        }

        
        if (moveInput > 0 && !isClimbing) direction = 1;  else if (moveInput < 0 && !isClimbing) direction = -1; // Cập nhật hướng 

        // Cập nhật hoạt ảnh dựa trên trạng thái di chuyển
        animManager.UpdateAnimations(isGround, isWalk, isRunning, isClimbing, isFall, isDash); 
        
    }

    private void JumpInput()
    {
        //jump = Input.GetKeyDown(KeyCode.Space);
    }


    private void Jump()
    {


        if (isGround || isClimbing)
        {
            canDoubleJump = true; // Cho phép nhảy lần hai nếu đang ở trên mặt đất hoặc tường
            canTripleJump = false; // Đặt lại trạng thái nhảy ba

            if (jump)
            {
                if (isClimbing && (spaceA || spaceD))
                {
                    if (isTouchingLeftWall && spaceA)
                    {
                        rb.velocity = new Vector2(-jumpForce, jumpForce); // Nhảy sang bên phải khi leo tường
                    }
                    else if (isTouchingRightWall && spaceD)
                    {
                        rb.velocity = new Vector2(jumpForce, jumpForce); // Nhảy sang bên trái khi leo tường
                    }
                    isClimbing = false;
                    rb.gravityScale = customGravityScale; // Đặt trọng lực tùy chỉnh cho Rigidbody2D khi ko leo tg
                    wallCheck.enabled = false;
                    Invoke("EnableWallCheck", 0.2f); // Tạm thời tắt Collider để tránh va chạm không mong muốn
                    isJump = true;
                    animManager.TriggerClimbJump(); // Trigger hoạt ảnh ClimbJump
                }
                else
                 if (isGround)
                {
                    
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce); // Nhảy thẳng lên khi tiếp đất
                    isGround = false; // Đặt lại trạng thái không tiếp đất
                    isJump = true;
                    animManager.TriggerJump(); // Trigger hoạt ảnh Jump
                }
               
            }
        }
        else if (canDoubleJump)
        {
            if (jump)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce); // Nhảy lần hai khi đang ở trên không
                canDoubleJump = false; // Đặt lại trạng thái không cho phép nhảy lần hai
                canTripleJump = true; // Cho phép nhảy lần ba
                isJump = true;
                animManager.TriggerSpaceJump(); // Trigger hoạt ảnh SpaceJump
            }
        }
        else if (canTripleJump)
        {
            if (jump)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce); // Nhảy lần ba khi đang ở trên không
                canTripleJump = false; // Đặt lại trạng thái không cho phép nhảy lần ba
                isJump = true;
                animManager.TriggerSpaceJump(); // Trigger hoạt ảnh SpaceJump
            }
        }
        
        if (isGround || isClimbing || rb.velocity.y <= 0)
        {
            isJump = false; // Đặt lại trạng thái không nhảy khi tiếp đất hoặc độ coa trục y giảm 
        }
        animManager.UpdateAnimations(isGround, isWalk, isRunning, isClimbing, isFall, isDash);  // Cập nhật hoạt ảnh


    }

private void Fall()
    {
        
        if (rb.velocity.y > 0||isJump)
        {
            isFall = false;

        }
        else if(rb.velocity.y < 0 && !isGround && !isClimbing)
        {
            animManager.TriggerJumpToFall(); // Trigger JumpToFall animation
            if (!animManager.IsPlaying("JumpToFall"))
            {
                //animManager.TriggerFall(); // Trigger hoạt ảnh Fall
                isFall = true;

            }
            
        }
        else  if (isGround)
        {
            animManager.TriggerFallToIdle();// Trigger hoạt ảnh FallToIdle
            isFall = false;
            // Cập nhật hoạt ảnh dựa trên trạng thái rơi
            
        }

        animManager.UpdateAnimations(isGround, isWalk, isRunning, isClimbing, isFall, isDash);
    }



    private void SpaceInput()
    {
        spaceA = Input.GetKey(KeyCode.A);
        spaceD = Input.GetKey(KeyCode.D);
    }

    private void SpaceMove()
    {
        if (isDash|| isGround) return;// Nếu đang Dash,trên nền thì không thực hiện di chuyển trên không  
        float moveInput = spaceA ? -1 : (spaceD ? 1 : 0); // Lấy giá trị điều khiển ngang
        Vector2 moveVelocity = new Vector2(moveInput * moveSpeed, rb.velocity.y); // Tạo vận tốc mới cho nhân vật
        rb.velocity = moveVelocity; // Cập nhật vận tốc của nhân vật
        if (!isClimbing) //Chỉnh hướng quay nhan vật trên không 
        {
            if (moveInput > 0 && !isClimbing) direction = 1; else if (moveInput < 0 && !isClimbing) direction = -1;
        }
    }


    private void Climb()
    {
        WallCheck();
       
       
        if (isClimbing)
        {
            if (isTouchingLeftWall) //Nếu chạm tường trái mà bấm D thì treo 
            {
                direction = -1;
                if (spaceD)
                {
                    
                    rb.gravityScale = 0; // Tắt trọng lực khi leo tường
                }
                rb.gravityScale = customGravityScale; // Đặt trọng lực tùy chỉnh cho Rigidbody2D khi không leo tường

            }
            else if (isTouchingRightWall) //Nếu chạm tường phải mà bấm A thì treo 
            {
                direction = 1;
                if(spaceA)
                {
                    
                    rb.gravityScale = 0; // Tắt trọng lực khi leo tường
                }
                rb.gravityScale = customGravityScale; // Đặt trọng lực tùy chỉnh cho Rigidbody2D khi không leo tường

            }
            

        }
        else
        {
            rb.gravityScale = customGravityScale; // Đặt trọng lực tùy chỉnh cho Rigidbody2D khi không leo tường
        }

        // Cập nhật hoạt ảnh dựa trên trạng thái leo tường
        animManager.UpdateAnimations(isGround, isWalk, isRunning, isClimbing, isFall, isDash);
    }



    private void DashInput()
    {
        dash = Input.GetKey(KeyCode.LeftControl);
    }

    private void Dash()
    {
        if (dash && allowDash)
        {
            isDash = true; // Đặt trạng thái đang Dash
            dashTimer = dashDuration; // Đặt thời gian Dash
            dashCDTimer = dashCD; // Đặt thời gian hồi chiêu Dash
            allowDash = false; // Không cho phép Dash tiếp theo cho đến khi thời gian hồi chiêu kết thúc
            rb.gravityScale = 0;
            rb.velocity = new Vector2(rb.velocity.x, 0);// Đặt lại vận tốc y trước khi Dash 
            rb.velocity = new Vector2(dashStrength * direction, rb.velocity.y); // Cập nhật vận tốc Dash
            animManager.TriggerDash();
            //animManager.DashAnimationTrigger(); // Kích hoạt hoạt ảnh Dash
        }

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





}
