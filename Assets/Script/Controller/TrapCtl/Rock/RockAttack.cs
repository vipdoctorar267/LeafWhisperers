using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockAttack : MonoBehaviour
{
    private PlayerManager _playerManager;
    void Start()
    {
        if (_playerManager == null)
        {
            _playerManager = FindObjectOfType<PlayerManager>();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            FallingRockTrap _fallRock = GetComponentInParent<FallingRockTrap>();
            Debug.Log("----------------Rockon----------------");
            // Khi người chơi vừa vào vùng bẫy, trừ máu ngay lập tức
            if (_playerManager != null)
            {
                _playerManager.PlayerTakeDamage(_fallRock.damage);

                Debug.Log("Player hit by vine! Current health: " + _playerManager._playerData._currentHealth);
            }
        }
       
    }
}
