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

    public override void Reset ()
    {
        foreach (Hamster hamster in Hamsters)
        {
            hamster.Reset();
        }
    }

    [Button("Animate Hamsters")]
    public void AnimateHamsters ()
    {
        foreach (Hamster hamster in Hamsters)
        {
            List<Tube> path = TubeManager.Instance.GetPath(hamster.Entrance, TubeManager.Instance.Exit);
            hamster.AnimatePath(path);
        }
    }
}
