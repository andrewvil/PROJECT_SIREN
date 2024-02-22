using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    [SerializeField]
    private List<Checkpoint> checkpoints;

    [SerializeField]
    private PlayerData playerData;

    // Start is called before the first frame update
    void Start()
    {
        checkpoints = new List<Checkpoint>();
        GetCheckpoints();
    }

    private void GetCheckpoints()
    {
        foreach(Checkpoint obj in GetComponentsInChildren<Checkpoint>())
        {
            checkpoints.Add(obj);
            obj.GetComponent<Checkpoint>().cmp = this;
        }
    }

    public void SetCheckpoint(int index)
    {
        playerData.lastCheckpoint = index;
        playerData.lastAccessLevel = GameManager.instance.accessLevel;
    }
}
