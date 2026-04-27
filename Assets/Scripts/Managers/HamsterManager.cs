using System.Collections.Generic;
using NaughtyAttributes;

public class HamsterManager : Manager<HamsterManager>
{
    public List<Hamster> Hamsters { get; private set; } = new();

    public Tube start = null;
    public Tube end = null;

    public void RegisterHamster (Hamster hamster)
    {
        Hamsters.Add(hamster);
    }

    public void UnregisterHamster (Hamster hamster)
    {
        Hamsters.Remove(hamster);
    }

    [Button("Animate Hamsters")]
    public void AnimateHamsters ()
    {
        foreach (Hamster hamster in Hamsters)
        {
            List<Tube> path = TubeManager.Instance.GetPath(start, end);
            hamster.AnimatePath(path);
        }
    }
}
