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
    }

    public void OnGrabStart() => handRenderer.sprite = grab[_epoch];
    public void OnGrabEnd()   => handRenderer.sprite = open[_epoch];

    public void SetEpoch(int index)
    {
        _epoch = index;
        handRenderer.sprite = open[_epoch];
    }

    void OnApplicationFocus(bool focus) => Cursor.visible = !focus;
}
