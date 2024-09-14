using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ClimbSpiderData
{
    public int maxHealth = 500;           // Máu tối đa
    public int attack = 200;              // Sức tấn công
    public int defense = 0;               // Phòng thủ
}

public class ClimbSpider : MonoBehaviour
{
    public enum ClimbSpiderState
    {
        Hang,
        Descend,
        Attack, 
        OnDMG,
        Dead  
    }

    public ClimbSpiderState GetCurrentState()
    {
        return currentState; // Trả về trạng thái hiện tại của RunSpider
    }
    [HideInInspector] public Transform Player = null;
    public Transform playerTransform;
    public Transform enemyTransform;
    public PlayerManager _playerManager;
    public GameObject _audio;

    [Header("ClimbSpider Settings")]
    public LayerMask groundLayer;
    public Collider2D enemyCollider;
    [HideInInspector] public bool isGround;
    [HideInInspector] public bool isChasingPlayer = false;
    private Rigidbody2D rb;
    public float DescendSpeed = 10f;
    public float DescendHeight = 3f;
    private Vector3 initialPosition;


    public ClimbSpiderState currentState = ClimbSpiderState.Hang; // Trạng thái hiện tại

    public ClimbSpiderData _climbSpiderData;
    public Slider _climbSpiderHpSlider;
    public int _climbSpiderCurrentHP;
    private float direction = 1; // 1 cho phải, -1 cho trái


    [Header("Shooting Settings")]
    public GameObject _bulletPrefab; // Prefab đạn
    public float _bulletSpeed = 5f;  // Tốc độ của đạn
    

    [Header("ItemDrop Settings")]
    public GameObject coinPrefab;//prefab đạn
    public int _dropcount  = 200;//số lg rớt 100 = 1 prefab 

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
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        initialPosition = transform.position;
        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;
        _audio.SetActive(false);


        _climbSpiderCurrentHP = _climbSpiderData.maxHealth;
        _climbSpiderHpSlider.interactable = false;
        _climbSpiderHpSlider.maxValue = _climbSpiderData.maxHealth;
        _climbSpiderHpSlider.value = _climbSpiderCurrentHP;
    }

    void FixedUpdate()
    {
        if (currentState != ClimbSpiderState.Dead) UpdateDirection(); // Cập nhật hướng 
        SpiderDescendFromCeiling();
        GoBack();
        HpValueCtrl();
        Groundcheck(enemyCollider, groundLayer);
    }

    void SpiderDescendFromCeiling()
    {
        switch (currentState)
        {
            case ClimbSpiderState.Hang:
                
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                if (isChasingPlayer) currentState = ClimbSpiderState.Descend;
                break;
            case ClimbSpiderState.Descend: 
                Decend(); 
                if (transform.position.y <= initialPosition.y - DescendHeight)
                {
                    rb.velocity = Vector2.zero;
                    currentState = ClimbSpiderState.Attack;
                    StartCoroutine(ShootingRoutine());
                }
                   
                break;
            case ClimbSpiderState.Attack:
                // Ở trạng thái Attack, nhện có thể thực hiện các hành động tấn công
                // Thêm logic tấn công ở đây nếu cần
                if (!isChasingPlayer)
                {
                    currentState = ClimbSpiderState.Hang;
                    
                }
                break;
            case ClimbSpiderState.Dead:
                rb.gravityScale = 1;
                break;
            default:
                break;
        }
    }
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
    public void Decend()
    {
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        transform.position = Vector3.MoveTowards(transform.position,
        new Vector3(transform.position.x, initialPosition.y - DescendHeight, transform.position.z), DescendSpeed * Time.deltaTime);
    }
    public void GoBack()
    {
        if (currentState == ClimbSpiderState.Hang && transform.position != initialPosition)
        {
            rb.gravityScale = 0;
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            transform.position = Vector3.MoveTowards(transform.position, initialPosition, DescendSpeed * Time.deltaTime);

            if (transform.position == initialPosition)
            {
                rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Bỏ khóa khi về đúng vị trí ban đầu
                rb.velocity = Vector2.zero;
            }
           
        }
    }
    //------------------------------------------------------------
    private IEnumerator ShootingRoutine()
    {
        while (currentState == ClimbSpiderState.Attack)
        {
            if (_bulletPrefab != null && Player != null)
            {
                // Tạo viên đạn tại vị trí của ClimbSpider
                GameObject bullet = Instantiate(_bulletPrefab, transform.position, Quaternion.identity);
                Debug.Log("---------------Shoot---------------");
                // Tính toán hướng bắn về phía người chơi
                Vector2 direction = (Player.position - transform.position).normalized;

                // Gán vận tốc cho đạn để nó di chuyển về phía người chơi
                Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
                if (bulletRb != null)
                {
                    bulletRb.velocity = direction * _bulletSpeed;
                }
            }

            yield return new WaitForSeconds(3f); // Đợi 3 giây trước khi bắn đợt tiếp theo
            
        }
    }
    
    private void Groundcheck(Collider2D enemyCollider, LayerMask groundLayer)
    {
        isGround = enemyCollider.IsTouchingLayers(groundLayer); 
    }
    //----------------------------------------------------------------------------

    public void HpValueCtrl()
    {
        //update value
        _climbSpiderHpSlider.value = _climbSpiderCurrentHP;
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
        _climbSpiderCurrentHP -= damage;
        Debug.Log("ClimbSpider -" + damage + " Hp");

        // Kiểm tra nếu máu của JumpSpider giảm xuống 0 hoặc thấp hơn
        if (_climbSpiderCurrentHP <= 0)
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
        Debug.Log("ClimbSpider đã bị tiêu diệt!");
        StopAllCoroutines(); // Dừng tất cả các Coroutine
        currentState = ClimbSpiderState.Dead;
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
        Debug.Log("ClimbSpider bị tấn công, còn " + _climbSpiderCurrentHP + " HP");

        // Dừng Coroutine IAttacking nhưng không dừng các Coroutine khác
        StopAllCoroutines();
        currentState = ClimbSpiderState.OnDMG;

        // Gọi Coroutine xử lý Knockback và chuyển trạng thái sau khi knockback hoàn tất
        StartCoroutine(WaitOnDmgAnimEnd());
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
            rb.AddForce(knockbackDirection * knockbackForce*0, ForceMode2D.Impulse);
        }
    }

    private IEnumerator WaitOnDmgAnimEnd()
    {
        yield return new WaitForSeconds(19f / 60f); // Thời gian hoạt ảnh knockback


        // Khởi động lại Coroutine IAttacking nếu cần
        if (isChasingPlayer)
        {
            currentState = ClimbSpiderState.Descend;
        }
    }

}
