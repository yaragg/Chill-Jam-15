using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

class LevelOrderDef : ScriptableObject
{
    [ReorderableList]
    public List<LevelDef> levels;
}