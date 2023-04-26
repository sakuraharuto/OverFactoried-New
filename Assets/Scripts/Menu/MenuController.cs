using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Scenes/SampleScene");
    }

    public void ViewHelp()
    {
        SceneManager.LoadScene("Scenes/Help");
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("Scenes/MainMenu");
    }

    public void ExitGame()
    {
        Debug.Log("You quit the game");
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }
}
