using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        this.TryToUseEraManager();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    
    public void QuitGame()
    {
        Application.Quit();    
    }


    private void TryToUseEraManager()
    {
        if (EraManager.Instance != null)
        {
            EraManager.Instance.PlayEraMusic();
        }
        else
        {
            Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
        }
    }
}
