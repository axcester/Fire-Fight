using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using StarterAssets;

public class MainMenu : MonoBehaviour
{
    public GameObject player;

    public GameObject Menu;
    public GameObject Hud;
    // Start is called before the first frame update
    void Start()
    {
        player.GetComponent<PlayerInput>().DeactivateInput();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        //player.enabled = false;
        //playerShooter.enabled = false;
        //input.cursorLocked = false;
        //input.cursorInputForLook = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        print("Start");
        player.GetComponent<PlayerInput>().ActivateInput();
        Menu.SetActive(false);
        Hud.SetActive(true);

        Cursor.visible = false;
    }
}
