using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerData : ScriptableObject
{
    public List<int> inventoryIds;
    public int lastCheckpoint;
    public int health;
    public int lastAccessLevel;
    public List<string> discoveryIndex;
    public List<int> floorLayers;
    public List<int> doorLayers;
    public bool hasLoad;
    public void SaveData()
    {
        string s = JsonUtility.ToJson(this);
        if (FileManager.WriteToFile("playerdata.json", s))
        {
            Debug.Log("Save player data successful");
            hasLoad = true;
        }
    }
    public void LoadData()
    {
        if (FileManager.LoadFromFile("playerdata.json", out string s))
        {
            JsonUtility.FromJsonOverwrite(s, this);
        }
    }

    public void ResetData()
    {
        inventoryIds.Clear();
        discoveryIndex.Clear();
        floorLayers.Clear();
        doorLayers.Clear();
        lastCheckpoint = 0;
        health = 100;
        lastAccessLevel = 0;
        hasLoad = false;

        SaveData();
    }
}