using UnityEngine;

public class UserHealth : MonoBehaviour
{
    public static UserHealth Instance { get; private set; }

    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth = 100f;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("UserHealth singleton already exists. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (maxHealth < 1f)
        {
            maxHealth = 1f;
        }

        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        if (currentHealth <= 0f)
        {
            currentHealth = maxHealth;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void TakeDamage(float damage)
    {
        if (damage <= 0f || IsDead())
        {
            return;
        }

        currentHealth = Mathf.Max(0f, currentHealth - damage);
    }

    public bool IsDead()
    {
        return currentHealth <= 0f;
    }
}
