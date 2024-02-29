using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestTeleporter1 : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        SceneManager.LoadScene("Main", LoadSceneMode.Additive);
        Scene test = SceneManager.GetSceneByName("Test Scene");
        SceneManager.MoveGameObjectToScene(other.gameObject,test);
    }

    private void OnTriggerExit(Collider other)
    {
        SceneManager.UnloadSceneAsync("Test Scene");
    }
}
