using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
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
