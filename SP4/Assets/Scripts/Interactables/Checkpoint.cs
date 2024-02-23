using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour, ISightObserver, IPhotoObserver
{
    [SerializeField] private AudioSource src;
    [SerializeField] private AudioClip saveSfx;
    [SerializeField] private Light glow;
    [SerializeField] private MeshRenderer mr;

    public CheckpointManager cmp;
    public int index;

    bool checkpointActive;

    void Awake()
    {
        checkpointActive = false;
        mr.material.color = Color.gray;
    }

    public void OnLookAway()
    {
    }

    public void OnPhotoTaken()
    {
        //TODO: Save
        
        UIManager.instance.DisplayTip("Checkpoint", "Game saved!", false);
        GameManager.instance.OnCheckpointChanged();
        GameManager.instance.checkpt = this;
        GameManager.instance.currCheckpoint = index;
        src.PlayOneShot(saveSfx);
        checkpointActive = true;
        cmp.SetCheckpoint(index);

        if(TutorialManager.instance)
        {
            TutorialManager.instance.picDone = true;
        }
        GameManager.instance.Save();
    }

    void Update()
    {
        if(!checkpointActive) return;
        mr.material.color = Color.Lerp(mr.material.color, Color.green * 0.7f, 0.03f);
        glow.intensity = Mathf.Lerp(glow.intensity,1f,0.03f);
        mr.material.SetColor("_EmissionColor",Color.Lerp(Color.black,Color.green * 0.1f,glow.intensity));
        mr.material.EnableKeyword("_EMISSION");
    }

    public void OnSighted()
    {
    }

    public string GetDetails()
    {
        return "CHECKPOINT";
    }

    public void DisableCheckpoint()
    {
        checkpointActive = false;
        mr.material.color = Color.white;
        glow.intensity = 0f;
        mr.material.SetColor("_EmissionColor",Color.black);
        mr.material.EnableKeyword("_EMISSION");
    }
}
