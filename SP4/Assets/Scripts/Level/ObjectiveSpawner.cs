using System.Collections.Generic;
using UnityEngine;

public class ObjectiveSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject exitDoorPrefab;
    [SerializeField] private GameObject lvl3KeycardPrefab;
    [SerializeField] private GameObject lvl4KeycardPrefab;
    [SerializeField] private GameObject lvl5KeycardPrefab;

    [Header("Spawn Positions")]
    [SerializeField] private List<Transform> exitDoorSpawnpoints;
    [SerializeField] private List<Transform> lvl3KeycardSpawnpoints;
    [SerializeField] private List<Transform> lvl4KeycardSpawnpoints;
    [SerializeField] private List<Transform> lvl5KeycardSpawnpoints;
    
    private void Start()
    {
        SpawnExitDoor();
        SpawnLvl3Keycard();
        SpawnLvl4Keycard();
        SpawnLvl5Keycard();
    }

    private void SpawnExitDoor()
    {
        int rng = Random.Range(0, exitDoorSpawnpoints.Count);
        Instantiate(exitDoorPrefab, exitDoorSpawnpoints[rng].position, exitDoorSpawnpoints[rng].rotation);
    }

    private void SpawnLvl3Keycard()
    {
        List<Transform> list = new List<Transform>();
        foreach (Transform pos in lvl3KeycardSpawnpoints)
            list.Add(pos);

        for (int i = 0; i < 2; i++)
        {
            int rng = Random.Range(0, lvl3KeycardSpawnpoints.Count - i);
            float rot = Random.Range(-180f, 180f);
            Instantiate(lvl3KeycardPrefab, list[rng].position, Quaternion.Euler(new Vector3(-90f, rot, 0f)));
            list.Remove(list[rng]);
        }
    }

    private void SpawnLvl4Keycard()
    {
        List<Transform> list = new List<Transform>();
        foreach (Transform pos in lvl4KeycardSpawnpoints)
            list.Add(pos);
        
        for (int i = 0; i < 2; i++)
        {
            int rng = Random.Range(0, lvl4KeycardSpawnpoints.Count - i);
            float rot = Random.Range(-180f, 180f);
            Instantiate(lvl4KeycardPrefab, list[rng].position, Quaternion.Euler(new Vector3(-90f, rot, 0f)));
            list.Remove(list[rng]);
        }
    }

    private void SpawnLvl5Keycard()
    {
        List<Transform> list = new List<Transform>();
        foreach (Transform pos in lvl5KeycardSpawnpoints)
            list.Add(pos);
        
        for (int i = 0; i < 2; i++)
        {
            int rng = Random.Range(0, lvl5KeycardSpawnpoints.Count - i);
            float rot = Random.Range(-180f, 180f);
            Instantiate(lvl5KeycardPrefab, list[rng].position, Quaternion.Euler(new Vector3(-90f, rot, 0f)));
            list.Remove(list[rng]);
        }
    }
}
