using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ContainerTransition : MonoBehaviour
{
    [Header("Objects to Control")]
    public GameObject leftDoor; // Only one door now!
    public GameObject hiddenPackages;
    public Light containerLight; // It's okay if this is empty/null
    
    [Header("Door Settings")]
    public Vector3 leftDoorOpenRot;
    public Vector3 leftDoorClosedRot;
    public float doorAnimSpeed = 1.5f;

    [Header("Sequence Timing")]
    public float totalSequenceTime = 60f; // 1 minute inside the container

    private bool sequenceStarted = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MainCamera") && !sequenceStarted)
        {
            sequenceStarted = true;
            StartCoroutine(RunShippingSequence());
        }
    }

    private IEnumerator RunShippingSequence()
    {
        // 1. Slam the door shut
        yield return StartCoroutine(AnimateDoor(leftDoorClosedRot));

        // 2. Flicker lights (if you add one later) and spawn packages
        yield return StartCoroutine(FlickerLight());
        
        // --- We will add the Shaking, Audio, and Clock code here next! ---

        // 3. Wait for the duration of the ride (1 minute)
        yield return new WaitForSeconds(totalSequenceTime);

        // 4. Open the door again to reveal the new room!
        yield return StartCoroutine(AnimateDoor(leftDoorOpenRot));
        
        // Reset
        sequenceStarted = false; // Unlock the trigger
        if (hiddenPackages != null) hiddenPackages.SetActive(false); // Hide the boxes again
    }

    private IEnumerator AnimateDoor(Vector3 targetRotation)
    {
        Quaternion startRot = leftDoor.transform.localRotation;
        Quaternion endRot = Quaternion.Euler(targetRotation);
        float timeElapsed = 0f;

        while (timeElapsed < doorAnimSpeed)
        {
            float t = timeElapsed / doorAnimSpeed;
            t = t * t * (3f - 2f * t); // Smooth easing so it doesn't look robotic

            leftDoor.transform.localRotation = Quaternion.Slerp(startRot, endRot, t);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        leftDoor.transform.localRotation = endRot; // Snap precisely to the final angle
    }

    private IEnumerator FlickerLight()
    {
        // The "!= null" checks prevent the game from crashing if you haven't assigned a light yet
        if (containerLight != null) containerLight.enabled = false;
        yield return new WaitForSeconds(0.1f);
        
        if (containerLight != null) containerLight.enabled = true;
        yield return new WaitForSeconds(0.05f);
        
        if (containerLight != null) containerLight.enabled = false;
        
        // Spawn the packages while it is completely dark!
        if (hiddenPackages != null) hiddenPackages.SetActive(true);
        
        yield return new WaitForSeconds(0.4f);
        if (containerLight != null) containerLight.enabled = true;
    }
}