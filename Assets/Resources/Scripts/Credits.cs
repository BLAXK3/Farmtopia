using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour
{
    private void Start()
    {
        Invoke("WaitToEnd", 60);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            SceneManager.LoadSceneAsync("Main Menu");
            Time.timeScale = 1;
        }
    }
    
    public void MainMenu()
    {
        SceneManager.LoadSceneAsync("Main Menu");
    }
    
    public void WaitToEnd()
    {
        SceneManager.LoadSceneAsync("Main Menu");
        Time.timeScale = 1;
    }
}
