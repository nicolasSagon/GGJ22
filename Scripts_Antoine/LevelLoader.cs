using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    public Button bouttonMenu;
    public Animator transition;
    public float transitionTime = 1f;

    public void LoadSceneMenuFonction()
    {
        StartCoroutine(LoadSceneMenu());
    }

    IEnumerator LoadSceneMenu()
    {
        //if (LevelIndex >= 2) yield break;
        //play animation
        transition.SetTrigger("Start");

        //wait
        yield return new WaitForSeconds(transitionTime);

        //load scene
        SceneManager.LoadScene("SceneMenu");
    }

    public void LoadScenePrincipaleFonction()
        {
           StartCoroutine(LoadScenePrincipale());
        }

    IEnumerator LoadScenePrincipale()
    {
        //if (LevelIndex >= 2) yield break;
        //play animation
        transition.SetTrigger("Start");

        //wait
        yield return new WaitForSeconds(transitionTime);

        //load scene
        SceneManager.LoadScene("ScenePrincipale");
    }

    public void LoadSceneCreditsFonction()
    {
        StartCoroutine(LoadSceneCredits());
    }

    IEnumerator LoadSceneCredits()
    {
        //if (LevelIndex >= 2) yield break;
        //play animation
        transition.SetTrigger("Start");

        //wait
        yield return new WaitForSeconds(transitionTime);

        //load scene
        SceneManager.LoadScene("SceneCredits");
    }

    public void QuitGame()
    {
        print("quit game!");
        Application.Quit();
    }
}
