using System.Collections;
using System.Collections.Generic;
using deVoid.Utils;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Manager<GameManager>
{
    public bool IsSimulationRunning { get; private set; } = false;

    [Foldout("Internal Config")]
    public string GameSceneName;

    private int _hamstersArrived = 0;
    private int _hamstersExited = 0;
    private LevelOrderDef _levelOrderDef;
    private int _levelIndex;

    protected override IEnumerator Initialize ()
    {
        _levelOrderDef = Resources.Load<LevelOrderDef>("LevelOrderDef");

        Signals.Get<SimulationStartSignal>().AddListener(HandleSimulationStart);
        Signals.Get<HamsterArrivedSignal>().AddListener(HandleHamsterArrived);
        Signals.Get<SimulationResetSignal>().AddListener(ResetPuzzle);

        yield return null;
    }

    private void HandleSceneChange ()
    {
        if (SceneManager.GetActiveScene().name == GameSceneName)
        {
            
        }
    }

    [Button("Start")]
    public void HandleSimulationStart ()
    {
        IsSimulationRunning = true;
        _hamstersArrived = 0;
        _hamstersExited = 0;
        TubeManager.Instance.StartPuzzle();
        HamsterManager.Instance.AnimateHamsters();
    }

    private void HandleSimulationEnd ()
    {
        bool success = _hamstersExited == HamsterManager.Instance.Hamsters.Count;
        Utils.LogMessage(this, $"Simulation end: {success}");
        
        if (success) StartNextPuzzle();
        else Signals.Get<SimulationResetSignal>().Dispatch();
        IsSimulationRunning = false;
    }

    private void HandleHamsterArrived (bool foundExit)
    {
        _hamstersArrived++;
        if (foundExit) _hamstersExited++;
        Utils.LogMessage(this, $"Hamster arrived! Total: {_hamstersArrived}, Exited: {_hamstersExited}");

        if (_hamstersArrived == HamsterManager.Instance.Hamsters.Count)
        {
            HandleSimulationEnd();
        }   
    }

    private void StartNextPuzzle ()
    {
        Signals.Get<SimulationResetSignal>().Dispatch();
        Utils.LogMessage(this, "Starting next puzzle...");
        SceneManager.LoadScene(GameSceneName);
    }

    private void ResetPuzzle ()
    {
        IsSimulationRunning = false;
        Utils.LogMessage(this, "Resetting puzzle...");
        HamsterManager.Instance.Reset();
        TubeManager.Instance.Reset();
    }

    public LevelDef GetCurrentLevelDef ()
    {
        return _levelOrderDef.levels[_levelIndex];
    }
}