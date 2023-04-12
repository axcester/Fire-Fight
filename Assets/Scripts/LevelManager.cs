using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    // Start is called before the first frame update
    public int numberOfEnemies;
    public EnemyController enemy1;
    public EnemyController enemy2;
    public Transform endPoint;
    public Transform Player;
    void Start()
    {
        numberOfEnemies = 2;
    }

    // Update is called once per frame
    void Update()
    {
        if(numberOfEnemies == 0)
        {
            // Check if Player position is inside enPoint

        }
        
        if(enemy1.health == 0)
        {
            numberOfEnemies -= 1;
        }
        if(enemy2.health == 0)
        {
            numberOfEnemies -= 1;
        }

        
    }
}
