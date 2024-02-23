using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    [SerializeField]
    private PlayerData playerData;
    public void SceneSwitch(int scene)
    {
        volume.enabled = false;
        switch(scene)
        {
            case 0:
                playerData.ResetData();
                break;
            case 1:
                playerData.LoadData();
                break;
            case 2:
                Application.Quit();
                break;
        }
        
        SceneManager.LoadScene("DemoScene");
    }

    [SerializeField] private Volume volume;
}
