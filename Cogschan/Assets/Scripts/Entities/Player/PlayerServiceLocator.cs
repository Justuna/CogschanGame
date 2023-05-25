using UnityEngine;

public class PlayerServiceLocator : MonoBehaviour
{
    public CharacterController CharacterController;
    public CogschanKinematicPhysics KinematicPhysics;
    public GroundChecker GroundChecker;
    public PlayerActionController ActionController;
    public PlayerCameraController CameraController;
    public PlayerMovementController MovementController;
    public WeaponCache WeaponCache;
}