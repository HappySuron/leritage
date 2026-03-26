using UnityEngine;
using UnityEngine.UI;

public class TwoHandCursor : MonoBehaviour
{
    [System.Serializable]
    public class Hand
    {
        public RectTransform handImage;
        public Image handRenderer;
        public Sprite[] open;
        public Sprite[] grab;

        [HideInInspector] public int epoch;
    }

    public Hand rightHand;
    public Hand leftHand;

    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

        SetEpoch(0);
    }

    void Update()
    {
        // Правая рука — всегда за мышью
        rightHand.handImage.position = Input.mousePosition;

        // Левая — например по кнопке Q
        if (Input.GetKey(KeyCode.Q))
        {
            leftHand.handImage.position = Input.mousePosition;
        }
    }

    // --- Правая ---
    public void RightGrabStart() => rightHand.handRenderer.sprite = rightHand.grab[rightHand.epoch];
    public void RightGrabEnd()   => rightHand.handRenderer.sprite = rightHand.open[rightHand.epoch];

    // --- Левая ---
    public void LeftGrabStart() => leftHand.handRenderer.sprite = leftHand.grab[leftHand.epoch];
    public void LeftGrabEnd()   => leftHand.handRenderer.sprite = leftHand.open[leftHand.epoch];

    public void SetEpoch(int index)
    {
        rightHand.epoch = index;
        leftHand.epoch = index;

        rightHand.handRenderer.sprite = rightHand.open[index];
        leftHand.handRenderer.sprite = leftHand.open[index];
    }

    void OnApplicationFocus(bool focus) => Cursor.visible = !focus;
}