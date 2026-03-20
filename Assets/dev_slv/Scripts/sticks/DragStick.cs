using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DragStick : MonoBehaviour
{
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("StickZone") && DragController.Instance != null)
        {
            DragController.Instance.ResetStick(rb);
        }
    }
}