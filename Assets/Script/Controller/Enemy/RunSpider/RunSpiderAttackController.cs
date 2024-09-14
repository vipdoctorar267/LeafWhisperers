using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunSpiderAttackController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Khi phát hiện người chơi, có thể thực hiện các hành động như bắt đầu đuổi theo
            RunSpider runSpider = GetComponentInParent<RunSpider>();
            if (runSpider != null && runSpider._currentState != RunSpider.RunSpiderState.Dead)
            {
                runSpider.RunSpiderAllowDash = true;
                runSpider.Player = other.transform;
                if (runSpider.RunSpiderAllowDash && !runSpider.isDashing )
                {
                    StartCoroutine(runSpider.DashCoroutine()); // Bắt đầu Coroutine Dash
                }

            }
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Khi phát hiện người chơi, có thể thực hiện các hành động như bắt đầu đuổi theo
            RunSpider runSpider = GetComponentInParent<RunSpider>();
            if (runSpider != null && runSpider._currentState != RunSpider.RunSpiderState.Dead)
            {
                runSpider.RunSpiderAllowDash = true;  
                runSpider.Player = other.transform;
                if (runSpider.RunSpiderAllowDash && !runSpider.isDashing )
                {
                    StartCoroutine(runSpider.DashCoroutine()); // Bắt đầu Coroutine Dash
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Khi người chơi ra khỏi vùng phát hiện, có thể dừng việc đuổi theo
            RunSpider runSpider = GetComponentInParent<RunSpider>();
            if (runSpider != null && runSpider._currentState != RunSpider.RunSpiderState.Dead)
            {
                runSpider.RunSpiderAllowDash = false;
               
                
            }
        }
    }
}
