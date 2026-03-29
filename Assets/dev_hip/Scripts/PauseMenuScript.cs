using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuScript : MonoBehaviour
{
    [Header("Main Panel")]
    public GameObject pausePanel;

    [Header("Menus")]
    public GameObject pauseMenu;
    public GameObject winMenu;
    public GameObject failMenu;
    public GameObject optionMenu;
    public GameObject controlsMenu;
    public GameObject aboutMenu;


    private MenuState _previousState = MenuState.None;

    private MenuState _currentState = MenuState.None;

    private enum MenuState
    {
        None,
        Pause,
        Options,
        Controls,
        About,
        Win,
        Fail
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            HandleEscape();

        CheckGameState();
    }

    void HandleEscape()
    {
        // нельзя выйти с экранов конца игры
        if (_currentState == MenuState.Win || _currentState == MenuState.Fail)
            return;

        // если играем — открываем паузу
        if (_currentState == MenuState.None)
        {
            SetState(MenuState.Pause);
            return;
        }

        // если мы НЕ в главном меню паузы — возвращаемся в него
        if (_currentState != MenuState.Pause)
        {
            SetState(MenuState.Pause);
            return;
        }

        // если уже в паузе — закрываем её
        SetState(MenuState.None);
    }

    void CheckGameState()
    {
        if (_currentState == MenuState.Win || _currentState == MenuState.Fail)
            return;

        if (CheckLetterKeyboard.Instance.learendLettersCount >= CheckLetterKeyboard.Instance.letterToWin)
        {
            SetState(MenuState.Win);
        }

        if (UserHealth.Instance.CurrentHealth <= 0)
        {
            SetState(MenuState.Fail);
        }
    }

void SetState(MenuState newState)
{
    // сохраняем прошлое состояние
    if (newState != MenuState.None)
        _previousState = _currentState;

    _currentState = newState;

    // выключаем ВСЕ меню
    pauseMenu.SetActive(false);
    winMenu.SetActive(false);
    failMenu.SetActive(false);
    optionMenu.SetActive(false);
    controlsMenu.SetActive(false);
    aboutMenu.SetActive(false);

    pausePanel.SetActive(newState != MenuState.None);

    switch (newState)
    {
        case MenuState.Pause:
            pauseMenu.SetActive(true);
            break;

        case MenuState.Options:
            optionMenu.SetActive(true);
            break;

        case MenuState.Controls:
            controlsMenu.SetActive(true);
            break;

        case MenuState.About:
            aboutMenu.SetActive(true);
            break;

        case MenuState.Win:
            winMenu.SetActive(true);
            break;

        case MenuState.Fail:
            failMenu.SetActive(true);
            break;
    }

    Time.timeScale = (newState == MenuState.None) ? 1f : 0f;
}

    // --- КНОПКИ ---

    public void Resume()
    {
        SetState(MenuState.None);
    }

    public void OpenOptions()
    {
        SetState(MenuState.Options);
    }

    public void OpenControls()
    {
        SetState(MenuState.Controls);
    }

    public void OpenAbout()
    {
        SetState(MenuState.About);
    }

    public void BackToPause()
    {
        SetState(MenuState.Pause);
    }

    public void Back()
    {
        SetState(_previousState);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        Cleanup();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ToMainMenu()
    {
        Time.timeScale = 1f;
        Cleanup();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    void Cleanup()
    {
        if (EraManager.Instance != null)
        {
            EraManager.Instance.StopEraMusic();
            Destroy(EraManager.Instance.gameObject);
        }

        if (CheckLetterKeyboard.Instance != null)
        {
            Destroy(CheckLetterKeyboard.Instance.gameObject);
        }
    }
}