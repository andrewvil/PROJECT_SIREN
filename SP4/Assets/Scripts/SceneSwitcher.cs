using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    [SerializeField]
    private PlayerData playerData;
    public void SceneSwitch(int scene)
    {
        switch(scene)
        {
            case 0:
                playerData.ResetData();
                break;
            case 1:
                playerData.LoadData();
                break;
        }
        SceneManager.LoadScene("DemoScene");
    }
}
