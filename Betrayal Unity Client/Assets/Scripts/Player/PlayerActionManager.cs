using UnityEngine;

public class PlayerActionManager : MonoBehaviour
{
    [SerializeField] private bool _logAction;
    [SerializeField] private bool _logState;

    [Header("External References")]
    [SerializeField] private RoomController _roomController;
    
	[Header("Player References")]
	[SerializeField] private Player _localPlayer;
    [SerializeField] private MovementController _firstPersonMovement;
    [SerializeField] private InteractionController _firstPersonInteraction;
	[SerializeField] private DoorOpenSequence _doorOpenSequence;
    
	private bool InGame => !PlayerManager.MenuOpen && GameController.CurrentTurn;

	public MovementController PlayerMovement => _firstPersonMovement;
    
	private void Update()
	{
		if (InGame) _firstPersonMovement.ProcessMovement();
	}
	
	private void OnEnable()
	{
		PlayerInputManager.PrimaryInteract += PrimaryInteract;
		PlayerInputManager.Interact += Interact;
	}
	
	private void OnDisable()
	{
		PlayerInputManager.PrimaryInteract -= PrimaryInteract;
		PlayerInputManager.Interact -= Interact;
	}

	public void DisablePlayerMovement() => _firstPersonMovement.SetCanMove(false);
	public void SetPlayerEnabled(bool active)
	{
		_firstPersonMovement.SetCanMove(active);
		_firstPersonInteraction.SetCameraActive(active);
		_firstPersonInteraction.SetCanOpenDoor(active && GameController.Phase == GamePhase.ExplorationPhase);
	}
	
	public void PlayDoorOpenSequence(DoorController door)
	{
		_doorOpenSequence.PlaySequence(door);
	}
	
	public void EquipItem(Item item) => _localPlayer.EquipItem(item);
	
	private void PrimaryInteract()
	{
		if (GameController.Phase == GamePhase.EndTurnPhase) CanvasController.EndTurn();
	}
    
	private void Interact(bool interact)
	{
		if (!interact || !InGame) return;
		LogAction("Interact");
		_firstPersonInteraction.Interact();
	}

    private void LogAction(string message)
    {
        if (_logAction) Debug.Log(message, gameObject);
    }
}
