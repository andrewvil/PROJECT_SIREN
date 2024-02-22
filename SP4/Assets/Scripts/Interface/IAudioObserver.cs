using UnityEngine;

public interface IAudioObserver
{   
    void Notify(Vector3 position, GameObject source);
}
