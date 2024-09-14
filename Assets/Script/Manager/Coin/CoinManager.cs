using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public PlayerManager _playerManager;
    private Rigidbody2D rb;

    private AudioSource _audioSource; // AudioSource riêng cho mỗi coin

    void Start()
    {
        if (_playerManager == null)
        {
            _playerManager = FindObjectOfType<PlayerManager>();
        }
        rb = GetComponent<Rigidbody2D>();

        // Cài đặt AudioSource
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Destroy");
            _playerManager.AddCoin(100);

            // Sử dụng InGameAudioManager để phát âm thanh khi coin bị phá hủy
            InGameAudioManager.Instance.PlayCoinCollectSound(_audioSource);

            // Thay vì phá hủy ngay lập tức, trì hoãn cho đến khi âm thanh phát xong
            StartCoroutine(DestroyAfterSound());
        }
    }

    // Coroutine để trì hoãn việc phá hủy
    private IEnumerator DestroyAfterSound()
    {
        // Đợi cho đến khi âm thanh phát xong
        yield return new WaitForSeconds(InGameAudioManager.Instance.GetClipData("CoinClip").clip.length);
        Destroy(gameObject);
    }
}
