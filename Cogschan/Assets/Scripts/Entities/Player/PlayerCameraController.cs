using Cinemachine;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script responsible for translating player input into camera movement.
/// </summary>
public class PlayerCameraController : MonoBehaviour
{
    [SerializeField] private EntityServiceLocator _services;
    [SerializeField] private CinemachineVirtualCamera _moveCamera;
    [SerializeField] private CinemachineVirtualCamera _aimCamera;
    [SerializeField] private Transform _cameraFollowTarget;
    [SerializeField] private Transform _rigFollowTarget;
    [SerializeField] private Transform _targetReticle;
    [SerializeField] private Transform _cogschanPOV;
    [SerializeField] private LayerMask _targetableMask;
    [SerializeField] private float _mouseHorizontalSensitivity;
    [SerializeField] private float _mouseVerticalSensitivity;
    [SerializeField] private float _maxYaw;
    [SerializeField] private float _minYawWithRecoil;
    [SerializeField] private float _minYaw;

    private List<IRecoilEvent> _recoilEvents = new List<IRecoilEvent>();
    private Vector3 _idealRotation = Vector3.zero;

    /// <summary>
    /// The point in space that Cogschan would look at trying to point toward whatever point in space the player is looking at.
    /// </summary>
    /// <remarks>
    /// If the player is not looking at an object, this will be null. Consider setting Cogschan to look in the camera's direction instead
    /// (or set up a skybox).
    /// </remarks>
    public Vector3? TargetPosition { get; private set; }
    /// <summary>
    /// The y-rotation of the camera, which corresponds to the cardinal direction in which the player is looking.
    /// </summary>
    public Vector3 CameraLateralDirection { get; private set; }

    public void Init(Transform targetReticle)
    {
        _targetReticle = targetReticle;
    }

    private void Start()
    {
        // Make sure the aim camera is off by default
        _aimCamera.gameObject.SetActive(false);
    }

    private void Update()
    {
        UpdateCameraRotation();
        PickCamera();
        UpdateReticle();
    }

    /// <summary>
    /// Takes the x and y movement of the mouse during the last frame and translates it into rotation.
    /// Also takes care of recoil.
    /// </summary>
    private void UpdateCameraRotation()
    {
        // Make sure time is not stopped
        // If it is, just don't do anything
        if (Time.timeScale == 0f) return;

        // First, grab the mouse deltas from the input singleton
        float hRotDelta = CogschanInputSingleton.Instance.MouseDeltaHorizontal * _mouseHorizontalSensitivity;
        float vRotDelta = CogschanInputSingleton.Instance.MouseDeltaVertical * _mouseVerticalSensitivity * -1;

        // Next, perform the rotation, and save it to an intermediate value
        Vector3 angles = (Quaternion.Euler(_idealRotation) * Quaternion.Euler(vRotDelta, hRotDelta, 0)).eulerAngles;

        // If the yaw (X rotation, like nodding your head) is too far, clamp it
        if (angles.x > 180) angles.x -= 360;
        angles.x = Mathf.Clamp(angles.x, _minYaw, _maxYaw);

        // Also make sure z is zero; sometimes the rotation will affect z in weird ways
        angles.z = 0;

        // Save the rotation before applying recoil
        _idealRotation = angles;
        CameraLateralDirection = new Vector3(0, angles.y, 0);

        // Add the effects of all of the recoil
        if (_recoilEvents.Count > 0)
        {
            float hRecoil = 0;
            float vRecoil = 0;
            foreach (IRecoilEvent e in _recoilEvents)
            {
                Vector2 recoil = e.GetRecoil();
                hRecoil += recoil.x;
                vRecoil -= recoil.y;
            }
            _recoilEvents.RemoveAll(e => e.StepTime());

            angles.x += vRecoil;
            angles.y += hRecoil;
            angles.x = Mathf.Max(angles.x, _minYawWithRecoil);
        }

        // Now we actually set the new rotation
        _cameraFollowTarget.transform.localEulerAngles = angles;
    }

    /// <summary>
    /// Selects the camera to use based on Cogschan's state.
    /// </summary>
    private void PickCamera()
    {
        // Pick which camera to use
        // Aim camera has higher priority, so it will automatically take hold if it's active
        if (_services.Animator.GetBool("IsAiming"))
        {
            _aimCamera.gameObject.SetActive(true);
        }
        else
        {
            _aimCamera.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Positions the target reticle and updates <c>TargetDirection</c> based on what would be hit if Cogschan tried to shoot where 
    /// the camera was looking. If nothing is hit (staring off into the void), hit-reticle and hit-direction match
    /// the aim-reticle and the camera's direction, respectfully.
    /// </summary>
    private void UpdateReticle()
    {
        Vector2 cameraCenter = new Vector2(Screen.width / 2, Screen.height / 2);

        // Get the ray coming out of the camera
        Ray playerView = Camera.main.ScreenPointToRay(cameraCenter);

        // Check if the ray hit anything
        // Only checks against anything we specified is a valid target
        // Includes triggers
        if (Physics.Raycast(playerView, out RaycastHit playerViewHit, Mathf.Infinity, _targetableMask, QueryTriggerInteraction.Collide))
        {
            // If it hits something, figure out what Cogschan would see trying to look at what we see
            Vector3 dir = playerViewHit.point - _cogschanPOV.transform.position;
            Ray cogschanView = new Ray(_cogschanPOV.transform.position, dir);

            if (Physics.Raycast(cogschanView, out RaycastHit cogschanViewHit, Mathf.Infinity, _targetableMask, QueryTriggerInteraction.Collide))
            {
                Vector3 target = Camera.main.WorldToScreenPoint(cogschanViewHit.point);

                // If you're looking at something really far away, the Z will be very big, which basically means the UI will be super far away, too
                // So set the Z to zero to bring the reticle back to a visible distance.
                target.z = 0;
                _targetReticle.position = target;

                TargetPosition = cogschanViewHit.point;
            }
        }
        // Since the raycast goes until infinity, the view direction and the target direction are effectively parallel
        else
        {
            _targetReticle.position = cameraCenter;
            TargetPosition = null;
        }

        _rigFollowTarget.position = TargetPosition.HasValue ? TargetPosition.Value : _cogschanPOV.position + playerView.direction;
    }

    public void AddRecoil(SimpleRecoilPattern pattern)
    {
        SimpleRecoilEvent recoil = new SimpleRecoilEvent(pattern);
        _recoilEvents.Add(recoil);
    }

    public void AddRecoil(ContinuousRecoilPattern pattern, Func<bool> endCondition)
    {
        ContinuousRecoilEvent recoil = new ContinuousRecoilEvent(pattern, endCondition);
        _recoilEvents.Add(recoil);
    }
}
