using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerAudioController : MonoBehaviour
{
    //singleton for easier use, we only have one player audio ctrl anyway 
    public static PlayerAudioController instance;

    [SerializeField]
    private List<AudioClip> audioList;

    [SerializeField]
    private AudioSource src;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayAudio(AUDIOSOUND sound)
    {
        src.PlayOneShot(audioList[(int)sound]);
    }
}

public enum AUDIOSOUND
{
    KEYCARD = 0,
    ITEMTIP = 1,
    FOOTSTEP_LEFT = 2,
    FOOTSTEP_RIGHT = 3,

    DISCOVINDEX = 4,
    TIRED = 5,
    SCROLL = 6,

    DINDEX_PAGETURN = 7,

    FLASHLIGHT=8,

    JUMPSCARE = 9,

    ADRENALINE = 10,
    DANGER = 11,
    BUTTONHOVER = 12,

    HEARTBEAT = 13,

    MEDKIT = 14,
    HEAL = 15,

    SYSTEMSDOWN = 16,
    CRITICAL = 17,

    DECOYEQUIP = 18,

    HURT = 19,

    GLASS1 = 20,
    GLASS2 = 21,

    GLASS3 = 22,
    DIE = 23,

    SIREN = 24


    //more to add
}
