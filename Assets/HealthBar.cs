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
        print(playerHealth);
        for (int i = 0; i < healthpoints.Length; i++)
        {
            healthpoints[i].gameObject.SetActive(i < playerHealth);
        }
    }
}
