using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DeathButtons : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private string text;

    [SerializeField]
    private TMP_Text buttonTMP;
    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonTMP.text = ">" + text;
        PlayerAudioController.instance.PlayAudio(AUDIOSOUND.BUTTONHOVER);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonTMP.text = text;
    }

    void Start()
    {
        buttonTMP.text = text;
    }

    public void OnRestartClicked()
    {
        GameManager.instance.RestartGame();
    }

    public void OnMainMenuClicked()
    {
        GameManager.instance.ToMainMenu();
    }
}
