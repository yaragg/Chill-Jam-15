using System.Collections.Generic;

public class TubeManager : Manager<TubeManager>
{
    public List<Tube> Tubes { get; private set; } = new();

    public void RegisterTube (Tube tube)
    {
        Tubes.Add(tube);
    }

    public void UnregisterTube (Tube tube)
    {
        Tubes.Remove(tube);
    }
}
