using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public float hoverAmplitude = 0.5f;
    public float hoverFrequency = 2f;
    public float bounceAmount = 0.2f;
    public float bounceSpeed = 5f;

    private float initialY;
    private bool playerOnPlatform;
    private Vector3 bounceTarget;

    void Start()
    {
        initialY = transform.position.y;
        bounceTarget = transform.position;
    }

    void Update()
    {
        if (playerOnPlatform)
        {
            transform.position = Vector3.Lerp(transform.position, bounceTarget, Time.deltaTime * bounceSpeed);
        }
        else
        {
            float yOffset = hoverAmplitude * Mathf.Sin(hoverFrequency * Time.time);
            transform.position = new Vector3(transform.position.x, initialY + yOffset, transform.position.z);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerOnPlatform = true;
            bounceTarget = new Vector3(transform.position.x, initialY - bounceAmount, transform.position.z);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerOnPlatform = false;
            bounceTarget = new Vector3(transform.position.x, initialY, transform.position.z);
        }
    }
}