using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

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
        playerData.LoadData();
        if(playerData.hasLoad)
        {
            Load();
        }
        bGameOver = false;
        instance = this;
        accessLevel = 0;
        currCheckpoint = 0;
        isInUI = false;
        deathScreen.SetActive(false);
        winScreen.SetActive(false);
        settings.SetActive(false);
        deathText.text = "";
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
        //Inventory
        inventoryIds.Clear();
        foreach(GameObject obj in inventoryObject)
        {
            inventoryIds.Add(obj.GetComponent<IItem>().GetItemID());
        }

        //Floors
        playerData.floorLayers.Clear();
        GameObject[] floors = GameObject.FindGameObjectsWithTag("Floor");
        foreach (GameObject floor in floors)
        {
            playerData.floorLayers.Add(floor.layer);
        }

        //Doors
        playerData.doorLayers.Clear();
        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
        foreach (GameObject door in doors)
        {
            playerData.doorLayers.Add(door.layer);
        }

        //Saving player data
        playerData.inventoryIds = inventoryIds;
        playerData.lastCheckpoint = currCheckpoint;
        playerData.lastAccessLevel = accessLevel;
        playerData.health = player.GetComponent<IHealth>().GetHealth();
        if (!playerData.hasLoad)
        {
            playerData.hasLoad = true;
        }
        playerData.SaveData();
    }

    public void Load()
    {
        GameObject[] floors = GameObject.FindGameObjectsWithTag("Floor");
        for (int i = 0; i < floors.Length; i++)
        {
            floors[i].layer = playerData.floorLayers[i];
        }

        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
        for (int i = 0; i < doors.Length; i++)
        {
            doors[i].layer = playerData.doorLayers[i];
        }
    }

    public void OnCheckpointChanged()
    {
        checkpt?.DisableCheckpoint();
    }

    [SerializeField] private Image deathImage;
    [SerializeField] private Image skullImage;
    [SerializeField] private GameObject deathButtons;

    public void Die()
    {
        deathButtons.SetActive(false);
        deathImage.material.SetFloat("_Effect", 0f);
        skullImage.material.SetFloat("_Effect", 0f);
        deathScreen.SetActive(true);
        StartCoroutine(FadeDeathScreen());
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
        StartCoroutine(ScrollDeathText());
    }

    void Update()
    {

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

    public string deathTip;

    [SerializeField]
    private TMP_Text deathText;

    public IEnumerator ScrollDeathText()
    {
        deathText.text = "";

        foreach(char character in deathTip)
        {
            deathText.text = deathText.text + character;
            yield return new WaitForSeconds(0.02f);
        }

    }
}
