using UnityEngine;
using UnityEngine.UI;

public class LeftHandMovement : MonoBehaviour
{
    public static LeftHandMovement Instance { get; private set; }

    public Image handRenderer;
    public Sprite[] idle;
    public Sprite[] press;

    public float moveSpeed = 500f;
    public float pressDelay = 0.2f;

    private Vector3 _homePosition;
    private Vector3 _targetPosition;
    private int _epoch = 0;
    private float _pressTimer = 0f;

    private enum State { Idle, MovingToTarget, Pressing, ReturningHome }
    private State _state = State.Idle;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        _homePosition = transform.position;
        UpdateSprite();
    }

    void Update()
    {
        if (EraManager.Instance != null)
            _epoch = (int)EraManager.Instance.currentEra;
        _epoch = ClampEpochIndex(_epoch);

        switch (_state)
        {
            case State.MovingToTarget:
                transform.position = Vector3.MoveTowards(transform.position, _targetPosition, moveSpeed * Time.deltaTime);
                if (Vector3.Distance(transform.position, _targetPosition) < 0.01f)
                {
                    transform.position = _targetPosition;
                    _pressTimer = pressDelay;
                    _state = State.Pressing;
                    UpdateSprite();
                }
                break;

            case State.Pressing:
                _pressTimer -= Time.deltaTime;
                if (_pressTimer <= 0f)
                {
                    _state = State.ReturningHome;
                    UpdateSprite();
                }
                break;

            case State.ReturningHome:
                transform.position = Vector3.MoveTowards(transform.position, _homePosition, moveSpeed * Time.deltaTime);
                if (Vector3.Distance(transform.position, _homePosition) < 0.01f)
                {
                    transform.position = _homePosition;
                    _state = State.Idle;
                    UpdateSprite();
                }
                break;
        }
    }

    public void MoveToPoint(Vector3 worldPoint)
    {
        // if (_state != State.Idle) return;
        _targetPosition = new Vector3(worldPoint.x, _homePosition.y,worldPoint.z);
        handRenderer.GetComponent<RectTransform>().pivot = handRenderer.GetComponent<Image>().sprite.pivot/1000;
        _state = State.MovingToTarget;
        UpdateSprite();
    }

    public void SetEpoch(int index)
    {
        _epoch = ClampEpochIndex(index);
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        if (handRenderer == null) return;

        bool isPressing = _state == State.Pressing;
        Sprite[] sprites = isPressing ? press : idle;

        if (sprites == null || sprites.Length == 0) return;
        int idx = ClampEpochIndex(_epoch);
        if (idx < sprites.Length)
            handRenderer.sprite = sprites[idx];
    }

    private int ClampEpochIndex(int index)
    {
        if (idle == null || press == null) return 0;
        int n = Mathf.Min(idle.Length, press.Length);
        if (n <= 0) return 0;
        return Mathf.Clamp(index, 0, n - 1);
    }
}
