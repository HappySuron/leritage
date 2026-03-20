using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private GameObject enemyPrefab;

    [Header("Positions")]
    [SerializeField] private Transform[] startPositions;
    [SerializeField] private Transform[] finishPositions;

    void Start()
    {
        if (startPositions.Length == 0 || finishPositions.Length == 0)
        {
            Debug.LogWarning("Нет позиций для спавна!");
            return;
        }

        Transform start = startPositions[0];
        Transform finish = finishPositions[0];

        GameObject enemyObj = WorldGenerator.Instance.GenerateEnemyWithWord(
            enemyPrefab,
            start.position,
            "IMTREX"
        );

        Enemy enemy = enemyObj.GetComponent<Enemy>();
        enemy.MoveTo(finish.position);
    }
}