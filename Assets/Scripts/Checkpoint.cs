using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Checkpoint : MonoBehaviour
{
    // Start is called before the first frame update
    public EnemyController enemy1;
    public EnemyController enemy2;
    public bool enemy1Dead;
    public bool enemy2Dead;
    public int numberOfEnemies;
    void Start()
    {
        numberOfEnemies = 2;  
        enemy1Dead = false;
        enemy2Dead = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (enemy1.health <= 0 )
        {
            enemy1Dead= true;
        }
        if (enemy2.health <= 0)
        {
           enemy2Dead= true;
        }


    }
    private void OnTriggerEnter(Collider other)
    {
        Console.WriteLine(other.tag);
        if (other.CompareTag("Player") && enemy1Dead && enemy2Dead){
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

    }
}
