using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{

    public float speed = 2f;
    public float height = 0.05f;
    // Start y position of the gameobject
    private float startY;




    // Start is called before the first frame update
    void Start()
    {
        startY = transform.position.y;
        
    }

    // Update is called once per frame
    void Update()
    {
        // var pos = transform.position;
        // var newY = startY + height*Mathf.Sin(Time.time * speed);
        // transform.position = new Vector3(pos.x, newY, pos.z);




    }

    // Fized update
    void FixedUpdate()
    {
        var pos = transform.localPosition;
        var newY = startY + height*Mathf.Sin(Time.time * speed);
        transform.localPosition = new Vector3(pos.x, newY, pos.z);
    }
}
