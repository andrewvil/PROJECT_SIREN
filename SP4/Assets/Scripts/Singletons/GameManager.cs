using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int accessLevel {get; private set;}

    [SerializeField]
    private List<GameObject> inventoryObject = new();

    [SerializeField]
    private GameObject player;

    [SerializeField]
    private PlayerData playerData;

    [SerializeField]
    private GameObject deathScreen;

    [SerializeField] 
    private GameObject winScreen;

    private List<int> inventoryIds = new();

    public int currCheckpoint;

    public bool isInUI;

    public bool bGameOver;

    public Checkpoint checkpt;

    public GameObject lastHitEnemy;

    public bool nvIsOn;

    [SerializeField]
    private GameObject settings;
    
    void Awake()
    {
        bGameOver = false;
        instance = this;
        accessLevel = 0;
        currCheckpoint = 0;
        isInUI = false;
        deathScreen.SetActive(false);
        winScreen.SetActive(false);
        settings.SetActive(false);
    }

    void Start()
    {
        
    }

    public void SetAccessLevel(int lvl)
    {
        if(accessLevel < lvl)
            accessLevel = lvl;
    }

    public void Save()
    {
        inventoryIds.Clear();
        foreach(GameObject obj in inventoryObject)
        {
            inventoryIds.Add(obj.GetComponent<IItem>().GetItemID());
        }

        playerData.floorIds.Clear();
        playerData.floorLayers.Clear();
        GameObject[] floors = GameObject.FindGameObjectsWithTag("Floor");
        foreach (GameObject floor in floors)
        {
            playerData.floorIds.Add(floor.GetInstanceID());
            playerData.floorLayers.Add(floor.layer);
        }

        //Saving player data
        playerData.inventoryIds = inventoryIds;
        playerData.lastCheckpoint = currCheckpoint;
        playerData.lastAccessLevel = accessLevel;
        playerData.health = player.GetComponent<IHealth>().GetHealth();
        playerData.SaveData();
    }

    public void Load()
    {
        playerData.LoadData();

        GameObject[] floors = GameObject.FindGameObjectsWithTag("Floor");
        for (int i = 0; i < playerData.floorIds.Count; i++)
        {
            for (int j = 0; j < playerData.floorIds.Count; j++)
            {
                if (floors[i].GetInstanceID() == playerData.floorIds[j])
                {
                    floors[i].layer = playerData.floorLayers[j];
                    continue;
                }
            }
        }
    }

    public void OnCheckpointChanged()
    {
        checkpt?.DisableCheckpoint();
    }

    [SerializeField] private Image deathImage;
    [SerializeField] private Image skullImage;
    [SerializeField] private GameObject deathButtons;
    [SerializeField] private GameObject map;

    public void Die()
    {
        deathButtons.SetActive(false);
        deathImage.material.SetFloat("_Effect", 0f);
        skullImage.material.SetFloat("_Effect", 0f);
        deathScreen.SetActive(true);
        StartCoroutine(FadeDeathScreen());
    }

    public void DisableMap()
    {
        map.SetActive(false);
    }

    private IEnumerator FadeDeathScreen()
    {
        for(float eff = 0f; eff < 1f; eff+=0.1f)
        {
            deathImage.material.SetFloat("_Effect",eff);
            skullImage.material.SetFloat("_Effect",eff);
            yield return new WaitForSeconds(0.1f);
        }
        Cursor.lockState = CursorLockMode.None;
        deathButtons.SetActive(true);
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape) && !bGameOver)
        {
            if (!settings.active)
            {
                isInUI = true;
                Cursor.lockState = CursorLockMode.None;
                Time.timeScale = 0f;
                settings.SetActive(true);
            }
            else
            {
                isInUI = false;
                Cursor.lockState = CursorLockMode.Locked;
                Time.timeScale = 1;
                settings.SetActive(false);
            }
        }

        if(deathButtons.activeInHierarchy)
        {
            if(Input.GetKeyDown(KeyCode.R))
            {
                bGameOver = true;
                RestartGame();
            }
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public GameObject GetPlayer()
    {
        return player;
    }

    public void Escape()
    {
        winScreen.SetActive(true);
        isInUI = true;
        bGameOver = true;
    }
}
