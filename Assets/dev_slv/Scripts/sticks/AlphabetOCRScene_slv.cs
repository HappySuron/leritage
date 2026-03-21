using UnityEngine;
using OpenCvSharp.Demo;

public class LetterRecognizer : MonoBehaviour
{
    public Camera stickCamera;        // твоя камера над областью сборки
    public TextAsset knnModel;        // OCRHMM_knn_model_data.xml.gz

    private AlphabetOCR ocr;

    void Start()
    {
        ocr = new AlphabetOCR(knnModel.bytes);
        stickCamera.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            char? result = Recognize();
            if (result.HasValue)
                Debug.Log("Распознана буква: " + result.Value);
        }
    }

    char? Recognize()
    {
        // 1. Делаем скрин с камеры в RenderTexture
        RenderTexture rt = new RenderTexture(800, 512, 24);
        stickCamera.targetTexture = rt;
        stickCamera.Render();

        // 2. Читаем пиксели в Texture2D
        RenderTexture.active = rt;
        Texture2D snapshot = new Texture2D(800, 512, TextureFormat.RGB24, false);
        snapshot.ReadPixels(new Rect(0, 0, 800, 512), 0, 0);
        snapshot.Apply();

        // 3. Сохраняем скриншот
        string folder = Application.dataPath + "/Screenshots";
        if (!System.IO.Directory.Exists(folder))
            System.IO.Directory.CreateDirectory(folder);

        string filename = folder + "/screenshot_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmssfff") + ".png";
        byte[] bytes = snapshot.EncodeToPNG();
        System.IO.File.WriteAllBytes(filename, bytes);
        Debug.Log("Скрин сохранён: " + filename);

        // 4. Чистим
        stickCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        // 5. Отправляем в AlphabetOCR
        var mat = OpenCvSharp.Unity.TextureToMat(snapshot);
        var letters = ocr.ProcessImage(mat);
        Destroy(snapshot);

        // 6. Возвращаем лучший результат
        if (letters.Count == 0) return null;

        var best = letters[0];
        foreach (var l in letters)
            if (l.Confidence > best.Confidence)
                best = l;

        return best.Data[0];
    }
}
