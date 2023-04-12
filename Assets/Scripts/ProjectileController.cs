using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ProjectileController : MonoBehaviour
{
    private AudioSource audio;
    private Rigidbody rigidbody;
    public AudioClip blast;

    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
        rigidbody = GetComponent<Rigidbody>();

        rigidbody.velocity = transform.forward * 8;

        audio.clip = blast;
        audio.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        ThirdPersonShooterController player;
        if (other.TryGetComponent<ThirdPersonShooterController>(out player))
        {
            player.Damage(1);
        }

        Destroy(this);
    }

}
