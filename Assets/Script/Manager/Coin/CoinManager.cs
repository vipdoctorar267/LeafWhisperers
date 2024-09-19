using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public PlayerManager _playerManager;
    private Rigidbody2D rb;
    private AudioSource _audioSource; 
    private SpriteRenderer _spriteRenderer;
    public GameObject Light2d;

    void Start()
    {
        Light2d.SetActive(true);
        if (_playerManager == null)
        {
            _playerManager = FindObjectOfType<PlayerManager>();
        }
        rb = GetComponent<Rigidbody2D>();
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Destroy");
            _playerManager.AddCoin(100);
            _spriteRenderer.enabled = false;
            Light2d.SetActive(false);
            InGameAudioManager.Instance.PlayCoinCollectSound(_audioSource);
            StartCoroutine(DestroyAfterSound());
        }
    }
    private IEnumerator DestroyAfterSound()
    {
        // Đợi cho đến khi âm thanh phát xong
        yield return new WaitForSeconds(InGameAudioManager.Instance.GetClipData("CoinClip").clip.length);
        Destroy(gameObject);
    }
}
