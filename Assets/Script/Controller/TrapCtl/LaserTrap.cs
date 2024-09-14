using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserTrap : MonoBehaviour
{
    private float damageInterval = 0.05f; 
    private float nextDamageTime = 0f;
    public float activeTime = 2f; // Thời gian laser bật
    public float inactiveTime = 2f; // Thời gian laser tắt
    public int damage = 60; // Sát thương gây ra cho người chơi
    private bool isActive = false;
    private Collider2D laserCollider;

    [SerializeField] private GameObject _lazeTrap;
    private PlayerManager _playerManager;
    void Start()
    {
        if (_playerManager == null)
        {
            _playerManager = FindObjectOfType<PlayerManager>();
        }
        laserCollider = GetComponent<Collider2D>();
        StartCoroutine(LaserControl());
    }

    IEnumerator LaserControl()
    {
        while (true)
        {
            isActive = !isActive;
            laserCollider.enabled = isActive;
            if (isActive)
            _lazeTrap.SetActive(true);
            else
            _lazeTrap.SetActive(false);
            yield return new WaitForSeconds(isActive ? activeTime : inactiveTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("----------------Lazeon----------------");
            // Khi người chơi vừa vào vùng bẫy, trừ máu ngay lập tức
            if (_playerManager != null)
            {
                _playerManager.PlayerTakeDamage(damage);
                nextDamageTime = Time.time + damageInterval;
                Debug.Log("Player hit by vine! Current health: " + _playerManager._playerData._currentHealth);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("----------------LazeStay----------------");
            if (Time.time >= nextDamageTime)
            {
                if (_playerManager != null)
                {
                    _playerManager.PlayerTakeDamage(damage);
                    nextDamageTime = Time.time + damageInterval;
                    Debug.Log("Player hit by vine! Current health: " + _playerManager._playerData._currentHealth);
                }
            }
        }
    }
}
