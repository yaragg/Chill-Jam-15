using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class DataManager : Manager<DataManager>
{
    // private List<LevelDef> levelDefs;
    private LevelOrderDef levelOrderDef;

    protected override IEnumerator Initialize ()
    {
        // levelDefs = Resources.LoadAll<LevelDef>("LevelDefs").ToList();

        yield return base.Initialize();
    }

    public LevelDef GetLevelDef (int levelIndex)
    {
        return levelOrderDef.levels[levelIndex];
    }
}