using System.Collections.Generic;

public class HamsterManager : Manager<HamsterManager>
{
    public List<Hamster> Hamsters { get; private set; } = new();

    public void RegisterTube (Hamster hamster)
    {
        Hamsters.Add(hamster);
    }

    public void UnregisterTube (Hamster hamster)
    {
        Hamsters.Remove(hamster);
    }
}
