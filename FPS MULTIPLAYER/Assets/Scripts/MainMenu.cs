using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    private void Start()
    {
        Pause.paused = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public Launcher launcher;

   public void JoinMatch()
   {
        launcher.Join();
   }
    
    public void CreateMatch()
    {
        launcher.Create();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
