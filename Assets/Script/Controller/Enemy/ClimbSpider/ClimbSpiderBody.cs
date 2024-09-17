using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbSpiderBody : MonoBehaviour
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
            ClimbSpider climbSpider = GetComponentInParent<ClimbSpider>();
            if (climbSpider != null)
            {
                Debug.Log("enemy take dmg");
                climbSpider.EnemyOnAttack("Attack01");
            }
        }
        if (other.CompareTag("Attack02Area"))
        {
            // Khi phát hiện người chơi, có thể thực hiện các hành động như bắt đầu đuổi theo
            ClimbSpider climbSpider = GetComponentInParent<ClimbSpider>();
            if (climbSpider != null)
            {
                Debug.Log("enemy take dmg");
                climbSpider.EnemyOnAttack("Attack02");
            }
        }
        if (other.CompareTag("Attack03Area"))
        {
            // Khi phát hiện người chơi, có thể thực hiện các hành động như bắt đầu đuổi theo
            ClimbSpider climbSpider = GetComponentInParent<ClimbSpider>();
            if (climbSpider != null)
            {
                Debug.Log("enemy take dmg");
                climbSpider.EnemyOnAttack("Attack03");
            }
        }

    }
}
