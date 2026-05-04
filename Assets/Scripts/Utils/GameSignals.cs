using deVoid.Utils;
using UnityEngine;

public class SimulationStartSignal : ASignal {};
public class SimulationResetSignal : ASignal {};

public class HamsterArrivedSignal : ASignal<bool> {}; // bool indicates whether it found the exit
public class TapSignal : ASignal<Vector2> {};
public class DragSignal : ASignal<Vector2, bool> {}; // bool indicates whether the drag is starting or ending
public class RotateSignal : ASignal<Vector2> {};

public class RecoverTubeSignal : ASignal<Tube.TubeType>{};