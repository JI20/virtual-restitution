using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ContainerTransition : MonoBehaviour
{
    [Header("Objects to Control")]
    public GameObject leftDoor;
    public GameObject rightDoor;
    public GameObject hiddenPackages;
    public Light containerLight;

    [Header("Door Rotations (Degrees)")]
    public Vector3 leftDoorOpenRot;
    public Vector3 leftDoorClosedRot;
    public Vector3 rightDoorOpenRot;
    public Vector3 rightDoorClosedRot;
    public float doorAnimSpeed = 1.5f;

    private bool sequenceStarted = false;

    // This runs the moment a collider touches our Trigger Zone
    private void OnTriggerEnter(Collider other)
    {
        // Check if the thing touching it is the Player
        if (other.CompareTag("Player") && !sequenceStarted)
        {
            sequenceStarted = true;
            StartCoroutine(RunShippingSequence());
        }
    }

    // This is a Coroutine, allowing us to pause time between actions
    private IEnumerator RunShippingSequence()
    {
        // 1. Slam the doors shut
        StartCoroutine(AnimateDoors(leftDoorClosedRot, rightDoorClosedRot));
        yield return new WaitForSeconds(doorAnimSpeed);

        // 2. Flicker lights and spawn packages
        yield return StartCoroutine(FlickerLight());

        // --- We will add the Shaking, Audio, and Clock code here later! ---
    }

    private IEnumerator AnimateDoors(Vector3 leftTarget, Vector3 rightTarget)
    {
        Quaternion leftStart = leftDoor.transform.localRotation;
        Quaternion rightStart = rightDoor.transform.localRotation;
        Quaternion leftEnd = Quaternion.Euler(leftTarget);
        Quaternion rightEnd = Quaternion.Euler(rightTarget);

        float timeElapsed = 0f;

        while (timeElapsed < doorAnimSpeed)
        {
            float t = timeElapsed / doorAnimSpeed;
            t = t * t * (3f - 2f * t); // Smooth easing

            leftDoor.transform.localRotation = Quaternion.Slerp(leftStart, leftEnd, t);
            rightDoor.transform.localRotation = Quaternion.Slerp(rightStart, rightEnd, t);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        leftDoor.transform.localRotation = leftEnd;
        rightDoor.transform.localRotation = rightEnd;
    }

    private IEnumerator FlickerLight()
    {
        containerLight.enabled = false;
        yield return new WaitForSeconds(0.1f);
        containerLight.enabled = true;
        yield return new WaitForSeconds(0.05f);
        containerLight.enabled = false;

        // Spawn the packages while it is completely dark!
        hiddenPackages.SetActive(true);

        yield return new WaitForSeconds(0.4f);
        containerLight.enabled = true;
    }
}