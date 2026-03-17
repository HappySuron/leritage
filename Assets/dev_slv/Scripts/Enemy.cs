using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform wordSpawnPoint;
    private GameObject wordObject;


    [Header("Movement")]
    public float speed = 2f;
    private Vector3 targetPosition;
    private bool hasTarget = false;


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

    public void Die()
    {
        // тут можно добавить эффекты, анимацию и т.д.
        Destroy(gameObject);
    }
}