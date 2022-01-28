using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPause = false;
    //public Button Button;

    void Update()
    {
        //Button.onClick.AddListener(ButtonClick);
    }

    //void ButtonClick()
    //{
    //   if (GameIsPause)
    //   {
    //        Resume();
    //   }
    //   else
    //   {
    //       Pause();
    //   }
    //}

    public void Resume()
    {
        Time.timeScale = 1f;
        GameIsPause = false;
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        GameIsPause = true;
    }

    public void LoadMenu ()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("SceneMenu");
    }

    public void Quit()
    {
        print("quit");
        Application.Quit();
    }
}
