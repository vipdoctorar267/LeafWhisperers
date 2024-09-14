using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class RunspiderData 
{
    public int _maxHealth =500;           // Máu tối đa  
    public int _attack=200;              // Sức tấn công
    public int _defense=0;             // Phòng thủ  
}
public class RunSpider : MonoBehaviour
{

    public enum RunSpiderState
    {
        Wander, // Đi tuần
        Chase,    // Đuổi theo
        Dash,    // Tấn công mạnh
        Idle,         // Nghỉ
        OnDMG, 
        Dead
    }
    public RunSpiderState GetCurrentState()
    {
        return _currentState; // Trả về trạng thái hiện tại của RunSpider
    }

    public RunSpiderState _currentState = RunSpiderState.Wander;
    public PlayerManager _playerManager;
    public Transform playerTransform;
    public Transform enemyTransform;
    //--------------------------------------------------------------
    public RunspiderData _runspiderData;
    public Slider _runSpiderHpSlider;
    public int _runSpiderCurrentHP;
    public GameObject _audio;
    public GameObject _atRange;
    public GameObject _dtRange;

    //--------------------------------------------------------------
    [HideInInspector] public bool isChasingPlayer = false;
    [HideInInspector] public Transform Player = null;
    private Rigidbody2D rb;
    private bool IsMovingRight = true;
    [HideInInspector] public bool isDashing = false;
    private float direction = 1; // 1 cho phải, -1 cho trái
    public float PatrolSpeed = 2f;    // Tốc độ đi tuần
    public float ChaseSpeed = 4f;     // Tốc độ đuổi theo
    private float PatrolStartPos;
    private float PatrolEndPos;
    public float PatrolDistance = 5f; // Khoảng cách đi tuần

    [HideInInspector] public bool RunSpiderAllowDash = false;
    private float RunSpiderDashStrength = 10f; // Độ mạnh của đợt tấn công
    private float RunSpiderDirection;

    [SerializeField] private Collider2D _AttackArea; // Thêm Detect Collider

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
    void Start()
    {
        PatrolStartPos = transform.position.x;
        PatrolEndPos = PatrolStartPos + PatrolDistance;
        rb = GetComponent<Rigidbody2D>();
        _currentState = RunSpiderState.Wander;
        _audio.SetActive(false);
        _atRange.SetActive(true);
        _dtRange.SetActive(true);
        //------------------------------------------
        _runSpiderCurrentHP = _runspiderData._maxHealth;
        //-------
        _runSpiderHpSlider.interactable = false;
        _runSpiderHpSlider.maxValue = _runspiderData._maxHealth;
        _runSpiderHpSlider.value = _runSpiderCurrentHP;
    }

    void Update()
    {
        
        HpValueCtrl();
    }

    void FixedUpdate()
    { 
        if (_currentState != RunSpiderState.Dead) RunSpiderBehavior();


    }
    void RunSpiderBehavior()
    {
        if(_currentState == RunSpiderState.Dead)
        {
            return; // Không làm gì cả nếu đang ở trạng thái Dead
        }
        // Chỉ cập nhật hướng khi không phải là trạng thái Idle
        if (_currentState != RunSpiderState.Idle)
        {
            UpdateDirection();
        }

        switch (_currentState)
        {
            case RunSpiderState.Wander:
                rb.gravityScale = 1;
                RunSpiderWander();
                break;
            case RunSpiderState.Chase:
                rb.gravityScale = 1;
                RunSpiderChasePlayer();
                break;
            case RunSpiderState.Dash:
                // Dash được xử lý bởi Coroutine
                break;
            case RunSpiderState.Idle:
                rb.gravityScale = 1;
                break;
            case RunSpiderState.OnDMG:
                rb.gravityScale = 1;
                // Không di chuyển hoặc hành động, đã có Coroutine xử lý
                break;
            case RunSpiderState.Dead:
                RunSpiderAllowDash = false;
                _atRange.SetActive(false);
                _dtRange.SetActive(false);
                rb.gravityScale = 1;
                break;
        }
    }
    //--------------------------------------------------------------------------------------------
    public void HpValueCtrl()
    {
        //update value
        _runSpiderHpSlider.value = _runSpiderCurrentHP;
        if(_runSpiderCurrentHP <= 0) _currentState = RunSpiderState.Dead;

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
        // Lấy transform của Player và RunSpider
        Transform playerTransform, enemyTransform;
        GetTransforms(out playerTransform, out enemyTransform);

        // Tính toán sát thương và knockback từ người chơi
        PlayerManager.AttackResult result = _playerManager.CalculatePlayerDamage(attackType, playerTransform.position, enemyTransform.position);

        // Gọi phương thức nhận sát thương cho RunSpider và truyền kết quả sát thương
        RunSpiderTakeDmg(result.Damage);

        // Gọi phương thức xử lý knockback cho RunSpider với lực và hướng knockback
        RunSpiderKnockback(result.KnockbackForce, result.KnockbackDirection);
    }

    //---------------------------------------------------------------------------------------------
    public void RunSpiderTakeDmg(int damage)
    {
        // Giảm lượng máu của RunSpider dựa trên sát thương nhận được
        _runSpiderCurrentHP -= damage;
        Debug.Log("RunSpider -" + damage+ " Hp");

        // Kiểm tra nếu máu của RunSpider giảm xuống 0 hoặc thấp hơn
        if (_runSpiderCurrentHP <= 0)
        {
            // Gọi phương thức để xử lý khi RunSpider chết
            Die();
        }
        else
        {
            // Xử lý khi RunSpider bị tấn công nhưng chưa chết (ví dụ: phát âm thanh đau đớn, chuyển sang trạng thái phòng thủ, v.v.)
            OnDamageTaken();
        }
    }

    private void Die()
    {
        // Xử lý logic khi RunSpider chết, như phá hủy object, cập nhật UI, v.v.
        Debug.Log("RunSpider đã bị tiêu diệt!");
        StopAllCoroutines(); // Dừng tất cả các Coroutine
        _currentState = RunSpiderState.Dead;
        StartCoroutine(WaitDieAnimEnd());
    }

    private IEnumerator WaitDieAnimEnd()
    {
        yield return new WaitForSeconds(30f / 60f+3f);
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
        Debug.Log("RunSpider bị tấn công, còn " + _runSpiderCurrentHP + " HP");
        StopAllCoroutines();
        _currentState = RunSpiderState.OnDMG;

        // Gọi Coroutine xử lý Knockback và chuyển trạng thái sau khi knockback hoàn tất
        StartCoroutine(WaitKnockBackAnimEnd());
    }
    public void RunSpiderKnockback(float knockbackForce, Vector2 knockbackDirection)
    {
        // Đảm bảo knockbackDirection được chuẩn hóa để chỉ là hướng (không chứa thông tin về độ lớn)
        knockbackDirection = knockbackDirection.normalized;

        // Lấy Rigidbody2D của RunSpider để áp dụng lực
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
        yield return new WaitForSeconds(13f / 60f); // Thời gian hoạt ảnh knockback

        // Đảm bảo chuyển trạng thái về Idle
        _currentState = RunSpiderState.Idle;

        // Đợi thêm thời gian để đảm bảo knockback hoàn tất
        yield return new WaitForSeconds(0.2f);

        // Chuyển trạng thái về Wander nếu không còn ở trạng thái Chase
        if (!isChasingPlayer)
        {
            _currentState = RunSpiderState.Wander;
        }
    }

    //--------------------------------------------------------------------------------------------
    public int CalculateRunSpiderDamage()
    {
        int damage = Mathf.FloorToInt(_runspiderData._attack);
        return damage;
    }

    

    //--------------------------------------------------------------------------------------------
    void RunSpiderWander()
    {
        if (_currentState == RunSpiderState.Dash || _currentState == RunSpiderState.OnDMG
            || _currentState == RunSpiderState.Dead) return;
        float patrolSpeedThisFrame = PatrolSpeed * Time.deltaTime;

        if (IsMovingRight)
        {
            transform.Translate(Vector2.right * patrolSpeedThisFrame);
            if (transform.position.x >= PatrolEndPos)
            {
                IsMovingRight = false;
                StartCoroutine(IdleCoroutine(1f)); // Chuyển sang trạng thái Idle 1 giây
            }
        }
        else
        {
            transform.Translate(Vector2.left * patrolSpeedThisFrame);
            if (transform.position.x <= PatrolStartPos)
            {
                IsMovingRight = true;
                StartCoroutine(IdleCoroutine(1f)); // Chuyển sang trạng thái Idle 1 giây
            }
        }

        if (isChasingPlayer)
        {
            _currentState = RunSpiderState.Chase;
        }
    }

    void RunSpiderChasePlayer()
    {
        if (_currentState == RunSpiderState.Dash || _currentState == RunSpiderState.Idle
            || _currentState == RunSpiderState.OnDMG || _currentState == RunSpiderState.Dead) return;


        float chaseSpeedThisFrame = ChaseSpeed * Time.deltaTime;

        if (Player.position.x < transform.position.x)
        {
            transform.Translate(Vector2.left * chaseSpeedThisFrame);
            IsMovingRight = false;
        }
        else if (Player.position.x > transform.position.x)
        {
            transform.Translate(Vector2.right * chaseSpeedThisFrame);
            IsMovingRight = true;
        }
    }


    void UpdateDirection()
    {
        if (_currentState == RunSpiderState.Dash)
        {
            direction = RunSpiderDirection;
        }
        else if (_currentState == RunSpiderState.Chase && Player != null)
        {
            direction = Player.position.x < transform.position.x ? -1 : 1;
        }
        else
        {
            direction = IsMovingRight ? 1 : -1;
        }

        transform.localScale = new Vector3(direction, transform.localScale.y, transform.localScale.z);
    }

    public IEnumerator DashCoroutine()
    {
        if (isDashing || _currentState == RunSpiderState.Dead) yield break; // Nếu đang dashing, thì không làm gì cả
        isDashing = true; // Đánh dấu đang dashing
        
        // Bật Collider AttackArea
        if (_AttackArea != null)
        {
            _AttackArea.enabled = true;
        }
        Debug.Log("----------------RunSpDash------------------");

        // Chuẩn bị cho Dash (nghỉ 2 giây)
        _currentState = RunSpiderState.Idle;
        yield return new WaitForSeconds(1f);

        // Bắt đầu Dash
        RunSpiderDirection = Player.position.x < transform.position.x ? -1 : 1;
        _currentState = RunSpiderState.Dash;
        rb.gravityScale = 0;
        rb.velocity = new Vector2(RunSpiderDashStrength * RunSpiderDirection, rb.velocity.y);

        // Đợi cho đến khi Dash kết thúc
        yield return new WaitForSeconds(0.8f);

        // Cooldown Dash và trạng thái Idle
        rb.velocity = new Vector2(0, rb.velocity.y);
        rb.gravityScale = 1;
        _currentState = RunSpiderState.Idle;

        // Nghỉ 3 giây sau Dash
        yield return new WaitForSeconds(2f);

        // Tắt Collider AttackArea
        if (_AttackArea != null)
        {
            _AttackArea.enabled = false;
        }

        isDashing = false; // Đánh dấu dashing kết thúc
    }


    IEnumerator IdleCoroutine(float idleTime)
    {
        // Giữ hướng hiện tại
        float initialDirection = direction;

        _currentState = RunSpiderState.Idle;
        yield return new WaitForSeconds(idleTime);

        if (_currentState == RunSpiderState.Idle) // Đảm bảo không thay đổi trạng thái trong khi đang nghỉ
        {
            // Giữ hướng hiện tại
            transform.localScale = new Vector3(initialDirection, transform.localScale.y, transform.localScale.z);
           
            _currentState = isChasingPlayer ? RunSpiderState.Chase : RunSpiderState.Wander;
        }
    }
}
