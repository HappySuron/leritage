using UnityEngine;

public class Word : MonoBehaviour
{
    public AudioClip[] dinosaurDeathClips; // звуки смерти динозавра

    public Transform lettersContainer;

    public Enemy ownerEnemy; // 👈 ссылка на врага

    private void Awake()
    {
        if (lettersContainer == null)
            lettersContainer = transform.Find("LettersContainer");
    }


    public void SetOwner(Enemy enemy)
    {
        ownerEnemy = enemy;
        Debug.Log(ownerEnemy);
    }

    /// <summary>
    /// Проверяет текущую букву. Если совпадает - удаляет, если нет - подсвечивает красным
    /// </summary>
    /// <param name="letterToCheck">Буква для проверки</param>
    public bool CheckLetter(char letterToCheck)
    {
        bool found = CheckLetter(letterToCheck, false);
        return found;
    }

    public bool CheckLetter(char letterToCheck, bool checkOnlyFirst = false)
    {
        if (lettersContainer == null || lettersContainer.childCount == 0)
            return false;

        bool foundMatch = false;

        if (checkOnlyFirst)
        {
            Letter firstLetter = lettersContainer.GetChild(0).GetComponent<Letter>();
            if (firstLetter == null)
                return false;

            if (firstLetter.GetChar() == letterToCheck)
            {
                firstLetter.RemoveLetter();
                foundMatch = true;
                Invoke(nameof(CheckEmptyWord), 0.01f);
            }
            else
            {
                firstLetter.HighlightWrong();
            }

            return foundMatch;
        }

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
        return foundMatch;
    }


    

    private void CheckEmptyWord()
    {
        if (lettersContainer.childCount == 0)
        {
            if (ownerEnemy != null)

                
                ownerEnemy.Die(); // 👈 убиваем врага
            Destroy(gameObject); // удаляем само слово
            AudioManager.instance.PlaySoundFXClip(dinosaurDeathClips, transform, 1f); // звук смерти
        }
            
    }
}