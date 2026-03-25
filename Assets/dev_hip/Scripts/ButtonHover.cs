using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float hoverScale = 1.1f;
    public float speed = 10f;

    private Vector3 _targetScale;

    void Start() => _targetScale = Vector3.one;

    void Update()
    {
        transform.localScale = Vector3.Lerp(
            transform.localScale, _targetScale, Time.deltaTime * speed);
    }

    public void OnPointerEnter(PointerEventData e) => 
        _targetScale = Vector3.one * hoverScale;

    public void OnPointerExit(PointerEventData e) => 
        _targetScale = Vector3.one;
}