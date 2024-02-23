using System.Collections.Generic;
using UnityEngine;

public class ObjectiveSpawner : MonoBehaviour
{
    [Header("Objective Prefabs")]
    [SerializeField] private GameObject exitDoorPrefab;
    [SerializeField] private GameObject lvl3KeycardPrefab;
    [SerializeField] private GameObject lvl4KeycardPrefab;
    [SerializeField] private GameObject lvl5KeycardPrefab;

    [Header("Drops Prefabs")]
    [SerializeField] private GameObject decoyPrefab;
    [SerializeField] private GameObject healthPrefab;

    [Header("Spawn Positions")]
    [SerializeField] private List<Transform> exitDoorSpawnpoints;
    [SerializeField] private List<Transform> lvl3KeycardSpawnpoints;
    [SerializeField] private List<Transform> lvl4KeycardSpawnpoints;
    [SerializeField] private List<Transform> lvl5KeycardSpawnpoints;
    
    [Header("Drops Settings")]
    [SerializeField] private Vector2 xLimit; 
    [SerializeField] private Vector2 zLimit;
    [SerializeField] private float numDecoy;
    [SerializeField] private float numHealth;
    [SerializeField] private LayerMask spawnLayer;
    
    private void Start()
    {
        SpawnExitDoor();
        SpawnLvl3Keycard();
        SpawnLvl4Keycard();
        SpawnLvl5Keycard();

        for (int i = 0; i < numDecoy; i++)
            SpawnDrop(decoyPrefab);
        for (int i = 0; i < numHealth; i++)
            SpawnDrop(healthPrefab);
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

    private void SpawnDrop(GameObject dropPrefab)
    {
        bool spawned = false;
        float x;
        float z;
        RaycastHit hit;

        x = Random.Range(xLimit.x, xLimit.y);
        z = Random.Range(zLimit.x, zLimit.y);

        if (Physics.Raycast(new Vector3(x, 1, z), Vector3.down, out hit, 1.2f, spawnLayer))
        {
            Instantiate(dropPrefab, hit.point, Quaternion.identity);
            spawned = true;
        }

        if (!spawned)
            SpawnDrop(dropPrefab);
    }
}
