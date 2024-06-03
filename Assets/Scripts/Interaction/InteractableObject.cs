using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InteractableObject : MonoBehaviour
{
    public bool isTriggerNearby;
    public UnityEvent onClickFunctionalities;
    public UnityEvent onHoldFunctionalities;
    public UnityEvent onEndClickFunctionalities;

    private TriggerController _triggerController;
    [SerializeField] private InputActionAsset inputActions;
    private AudioSource audioSource;

    private void Start()
    {
        _triggerController = GetComponent<TriggerController>();

        if (TryGetComponent(out PlayerInput _input))
            _input.actions = inputActions;

        if (TryGetComponent(out AudioSource _audio))
            audioSource = _audio;
    }

    private void Update()
    {
        if (InteractionSystem.isInteracting) 
            return;

        isTriggerNearby = _triggerController.isTriggered;
    }

    public void Click(InputAction.CallbackContext context)
    {
        if (onClickFunctionalities.GetPersistentEventCount() > 0 && isTriggerNearby && context.performed)
            StartOnClickInteraction();
    }

    public void Hold(InputAction.CallbackContext context)
    {
        if (isTriggerNearby && context.performed) 
            StartOnHoldInteraction();
    }

    public void End(InputAction.CallbackContext context)
    {
        if (isTriggerNearby && context.canceled) 
            StartOnEndInteraction();
    }

    private void StartOnClickInteraction()
    {
/*        if (audioSource != null)
            audioSource.Play();
        else
            Debug.LogError(transform.parent.name + ": does not have Audio Source");

*/
        onClickFunctionalities.Invoke();
    }

    private void StartOnHoldInteraction()
    {
        onHoldFunctionalities.Invoke();
    }

    private void StartOnEndInteraction()
    {
        onEndClickFunctionalities.Invoke();
    }
}
