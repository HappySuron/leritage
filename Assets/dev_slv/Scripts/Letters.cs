using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class Letter : MonoBehaviour
{
    public TextMeshProUGUI tmpText;
    public Image background;

    private Color originalColor;

    private void Awake()
    {
        if (tmpText == null)
            tmpText = GetComponentInChildren<TextMeshProUGUI>(true);
        if (background == null)
            background = GetComponent<Image>();

        originalColor = background != null ? background.color : tmpText.color;
    }

    public void HighlightWrong(float duration = 0.5f)
    {
        StartCoroutine(DoHighlightWrong(duration));
    }

    private IEnumerator DoHighlightWrong(float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // плавно меняем цвет с красного обратно на оригинальный
            if (background != null)
                background.color = Color.Lerp(Color.red, originalColor, t);
            else if (tmpText != null)
                tmpText.color = Color.Lerp(Color.red, originalColor, t);

            yield return null;
        }

        // гарантируем возврат к оригинальному цвету
        if (background != null)
            background.color = originalColor;
        else if (tmpText != null)
            tmpText.color = originalColor;
    }

    public void RemoveLetter()
    {
        Destroy(gameObject);
    }

    public char GetChar()
    {
        return tmpText != null && !string.IsNullOrEmpty(tmpText.text) ? tmpText.text[0] : '\0';
    }
}