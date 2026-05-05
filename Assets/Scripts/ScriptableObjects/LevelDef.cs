using UnityEngine;

public class LevelDef : ScriptableObject
{
    public GameObject levelPrefab;
    public string levelName;
    public int numITubes;
    public int numLTubes;
    public int numTTubes;
    public int numCrossTubes;

    public string GetName ()
    {
        return string.IsNullOrEmpty(levelName) ? levelPrefab.name : levelName;
    }
}