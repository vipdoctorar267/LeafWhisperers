using UnityEngine;
using System.Collections;

public class FallingRockTrap : MonoBehaviour
{
    private CharacterStateMachine _charStateMachine;
    private PlayerManager _playerManager;
    private Rigidbody2D rb;
    //-----------------------
    public float detectionDistance = 10f; // Khoảng cách raycast phát hiện người chơi
    public float fallSpeed = 5f; // Tốc độ rơi của bẫy
    private Vector3 initialPosition; // Lưu vị trí ban đầu của bẫy
    public float returnSpeed = 2f; // Tốc độ quay trở lại vị trí ban đầu
    public int damage = 100000; // Sát thương gây ra (HP = 0)
    public LayerMask playerLayer; // Lớp chứa Player
    public Transform raycastOrigin; // Điểm xuất phát của raycast (vị trí của bẫy)
    public Collider2D _AtCollider;
    public Collider2D _BdCollider;


    public bool playerDetected = false;
    private bool isFalling = false;
    void Start()
    {
        if (_playerManager == null)  _playerManager = FindObjectOfType<PlayerManager>();
        if (_charStateMachine == null) _charStateMachine = FindObjectOfType<CharacterStateMachine>();

        // Lưu vị trí ban đầu
        initialPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
    }
    void Update()
    {
        _AtCollider.enabled = isFalling;
        _AtCollider.isTrigger = isFalling;
        _BdCollider.isTrigger = isFalling;
        if (!playerDetected && !_charStateMachine.isDead)
        {
            RaycastHit2D hit = Physics2D.Raycast(raycastOrigin.position, Vector2.down, detectionDistance, playerLayer);
            if (hit.collider != null && hit.collider.CompareTag("Player") )
            {
                playerDetected = true;
                StartCoroutine(TriggerFall());
            }
        }
        else if (isFalling)
        {
            Debug.Log("--------------------Falllllllllllllllllllllllll-----------------");
            transform.Translate(Vector2.down * fallSpeed * Time.deltaTime);
        }
    }

    IEnumerator TriggerFall()
    {
        
            yield return new WaitForSeconds(1f); // Delay trước khi đá rơi
            isFalling = true;
       
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.CompareTag("Ground")) // Kiểm tra va chạm với Ground
        {
            isFalling = false; // Dừng rơi
            StartCoroutine(ReturnToInitialPosition()); // Bắt đầu quay lại vị trí ban đầu
        }
    }

    IEnumerator ReturnToInitialPosition()
    {
        yield return new WaitForSeconds(3f); // Chờ 3 giây trước khi quay lại

        while (Vector3.Distance(transform.position, initialPosition) > 0.1f) // Kiểm tra khoảng cách tới vị trí ban đầu
        {
            transform.position = Vector3.MoveTowards(transform.position, initialPosition, returnSpeed * Time.deltaTime);
            yield return null;
        }

        playerDetected = false; // Reset trạng thái bẫy để có thể kích hoạt lại
    }

    private void OnDrawGizmosSelected()
    {
        // Hiển thị Raycast trong Scene View
        Gizmos.color = Color.red;
        Gizmos.DrawLine(raycastOrigin.position, raycastOrigin.position + Vector3.down * detectionDistance);
    }
}
