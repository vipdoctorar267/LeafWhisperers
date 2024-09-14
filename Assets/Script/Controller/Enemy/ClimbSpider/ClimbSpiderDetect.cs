using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbSpiderDetect : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Khi phát hiện người chơi, có thể thực hiện các hành động như bắt đầu đuổi theo
            ClimbSpider climbSpider = GetComponentInParent<ClimbSpider>();
            if (climbSpider != null)
            {
                climbSpider.isChasingPlayer = true;
                climbSpider.Player = other.transform;

            }
        }
        if (other.CompareTag("SoundRange"))
        {
            ClimbSpider climbSpider = GetComponentInParent<ClimbSpider>();
            if (climbSpider != null) climbSpider._audio.SetActive(true);
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Khi phát hiện người chơi, có thể thực hiện các hành động như bắt đầu đuổi theo
            ClimbSpider climbSpider = GetComponentInParent<ClimbSpider>();
            if (climbSpider != null)
            {
                climbSpider.isChasingPlayer = true;
                climbSpider.Player = other.transform;

            }


        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Khi người chơi ra khỏi vùng phát hiện, có thể dừng việc đuổi theo
            ClimbSpider climbSpider = GetComponentInParent<ClimbSpider>();
            if (climbSpider != null)
            {
                climbSpider.isChasingPlayer = false;
                

            }
        }
        if (other.CompareTag("SoundRange"))
        {
            ClimbSpider climbSpider = GetComponentInParent<ClimbSpider>();
            if (climbSpider != null) climbSpider._audio.SetActive(false);
        }
    }
}
