using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.Animations.Rigging;

public class ThirdPersonShooterController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform debugTransform;
    [SerializeField] private Transform pfBulletProjectile;
    [SerializeField] private Transform spawnBulletPosition;
    [SerializeField] private Transform pfMuzzleFlash;
    [SerializeField] private List<GameObject> aimConstraints = new List<GameObject>();
    [SerializeField] private GameObject headConstraint;
    [SerializeField] private List<float> aimConstraintsWeights = new List<float>();

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
        else
        {
            debugTransform.position = mainCamera.transform.position + mainCamera.transform.forward * 200f;
            mouseWorldPosition = mainCamera.transform.position + mainCamera.transform.forward * 200f;
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


        if (starterAssetsInputs.shoot)
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
            animator.SetLayerWeight(2, 1f);
            transform.forward = aimDirection;
        }
        else
        {
            animator.SetLayerWeight(2, Mathf.Lerp(animator.GetLayerWeight(2), 1f, Time.deltaTime * 10f));
            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
        }

        int indexCounter = 0;
        foreach (GameObject aimConstraint in aimConstraints)
        {
            aimConstraint.GetComponent<MultiAimConstraint>().weight = Mathf.Lerp(aimConstraint.GetComponent<MultiAimConstraint>().weight, aimConstraintsWeights[indexCounter], Time.deltaTime * 10f);
            indexCounter += 1;
        }
        //headConstraint.GetComponent<MultiAimConstraint>().weight = 0f;
    }

    private void SetIdleMode()
    {
        aimVirtualCamera.gameObject.SetActive(false);
        thirdPersonController.SetRotationOnMove(true);
        animator.SetLayerWeight(2, Mathf.Lerp(animator.GetLayerWeight(2), 0f, Time.deltaTime * 10f));

        foreach (GameObject aimConstraint in aimConstraints)
        {
            aimConstraint.GetComponent<MultiAimConstraint>().weight = Mathf.Lerp(aimConstraint.GetComponent<MultiAimConstraint>().weight, 0f, Time.deltaTime * 10f);
        }
        //headConstraint.GetComponent<MultiAimConstraint>().weight = 1f;
    }

    IEnumerator Shoot(float secs)
    {
        yield return new WaitForSeconds(secs);
        Vector3 aimDir = (mouseWorldPosition - spawnBulletPosition.position).normalized;
        Instantiate(pfBulletProjectile, spawnBulletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
        StartCoroutine(MuzzleFlash());
    }

    IEnumerator MuzzleFlash()
    {
        Transform pfMuzzleFlashClone = Instantiate(pfMuzzleFlash, spawnBulletPosition.position, Quaternion.identity);
        Destroy(pfMuzzleFlashClone.gameObject, 2f);
        yield return new WaitForSeconds(0.1f);
        pfMuzzleFlashClone.gameObject.GetComponent<Light>().enabled = false;
    }
}
