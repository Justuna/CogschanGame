using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class AIFollow : MonoBehaviour
{
    [SerializeField] private EntityServiceLocator _services;
    public float lookRadius = 10f;
    public Transform Player;
    public Transform Model;
    public float RecalculateTimer = 0.5f;
    public float Speed = 5;
    public float TurnSpeed = 10;

    private NavMeshPath _path;
    private float _timer = 0;
    private int _currentCorner = 0;

    void Start()
    {
        _path = new NavMeshPath();
    }

    void Update()
    {
        _timer -= Time.deltaTime;
        if (_timer <= 0)
        {
            NavMesh.SamplePosition(transform.position, out NavMeshHit hit, Mathf.Infinity, NavMesh.AllAreas);
            NavMesh.SamplePosition(Player.position, out NavMeshHit hit2, Mathf.Infinity, NavMesh.AllAreas);
            NavMesh.CalculatePath(hit.position, hit2.position, NavMesh.AllAreas, _path);
            _timer = RecalculateTimer;
            _currentCorner = 0;
        }

        Vector3 targetPosition = _path.corners[_currentCorner];
        Debug.DrawLine(transform.position, targetPosition, Color.red);
        if (Vector3.Distance(targetPosition, transform.position) <= 0.05f && _currentCorner < _path.corners.Length - 1)
        {
            _currentCorner += 1;
        }
        else if (Vector3.Distance(targetPosition, transform.position) > 0.05f)
        {
            Vector3 moveDir = (targetPosition - transform.position).normalized;
            Vector3 moveDirHorizontal = moveDir;
            moveDirHorizontal.y = 0;

            _services.KinematicPhysics.DesiredVelocity = moveDir * Speed;
            Model.rotation = Quaternion.Lerp(Model.rotation, Quaternion.LookRotation(moveDirHorizontal), Time.deltaTime * TurnSpeed);
        }
    }
}
