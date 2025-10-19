using UnityEngine;
using UnityEditor;


public class FoodPrefabSetup : EditorWindow
{
    [MenuItem("Tools/Setup Food Prefabs for VR Grab")]
    public static void ShowWindow()
    {
        GetWindow<FoodPrefabSetup>("Food Prefab Setup");
    }

    private void OnGUI()
    {
        GUILayout.Label("Food Prefab VR Setup", EditorStyles.boldLabel);
        GUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "This tool will add required components to selected food prefabs:\n" +
            "- Rigidbody\n" +
            "- Collider (if missing)\n" +
            "- XRGrabInteractable\n" +
            "- FoodGrabbable script", 
            MessageType.Info
        );

        GUILayout.Space(10);

        if (GUILayout.Button("Setup Selected Prefabs", GUILayout.Height(40)))
        {
            SetupSelectedPrefabs();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Setup Selected GameObjects in Scene", GUILayout.Height(40)))
        {
            SetupSelectedSceneObjects();
        }
    }

    private void SetupSelectedPrefabs()
    {
        GameObject[] selectedObjects = Selection.gameObjects;
        
        if (selectedObjects.Length == 0)
        {
            EditorUtility.DisplayDialog("No Selection", "Please select prefabs in the Project window first.", "OK");
            return;
        }

        int successCount = 0;

        foreach (GameObject obj in selectedObjects)
        {
            string assetPath = AssetDatabase.GetAssetPath(obj);
            
            if (string.IsNullOrEmpty(assetPath))
            {
                Debug.LogWarning($"{obj.name} is not a prefab asset. Skipping...");
                continue;
            }

            // Load prefab
            GameObject prefabRoot = PrefabUtility.LoadPrefabContents(assetPath);
            
            if (prefabRoot != null)
            {
                SetupFoodObject(prefabRoot);
                
                // Save prefab
                PrefabUtility.SaveAsPrefabAsset(prefabRoot, assetPath);
                PrefabUtility.UnloadPrefabContents(prefabRoot);
                
                successCount++;
                Debug.Log($"Setup completed for prefab: {obj.name}");
            }
        }

        EditorUtility.DisplayDialog("Setup Complete", 
            $"Successfully set up {successCount} prefab(s) for VR grabbing.", "OK");
        
        AssetDatabase.Refresh();
    }

    private void SetupSelectedSceneObjects()
    {
        GameObject[] selectedObjects = Selection.gameObjects;
        
        if (selectedObjects.Length == 0)
        {
            EditorUtility.DisplayDialog("No Selection", "Please select GameObjects in the Scene first.", "OK");
            return;
        }

        int successCount = 0;

        foreach (GameObject obj in selectedObjects)
        {
            SetupFoodObject(obj);
            successCount++;
            Debug.Log($"Setup completed for scene object: {obj.name}");
        }

        EditorUtility.DisplayDialog("Setup Complete", 
            $"Successfully set up {successCount} scene object(s) for VR grabbing.", "OK");
    }

    private static void SetupFoodObject(GameObject obj)
    {
        // Add or configure Rigidbody
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = obj.AddComponent<Rigidbody>();
        }
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.mass = 0.5f;

        // Ensure there's a collider
        Collider col = obj.GetComponent<Collider>();
        if (col == null)
        {
            col = obj.GetComponentInChildren<Collider>();
        }
        
        if (col == null)
        {
            // Try to add appropriate collider based on mesh
            MeshFilter meshFilter = obj.GetComponent<MeshFilter>();
            if (meshFilter != null && meshFilter.sharedMesh != null)
            {
                MeshCollider meshCol = obj.AddComponent<MeshCollider>();
                meshCol.convex = true; // Required for Rigidbody interaction
                Debug.Log($"Added MeshCollider to {obj.name}");
            }
            else
            {
                // Fallback to box collider
                obj.AddComponent<BoxCollider>();
                Debug.Log($"Added BoxCollider to {obj.name}");
            }
        }
        else
        {
            // If it's a mesh collider, ensure it's convex
            MeshCollider meshCol = col as MeshCollider;
            if (meshCol != null)
            {
                meshCol.convex = true;
            }
        }

        // Add XRGrabInteractable
        UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable = obj.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grabInteractable == null)
        {
            grabInteractable = obj.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        }
        
        grabInteractable.movementType = UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable.MovementType.Instantaneous;
        grabInteractable.throwOnDetach = true;
        grabInteractable.throwVelocityScale = 1.5f;

        // Add FoodGrabbable script
        FoodGrabbable foodGrabbable = obj.GetComponent<FoodGrabbable>();
        if (foodGrabbable == null)
        {
            obj.AddComponent<FoodGrabbable>();
        }

        // Mark as dirty for saving
        EditorUtility.SetDirty(obj);
    }

    [MenuItem("GameObject/Setup Food for VR Grab", false, 0)]
    private static void SetupSelectedFromContextMenu()
    {
        if (Selection.gameObjects.Length > 0)
        {
            foreach (GameObject obj in Selection.gameObjects)
            {
                SetupFoodObject(obj);
                Debug.Log($"Setup completed for: {obj.name}");
            }
            
            EditorUtility.DisplayDialog("Setup Complete", 
                $"Successfully set up {Selection.gameObjects.Length} object(s) for VR grabbing.", "OK");
        }
    }
}

