using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.InputSystem;

public class PlayerWeapon : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimSensitivity;
    [SerializeField] private LayerMask aimColliderLayerMask;
    [SerializeField] private Transform debugTransform;
    [SerializeField] private Gun gun;
    [SerializeField] private Transform gunLocation; //new
    public float forwardCameraDisplacement; //new

    private PlayerMovement thirdPersonController;

    private void Awake()
    {
        thirdPersonController = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        Vector3 mouseWorldPosition = Vector3.zero;


        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);

        Ray cameraRay = new Ray(Camera.main.ScreenToWorldPoint(screenCenterPoint) + (Camera.main.transform.forward)*forwardCameraDisplacement, Camera.main.transform.forward);
        Debug.DrawRay(Camera.main.ScreenToWorldPoint(screenCenterPoint) + (Camera.main.transform.forward)*forwardCameraDisplacement, Camera.main.transform.forward, Color.green);

        Transform hitTransform = null;
        if (Physics.Raycast(cameraRay, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            debugTransform.position = raycastHit.point;
            mouseWorldPosition = raycastHit.point;
            hitTransform = raycastHit.transform;
        }

        Ray gunRay = new Ray(gunLocation.position, raycastHit.point - gunLocation.position);

        //Transform hitTransform = null;
        if (Physics.Raycast(gunRay, out raycastHit, 999f, aimColliderLayerMask))
        {
            debugTransform.position = raycastHit.point;
            mouseWorldPosition = raycastHit.point;
            hitTransform = raycastHit.transform;
        }

        Debug.DrawRay(gunLocation.position, raycastHit.point - gunLocation.position);

        if (PlayerInputController.Singleton.MoveState == MovementState.ADS)
        {
            aimVirtualCamera.gameObject.SetActive(true);
            thirdPersonController.SetSensitivity(aimSensitivity);
            thirdPersonController.SetRotateOnMove(false);

            Vector3 worldAimTarget = mouseWorldPosition;
            worldAimTarget.y = transform.position.y;
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
        }
        else
        {
            aimVirtualCamera.gameObject.SetActive(false);
            thirdPersonController.SetSensitivity(normalSensitivity);
            thirdPersonController.SetRotateOnMove(true);
        }

        if (PlayerInputController.Singleton.ActState == ActionState.Fire)
        {
            if (PlayerInputController.Singleton.MoveState == MovementState.ADS)
            {
                gun.ADSFire(hitTransform);
                //Debug.Log("Firing scoped in");
            }
            else
            {
                gun.HipFire(hitTransform);
                //Debug.Log("Firing from hip");
            }
        }
    }
}
