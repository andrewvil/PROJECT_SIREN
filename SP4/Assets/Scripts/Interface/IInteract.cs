using UnityEngine;

public interface IInteract
{
    public string GetItemName();

    public void OnHover();

    public void OnInteract(GameObject inventory);
}
