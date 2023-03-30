using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager1 : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform Player;
    public Transform Checkpoint;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // If the Player goes below -500 on the y-axis reload the scene
        if (Player.position.y <= -60)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if(Player.position == Checkpoint.position)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
