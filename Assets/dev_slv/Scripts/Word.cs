using UnityEngine;

public class Word : MonoBehaviour
{
    public Transform lettersContainer;

    private void Awake()
    {
        if (lettersContainer == null)
            lettersContainer = transform.Find("LettersContainer");
    }

    /// <summary>
    /// Проверяет текущую букву. Если совпадает - удаляет, если нет - подсвечивает красным
    /// </summary>
    /// <param name="letterToCheck">Буква для проверки</param>
    public void CheckLetter(char letterToCheck)
    {
        if (lettersContainer == null || lettersContainer.childCount == 0)
            return;

        // Берем первую букву
        Letter firstLetter = lettersContainer.GetChild(0).GetComponent<Letter>();
        if (firstLetter == null) return;

        if (firstLetter.GetChar() == letterToCheck)
        {
            firstLetter.RemoveLetter(); // совпало, удаляем

            // Проверяем через Invoke, чтобы Unity успела удалить объект
            Invoke(nameof(CheckEmptyWord), 0.01f);
        }
        else
        {
            firstLetter.HighlightWrong(); // не совпало, красим
        }
    }

    private void CheckEmptyWord()
    {
        if (lettersContainer.childCount == 0)
            Destroy(gameObject); // удаляем само слово
    }
}