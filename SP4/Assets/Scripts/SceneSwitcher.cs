using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public void SceneSwitch(int scene)
    {
        switch(scene)
        {
            case 0:
                GameManager.instance.NewGame();
                break;
            case 1:
                GameManager.instance.Load();
                break;
        }
        SceneManager.LoadScene("DemoScene");
    }
}
