using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    public Light flickeringLight;
    public float minIntensity = 0.25f;
    public float maxIntensity = 0.5f;
    public float flickerRate = 0.1f;

    private float timeSinceFlicker;

    void Update()
    {
        timeSinceFlicker += Time.deltaTime;

        if (timeSinceFlicker >= flickerRate)
        {
            float newIntensity = Random.Range(minIntensity, maxIntensity);
            flickeringLight.intensity = newIntensity;
            timeSinceFlicker = 0;
        }
    }
}