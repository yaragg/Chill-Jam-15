using System.Collections;
using System.Collections.Generic;
using deVoid.Utils;
using NaughtyAttributes;
using UnityEngine;

public class GameManager : Manager<GameManager>
{
    public bool IsSimulationRunning { get; private set; } = false;

    private int _hamstersArrived = 0;
    private int _hamstersExited = 0;

    protected override IEnumerator Initialize ()
    {
        Signals.Get<SimulationStartSignal>().AddListener(HandleSimulationStart);
        Signals.Get<HamsterArrivedSignal>().AddListener(HandleHamsterArrived);
        Signals.Get<SimulationResetSignal>().AddListener(ResetPuzzle);

        yield return null;
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
    }

    private void ResetPuzzle ()
    {
        IsSimulationRunning = false;
        Utils.LogMessage(this, "Resetting puzzle...");
        HamsterManager.Instance.Reset();
        TubeManager.Instance.Reset();
    }
}