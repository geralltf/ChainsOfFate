using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
  public void PlayGame()
    {
        SceneManager.LoadScene("01_OldMan_Inteior");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("The game has quit");
    }
}
