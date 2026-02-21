using UnityEngine;

public class Drawbridge : MonoBehaviour
{
    public GameObject bridgePart; // Das Teil aus Blender
    public Vector3 fallRotation = new Vector3(0, 90, 0);
    public float acceleration = 50.0f; // Wie schnell die Brücke an Fahrt gewinnt
    private float currentSpeed = 0.0f;

    private bool isFalling = false;
    private Quaternion targetRot;

    void Start()
    {
        if (bridgePart != null)
        {
            targetRot = Quaternion.Euler(fallRotation) * bridgePart.transform.localRotation;
        }
    }

    void Update()
{
    if (isFalling && bridgePart != null)
    {
        // Die Geschwindigkeit nimmt über Zeit zu
        currentSpeed += acceleration * Time.deltaTime;

        bridgePart.transform.localRotation = Quaternion.RotateTowards(
            bridgePart.transform.localRotation, 
            targetRot, 
            currentSpeed * Time.deltaTime
        );
    }
}

    // Diese Funktion wird aufgerufen, sobald IRGENDWAS den Cube berührt
    private void OnTriggerEnter(Collider other)
    {
        isFalling = true;
    }
}