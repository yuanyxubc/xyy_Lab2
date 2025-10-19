using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class TriggerMenu : MonoBehaviour
{
    [Header("Panel Settings")]
    [Tooltip("The Spatial Panel to control show/hide")]
    public GameObject spatialPanel;

    [Header("Input Settings")]
    [Tooltip("Right controller Primary Button (B button)")]
    public InputActionProperty bButtonAction;

    private bool isMenuVisible = true;

    private void OnEnable()
    {
        // Enable input action and subscribe to event
        bButtonAction.action?.Enable();
        if (bButtonAction.action != null)
        {
            bButtonAction.action.performed += OnBButtonPressed;
        }
    }

    private void OnDisable()
    {
        // Unsubscribe from event
        if (bButtonAction.action != null)
        {
            bButtonAction.action.performed -= OnBButtonPressed;
        }
    }

    private void Start()
    {
        // Ensure the panel's initial state is correct
        if (spatialPanel != null)
        {
            spatialPanel.SetActive(isMenuVisible);
        }
        else
        {
            Debug.LogWarning("TriggerMenu: Spatial Panel not assigned! Please assign it in the Inspector.");
        }
    }

    private void OnBButtonPressed(InputAction.CallbackContext context)
    {
        ToggleMenu();
    }

    /// <summary>
    /// Toggle the menu visibility
    /// </summary>
    public void ToggleMenu()
    {
        if (spatialPanel != null)
        {
            isMenuVisible = !isMenuVisible;
            spatialPanel.SetActive(isMenuVisible);
            Debug.Log($"Spatial Panel {(isMenuVisible ? "shown" : "hidden")}");
        }
    }

    /// <summary>
    /// Show the menu
    /// </summary>
    public void ShowMenu()
    {
        if (spatialPanel != null)
        {
            isMenuVisible = true;
            spatialPanel.SetActive(true);
        }
    }

    /// <summary>
    /// Hide the menu
    /// </summary>
    public void HideMenu()
    {
        if (spatialPanel != null)
        {
            isMenuVisible = false;
            spatialPanel.SetActive(false);
        }
    }
}
