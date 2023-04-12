using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderChecker : MonoBehaviour
{
    private Collider collider;
    
    public bool IsCollidingWithPlayer = false;
    void Awake()
    {
        collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") 
            IsCollidingWithPlayer = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player") 
            IsCollidingWithPlayer = false;
    }

}
