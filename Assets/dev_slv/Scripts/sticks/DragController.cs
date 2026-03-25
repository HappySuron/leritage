using UnityEngine;

public enum DragRotationMode { ScrollWheel, Horizontal, Vertical, Tangential }

public class DragController : MonoBehaviour
{
    public static DragController Instance; // Singleton

    [Header("Camera")]
    public Camera cam;

    [Header("Settings")]
    public float rotateSpeed = 200f;
    public float liftHeight = 0.2f;
    public LayerMask draggableLayer;

    [Header("Rotation Mode")]
    public DragRotationMode rotationMode = DragRotationMode.ScrollWheel;
    public float mouseDragRotateSpeed = 0.5f;

    [Header("Sticks")]
    public Rigidbody[] sticks; // массив палочек

    // стартовые позиции и вращения каждой палочки
    private Vector3[] startPositions;
    private Quaternion[] startRotations;

    private Rigidbody grabbed;
    private int grabbedIndex = -1;
    private bool isDragging = false;
    private Vector2 lastMousePos;
    private bool isRotatingByDrag = false;

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

        if (Input.GetMouseButton(0) && grabbed != null)
        {
            bool bothButtons = rotationMode != DragRotationMode.ScrollWheel && Input.GetMouseButton(1);
            if (bothButtons)
            {
                if (!isRotatingByDrag) { isRotatingByDrag = true; lastMousePos = Input.mousePosition; }
                RotateByMouse();
            }
            else
            {
                isRotatingByDrag = false;
                Move();
                if (rotationMode == DragRotationMode.ScrollWheel) Rotate();
            }
        }

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

    void RotateByMouse()
    {
        if (grabbed == null) return;

        Vector2 currMousePos = Input.mousePosition;
        float angle = 0f;

        switch (rotationMode)
        {
            case DragRotationMode.Horizontal:
                angle = currMousePos.x - lastMousePos.x;
                grabbed.MoveRotation(grabbed.rotation * Quaternion.Euler(0, angle * mouseDragRotateSpeed, 0));
                break;

            case DragRotationMode.Vertical:
                angle = currMousePos.y - lastMousePos.y;
                grabbed.MoveRotation(grabbed.rotation * Quaternion.Euler(0, angle * mouseDragRotateSpeed, 0));
                break;

            case DragRotationMode.Tangential:
                Vector2 objOnScreen = cam.WorldToScreenPoint(grabbed.position);
                Vector2 prevDir = (lastMousePos - objOnScreen).normalized;
                Vector2 currDir = (currMousePos - objOnScreen).normalized;
                if (prevDir.magnitude > 0.01f && currDir.magnitude > 0.01f)
                {
                    angle = Vector2.SignedAngle(prevDir, currDir);
                    grabbed.MoveRotation(grabbed.rotation * Quaternion.Euler(0, -angle * mouseDragRotateSpeed, 0));
                }
                break;
        }

        lastMousePos = currMousePos;
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