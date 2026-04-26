using System.Collections.Generic;
using NaughtyAttributes;

public class TubeManager : Manager<TubeManager>
{
    public List<Tube> Tubes { get; private set; } = new();

    public void RegisterTube(Tube tube)
    {
        Tubes.Add(tube);
    }

    public void UnregisterTube(Tube tube)
    {
        Tubes.Remove(tube);
    }

    [Button("Test")]
    public List<Tube> GetPath(Tube start, Tube end)
    {
        FindAllConnections();
        List<Tube> path = new List<Tube>();
        HashSet<Tube> visited = new HashSet<Tube>();
        _FindPath(start, end, path, visited);
        return path;
    }

    private void FindAllConnections()
    {
        foreach (Tube tube in Tubes)
        {
            tube.FindConnections();
        }
    }

    private void _FindPath(Tube start, Tube end, List<Tube> path, HashSet<Tube> visited)
    {
        if (start == end)
        {
            path.Add(start);
            return;
        }

        visited.Add(start);

        foreach (Tube connection in start.Connections)
        {
            if (!visited.Contains(connection))
            {
                _FindPath(connection, end, path, visited);
                if (path.Count > 0)
                {
                    path.Insert(0, start);
                    return;
                }
            }
        }
    }
}
