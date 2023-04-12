using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulse : MonoBehaviour
{
    public float min, max;
    public float speed;
    public float intencity;
    private Color emission;
    private Material material;
    private MeshRenderer meshRenderer;
    // Start is called before the first frame update
    void Start()
    {
        intencity = min;
        meshRenderer = gameObject.GetComponent<MeshRenderer>();
        material = meshRenderer.material;
        emission = material.GetColor("_EmissionColor");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        intencity = intencity + speed * Time.deltaTime;

        if (intencity >= max) speed = -speed;
        else if (intencity <= min) speed = -speed;

        material.SetColor("_EmissionColor", emission * intencity);

        meshRenderer.material = material;
    }
}
