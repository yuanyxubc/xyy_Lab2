using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FoodMenuController : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("The dropdown menu for selecting food type")]
    public TMP_Dropdown foodDropdown;
    
    [Tooltip("The order button")]
    public Button orderButton;
    
    [Tooltip("The scale slider")]
    public Slider scaleSlider;
    
    [Tooltip("The boom toggle")]
    public Toggle boomToggle;

    [Header("Food Prefabs")]
    [Tooltip("Burger prefab")]
    public GameObject burgerPrefab;
    
    [Tooltip("Pizza prefab")]
    public GameObject pizzaPrefab;
    
    [Tooltip("Cola prefab")]
    public GameObject colaPrefab;

    [Header("Spawn Settings")]
    [Tooltip("Position where food will spawn")]
    public Transform spawnPoint;
    
    [Tooltip("Offset between spawned items")]
    public Vector3 spawnOffset = new Vector3(0.2f, 0, 0);

    [Header("Particle Effects")]
    [Tooltip("Particle effect when boom is activated (food disappears)")]
    public GameObject boomActivateParticle;
    
    [Tooltip("Particle effect when boom is deactivated (food appears)")]
    public GameObject boomDeactivateParticle;

    [Header("Scale Settings")]
    [Tooltip("Minimum scale value")]
    public float minScale = 0.5f;
    
    [Tooltip("Maximum scale value")]
    public float maxScale = 3.0f;

    // List to track all spawned food objects
    private List<GameObject> spawnedFoodList = new List<GameObject>();
    
    // Counter for positioning spawned items
    private int spawnCounter = 0;

    private void Start()
    {
        // Setup dropdown options
        SetupDropdown();
        
        // Add listeners
        if (orderButton != null)
        {
            orderButton.onClick.AddListener(OnOrderButtonClicked);
        }
        
        if (scaleSlider != null)
        {
            scaleSlider.onValueChanged.AddListener(OnScaleChanged);
            scaleSlider.minValue = 0f;
            scaleSlider.maxValue = 1f;
            scaleSlider.value = 0.5f; // Default to middle
        }
        
        if (boomToggle != null)
        {
            boomToggle.onValueChanged.AddListener(OnBoomToggled);
        }
    }

    private void SetupDropdown()
    {
        if (foodDropdown != null)
        {
            foodDropdown.ClearOptions();
            
            List<string> options = new List<string>
            {
                "Burger",
                "Pizza",
                "Cola"
            };
            
            foodDropdown.AddOptions(options);
            foodDropdown.value = 0;
        }
    }

    private void OnOrderButtonClicked()
    {
        if (foodDropdown == null) return;
        
        GameObject prefabToSpawn = null;
        
        // Determine which prefab to spawn based on dropdown selection
        switch (foodDropdown.value)
        {
            case 0: // Burger
                prefabToSpawn = burgerPrefab;
                Debug.Log("Ordering Burger");
                break;
            case 1: // Pizza
                prefabToSpawn = pizzaPrefab;
                Debug.Log("Ordering Pizza");
                break;
            case 2: // Cola
                prefabToSpawn = colaPrefab;
                Debug.Log("Ordering Cola");
                break;
        }
        
        if (prefabToSpawn != null)
        {
            SpawnFood(prefabToSpawn);
        }
        else
        {
            Debug.LogWarning("Food prefab not assigned!");
        }
    }

    private void SpawnFood(GameObject prefab)
    {
        Vector3 spawnPosition = spawnPoint != null 
            ? spawnPoint.position + (spawnOffset * spawnCounter)
            : transform.position + (spawnOffset * spawnCounter);
        
        Quaternion spawnRotation = spawnPoint != null 
            ? spawnPoint.rotation 
            : Quaternion.identity;
        
        GameObject spawnedFood = Instantiate(prefab, spawnPosition, spawnRotation);
        
        // Ensure the food has a FoodGrabbable component for VR interaction
        FoodGrabbable grabbable = spawnedFood.GetComponent<FoodGrabbable>();
        if (grabbable == null)
        {
            grabbable = spawnedFood.AddComponent<FoodGrabbable>();
        }
        
        // Apply current scale
        float currentScale = Mathf.Lerp(minScale, maxScale, scaleSlider.value);
        grabbable.UpdateScale(Vector3.one * currentScale);
        
        // Add to list
        spawnedFoodList.Add(spawnedFood);
        
        // Hide if boom is active
        if (boomToggle != null && boomToggle.isOn)
        {
            spawnedFood.SetActive(false);
        }
        
        spawnCounter++;
        
        Debug.Log($"Spawned food. Total count: {spawnedFoodList.Count}");
    }

    private void OnScaleChanged(float value)
    {
        // Map slider value (0-1) to scale range (minScale-maxScale)
        float targetScale = Mathf.Lerp(minScale, maxScale, value);
        
        // Apply scale to all spawned food objects
        foreach (GameObject food in spawnedFoodList)
        {
            if (food != null)
            {
                FoodGrabbable grabbable = food.GetComponent<FoodGrabbable>();
                if (grabbable != null)
                {
                    grabbable.UpdateScale(Vector3.one * targetScale);
                }
                else
                {
                    food.transform.localScale = Vector3.one * targetScale;
                }
            }
        }
        
        Debug.Log($"Scale changed to: {targetScale:F2}");
    }

    private void OnBoomToggled(bool isActive)
    {
        if (isActive)
        {
            // Boom activated - hide all food
            Debug.Log("BOOM! Food disappearing...");
            
            // Play activate particle effect
            if (boomActivateParticle != null)
            {
                PlayParticleEffect(boomActivateParticle);
            }
            
            // Hide all food
            foreach (GameObject food in spawnedFoodList)
            {
                if (food != null)
                {
                    food.SetActive(false);
                }
            }
        }
        else
        {
            // Boom deactivated - show all food
            Debug.Log("Boom off! Food reappearing...");
            
            // Play deactivate particle effect
            if (boomDeactivateParticle != null)
            {
                PlayParticleEffect(boomDeactivateParticle);
            }
            
            // Show all food
            foreach (GameObject food in spawnedFoodList)
            {
                if (food != null)
                {
                    food.SetActive(true);
                }
            }
        }
    }

    private void PlayParticleEffect(GameObject particlePrefab)
    {
        if (particlePrefab == null) return;
        
        Vector3 effectPosition = spawnPoint != null 
            ? spawnPoint.position 
            : transform.position;
        
        GameObject effect = Instantiate(particlePrefab, effectPosition, Quaternion.identity);
        
        // Auto-destroy after particle finishes
        ParticleSystem ps = effect.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            Destroy(effect, ps.main.duration + ps.main.startLifetime.constantMax);
        }
        else
        {
            Destroy(effect, 5f); // Default 5 seconds
        }
    }

    /// <summary>
    /// Clear all spawned food objects
    /// </summary>
    public void ClearAllFood()
    {
        foreach (GameObject food in spawnedFoodList)
        {
            if (food != null)
            {
                Destroy(food);
            }
        }
        
        spawnedFoodList.Clear();
        spawnCounter = 0;
        
        Debug.Log("All food cleared");
    }

    private void OnDestroy()
    {
        // Remove listeners
        if (orderButton != null)
        {
            orderButton.onClick.RemoveListener(OnOrderButtonClicked);
        }
        
        if (scaleSlider != null)
        {
            scaleSlider.onValueChanged.RemoveListener(OnScaleChanged);
        }
        
        if (boomToggle != null)
        {
            boomToggle.onValueChanged.RemoveListener(OnBoomToggled);
        }
    }
}

