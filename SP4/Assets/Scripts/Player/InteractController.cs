using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractController : MonoBehaviour
{
    [SerializeField] private GameObject inventory;
    private GameObject prev;

    [SerializeField]
    private PlayerController pc;

    private void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 2))
        {
            if(hit.transform.gameObject != prev)
            {
                UIManager.instance.OnHoverExit();
            }
            if (hit.transform.GetComponent<IInteract>() != null)
            {
                prev = hit.transform.gameObject;
                hit.transform.GetComponent<IInteract>()?.OnHover();
                if (Input.GetKeyDown(KeyCode.E))
                {
                    hit.transform.GetComponent<IInteract>().OnInteract(inventory);
                    if (hit.transform.GetComponent<ItemHealth>() != null)
                        GetComponent<PlayerController>().UpdateHealth(hit.transform.GetComponent<ItemHealth>().health);
                    pc.GetInventory();
                }
            }
        }
        else
            UIManager.instance.OnHoverExit();
    }
}
