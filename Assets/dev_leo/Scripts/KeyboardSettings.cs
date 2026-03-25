using System.Collections.Generic;
using UnityEngine;
using TMPro;

[ExecuteInEditMode]
public class KeyboardSettings : MonoBehaviour
{
    [Header("Key Prefab")]
    [Tooltip("Prefab для одной клавиши. В нем может быть UI Button или 3D object.")]
    public GameObject keyPrefab;

    [Header("Keyboard layout")]
    [Tooltip("Строки определяют последовательность клавиш в каждом ряду.")]
    public string[] rows = new string[]
    {
        "`1234567890-=",
        "qwertyuiop[]\\",
        "asdfghjkl;'",
        "zxcvbnm,./"
    };

    [Header("Spacing")]
    public float keyWidth = 1f;
    public float keyHeight = 1f;
    public float spacingX = 0.2f;
    public float spacingY = 0.25f;

    [Header("Row offsets")]
    [Tooltip("Смещение по X для каждого ряда, чтобы получить традиционное расположение.")]
    public float row1OffsetX = 0f;
    public float row2OffsetX = 0.3f;
    public float row3OffsetX = 0.55f;
    public float row4OffsetX = 0.9f;

    [Header("Advanced")]
    [Tooltip("Если включено, перестроить клавиатуру при каждом изменении в инспекторе.")]
    public bool rebuildOnValidate = true;

    [ContextMenu("Build Keyboard Layout")]
    public void BuildKeyboardLayout()
    {
        if (keyPrefab == null)
        {
            Debug.LogWarning("KeyboardSettings: keyPrefab не задан.");
            return;
        }

        ClearKeyboard();

        Vector2[] rowOffsets = new Vector2[]
        {
            new Vector2(row1OffsetX, 0),
            new Vector2(row2OffsetX, 0),
            new Vector2(row3OffsetX, 0),
            new Vector2(row4OffsetX, 0)
        };

        for (int row = 0; row < rows.Length; row++)
        {
            string rowText = rows[row];
            float y = -row * (keyHeight + spacingY);
            float offsetX = row < rowOffsets.Length ? rowOffsets[row].x : 0f;

            for (int col = 0; col < rowText.Length; col++)
            {
                char keyChar = rowText[col];
                Vector3 localPos = new Vector3((col * (keyWidth + spacingX)) + offsetX, y, 0);

                GameObject key = Instantiate(keyPrefab, transform);
                if (key == null)
                {
                    Debug.LogError("KeyboardSettings: Ошибка инстанциирования префаба.");
                    continue;
                }

                // 🔥 НОВОЕ — записываем букву в скрипт
                var keyVisual = key.GetComponent<LetterKeyVisual>();
                if (keyVisual != null)
                {
                    keyVisual.SetLetter(keyChar);
                }

                key.name = GetKeyName(keyChar, row, col);
                key.transform.localPosition = localPos;
                key.transform.localRotation = Quaternion.Euler(0f, -180f, 0f);
                //key.transform.localScale = Vector3.one;
                key.transform.localScale = new Vector3(keyWidth, keyHeight, 1f);
                

                // Если есть компонент TextMeshPro или Text, можно проставить символ.

                // commented by slv --
                // var text = key.GetComponentInChildren<UnityEngine.UI.Text>();
                // if (text != null)
                // {
                //     text.text = keyChar.ToString();
                // }
                // -- slv
                var text = key.GetComponentInChildren<TextMeshProUGUI>();
                if (text != null)
                {
                    text.text = keyChar.ToString().ToUpper();
                }

            }
        }

        Debug.Log("KeyboardSettings: Клавиатура построена.");
    }

    private string GetKeyName(char keyChar, int row, int col)
    {
        if (keyChar == ' ') return $"Key_Row{row}_Col{col}";
        return $"Key_{keyChar}_R{row}_C{col}";
    }

    [ContextMenu("Clear Keyboard")] 
    public void ClearKeyboard()
    {
        List<GameObject> children = new List<GameObject>();
        foreach (Transform child in transform)
        {
            children.Add(child.gameObject);
        }

        foreach (var child in children)
        {
            if (Application.isEditor)
            {
                DestroyImmediate(child);
            }
            else
            {
                Destroy(child);
            }
        }
    }

    private void OnValidate()
    {
        if (!Application.isPlaying && rebuildOnValidate)
        {
            // В Unity 2020+ this can call from inspector changes
            BuildKeyboardLayout();
        }
    }
}
