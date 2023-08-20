using UnityEngine;

public class KinematicCharacterController : MonoBehaviour, ICharacterController
{

}

public class PhysicsConstantSingleton : MonoBehaviour
{
    #region Singleton Stuff
    /// <summary>
    /// The only instance of this class that is allowed to exist.
    /// </summary>
    public static PhysicsConstantSingleton Instance { get; private set; }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }

        Instance = this;
    }
    #endregion

    [SerializeField] private float _gravityAcceleration;
    [SerializeField] private float _normalForceAcceleration;
    [SerializeField] private float _terminalVelocity;

    public float GravityAcceleration => _gravityAcceleration;
    public float NormalForceAcceleration => _normalForceAcceleration;
    public float TerminalVelocity => _terminalVelocity;
}