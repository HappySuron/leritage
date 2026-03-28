using TMPro;
using UnityEngine;

public class LetterKeyVisual : MonoBehaviour
{
    [SerializeField] private char letter;
    [SerializeField] private TextMeshProUGUI text;

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color learnedColor = Color.green;

    public char Letter => letter;

    private void Awake()
    {
        if (text == null)
            text = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start()
    {
        CheckLetterKeyboard.Instance?.RegisterKey(this);
    }

    public void UpdateVisual(int currentLevel, int maxLevel)
    {
        float alphaFloat = (float)currentLevel / maxLevel;
        alphaFloat = Mathf.Clamp01(alphaFloat);
        Debug.Log("SLV "+ alphaFloat);
        // 🔥 выбираем цвет
        Color baseColor = currentLevel >= maxLevel ? learnedColor : normalColor;

        baseColor.a = alphaFloat;
        text.color = baseColor;
    }

    public void SetLetter(char newLetter)
    {
        letter = char.ToUpper(newLetter);
    }
}