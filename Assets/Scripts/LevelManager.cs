using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    // Start is called before the first frame update
    public int numberOfEnemies;
    public GameObject enemy1;
    public GameObject enemy2;
    void Start()
    {
        numberOfEnemies = 2;
    }

    // Update is called once per frame
    void Update()
    {
        if(numberOfEnemies == 0)
        {
            // Win Game

        }
        
    }
}
