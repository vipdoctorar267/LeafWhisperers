using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vine : MonoBehaviour
{
    private float damageInterval = 1.5f; // Thời gian giữa mỗi lần trừ máu
    private float nextDamageTime = 0f; // Thời gian để trừ máu lần tiếp theo
    private int damageAmount = 100; // Số máu bị trừ mỗi lần
    private PlayerManager _playerManager;
    private void Start()
    {
        _playerManager = FindObjectOfType<PlayerManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("----------------vineon----------------");
            // Khi người chơi vừa vào vùng bẫy, trừ máu ngay lập tức
            if (_playerManager != null)
            {
                _playerManager.PlayerTakeDamage(damageAmount);
                nextDamageTime = Time.time + damageInterval;
                Debug.Log("Player hit by vine! Current health: " + _playerManager._playerData._currentHealth);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("----------------vineStay----------------");
            if (Time.time >= nextDamageTime)
            {
                if (_playerManager != null)
                {
                    _playerManager.PlayerTakeDamage(damageAmount);
                    nextDamageTime = Time.time + damageInterval;
                    Debug.Log("Player hit by vine! Current health: " + _playerManager._playerData._currentHealth);
                }
            }
        }
    }
}
