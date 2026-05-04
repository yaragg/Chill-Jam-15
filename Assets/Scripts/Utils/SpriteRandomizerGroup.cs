using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpriteRandomizerGroup : MonoBehaviour
{
    public List<SpriteRandomizer> randomizers;

    public void Start()
    {
        randomizers = GetComponentsInChildren<SpriteRandomizer>().ToList();
        int maxIndex = randomizers.Select(r => r.sprites.Count).Aggregate(0, (max, next) => Math.Max(next, max));
        int randomIndex = UnityEngine.Random.Range(0, maxIndex);
        Utils.LogMessage(this, $"group picked {randomIndex}");
        randomizers.ForEach(r => r.SetFromIndex(randomIndex));
    } 
}