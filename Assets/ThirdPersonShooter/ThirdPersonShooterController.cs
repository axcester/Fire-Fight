using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.Animations.Rigging;
using UnityEngine.SceneManagement;

public class ThirdPersonShooterController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform debugTransform;
    [SerializeField] private Transform pfBulletProjectile;
    [SerializeField] private Transform pfSplash;
    [SerializeField] private Transform spawnBulletPosition;
    [SerializeField] private Transform pfMuzzleFlash;
    [SerializeField] private List<GameObject> aimConstraints = new List<GameObject>();
    [SerializeField] private GameObject headConstraint;
    [SerializeField] private List<float> aimConstraintsWeights = new List<float>();
    [SerializeField] private LayerMask mask;


    [SerializeField] private int MaxHealth = 5;
    [SerializeField] private int Health;

    private CharacterController controller;
    private ThirdPersonController thirdPersonController;
    private StarterAssetsInputs starterAssetsInputs;
    private Animator animator;
    private Vector3 mouseWorldPosition = Vector3.zero;
    private float mostRecentShot;
    private bool dead = false;

    private void Awake()
    {
        thirdPersonController = GetComponent<ThirdPersonController>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
        animator = GetComponent<Animator>();
        Health = MaxHealth;
        controller = GetComponent<CharacterController>();
        animator.SetLayerWeight(3, 0f);
        SetIdleMode();
    }

    private void Update()
    {
        if (!dead)
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
        

        if (Health <= 0)
        {
            animator.SetLayerWeight(4, Mathf.Lerp(animator.GetLayerWeight(4), 1f, Time.deltaTime * 2f));
            controller.height = 1f;
            headConstraint.GetComponent<MultiAimConstraint>().weight = 0f;
            dead = true;
            thirdPersonController.enabled = false;

            StartCoroutine(Reload());
        }

        else
        {
            animator.SetLayerWeight(3, 0f);
            controller.height = 1.8f;
            headConstraint.GetComponent<MultiAimConstraint>().weight = 1f;
            dead = false;
            thirdPersonController.enabled = true;
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
            animator.SetLayerWeight(3, 1f);
            transform.forward = aimDirection;
        }
        else
        {
            animator.SetLayerWeight(2, Mathf.Lerp(animator.GetLayerWeight(2), 1f, Time.deltaTime * 10f));
            animator.SetLayerWeight(3, Mathf.Lerp(animator.GetLayerWeight(3), 1f, Time.deltaTime * 10f));
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
        animator.SetLayerWeight(3, Mathf.Lerp(animator.GetLayerWeight(3), 0f, Time.deltaTime * 10f));

        foreach (GameObject aimConstraint in aimConstraints)
        {
            aimConstraint.GetComponent<MultiAimConstraint>().weight = Mathf.Lerp(aimConstraint.GetComponent<MultiAimConstraint>().weight, 0f, Time.deltaTime * 10f);
        }
        //headConstraint.GetComponent<MultiAimConstraint>().weight = 1f;
    }

    public int GetHealth()
    {
        return Health;
    }

    public void Damage(int d)
    {
        Health -= d;
    }

    IEnumerator Shoot(float secs)
    {
        yield return new WaitForSeconds(secs);
        Vector3 aimDir = (mouseWorldPosition - spawnBulletPosition.position).normalized;
        if(Physics.Raycast(spawnBulletPosition.position, aimDir, out RaycastHit hit, 999f, mask))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                hit.collider.GetComponent<EnemyController>().Damage();
            }
            Transform pfSplashClone = Instantiate(pfSplash, hit.point, Quaternion.LookRotation(hit.normal, Vector3.up));
            Destroy(pfSplashClone.gameObject, 2f);
        }
        //Instantiate(pfBulletProjectile, spawnBulletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up));
        Transform pfMuzzleFlashClone = Instantiate(pfMuzzleFlash, spawnBulletPosition.position, Quaternion.LookRotation(aimDir, Vector3.up), transform);
        Destroy(pfMuzzleFlashClone.gameObject, 2f);
    }

    IEnumerator Reload()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
