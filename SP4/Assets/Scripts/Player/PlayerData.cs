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
    public List<int> floorIds;
    public List<int> floorLayers;

    public void SaveData()
    {
        string s = JsonUtility.ToJson(this);
        if (FileManager.WriteToFile("playerdata.json", s))
        {
            Debug.Log("Save player data successful");
        }
    }
    public void LoadData()
    {
        if (FileManager.LoadFromFile("playerdata.json", out string s))
        {
            JsonUtility.FromJsonOverwrite(s, this);
        }
    }
}