using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Dialog/Events/Dialog Override Event")]
public class DialogOverrideEvent : DialogEvent
{
    [SerializeField] private DialogGraph _graph;

    public override void Execute()
    {
        DialogController.instance._graph = _graph;
        DialogController.instance.StartDialog();
    }
}
