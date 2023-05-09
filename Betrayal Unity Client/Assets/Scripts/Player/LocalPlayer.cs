using UnityEngine;

public class LocalPlayer : MonoBehaviour
{
    public static LocalPlayer Instance;
    
    [Header("References")]
    [SerializeField] private MovementController _firstPersonMovement;
    [SerializeField] private InteractionController _firstPersonInteraction;
    [SerializeField] private SpectatorMovement _spectatorMovement;
}
