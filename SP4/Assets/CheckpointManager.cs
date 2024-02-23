using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    [SerializeField]
    private List<Checkpoint> checkpoints;

    [SerializeField]
    private PlayerData playerData;

    [SerializeField] private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        checkpoints = new List<Checkpoint>();
        GetCheckpoints();
    }

    private void GetCheckpoints()
    {
        int indexes = 1;
        foreach(Checkpoint obj in GetComponentsInChildren<Checkpoint>())
        {
            checkpoints.Add(obj);
            obj.GetComponent<Checkpoint>().cmp = this;
            obj.GetComponent<Checkpoint>().index = indexes;
            indexes++;
        }

        if(playerData.hasLoad)
        {
            foreach(Checkpoint checkpoint in checkpoints)
            {
                if(checkpoint.index == playerData.lastCheckpoint)
                {
                    checkpoint.ActivateCheckpoint();
                    player.GetComponent<CharacterController>().enabled = false;
                    player.transform.position = checkpoint.gameObject.transform.position;
                    player.GetComponent<CharacterController>().enabled = true;
                    return;
                }
            }
        }
    }



    public void SetCheckpoint(int index)
    {
        playerData.lastCheckpoint = index;
        playerData.lastAccessLevel = GameManager.instance.accessLevel;
    }
}
