using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public float hoverScale = 1.1f;
    public float speed = 10f;

    private Vector3 _targetScale;

    void Start() => _targetScale = Vector3.one;

    void Update()
    {
        transform.localScale = Vector3.Lerp(
            transform.localScale, _targetScale, Time.unscaledDeltaTime * speed);
    }

    public void OnPointerEnter(PointerEventData e) =>
        _targetScale = Vector3.one * hoverScale;

    public void OnPointerExit(PointerEventData e) =>
        _targetScale = Vector3.one;

    public void OnPointerClick(PointerEventData e) =>
        _targetScale = Vector3.one;

    void OnDisable()
    {
        _targetScale = Vector3.one;
        transform.localScale = Vector3.one;
    }
}