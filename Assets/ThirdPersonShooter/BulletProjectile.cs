using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    private Rigidbody bulletRigidbody;
    [SerializeField] private Transform pfSplash;

    private void Awake()
    {
        bulletRigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        float speed = 50f;
        bulletRigidbody.velocity = transform.forward * speed;
        Destroy(gameObject, 2f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Gun") && !other.CompareTag("Player") && !other.CompareTag("Bullet"))
        {
            Vector3 point = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
            Transform pfSplashClone = Instantiate(pfSplash, point, Quaternion.LookRotation(transform.position - point, Vector3.up));
            Destroy(pfSplashClone.gameObject, 2f);
            Destroy(gameObject);
        }
    }
}
