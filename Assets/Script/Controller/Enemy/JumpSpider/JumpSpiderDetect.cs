using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpSpiderDetect : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Khi phát hiện người chơi, có thể thực hiện các hành động như bắt đầu đuổi theo
            JumpSpider jumpSpider = GetComponentInParent<JumpSpider>();
            if (jumpSpider != null )
            {
                jumpSpider.isChasingPlayer = true;
                jumpSpider.Player = other.transform;
               
            }
        }
        if (other.CompareTag("SoundRange"))
        {
            JumpSpider jumpSpider = GetComponentInParent<JumpSpider>();
            if (jumpSpider != null) jumpSpider._audio.SetActive(true);
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Khi phát hiện người chơi, có thể thực hiện các hành động như bắt đầu đuổi theo
            JumpSpider jumpSpider = GetComponentInParent<JumpSpider>();
            if (jumpSpider != null )
            {
                jumpSpider.isChasingPlayer = true;
                jumpSpider.Player = other.transform;
                
            }


        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Khi người chơi ra khỏi vùng phát hiện, có thể dừng việc đuổi theo
            JumpSpider jumpSpider = GetComponentInParent<JumpSpider>();
            if (jumpSpider != null)
            {
                jumpSpider.isChasingPlayer = false;
                jumpSpider.currentState = JumpSpider.JumpSpiderState.Idle;
               
            }
        }
        if (other.CompareTag("SoundRange"))
        {
            JumpSpider jumpSpider = GetComponentInParent<JumpSpider>();
            if (jumpSpider != null) jumpSpider._audio.SetActive(false);
        }
    }
}
