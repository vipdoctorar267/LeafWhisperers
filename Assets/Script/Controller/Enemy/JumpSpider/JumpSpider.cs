using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class JumpSpiderData
{
    public int maxHealth = 500;           // Máu tối đa
    public int attack = 200;              // Sức tấn công
    public int defense = 0;               // Phòng thủ
}

public class JumpSpider : MonoBehaviour
{
    public enum JumpSpiderState
    {
        Idle,
        Jump,
        Fall,
        Dead,
        OnDMG
    }
    public JumpSpiderState GetCurrentState()
    {
        return currentState; // Trả về trạng thái hiện tại của RunSpider
    }

    public JumpSpiderState currentState = JumpSpiderState.Idle; // Trạng thái khởi tạo ban đầu là Idle
    public PlayerManager _playerManager;
    public Transform playerTransform;
    public Transform enemyTransform;
    public GameObject _audio;
    [HideInInspector] public Transform Player = null;
    [HideInInspector] public bool isChasingPlayer = false;

    public JumpSpiderData _jumpSpiderData;
    public Slider _jumpSpiderHpSlider;
    public int _jumpSpiderCurrentHP;

    public LayerMask groundLayer;
    public Collider2D enemyCollider;
    public float customGravityScale = 5.0f;
    public float JumpSpiderJumpStrength = 35f;
    public float JumpSpiderHorizontalStrength = 0.5f; // Lực điều chỉnh chiều ngang
    public float JumpSpiderVerticalStrength = 8f;   // Lực điều chỉnh chiều dọc
    public float JumpSpiderAttackCooldown = 1f;
    public int JumpSpiderMaxJumpCount = 3;

    public bool isGround;
    private Rigidbody2D rb;
    
    private float direction = 1; // 1 cho phải, -1 cho trái

    [Header("ItemDrop Settings")]
    public GameObject coinPrefab;//prefab đạn
    public int _dropcount = 200;//số lg rớt 100 = 1 prefab 
    private void Awake()
    {
        if (_playerManager == null)
        {
            _playerManager = FindObjectOfType<PlayerManager>();
        }
        if (playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        }
    }

    private void Start()
    {
        
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = customGravityScale;
        _audio.SetActive(false);
        _jumpSpiderCurrentHP = _jumpSpiderData.maxHealth;
        _jumpSpiderHpSlider.interactable = false;
        _jumpSpiderHpSlider.maxValue = _jumpSpiderData.maxHealth;
        _jumpSpiderHpSlider.value = _jumpSpiderCurrentHP;
    }
    private void Update()
    {
        HpValueCtrl();
    }

    private void FixedUpdate()
    {
        if ( currentState != JumpSpiderState.Dead) UpdateDirection(); // Cập nhật hướng 


        if (isChasingPlayer)
        {
            JumpSpiderChasePlayer();
        }
        else
        {
            jumpAttacking = false;
        }

        // Cập nhật trạng thái rơi khi vận tốc theo trục y nhỏ hơn 0
        if (rb.velocity.y < 0 && currentState != JumpSpiderState.Fall
            && currentState != JumpSpiderState.Dead && currentState != JumpSpiderState.OnDMG)
        {
            currentState = JumpSpiderState.Fall;
        }
        if (rb.velocity.y > 0 && currentState != JumpSpiderState.Jump
            && currentState != JumpSpiderState.Dead && currentState != JumpSpiderState.OnDMG)
        {
            currentState = JumpSpiderState.Jump;
        }
        // Gọi Groundcheck với biến collider và layer mask
        Groundcheck(enemyCollider, groundLayer);
    }

    [HideInInspector] public bool jumpAttacking = false;

    void UpdateDirection()
    {
        if (isChasingPlayer && playerTransform != null)
        {
            // Cập nhật hướng dựa trên vị trí của Player
            direction = playerTransform.position.x < transform.position.x ? -1f : 1f;

            // Giữ nguyên localScale.y và localScale.z, chỉ thay đổi localScale.x
            Vector3 newScale = transform.localScale;
            newScale.x = Mathf.Abs(newScale.x) * direction; // Giữ nguyên chiều hướng mà không lật
            transform.localScale = newScale;
        }
    }
    


    public void JumpSpiderChasePlayer()
    {
        if (jumpAttacking) return;
        jumpAttacking = true;
        StartCoroutine(IAttacking());
    }

    IEnumerator IAttacking()
    {
        int jumpCount = 0;
        while (jumpAttacking)
        {
            yield return new WaitForSeconds(1f);
            yield return new WaitUntil(() => isGround);

            var directionIdentifier = playerTransform.position - transform.position;
            var direction = Mathf.Sign(directionIdentifier.x);
            Vector2 jumpDirection = new Vector2(JumpSpiderHorizontalStrength * direction, JumpSpiderVerticalStrength).normalized;
            rb.velocity = jumpDirection * JumpSpiderJumpStrength;
            jumpCount += 1;
            if (jumpCount == JumpSpiderMaxJumpCount)
            {
                yield return new WaitForSeconds(3f);
                jumpCount = 0;
            }
        }
    }

    private void Groundcheck(Collider2D enemyCollider, LayerMask groundLayer)
    {
        isGround = enemyCollider.IsTouchingLayers(groundLayer);
        if (isGround && currentState != JumpSpiderState.Idle
            && currentState != JumpSpiderState.Dead && currentState != JumpSpiderState.OnDMG)
        {
            currentState = JumpSpiderState.Idle;
        }
    }

    //--------------------------------------------------------------------------------------------
    public void HpValueCtrl()
    {
        //update value
        _jumpSpiderHpSlider.value = _jumpSpiderCurrentHP;
    }

    //--------------------------------------------------------------------------------------------
    public void GetTransforms(out Transform playerTransform, out Transform enemyTransform)
    {
        // Sử dụng các trường public đã kéo thả từ Inspector
        playerTransform = this.playerTransform;
        enemyTransform = this.enemyTransform;
    }

    public void EnemyOnAttack(string attackType)
    {
        // Lấy transform của Player và JumpSpider
        Transform playerTransform, enemyTransform;
        GetTransforms(out playerTransform, out enemyTransform);

        // Tính toán sát thương và knockback từ người chơi
        PlayerManager.AttackResult result = _playerManager.CalculatePlayerDamage(attackType, playerTransform.position, enemyTransform.position);

        // Gọi phương thức nhận sát thương cho JumpSpider và truyền kết quả sát thương
        JumpSpiderTakeDmg(result.Damage);

        // Gọi phương thức xử lý knockback cho JumpSpider với lực và hướng knockback
        JumpSpiderKnockback(result.KnockbackForce, result.KnockbackDirection);
    }

    //---------------------------------------------------------------------------------------------
    public void JumpSpiderTakeDmg(int damage)
    {
        // Giảm lượng máu của JumpSpider dựa trên sát thương nhận được
        _jumpSpiderCurrentHP -= damage;
        Debug.Log("JumpSpider -" + damage + " Hp");

        // Kiểm tra nếu máu của JumpSpider giảm xuống 0 hoặc thấp hơn
        if (_jumpSpiderCurrentHP <= 0)
        {
            // Gọi phương thức để xử lý khi JumpSpider chết
            Die();
        }
        else
        {
            // Xử lý khi JumpSpider bị tấn công nhưng chưa chết (ví dụ: phát âm thanh đau đớn, chuyển sang trạng thái phòng thủ, v.v.)
            OnDamageTaken();
        }
    }

    private void Die()
    {
        // Xử lý logic khi JumpSpider chết, như phá hủy object, cập nhật UI, v.v.
        Debug.Log("JumpSpider đã bị tiêu diệt!");
        StopAllCoroutines(); // Dừng tất cả các Coroutine
        currentState = JumpSpiderState.Dead;
        StartCoroutine(WaitDieAnimEnd());
    }

    private IEnumerator WaitDieAnimEnd()
    {
        yield return new WaitForSeconds(15f / 60f + 3f);
        DropItem();
        Destroy(gameObject); // Ví dụ: phá hủy object sau khi chết
    }

    private void DropItem()
    {
        int itemCount = _dropcount / 100; // Chia số lượng rớt cho 100 để xác định số lượng prefab coin
        for (int i = 0; i < itemCount; i++)
        {
            // Instantiate prefab coin tại vị trí của ClimbSpider
            Instantiate(coinPrefab, transform.position, Quaternion.identity);
        }
    }
    private void OnDamageTaken()
    {
        Debug.Log("JumpSpider bị tấn công, còn " + _jumpSpiderCurrentHP + " HP");

        // Dừng Coroutine IAttacking nhưng không dừng các Coroutine khác
        StopCoroutine("IAttacking");
        currentState = JumpSpiderState.OnDMG;

        // Gọi Coroutine xử lý Knockback và chuyển trạng thái sau khi knockback hoàn tất
        StartCoroutine(WaitKnockBackAnimEnd());
    }

    public void JumpSpiderKnockback(float knockbackForce, Vector2 knockbackDirection)
    {
        // Đảm bảo knockbackDirection được chuẩn hóa để chỉ là hướng (không chứa thông tin về độ lớn)
        knockbackDirection = knockbackDirection.normalized;

        // Lấy Rigidbody2D của JumpSpider để áp dụng lực
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            // Reset vận tốc hiện tại để knockback có tác dụng rõ ràng hơn
            rb.velocity = Vector2.zero;
            Debug.Log("-------------KnockBack-------------------- ");
            // Áp dụng lực knockback
            rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
        }
    }

    private IEnumerator WaitKnockBackAnimEnd()
    {
        if(isGround) yield return new WaitForSeconds(15f / 60f); // Thời gian hoạt ảnh knockback
        else yield return new WaitForSeconds(19f / 60f); // Thời gian hoạt ảnh knockback

        // Đảm bảo trạng thái Idle nếu không còn chase player
        if (!isChasingPlayer||isGround)
        {
            currentState = JumpSpiderState.Idle;
        }else if(!isGround && rb.velocity.y > 0) currentState = JumpSpiderState.Jump;
        else if (!isGround && rb.velocity.y < 0) currentState = JumpSpiderState.Fall;
        // Đợi thêm thời gian để đảm bảo knockback hoàn tất
        yield return new WaitForSeconds(0.2f);

        // Khởi động lại Coroutine IAttacking nếu cần
        if (isChasingPlayer)
        {
            StartCoroutine(IAttacking());
        }
    }

    //--------------------------------------------------------------------------------------------
    public int CalculateJumpSpiderDamage()
    {
        int damage = Mathf.FloorToInt(_jumpSpiderData.attack);
        return damage;
    }
}
