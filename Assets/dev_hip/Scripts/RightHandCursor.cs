using UnityEngine;
using UnityEngine.UI;

public class RightHandCursor : MonoBehaviour
{
    public RectTransform handImage;
    public Image handRenderer;
    public Sprite[] open;  // по эпохам
    public Sprite[] grab;  // по эпохам

    private int _epoch = 0;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    void Update()
    {
        handImage.position = Input.mousePosition;

        if (EraManager.Instance != null)
            _epoch = (int)EraManager.Instance.currentEra;

        _epoch = ClampEpochIndex(_epoch);

        if (handRenderer != null && open != null && grab != null &&
            _epoch < open.Length && _epoch < grab.Length)
        {
            handRenderer.sprite = Input.GetMouseButton(0) ? grab[_epoch] : open[_epoch];
        }
    }

    public void OnGrabStart()
    {
        _epoch = ClampEpochIndex(_epoch);
        if (handRenderer != null && grab != null && _epoch < grab.Length)
            handRenderer.sprite = grab[_epoch];
    }

    public void OnGrabEnd()
    {
        _epoch = ClampEpochIndex(_epoch);
        if (handRenderer != null && open != null && _epoch < open.Length)
            handRenderer.sprite = open[_epoch];
    }

    public void SetEpoch(int index)
    {
        _epoch = ClampEpochIndex(index);
        if (handRenderer != null && open != null && _epoch < open.Length)
            handRenderer.sprite = open[_epoch];
    }

    private int ClampEpochIndex(int index)
    {
        if (open == null || grab == null)
            return 0;
        int n = Mathf.Min(open.Length, grab.Length);
        if (n <= 0)
            return 0;
        return Mathf.Clamp(index, 0, n - 1);
    }

    void OnApplicationFocus(bool focus) => Cursor.visible = !focus;
}
