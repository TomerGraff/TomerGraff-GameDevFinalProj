using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menu : MonoBehaviour
{

    private void Start()
    {
        Cursor.visible = true;
        Screen.lockCursor = false;
    }

    public void playGame()
    {
        SceneManager.LoadScene("game");
    }
    public void quitGame()
    {
        Application.Quit();
    }


}
