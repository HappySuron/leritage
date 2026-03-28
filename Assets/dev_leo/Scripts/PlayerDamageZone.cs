using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerDamageZone : MonoBehaviour
{
    private enum TargetMode
    {
        Any = 0,
        Tag = 1,
        LayerMask = 2
    }

    [Header("Damage")]
    [SerializeField] private float damageAmount = 10f;
    [SerializeField] private float hitCooldown = 0.5f;

    [Header("Target Filter")]
    [SerializeField] private TargetMode targetMode = TargetMode.Any;
    [SerializeField] private string targetTag = "Enemy";
    [SerializeField] private LayerMask targetLayers = ~0;

    private readonly Dictionary<int, float> nextAllowedHitTimeByObject = new Dictionary<int, float>();

    private void OnTriggerEnter(Collider other)
    {
        if (other == null || damageAmount <= 0f)
        {
            return;
        }

        UserHealth userHealth = UserHealth.Instance;
        if (userHealth == null || userHealth.IsDead())
        {
            return;
        }

        if (!PassesFilter(other))
        {
            return;
        }

        int objectId = other.GetInstanceID();
        float currentTime = Time.time;
        if (nextAllowedHitTimeByObject.TryGetValue(objectId, out float nextAllowedTime) && currentTime < nextAllowedTime)
        {
            return;
        }

        userHealth.TakeDamage(damageAmount);
        nextAllowedHitTimeByObject[objectId] = currentTime + Mathf.Max(0f, hitCooldown);
        other.GetComponent<Enemy>().Die(); // 👈 убиваем врага
        //Destroy(other.gameObject);
    }

    private bool PassesFilter(Collider other)
    {
        switch (targetMode)
        {
            case TargetMode.Tag:
                return !string.IsNullOrWhiteSpace(targetTag) && other.CompareTag(targetTag);
            case TargetMode.LayerMask:
                return (targetLayers.value & (1 << other.gameObject.layer)) != 0;
            case TargetMode.Any:
            default:
                return true;
        }
    }

    private void OnDisable()
    {
        nextAllowedHitTimeByObject.Clear();
    }
}
