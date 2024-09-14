using System.Collections;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public enum EnemyType { RunSpider, JumpSpider, ClimbSpider }
    public EnemyType enemyType;

    public GameObject runSpiderPrefab;
    public GameObject jumpSpiderPrefab;
    public GameObject climbSpiderPrefab;

    private GameObject currentEnemy; // Enemy đang được quản lý bởi spawn point
    public float checkInterval = 300f; // 5 phút (có thể điều chỉnh)

    private void Start()
    {
        // Lần đầu spawn enemy
        SpawnEnemy();
        // Bắt đầu kiểm tra cứ sau mỗi checkInterval (5 phút)
        StartCoroutine(CheckEnemyStatusRoutine());
    }

    // Coroutine kiểm tra trạng thái của enemy
    private IEnumerator CheckEnemyStatusRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(checkInterval);
            CheckAndRespawnEnemy();
        }
    }

    // Kiểm tra và spawn enemy nếu enemy đã bị destroy
    private void CheckAndRespawnEnemy()
    {
        if (currentEnemy == null)
        {
            SpawnEnemy();
        }
    }

    // Hàm spawn enemy dựa trên loại quái được chọn
    private void SpawnEnemy()
    {
        GameObject enemyPrefab = null;

        switch (enemyType)
        {
            case EnemyType.RunSpider:
                enemyPrefab = runSpiderPrefab;
                break;
            case EnemyType.JumpSpider:
                enemyPrefab = jumpSpiderPrefab;
                break;
            case EnemyType.ClimbSpider:
                enemyPrefab = climbSpiderPrefab;
                break;
        }

        if (enemyPrefab != null)
        {
            currentEnemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        }
    }
}
