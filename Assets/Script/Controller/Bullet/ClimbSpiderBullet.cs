using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbSpiderBullet : MonoBehaviour
{
    public enum BulletState
    {
        Fly,
        Hit
    }

    [HideInInspector] public Transform Player = null;
    public PlayerManager _playerManager;
    private Rigidbody2D rb;
    public BulletState currentState = BulletState.Fly; // Trạng thái hiện tại
    public int _bulletAttack = 200;

    // Biến để truy cập InGameAudioManager
    private AudioSource _audioSource;

    // Âm lượng mặc định
    [Range(0f, 1f)]
    public float volume = 1f; // Giá trị âm lượng mặc định

    void Start()
    {
        if (_playerManager == null)
        {
            _playerManager = FindObjectOfType<PlayerManager>();
        }
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;

        // Cài đặt AudioSource
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Cài đặt âm lượng
        _audioSource.volume = volume;

        // Phát âm thanh khi Bullet bay từ InGameAudioManager
        InGameAudioManager.Instance.PlayBulletSound(_audioSource);
    }

    void FixedUpdate()
    {
        SetBulletState();
    }

    void SetBulletState()
    {
        switch (currentState)
        {
            case BulletState.Fly:
                // Thực hiện các hành động cho trạng thái Fly nếu cần
                break;
            case BulletState.Hit:
                // Thực hiện các hành động cho trạng thái Hit nếu cần
                break;
            default:
                break;
        }
    }

    public int CalculateBulletDamage()
    {
        int damage = Mathf.FloorToInt(_bulletAttack);
        return damage;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Attack01Area"))
        {
            Debug.Log("Destroy");
            Destroy(gameObject);
        }
        if (other.CompareTag("Player"))
        {
            Debug.Log("player take dmg");
            _playerManager.PlayerOnAttack(CalculateBulletDamage);
            Destroy(gameObject);
        }
        if (other.CompareTag("Ground"))
        {
            Debug.Log("Destroy");
            Destroy(gameObject);
        }
    }
}
