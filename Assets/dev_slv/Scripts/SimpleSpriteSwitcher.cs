using UnityEngine;
using UnityEngine.UI;

public class EraSpriteSwitcher : MonoBehaviour
{
    public SpriteRenderer image; // или SpriteRenderer если 2D объект
    public Sprite[] sprites;

    private void OnEnable()
    {
        EraManager.OnEraChanged += SetEra;
    }

    private void OnDisable()
    {
        EraManager.OnEraChanged -= SetEra;
    }

    private void SetEra(int epoch)
    {
        if (sprites == null || sprites.Length == 0) return;

        int idx = Mathf.Clamp(epoch, 0, sprites.Length - 1);
        image.sprite = sprites[idx];
    }
}