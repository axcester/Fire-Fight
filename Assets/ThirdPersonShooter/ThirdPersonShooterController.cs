using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.Animations.Rigging;

public class ThirdPersonShooterController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform debugTransform;
    [SerializeField] private Transform pfBulletProjectile;
    [SerializeField] private Transform pfSplash;
    [SerializeField] private Transform spawnBulletPosition;
    [SerializeField] private Transform pfMuzzleFlash;
    [SerializeField] private List<GameObject> aimConstraints = new List<GameObject>();
    [SerializeField] private GameObject headConstraint;

    private ThirdPersonController thirdPersonController;
    private StarterAssetsInputs starterAssetsInputs;
    private Animator animator;
    private Vector3 mouseWorldPosition = Vector3.zero;
    private float mostRecentShot;

    private void Awake()
    {
        thirdPersonController = GetComponent<ThirdPersonController>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        animator = GetComponent<Animator>();
        SetIdleMode();
    }

    private void Update()
    {
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            debugTransform.position = raycastHit.point;
            mouseWorldPosition = raycastHit.point;
        }

        if (starterAssetsInputs.aim)
        {
            aimVirtualCamera.gameObject.SetActive(true);
            SetAimingMode();
        }
        else if (Time.time - mostRecentShot < 0.2f)
        {
            SetAimingMode();
        }
        else if ((Time.time - mostRecentShot) > 0.2f)
        {
            SetIdleMode();
        }


        if (starterAssetsInputs.shoot && (Time.time - mostRecentShot) > 0.2f)
        {
            mostRecentShot = Time.time;

            if (!starterAssetsInputs.aim)
            {
                StartCoroutine(Shoot(Time.deltaTime * 10f));
            }
            else
            {
                StartCoroutine(Shoot(0));
            }

            starterAssetsInputs.shoot = false;
        }
    }

    private void SetAimingMode(bool instant = false)
    {
        thirdPersonController.SetRotationOnMove(false);

        Vector3 worldAimTarget = mouseWorldPosition;
        worldAimTarget.y = transform.position.y;
        Vector3 aimDirection = (worldAimTarget - transform.position).normalized;
        if (instant)
        {
            animator.SetLayerWeight(1, 1f);
            transform.forward = aimDirection;
        }
        else
        {
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1f, Time.deltaTime * 10f));
            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
        }

        foreach (GameObject aimConstraint in aimConstraints)
        {
            aimConstraint.GetComponent<MultiAimConstraint>().weight = 1f;
        }
        headConstraint.GetComponent<MultiAimConstraint>().weight = 0f;
    }

    private void SetIdleMode()
    {
        aimVirtualCamera.gameObject.SetActive(false);
        thirdPersonController.SetRotationOnMove(true);
        animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0f, Time.deltaTime * 10f));

        foreach (GameObject aimConstraint in aimConstraints)
        {
            aimConstraint.GetComponent<MultiAimConstraint>().weight = 0f;
        }
        headConstraint.GetComponent<MultiAimConstraint>().weight = 1f;
    }

    IEnumerator Shoot(float secs)
    {
        yield return new WaitForSeconds(secs);
        Vector3 aimDir = (mouseWorldPosition - spawnBulletPosition.position).normalized;
        if(Physics.Raycast(spawnBulletPosition.position, aimDir, out RaycastHit hit, 999f))
        {
            Transform pfSplashClone = Instantiate(pfSplash, hit.point, Quaternion.LookRotation(hit.normal, Vector3.up));
            Destroy(pfSplashClone.gameObject, 2f);
        }
        //Instantiate(pfBulletProjectile, spawnBulletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
        Transform pfMuzzleFlashClone = Instantiate(pfMuzzleFlash, spawnBulletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
        Destroy(pfMuzzleFlashClone.gameObject, 2f);
    }
}
