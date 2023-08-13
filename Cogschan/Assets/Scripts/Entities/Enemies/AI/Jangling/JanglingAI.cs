using System.Linq;
using UnityEngine;

public class JanglingAI : MonoBehaviour
{
    [SerializeField] private ES_JanglingIdle es_Idle;
    [SerializeField] private ES_JanglingRunning es_Running;
    [SerializeField] private ES_JanglingStartled es_Startled;
    [SerializeField] private ES_JanglingStunned es_Stunned;
    [SerializeField] private ES_JanglingWatching es_Watching;
    [SerializeField] private EntityServiceLocator _services;
    [SerializeField] private float _runDistance;
    [SerializeField] private int _startNode;

    /// <summary>
    /// The distance between the Jangling and Cogschan at which the Jangling will try to run.
    /// </summary>
    public float RunDistance => _runDistance;
    /// <returns>
    /// The current node that the Jangling resides at. If the Jangling is travelling between nodes, returns <c>null</c>.
    /// </returns>
    public int? CurrentNode { get; private set; }
    /// <returns>
    /// The current node that the Jangling is attempting to travel to. If the Jangling is stationary, returns <c>null</c>.
    /// </returns>
    public int? DestinationNode { get; private set; }

    private IJanglingState _state;
    private int[] _adjacentNodes;

    private void Start()
    {
        _services.Jangling.position = _services.PointGraph.GetNode(_startNode);
        CurrentNode = _startNode;
        _adjacentNodes = _services.PointGraph.GetAdjacent(_startNode);

        _services.HealthTracker.OnDefeat += () => { _state.OnStun(); };
        _services.HealthTracker.OnDamaged += (amount) => { _state.OnDamaged(); };

        es_Idle.IdleToWatching += IdleToWatching;
        es_Idle.IdleToStartled += IdleToStartled;
        es_Idle.IdleToStunned += XToStunned;
        es_Startled.StartledToRunning += XToRunning;
        es_Startled.StartledToStunned += XToStunned;
        es_Watching.WatchingToRunning += XToRunning;
        es_Watching.WatchingToIdle += WatchingToIdle;
        es_Watching.WatchingToStunned += XToStunned;
        es_Running.RunningToIdle += RunningToIdle;
        es_Running.RunningToStunned += XToStunned;
        es_Stunned.StunEnded += StunEnded;

        _state = es_Idle;
    }

    private void Update()
    {
        _state.Behavior();
    }

    #region Glue Methods

    public void XToStunned()
    {
        es_Stunned.Init();
        _state = es_Stunned;
    }

    public void RunningToIdle()
    {
        CurrentNode = DestinationNode;
        DestinationNode = null;
        _adjacentNodes = _services.PointGraph.GetAdjacent(CurrentNode.Value);
        _state = es_Idle;
    }

    public void IdleToStartled()
    {
        es_Startled.Init();
        _state = es_Startled;
    }

    public void IdleToWatching()
    {
        _state = es_Watching;
    }

    public void XToRunning()
    {
        // Pick farthest adjacent node from player, set as destination

        // To break it down: Aggregate goes through a list comparing the current object to the next object using the comparison
        // function you give them. The (anonymous) function given takes the best node so far and the next node and compares their 
        // viability. Depending on which one is larger, the ternary operator (condition ? true : false) chooses one and gives it to
        // Aggregate as the new largest, eventually returning the node that maximizes viability.

        DestinationNode = _adjacentNodes.Aggregate((max, next) => NodeViability(next) >= NodeViability(max) ? next : max);

        // Set current node and destination node as new adjacent nodes while in transit
        _adjacentNodes = new int[] { DestinationNode.Value, CurrentNode.Value };

        CurrentNode = null;

        _state = es_Running;
    }

    /// <summary>
    /// How good of a choice a node is when considering the next node; specifically, which one is in the most opposite direction compared to the player.
    /// </summary>
    /// <param name="index">The node in question.</param>
    /// <returns>
    /// Returns a value between -1 and 1, with -1 representing being in the direction of the player, and 1 being in the direction opposite to the player.
    /// </returns>
    private float NodeViability(int index)
    {
        Vector3 playerDir = _services.LOSChecker.LastSeenPosition - _services.Jangling.position;
        Vector3 nodeDir = _services.PointGraph.GetNode(index) - _services.Jangling.position;

        return -Mathf.Cos(Vector3.Angle(playerDir, nodeDir) * Mathf.Deg2Rad);
    }

    public void WatchingToIdle()
    {
        _state = es_Idle;
    }

    public void StunEnded()
    {
        _services.HealthTracker.ResetHealth();

        // If they were stationary (safe) before, they can continue to stay in place, at least for the next frame
        if (CurrentNode != null)
        {
            _state = es_Idle;
        }
        // If they were running before, they need to resume running
        // Find new most viable adjacent node in case the situation has changed 
        else
        {
            // In the case that they were travelling, there should only be two neighbors
            if (NodeViability(_adjacentNodes[0]) > NodeViability(_adjacentNodes[1]))
            {
                DestinationNode = _adjacentNodes[0];
            }
            else
            {
                DestinationNode = _adjacentNodes[1];
            }

            _state = es_Running;
        }
    }

    #endregion

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_services.Jangling.position, _runDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_services.Jangling.position, _services.LOSChecker.SightRange);
    }
}
