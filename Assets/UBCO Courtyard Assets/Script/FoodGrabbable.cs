using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
public class FoodGrabbable : MonoBehaviour
{
    [Header("Grab Settings")]
    [Tooltip("Should the object return to original scale when released?")]
    public bool maintainScale = true;
    
    [Tooltip("Add velocity when thrown")]
    public bool throwOnRelease = true;
    
    [Tooltip("Throw velocity multiplier")]
    public float throwVelocityScale = 1.5f;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private Rigidbody rb;
    private Vector3 originalScale;

    private void Awake()
    {
        SetupComponents();
    }

    private void SetupComponents()
    {
        // Get or add Rigidbody
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        
        // Configure Rigidbody
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        
        // Ensure there's a collider
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            // Try to find collider in children
            col = GetComponentInChildren<Collider>();
            
            if (col == null)
            {
                // Add a default box collider
                BoxCollider boxCol = gameObject.AddComponent<BoxCollider>();
                Debug.LogWarning($"No collider found on {gameObject.name}, added BoxCollider. You may want to adjust it.");
            }
        }
        
        // Get or add XRGrabInteractable
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grabInteractable == null)
        {
            grabInteractable = gameObject.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        }
        
        // Configure grab interactable
        grabInteractable.movementType = UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable.MovementType.Instantaneous;
        grabInteractable.throwOnDetach = throwOnRelease;
        grabInteractable.throwVelocityScale = throwVelocityScale;
        
        // Subscribe to events
        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);
        
        // Store original scale
        originalScale = transform.localScale;
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        Debug.Log($"{gameObject.name} grabbed!");
        
        // Optional: Add haptic feedback
        if (args.interactorObject is UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInputInteractor controllerInteractor)
        {
            // Trigger haptic pulse
            SendHapticFeedback(controllerInteractor, 0.3f, 0.1f);
        }
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        Debug.Log($"{gameObject.name} released!");
        
        // Restore original scale if needed
        if (maintainScale)
        {
            transform.localScale = originalScale;
        }
        
        // Optional: Add haptic feedback
        if (args.interactorObject is UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInputInteractor controllerInteractor)
        {
            SendHapticFeedback(controllerInteractor, 0.2f, 0.05f);
        }
    }

    private void SendHapticFeedback(UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInputInteractor interactor, float amplitude, float duration)
    {
        if (interactor.xrController != null)
        {
            interactor.xrController.SendHapticImpulse(amplitude, duration);
        }
    }

    /// <summary>
    /// Update the scale and maintain it as the new original scale
    /// </summary>
    public void UpdateScale(Vector3 newScale)
    {
        transform.localScale = newScale;
        originalScale = newScale;
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
            grabInteractable.selectExited.RemoveListener(OnReleased);
        }
    }
}

