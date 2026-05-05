using System.Collections;
using System.Collections.Generic;
using System.Linq;
using deVoid.Utils;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : Manager<GameManager>
{
    public bool IsSimulationRunning { get; private set; } = false;

    [Foldout("Internal Config")]
    public string GameSceneName;
    [Foldout("Internal Config")]
    public string EndSceneName;
    [Foldout("Internal Config")]
    public List<string> gameplayAudios;
    [Foldout("Internal Config")]
    public GameObject confettiPrefab;

    private int _hamstersArrived = 0;
    private int _hamstersExited = 0;
    private LevelOrderDef _levelOrderDef;
    private int _levelIndex;
    private float _fadeoutDuration = 5f;

    protected override IEnumerator Initialize()
    {
        _levelOrderDef = Resources.Load<LevelOrderDef>("LevelOrderDef");

        Signals.Get<SimulationStartSignal>().AddListener(HandleSimulationStart);
        Signals.Get<HamsterArrivedSignal>().AddListener(HandleHamsterArrived);
        Signals.Get<SimulationResetSignal>().AddListener(ResetPuzzle);
        Signals.Get<AudioEndedSignal>().AddListener(HandleAudioFinished);

        SceneManager.sceneLoaded += (scene, mode) => HandleSceneChange(scene);

        yield return null;
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            StartNextPuzzle();
        }
    }
#endif

    private void HandleSceneChange(Scene scene)
    {
        Utils.LogMessage(this, $"scene {scene.name} {GameSceneName}");
        if (scene.name == GameSceneName)
        {
            List<GameAudioClip> clips = AudioManager.Instance.GetCurrentClipsByTag("bgm");
            GameAudioClip bgm;
            if (clips.Count > 0) bgm = clips.First();
            else bgm = AudioManager.Instance.Play(gameplayAudios.First(), "bgm");
            bgm.IsLooping = false;
                bgm.fadeOutSecs = _fadeoutDuration;
            SetupFromLevel();
        }
    }

    private void HandleAudioFinished(string clipName, string tag)
    {
        if (tag == "bgm")
        {
            string previous = gameplayAudios[0];
            gameplayAudios.RemoveAt(0);
            gameplayAudios.Add(previous);
            GameAudioClip bgm = AudioManager.Instance.Play(gameplayAudios.First(), "bgm");
            bgm.fadeOutSecs = _fadeoutDuration;
        }
    }

    private void SetupFromLevel()
    {
        SetupGrid();

#if UNITY_EDITOR
        ObjectIdentifier levelTitle = ObjectManager.Instance.GetObject("level");
        if (levelTitle != null) levelTitle.GetComponent<TMP_Text>().text = $"Level: {GetCurrentLevelDef().GetName()}";
#endif

        int numFloors = 5;
        // Remove assets and hamsters from unused entrance floors
        for (int i = 1; i <= numFloors; i++)
        {
            string groupID = i.ToString();
            ObjectIdentifier entrance = ObjectManager.Instance.GetObjectInGroup("entrance", groupID);
            if (entrance != null)
            {
                ObjectIdentifier hamster = ObjectManager.Instance.GetObjectInGroup("hamster", groupID);
                hamster.GetComponent<Hamster>().SetEntrance(entrance.GetComponent<Tube>());
            }
            else
            {
                List<ObjectIdentifier> group = ObjectManager.Instance.GetGroup(i.ToString());
                group.ForEach(obj => Destroy(obj.gameObject));
            }
        }

        // Remove assets from unused exit floors
        for (int i = 1; i <= numFloors; i++)
        {
            string groupID = $"right-{i}";
            ObjectIdentifier exit = ObjectManager.Instance.GetObjectInGroup("exit", groupID);
            if (exit == null)
            {
                List<ObjectIdentifier> group = ObjectManager.Instance.GetGroup(groupID);
                group.ForEach(obj => Destroy(obj.gameObject));
            }
        }

    }

    private void SetupGrid()
    {
        LevelDef levelDef = GetCurrentLevelDef();
        GameObject tubeGridObj = Instantiate(levelDef.levelPrefab);
        TubeGrid tubeGrid = tubeGridObj.GetComponent<TubeGrid>();
        TubeManager.Instance.SetGrid(tubeGrid);

        float gridX = -tubeGrid.GetWidth() / 2;
        float gridY = ObjectManager.Instance.GetObject("grid-spawn").transform.position.y;
        tubeGrid.transform.position = new Vector3(gridX + tubeGrid.Offset.x, gridY, 0f);

        GameObject cageLeft = ObjectManager.Instance.GetObject("cage-left").gameObject;
        GameObject cageRight = ObjectManager.Instance.GetObject("cage-right").gameObject;
        float cageY = gridY - 1f + tubeGrid.Overlap.y;
        cageLeft.transform.position = new Vector3(gridX + tubeGrid.Overlap.x, cageY, 0f);
        cageRight.transform.position = new Vector3(gridX + tubeGrid.GetWidth() - tubeGrid.Overlap.x, cageY, 0f);
    }

    public void HandleSimulationStart()
    {
        IsSimulationRunning = true;
        _hamstersArrived = 0;
        _hamstersExited = 0;
        TubeManager.Instance.StartPuzzle();
        HamsterManager.Instance.AnimateHamsters();
    }

    private void HandleSimulationEnd()
    {
        bool success = _hamstersExited == HamsterManager.Instance.Hamsters.Count;
        Utils.LogMessage(this, $"Simulation end: {success}");

        // Add a delay so the player can see the hamster celebrating or getting stuck
        if (success)
        {
            Instantiate(confettiPrefab);
            AudioManager.Instance.Play("SFX_HamsterYay", "sfx");
            Utils.DelayCall(this, 2.5f, StartNextPuzzle);
        }
        else Utils.DelayCall(this, 1.5f, () => Signals.Get<SimulationResetSignal>().Dispatch());
        IsSimulationRunning = false;
    }

    private void HandleHamsterArrived(bool foundExit)
    {
        _hamstersArrived++;
        if (foundExit) _hamstersExited++;
        Utils.LogMessage(this, $"Hamster arrived! Total: {_hamstersArrived}, Exited: {_hamstersExited}");

        if (_hamstersArrived == HamsterManager.Instance.Hamsters.Count)
        {
            HandleSimulationEnd();
        }
    }

    [Button("Skip Level")]
    private void StartNextPuzzle()
    {
        _levelIndex++;
        if (_levelIndex >= _levelOrderDef.levels.Count)
        {
            SceneManager.LoadScene(EndSceneName);
            return;
        }

        Utils.LogMessage(this, "Starting next puzzle...");
        TubeManager.Instance.Reset();
        SceneManager.LoadScene(GameSceneName);
        IsSimulationRunning = false;
    }

    private void ResetPuzzle()
    {
        IsSimulationRunning = false;
        Utils.LogMessage(this, "Resetting puzzle...");
        HamsterManager.Instance.Reset();
        TubeManager.Instance.Reset();
    }

    public LevelDef GetCurrentLevelDef()
    {
        return _levelOrderDef.levels[_levelIndex];
    }
}