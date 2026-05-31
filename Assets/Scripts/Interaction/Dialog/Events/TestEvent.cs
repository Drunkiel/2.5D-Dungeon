using UnityEngine;

[CreateAssetMenu(menuName = "Dialog/Events/Test")]
public class TestEvent : DialogEvent
{
    public int a;
    public override void Execute()
    {
        Debug.Log("Test if event work");
    }
}
