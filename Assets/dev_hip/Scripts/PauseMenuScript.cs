using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class PauseMenuScript : MonoBehaviour
{
    public GameObject pausePanel;
    public GameObject pauseMenu;
    public GameObject winMenu;
    public GameObject failMenu;
    private bool _isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            SetPause(!_isPaused);

        if (CheckLetterKeyboard.Instance.learendLettersCount >= 27)
            SetWinScreen(!_isPaused);
        
        if (UserHealth.Instance.CurrentHealth <= 0)
            SetFailScreen(!_isPaused);
    }

    public void SetPause(bool pause)
    {
        _isPaused = pause;
        pausePanel.SetActive(_isPaused);
        pauseMenu.SetActive(_isPaused);
        Time.timeScale = _isPaused ? 0f : 1f;
    }

    public void SetWinScreen(bool pause)
    {
        _isPaused = pause;
        pausePanel.SetActive(_isPaused);
        winMenu.SetActive(_isPaused);
        Time.timeScale = _isPaused ? 0f : 1f;
    }

    public void SetFailScreen(bool pause)
    {
        _isPaused = pause;
        pausePanel.SetActive(_isPaused);
        failMenu.SetActive(_isPaused);
        Time.timeScale = _isPaused ? 0f : 1f;
    }

    public void Resume() => SetPause(false);

    public void ToMainMenu()
    {
        
        Time.timeScale = 1f;
        EraManager.Instance.StopEraMusic();
        Destroy(EraManager.Instance);
        Destroy(CheckLetterKeyboard.Instance);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1); // индекс главного меню

    }

    public void Restart()
    {
        Time.timeScale = 1f;
        EraManager.Instance.StopEraMusic();
        Destroy(EraManager.Instance);
        Destroy(CheckLetterKeyboard.Instance);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
