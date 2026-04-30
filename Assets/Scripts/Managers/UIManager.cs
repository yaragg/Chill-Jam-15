using deVoid.Utils;

public class UIManager : Manager<UIManager>
{
    public void HandleStartSimulationPressed()
    {
        Signals.Get<SimulationStartSignal>().Dispatch();
    }

    public void Test()
    {
        Utils.LogMessage(this, "Test button pressed!");
    }
}