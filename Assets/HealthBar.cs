using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public Transform[] healthpoints;
    public ThirdPersonShooterController player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<ThirdPersonShooterController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        int playerHealth = player.GetHealth();
        for (int i = 0; i < healthpoints.Length; i++)
        {
            if (i >= playerHealth)
            {
                healthpoints[i].gameObject.GetComponentInChildren<Pulse>().intencity = 0f;
                healthpoints[i].gameObject.GetComponentInChildren<Pulse>().speed = 0f;
            }
        }
    }
}
