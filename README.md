# LAB 2 – VR Interaction with Food Menu 🍔

This is my **LAB 2 assignment**.  
I designed and implemented a **VR spatial interaction system** in Unity, featuring a floating food menu, interactable food items, and dynamic scaling effects.  
This lab demonstrates how **UI, input actions, and XR grab interactions** can be combined to create an intuitive and immersive VR experience.

**YouTube link:**[https://youtube.com/shorts/azXF4TatzTE]

---

## 🎮 Controls

| Button | Function |
|--------|-----------|
| **B Button** (Right Hand) | Toggle the spatial menu on/off |
| **Grip Button** | Grab or release objects |
| **Trigger** | Interact with UI and make selections |
| **Joystick** | Move and navigate around the space |

---

## 📚 Script Documentation

### **TriggerMenu.cs**

Controls the visibility of the spatial menu.

**Main Features:**
- Listens for **B button** input  
- Toggles the activation state of the **Spatial Panel**  
- Provides public methods for show/hide control  

**Configurable Parameters:**
- `spatialPanel` — the target GameObject for the spatial panel  
- `bButtonAction` — input action reference for the B button  

**Why it works:**  
By using `InputActionReference` to listen for controller button events, the script enables smooth UI toggling in real time, improving the overall interaction flow.

---

### **FoodMenuController.cs**

Core controller for the food generation and menu system.

**Main Features:**
- Dropdown menu management (**Burger / Pizza / Cola**)  
- Handles **Order** button click events  
- **Scale slider** for real-time object scaling  
- **Boom toggle** for particle effect activation  
- Automatically assigns VR interaction components to spawned items  

**Why it works:**  
By dynamically instantiating food prefabs and attaching XR interaction components, this script creates a seamless and interactive ordering process within the VR environment.

---

### **FoodGrabbable.cs**

Adds VR grab functionality to spawned food objects.

**Main Features:**
- Automatically configures **Rigidbody** and **Collider**  
- Manages **XRGrabInteractable** component  
- Handles grab and release events  
- Provides **haptic feedback** for hand controllers  
- Maintains and updates object scale consistency  

**Adjustable Parameters:**
- `maintainScale` — keeps the original scale after release  
- `throwOnRelease` — enables realistic throw behavior  
- `throwVelocityScale` — adjusts throw velocity multiplier  

**Why it works:**  
By leveraging the XR Interaction Toolkit’s grab events and Unity’s physics system, the script provides realistic grab-and-throw interactions with tactile feedback for a more immersive VR experience.
