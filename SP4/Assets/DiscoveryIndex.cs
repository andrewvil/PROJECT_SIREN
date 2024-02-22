using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DiscoveryIndex : MonoBehaviour
{
    [SerializeField]
    private PlayerData playerData;

    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descText;

    int currPageIndex;

    void OnEnable()
    {
        if(playerData.discoveryIndex.Count <= 0) return;
        currPageIndex = 0;
        nameText.text = playerData.discoveryIndex[currPageIndex];
        descText.text = playerData.discoveryIndex[currPageIndex+1];
        PlayerAudioController.instance.PlayAudio(AUDIOSOUND.DISCOVINDEX);
    }

    void Update()
    {
        if(playerData.discoveryIndex.Count <= 0) return;
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            currPageIndex = (currPageIndex+2) % playerData.discoveryIndex.Count;
            nameText.text = playerData.discoveryIndex[currPageIndex];
            descText.text = playerData.discoveryIndex[currPageIndex+1];
        }
    }

    public void ChangePage(bool front)
    {
        if(playerData.discoveryIndex.Count <= 0) return;
        if(front)
        {
            currPageIndex = (currPageIndex+2) % playerData.discoveryIndex.Count;
        }
        else
        {
            currPageIndex = (currPageIndex+playerData.discoveryIndex.Count-2) % playerData.discoveryIndex.Count;
        }
        nameText.text = playerData.discoveryIndex[currPageIndex];
        descText.text = playerData.discoveryIndex[currPageIndex+1];
        PlayerAudioController.instance.PlayAudio(AUDIOSOUND.DINDEX_PAGETURN);
    }
}
