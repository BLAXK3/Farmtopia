using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindWithTag("Audio").GetComponent<AudioManager>();
    }

    public void Play()
    {
        SceneManager.LoadSceneAsync("InGame");
    }

    public void Home()
    {
        SceneManager.LoadSceneAsync("Main Menu");
        Time.timeScale = 1f;
    }

    public void Credits()
    {
        SceneManager.LoadSceneAsync("Credits");
    }


    public void Click(int val)
    {
        if (val == 0)
        {
            audioManager.PlaySFX(audioManager.Click);
        }
        else
        {
            audioManager.PlaySFX(audioManager.BackClick);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
