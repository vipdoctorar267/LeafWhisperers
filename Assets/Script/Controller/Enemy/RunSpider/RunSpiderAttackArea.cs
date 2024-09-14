
using UnityEngine;

public class RunSpiderAttackArea : MonoBehaviour
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
        if (other.CompareTag("Player"))
        {
            // Khi phát hiện người chơi, có thể thực hiện các hành động như bắt đầu đuổi theo
            RunSpider runSpider = GetComponentInParent<RunSpider>();
            if (runSpider != null)
            {
                Debug.Log("player take dmg");
                _playerManager.PlayerOnAttack(runSpider.CalculateRunSpiderDamage);
            }
        }
    } 
}
