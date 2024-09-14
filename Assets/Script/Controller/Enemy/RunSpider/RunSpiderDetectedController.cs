using UnityEngine;


public class RunSpiderDetectedController : MonoBehaviour
{
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
           
            // Khi phát hiện người chơi, có thể thực hiện các hành động như bắt đầu đuổi theo
            RunSpider runSpider = GetComponentInParent<RunSpider>();
            if (runSpider != null&& !runSpider.RunSpiderAllowDash && runSpider._currentState != RunSpider.RunSpiderState.Dead)
            {
                runSpider.isChasingPlayer = true;
                runSpider.Player = other.transform;
                runSpider._currentState = RunSpider.RunSpiderState.Chase;
            }
            
        }
        if (other.CompareTag("SoundRange"))
        {
            RunSpider runSpider = GetComponentInParent<RunSpider>();
            if (runSpider != null) runSpider._audio.SetActive(true);
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
           
            // Khi phát hiện người chơi, có thể thực hiện các hành động như bắt đầu đuổi theo
            RunSpider runSpider = GetComponentInParent<RunSpider>();
            if (runSpider != null && !runSpider.RunSpiderAllowDash && !runSpider.isDashing && runSpider._currentState != RunSpider.RunSpiderState.Dead)
            {
                runSpider.isChasingPlayer = true;
                runSpider.Player = other.transform;
                runSpider._currentState = RunSpider.RunSpiderState.Chase;
            }
            //-------------------------------
            

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
                runSpider.isChasingPlayer = false;
                runSpider._currentState = RunSpider.RunSpiderState.Wander;
            }
        }
        if (other.CompareTag("SoundRange"))
        {
            RunSpider runSpider = GetComponentInParent<RunSpider>();
            if (runSpider != null) runSpider._audio.SetActive(false);
        }
    }
}
