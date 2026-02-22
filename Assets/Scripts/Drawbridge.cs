using UnityEngine;

public class Drawbridge : MonoBehaviour
{
    public GameObject bridgePart; 
    public Vector3 fallRotation = new Vector3(0, 90, 0);
    public float acceleration = 50.0f; 
    
    [Header("Audio Einstellungen")]
    public AudioSource audioSource; // Der Lautsprecher am Objekt
    public AudioClip fallSound;     // Die Sounddatei

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
            currentSpeed += acceleration * Time.deltaTime;

            bridgePart.transform.localRotation = Quaternion.RotateTowards(
                bridgePart.transform.localRotation, 
                targetRot, 
                currentSpeed * Time.deltaTime
            );
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Nur abspielen, wenn sie nicht schon am Fallen ist
        if (!isFalling)
        {
            isFalling = true;

            // Sound abspielen
            if (audioSource != null && fallSound != null)
            {
                audioSource.PlayOneShot(fallSound);
            }
        }
    }
}