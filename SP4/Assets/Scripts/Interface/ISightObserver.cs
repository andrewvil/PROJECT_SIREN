
public interface ISightObserver
{
    public void OnSighted();

    public void OnLookAway();

    public string GetDetails()
    {
        return "???";
    }
}
