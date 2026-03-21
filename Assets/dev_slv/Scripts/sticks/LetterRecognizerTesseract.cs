using UnityEngine;

public class LetterRecognizerTesseract : MonoBehaviour
{
    public Camera stickCamera;

    private TesseractDriver _tesseractDriver;
    private bool _isReady = false;

    void Start()
    {
        _tesseractDriver = new TesseractDriver();

        // Инициализация Tesseract
        _tesseractDriver.Setup(OnSetupComplete);

        stickCamera.enabled = false;
    }

    void OnSetupComplete()
    {
        Debug.Log("Tesseract готов");
        _isReady = true;
    }

    void Update()
    {
        if (!_isReady) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            string result = Recognize();

            if (!string.IsNullOrEmpty(result))
                Debug.Log("Распознан текст: " + result);
            else
                Debug.Log("Ничего не распознано");
        }
    }

    string Recognize()
    {
        // 1. Рендер с камеры
        RenderTexture rt = new RenderTexture(800, 512, 24);
        stickCamera.targetTexture = rt;
        stickCamera.Render();

        // 2. Перенос в Texture2D
        RenderTexture.active = rt;
        Texture2D snapshot = new Texture2D(800, 512, TextureFormat.RGB24, false);
        snapshot.ReadPixels(new Rect(0, 0, 800, 512), 0, 0);
        snapshot.Apply();

        // 3. (опционально) сохранить скрин
        string folder = Application.dataPath + "/Screenshots";
        if (!System.IO.Directory.Exists(folder))
            System.IO.Directory.CreateDirectory(folder);

        string filename = folder + "/screenshot_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmssfff") + ".png";
        System.IO.File.WriteAllBytes(filename, snapshot.EncodeToPNG());
        Debug.Log("Скрин сохранён: " + filename);

        // 4. очистка
        stickCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        // 5. OCR через Tesseract
        string recognizedText = _tesseractDriver.Recognize(snapshot);

        // можно посмотреть ошибки
        string error = _tesseractDriver.GetErrorMessage();
        if (!string.IsNullOrEmpty(error))
            Debug.LogError(error);

        Destroy(snapshot);

        // 6. если нужна ОДНА буква — берём первую
        if (string.IsNullOrWhiteSpace(recognizedText))
            return null;

        recognizedText = recognizedText.Trim();

        return recognizedText.Length > 0 ? recognizedText[0].ToString() : null;
    }
}