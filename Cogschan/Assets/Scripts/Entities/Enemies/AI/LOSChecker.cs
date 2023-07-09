using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LOSChecker : MonoBehaviour
{
    [Tooltip("The layer(s) which can block line of sight.")]
    [SerializeField] private LayerMask _solidMask;
    [Tooltip("How far away the target can be before line of sight automatically fails.")]
    [SerializeField] private float _sightRange;
    [Tooltip("The delay after losing the target before line of sight is considered to be lost. This exists mainly to prevent line of sight from being broken by briefly walking past objects.")]
    [SerializeField] private float _interruptThreshold;
    [Tooltip("Whether or not the player should automatically be targeted by default.")]
    [SerializeField] private bool _targetPlayer = true;

    /// <summary>
    /// The transform of the target to check for line of sight on.
    /// </summary>
    public Transform Target;
    /// <summary>
    /// How far away the target can be before line of sight automatically fails.
    /// </summary>
    public float SightRange => _sightRange;
    /// <summary>
    /// Whether or not line of sight exists.
    /// </summary>
    public bool CanSee { get; private set; }
    /// <summary>
    /// The last position in world space in which the target was seen.
    /// </summary>
    public Vector3 LastSeenPosition { get; private set; }

    private float _interruptClock;   

    private void Start()
    {
        if (_targetPlayer) Target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (!Target || Time.timeScale == 0 ) return;

        Vector3 dir = Target.position - transform.position;
        Ray ray = new Ray(transform.position, dir);
        bool LOS = Vector3.Distance(Target.position, transform.position) <= _sightRange && !Physics.Raycast(ray, dir.magnitude, _solidMask);

        if (LOS)
        {
            _interruptClock = _interruptThreshold;
            LastSeenPosition = Target.position;
            CanSee = true;
        }
        else
        {
            if (_interruptClock > 0)
            {
                _interruptClock -= Time.deltaTime;
                LastSeenPosition = Target.position;
                CanSee = true;
            }
            else CanSee = false;
        }
    }
}
