using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunSpiderBody : MonoBehaviour
    
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Attack01Area"))
        {
            // Khi phát hiện người chơi, có thể thực hiện các hành động như bắt đầu đuổi theo
            RunSpider runSpider = GetComponentInParent<RunSpider>();
            if (runSpider != null)
            {
                Debug.Log("enemy take dmg");
                runSpider.EnemyOnAttack("Attack01");
            }
        }
        if (other.CompareTag("Attack02Area"))
        {
            // Khi phát hiện người chơi, có thể thực hiện các hành động như bắt đầu đuổi theo
            RunSpider runSpider = GetComponentInParent<RunSpider>();
            if (runSpider != null)
            {
                Debug.Log("enemy take dmg");
                runSpider.EnemyOnAttack("Attack02");
            }
        }
        if (other.CompareTag("Attack03Area"))
        {
            // Khi phát hiện người chơi, có thể thực hiện các hành động như bắt đầu đuổi theo
            RunSpider runSpider = GetComponentInParent<RunSpider>();
            if (runSpider != null)
            {
                Debug.Log("enemy take dmg");
                runSpider.EnemyOnAttack("Attack03");
            }
        }
    }
}
