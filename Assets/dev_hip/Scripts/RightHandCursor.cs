using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RightHandCursor : MonoBehaviour
{
    public RectTransform handImage;
    public Image handRenderer;
    public Sprite[] open;  // по эпохам
    public Sprite[] grab;  // по эпохам

    public float shiftXPercent;
    public float shiftYPercent;

    private int _epoch = 0;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
    }

    void Update()
    {
        // Vector2 pivotOld = new Vector2(0.5f, 0.5f); // старый пивот
        //Vector2 pivotNew = new Vector2(0.5f, 0.7f); // новый пивот

        // float width = handImage.rect.width;
        // float height = handImage.rect.height;

        // float shiftX = shiftXPercent * Screen.width;
        // float shiftY = shiftYPercent * Screen.height;

        // float X1 = Input.mousePosition.x + (pivotOld.x - handImage.GetComponent<Image>().sprite.pivot.x/1000) * width + shiftX;
        // float Y1 = Input.mousePosition.y + (pivotOld.y - handImage.GetComponent<Image>().sprite.pivot.y/1000) * height + shiftY;



        //Vector2 newPosition = new Vector2(X1, Y1);
        //handImage.position = newPosition;



        handImage.position = Input.mousePosition;
        //Debug.Log("handPosition: " + handImage.position + "Pivot: " + handImage.GetComponent<Image>().sprite.pivot);
        //handImage.anchoredPosition = Input.mousePosition / handImage.canvas.scaleFactor;


        if (EraManager.Instance != null)
            _epoch = (int)EraManager.Instance.currentEra;

        _epoch = ClampEpochIndex(_epoch);

        if (handRenderer != null && open != null && grab != null &&
            _epoch < open.Length && _epoch < grab.Length)
        {
            handRenderer.sprite = Input.GetMouseButton(0) ? grab[_epoch] : open[_epoch];
            handImage.GetComponent<RectTransform>().pivot = handImage.GetComponent<Image>().sprite.pivot/1000;
        }
    }



    // void Update()
    // {
    //     // Получаем родительский RectTransform (Canvas)
    //     RectTransform canvasRect = handImage.parent as RectTransform;

    //     Vector2 localPoint;
    //     // Преобразуем экранную позицию мыши в локальные координаты канваса
    //     RectTransformUtility.ScreenPointToLocalPointInRectangle(
    //         canvasRect,
    //         Input.mousePosition,
    //         null, // Камера не нужна, если Canvas в Screen Space Overlay
    //         out localPoint
    //     );

    //     // Устанавливаем локальную позицию руки (pivot учитывается автоматически)
    //     handImage.localPosition = localPoint;

    //     // Остальной код для смены спрайтов
    //     if (EraManager.Instance != null)
    //         _epoch = (int)EraManager.Instance.currentEra;

    //     _epoch = ClampEpochIndex(_epoch);

    //     if (handRenderer != null && open != null && grab != null &&
    //         _epoch < open.Length && _epoch < grab.Length)
    //     {
    //         handRenderer.sprite = Input.GetMouseButton(0) ? grab[_epoch] : open[_epoch];
    //     }
    // }



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
