using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    void Start()
    {
        videoPlayer.loopPointReached += OnVideoEnd;
        videoPlayer.Play();
    }

    void Update()
    {
        // пропуск по любой клавише
        if (Input.anyKeyDown)
            LoadMenu();
    }

    void OnVideoEnd(VideoPlayer vp) => LoadMenu();

    void LoadMenu()
    {
        videoPlayer.loopPointReached -= OnVideoEnd;
        SceneManager.LoadScene(1); // индекс главного меню
    }
}