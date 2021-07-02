using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtonHandler : MonoBehaviour
{
   
    public void StartGame()
    {
        SceneManager.LoadScene("Space");
    }
    public void HowTo()
    {
        SceneManager.LoadScene("HowToScene");
    }
    public void QuitGame()
    {

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
                     Application.Quit();
#endif
    }
}
