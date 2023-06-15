using UnityEngine;
using UnityEngine.AI;

public class EntityServiceLocator : MonoBehaviour
{
    public bool IsPlayer = false;

    [Header("General Services")]
    public CharacterController CharacterController;
    public GroundChecker GroundChecker;
    public HealthTracker HealthTracker;
    public InteractionChecker InteractionChecker;
    public KinematicPhysics KinematicPhysics;
    public GameObject Model;

    [Header("Player Only")]
    public PlayerActionController ActionController;
    public PlayerCameraController CameraController;
    public PlayerMovementController MovementController;
    public WeaponCache WeaponCache;

    [Header("Enemies Only")]
    public LOSChecker LOSChecker;

    [Header("Ground Enemies Only")]
    public NavMeshAgent NavMeshAgent;
    public GroundedEnemyAI GroundedAI;

    [Header("Flying Enemies Only")]
    public FlyingEnemyAI FlyingAI;
}