using UnityEngine;

public class DragController : MonoBehaviour
{
    public static DragController Instance; // Singleton

    [Header("Camera")]
    public Camera cam;

    [Header("Settings")]
    public float rotateSpeed = 200f;
    public float liftHeight = 0.2f;
    public LayerMask draggableLayer;

    [Header("Sticks")]
    public Rigidbody[] sticks; // массив палочек

    // стартовые позиции и вращения каждой палочки
    private Vector3[] startPositions;
    private Quaternion[] startRotations;

    private Rigidbody grabbed;
    private int grabbedIndex = -1;
    private bool isDragging = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Запоминаем стартовые позиции и вращения всех палочек
        startPositions = new Vector3[sticks.Length];
        startRotations = new Quaternion[sticks.Length];

        for (int i = 0; i < sticks.Length; i++)
        {
            startPositions[i] = sticks[i].position;
            startRotations[i] = sticks[i].rotation;
        }
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0)) TryGrab();
        if (Input.GetMouseButton(0) && grabbed != null) { Move(); Rotate(); }
        if (Input.GetMouseButtonUp(0) && grabbed != null) Release();
    }

    void TryGrab()
    {
        if (cam == null) return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, draggableLayer))
        {
            Rigidbody rb = hit.rigidbody;
            if (rb != null)
            {
                for (int i = 0; i < sticks.Length; i++)
                {
                    if (sticks[i] == rb)
                    {
                        grabbed = rb;
                        grabbedIndex = i;

                        grabbed.useGravity = false;
                        grabbed.linearVelocity = Vector3.zero;
                        grabbed.angularVelocity = Vector3.zero;

                        Vector3 lifted = grabbed.position;
                        lifted.y += liftHeight;
                        grabbed.MovePosition(lifted);

                        isDragging = true;
                        break;
                    }
                }
            }
        }
    }

    void Move()
    {
        if (grabbed == null) return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, startPositions[grabbedIndex]);

        if (plane.Raycast(ray, out float distance))
        {
            Vector3 point = ray.GetPoint(distance);
            point.y = startPositions[grabbedIndex].y + liftHeight;
            grabbed.MovePosition(point);
        }
    }

    void Rotate()
    {
        if (grabbed == null) return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
            grabbed.MoveRotation(grabbed.rotation * Quaternion.Euler(0, scroll * rotateSpeed, 0));
    }

    void Release()
    {
        if (grabbed == null) return;

        grabbed.useGravity = true;
        grabbed = null;
        grabbedIndex = -1;
        isDragging = false;
    }

    // Сбрасывает палочку на старт (вызывается палочкой при выходе из зоны)
    public void ResetStick(Rigidbody stick)
    {
        if (stick == null) return;

        // находим индекс палочки в массиве
        int index = -1;
        for (int i = 0; i < sticks.Length; i++)
            if (sticks[i] == stick) { index = i; break; }

        if (index == -1) return;

        stick.MovePosition(startPositions[index]);
        stick.MoveRotation(startRotations[index]);
        stick.useGravity = true;

        // если палочка была захвачена, сброс drag
        if (grabbed == stick)
        {
            grabbed = null;
            grabbedIndex = -1;
            isDragging = false;
        }
    }

    public void ResetAllSticks()
    {
        for (int i = 0; i < sticks.Length; i++)
        {
            ResetStick(sticks[i]);
        }
    }
}