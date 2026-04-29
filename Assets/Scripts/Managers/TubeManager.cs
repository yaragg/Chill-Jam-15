using System.Collections.Generic;
using NaughtyAttributes;

public class TubeManager : Manager<TubeManager>
{
    public List<Tube> Tubes { get; private set; } = new();
    public Tube Exit { get; private set;}

    private bool _hasCheckedConnections = false;

    public void RegisterTube(Tube tube)
    {
        Tubes.Add(tube);
    }

    public void UnregisterTube(Tube tube)
    {
        Tubes.Remove(tube);
    }

    public void StartPuzzle ()
    {
        Exit = Tubes.Find(t => t.IsExit);
    }

    public override void Reset ()
    {
        _hasCheckedConnections = false;
    }

    [Button("Test")]
    public List<Tube> GetPath(Tube start, Tube end)
    {
        if (!_hasCheckedConnections) FindAllConnections();

        List<Tube> path = new List<Tube>();
        List<Tube> wrongPath = new List<Tube>(); // Keep a record of the first wrong path to show as feedback
        List<Tube> visited = new List<Tube>();
        
        _FindPath(start, end, path, visited, wrongPath);
        return path.Count > 0 ? path : wrongPath;
    }

    private void FindAllConnections()
    {
        foreach (Tube tube in Tubes)
        {
            tube.FindConnections();
        }
        _hasCheckedConnections = true;
    }

    private void _FindPath(Tube start, Tube end, List<Tube> path, List<Tube> visited, List<Tube> wrongPath)
    {
        if (start == end)
        {
            path.Add(start);
            return;
        }

        visited.Add(start);

        bool hasVisitedAnything = false;
        foreach (Tube connection in start.Connections)
        {
            if (!visited.Contains(connection))
            {
                hasVisitedAnything = true;
                _FindPath(connection, end, path, visited, wrongPath);
                if (path.Count > 0)
                {
                    path.Insert(0, start);
                    return;
                }
            }

            if (!hasVisitedAnything && wrongPath.Count == 0)
            {
                wrongPath.Clear();
                wrongPath.AddRange(visited);
            }
        }
    }
}
