using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    // Singleton để dễ truy cập từ các script khác
    public static SpawnManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Giữ object khi đổi scene
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Hàm để Spawn enemy từ SpawnPoint
    public void SpawnEnemy(GameObject enemyPrefab, Transform spawnPosition)
    {
        Instantiate(enemyPrefab, spawnPosition.position, Quaternion.identity);
    }
}
