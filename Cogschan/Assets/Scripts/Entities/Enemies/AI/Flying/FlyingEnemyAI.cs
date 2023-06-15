using UnityEngine;

public abstract class FlyingEnemyAI : MonoBehaviour
{
    [Header("States")]
    [SerializeField] protected ES_Confused es_Confused;
    [SerializeField] protected ES_Alerted es_Alerted;
    [SerializeField] protected ES_PatrolFlying es_Patrol;
    [SerializeField] protected ES_ChaseFlying es_Chase;
    [SerializeField] protected ES_MeleeAttack es_MeleeAttack;
    [SerializeField] protected ES_RangedAttack es_RangedAttack;
    [Header("Other Attributes")]
    [SerializeField] protected bool _hasMeleeAttack;
    [SerializeField] protected bool _hasRangedAttack;
    [SerializeField] protected float _speed;

    public float Speed => _speed;

    protected IEnemyState _state;
}