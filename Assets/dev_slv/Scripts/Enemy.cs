using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform wordSpawnPoint;
    private GameObject wordObject;


    [Header("Movement")]
    public float speed = 2f;
    private Vector3 targetPosition;
    private bool hasTarget = false;



    [Header("Indexes")]
    public int lineId;
    public int indexInLine;
    
    


    void Update()
    {
        if (!hasTarget) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.deltaTime
        );

        // проверка — дошли ли
        if (Vector3.Distance(transform.position, targetPosition) < 0.05f)
        {
            hasTarget = false;
            OnReachedTarget();
        }
    }

    public void MoveTo(Vector3 target)
    {
        targetPosition = target;
        hasTarget = true;
    }

    private void OnReachedTarget()
    {
        // тут можно сделать атаку, урон базе и т.д.
        Debug.Log("Enemy reached target");
    }


    public void SetWordObject(GameObject word)
    {
        wordObject = word;
    }

    public void SetDataIndex(int line, int index)
    {
        lineId = line;
        indexInLine = index;
    }

    public void Die()
    {
        int myLine = lineId;

        Enemy[] allEnemies = Object.FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        // собираем только нужную линию
        List<Enemy> sameLine = new List<Enemy>();

        foreach (Enemy e in allEnemies)
        {
            if (e.lineId == myLine && e != this)
            {
                sameLine.Add(e);
            }
        }

        // сортируем по индексу
        sameLine.Sort((a, b) => a.indexInLine.CompareTo(b.indexInLine));

        // 🔥 пересчитываем заново
        for (int i = 0; i < sameLine.Count; i++)
        {
            sameLine[i].indexInLine = i;
        }

        Destroy(gameObject);
    }
}