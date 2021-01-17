using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene(1); // 1 is the index of the game scene.
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0); // 1 is the index of the game scene.
    }

    public void Quit()
    {
        Application.Quit();
    }

    private void Update()
    {
        //if (Input.GetKey(KeyCode.Escape)){
        //    Application.Quit();
        //}
    }
}
