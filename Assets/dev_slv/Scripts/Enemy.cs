using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform wordSpawnPoint;
    private GameObject wordObject;

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