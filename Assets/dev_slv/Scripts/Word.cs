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
        CheckLetter(letterToCheck, false);
    }

    public void CheckLetter(char letterToCheck, bool checkOnlyFirst)
    {
        if (lettersContainer == null || lettersContainer.childCount == 0)
            return;

        if (checkOnlyFirst)
        {
            Letter firstLetter = lettersContainer.GetChild(0).GetComponent<Letter>();
            if (firstLetter == null)
                return;

            if (firstLetter.GetChar() == letterToCheck)
            {
                firstLetter.RemoveLetter();
                Invoke(nameof(CheckEmptyWord), 0.01f);
            }
            else
            {
                firstLetter.HighlightWrong();
            }

            return;
        }

        bool foundMatch = false;

        for (int i = 0; i < lettersContainer.childCount; i++)
        {
            Letter letter = lettersContainer.GetChild(i).GetComponent<Letter>();
            if (letter == null) continue;

            if (letter.GetChar() == letterToCheck)
            {
                letter.RemoveLetter();
                foundMatch = true;
            }
        }

        if (!foundMatch)
        {
            for (int i = 0; i < lettersContainer.childCount; i++)
            {
                Letter letter = lettersContainer.GetChild(i).GetComponent<Letter>();
                if (letter != null)
                    letter.HighlightWrong();
            }
        }

        Invoke(nameof(CheckEmptyWord), 0.01f);
    }


    

    private void CheckEmptyWord()
    {
        if (lettersContainer.childCount == 0)
            Destroy(gameObject); // удаляем само слово
    }
}