using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpSpiderBody : MonoBehaviour
{
    public PlayerManager _playerManager;
    private void Awake()
    {
        if (_playerManager == null)
        {
            _playerManager = FindObjectOfType<PlayerManager>();
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Attack01Area"))
        {
            // Khi phát hiện người chơi, có thể thực hiện các hành động như bắt đầu đuổi theo
            JumpSpider jumpSpider = GetComponentInParent<JumpSpider>();
            if (jumpSpider != null)
            {
                Debug.Log("enemy take dmg");
                jumpSpider.EnemyOnAttack("Attack01");
            }
        }
        if (other.CompareTag("Player"))
        {
            // Khi phát hiện người chơi, có thể thực hiện các hành động như bắt đầu đuổi theo
            JumpSpider jumpSpider = GetComponentInParent<JumpSpider>();
            if (jumpSpider != null && jumpSpider.currentState != JumpSpider.JumpSpiderState.Dead)
            {
                Debug.Log("player take dmg");
                _playerManager.PlayerOnAttack(jumpSpider.CalculateJumpSpiderDamage);
            }
        }
    }
}
