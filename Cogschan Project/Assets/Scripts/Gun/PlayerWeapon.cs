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
    [SerializeField] private Transform originTransform;
    [SerializeField] private Gun gun;
    [SerializeField] private Transform gunLocation; //new
    public float forwardCameraDisplacement; //new
    //[SerializeField] private TrailRenderer bulletTrail;

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
        originTransform.position = Camera.main.ScreenToWorldPoint(screenCenterPoint) + (Camera.main.transform.forward)*forwardCameraDisplacement;

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
            /*
//trails
            if (PlayerInputController.Singleton.ActState == ActionState.Fire)
            {
                TrailRenderer trail = Instantiate(bulletTrail, gunLocation.position, Quaternion.identity);
                StartCoroutine(SpawnTrail(bulletTrail, raycastHit));
                //Debug.Log("Ayup.");
            }
//
    */
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
/*
            //trails
            //if (PlayerInputController.Singleton.ActState == ActionState.Fire)
            //{
                TrailRenderer trail = Instantiate(bulletTrail, gunLocation.position, Quaternion.identity);
                StartCoroutine(SpawnTrail(bulletTrail, raycastHit));
                //Debug.Log("Ayup.");
            //}
            //
            */
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
/*
    private IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit raycastHit)
    {
        float time = 0;
        Vector3 startPosition = trail.transform.position;

        while(time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPosition, raycastHit.point, time);
            time += Time.deltaTime / trail.time;

            yield return null;
        }
        trail.transform.position = raycastHit.point;
        Debug.Log("j");
        //Debug.Log("Nayup.");
        
        //Destroy(trail.gameObject, trail.time);
        //DestroyImmediate(trail, true);
    }
    */
}
