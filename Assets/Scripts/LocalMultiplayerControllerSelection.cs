using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;

[ExecuteAlways]

public class LocalMultiplayerControllerSelection : MonoBehaviour
{
    public static InputDevice[] AvailableInputDevices;
    public GameObject exampleToggle;
    public GameObject[] playerToggleGroups;
    public static InputDevice[] SelectedDevices;
    
    public void SelectDevice(int index, int playerNumber)
    {
        if (index >= 0 && index < AvailableInputDevices.Length)
        {
            SelectedDevices[playerNumber-1] = AvailableInputDevices[index];
            Debug.Log("Player " + playerNumber + " selected device: " + AvailableInputDevices[index].displayName);
        }
        else
        {
            Debug.Log("Invalid index: " + index);
        }
    }
    
    private void OnEnable()
    {
        InputSystem.onDeviceChange += OnDeviceChange;
        SelectedDevices = new InputDevice[playerToggleGroups.Length];
        UpdateInputDeviceList(); // Initialize the list on enable
    }

    private void OnDisable()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        UpdateInputDeviceList();
    }

    private void UpdateInputDeviceList()
    {
        var devices = new List<InputDevice>();
        foreach (var device in InputSystem.devices)
        {
            if (device is Gamepad || device is Keyboard || device is Joystick)
            {
                devices.Add(device);
                Debug.Log("Detected device: " + device.displayName + " Type: " + device.GetType().Name);
            }
        }
        AvailableInputDevices = devices.ToArray();
        UpdateToggleGroupOptions();
    }
    
    // Update the toggle group options
    private void UpdateToggleGroupOptions()
    {
        for (int i = 0; i < playerToggleGroups.Length; i++)
        {
            var toggleGroup = playerToggleGroups[i];
            var playerNumber = int.Parse(GetNumbers(toggleGroup.name));
            foreach (Transform child in toggleGroup.transform)
            {
                DestroyImmediate(child.gameObject);
            }
            for (int j = 0; j < AvailableInputDevices.Length; j++)
            {   
                //checking if there is already a toggle for this device
                var toggles = toggleGroup.GetComponentsInChildren<Toggle>();
                bool exists = false;
                foreach (var togggle in toggles)
                {
                    if (togggle.GetComponentInChildren<Text>().text == AvailableInputDevices[j].displayName)
                    {
                        Debug.Log("Toggle for device: " + AvailableInputDevices[j].displayName + " already exists");
                        exists = true;
                    }
                }
                if (exists)
                {
                    continue;
                }
                var inputDevice = AvailableInputDevices[j];
                var toggle = Instantiate(exampleToggle, toggleGroup.transform);
                toggle.GetComponent<Toggle>().group = toggleGroup.GetComponent<ToggleGroup>();
                toggle.GetComponentInChildren<Text>().text = inputDevice.displayName;
                var index = j;
                Debug.Log("Current index: " + j + " Device: " + inputDevice.displayName + " Player: " + playerNumber + " ToggleGroup: " + toggleGroup.name);
                toggle.SetActive(true);
                toggle.transform.localPosition = new Vector2(0, -30 * j);
                if (SelectedDevices[playerNumber - 1] == inputDevice)
                {
                    toggle.GetComponent<Toggle>().isOn = true;
                    Debug.Log("Setting toggle for device: " + inputDevice.displayName + " to true for player: " + playerNumber + "as it was previously selected");
                }
                else
                {
                    toggle.GetComponent<Toggle>().isOn = false;
                }
                toggle.GetComponent<Toggle>().onValueChanged.AddListener((value) => OnToggleValueChanged(index, playerNumber, value));
            }
        }
    }

    private void BlockTogglesUpdate(int playerNumber)
    {
        for (int i = 0; i < playerToggleGroups.Length; i++)
        {
            if (playerNumber-1 != i)
            {
                foreach (Transform child in playerToggleGroups[i].transform)
                {
                    if (SelectedDevices[playerNumber - 1] != null)
                    {
                        if (child.GetComponentInChildren<Text>().text == SelectedDevices[playerNumber - 1].displayName)
                        {
                            child.GetComponent<Toggle>().interactable = false;
                        }
                        else
                        {
                            child.GetComponent<Toggle>().interactable = true;
                        }
                    }
                    else
                    {
                        child.GetComponent<Toggle>().interactable = true;
                    }
                }
            }
        }
    }

    private void OnToggleValueChanged(int index, int playerNumber, bool isOn)
    {
        Debug.Log("The value is " + isOn + " for player " + playerNumber + " with device index of " + index);
        if (isOn == true)
        {
            SelectDevice(index, playerNumber);
        } else
        {
            SelectedDevices[playerNumber - 1] = null;
        }
        BlockTogglesUpdate(playerNumber);
    }
//     NullReferenceException: Object reference not set to an instance of an object
// LocalMultiplayerControllerSelection.BlockTogglesUpdate (System.Int32 playerNumber) (at Assets/Scripts/LocalMultiplayerControllerSelection.cs:122)
// LocalMultiplayerControllerSelection.OnToggleValueChanged (System.Int32 index, System.Int32 playerNumber, System.Boolean isOn) (at Assets/Scripts/LocalMultiplayerControllerSelection.cs:145)
// LocalMultiplayerControllerSelection+<>c__DisplayClass9_1.<UpdateToggleGroupOptions>b__0 (System.Boolean value) (at Assets/Scripts/LocalMultiplayerControllerSelection.cs:109)
// UnityEngine.Events.InvokableCall`1[T1].Invoke (T1 args0) (at <30adf90198bc4c4b83910c6fb1877998>:0)
// UnityEngine.Events.UnityEvent`1[T0].Invoke (T0 arg0) (at <30adf90198bc4c4b83910c6fb1877998>:0)
// UnityEngine.UI.Toggle.Set (System.Boolean value, System.Boolean sendCallback) (at ./Library/PackageCache/com.unity.ugui@1.0.0/Runtime/UI/Core/Toggle.cs:284)
// UnityEngine.UI.Toggle.set_isOn (System.Boolean value) (at ./Library/PackageCache/com.unity.ugui@1.0.0/Runtime/UI/Core/Toggle.cs:247)
// UnityEngine.UI.Toggle.InternalToggle () (at ./Library/PackageCache/com.unity.ugui@1.0.0/Runtime/UI/Core/Toggle.cs:317)
// UnityEngine.UI.Toggle.OnPointerClick (UnityEngine.EventSystems.PointerEventData eventData) (at ./Library/PackageCache/com.unity.ugui@1.0.0/Runtime/UI/Core/Toggle.cs:328)
// UnityEngine.EventSystems.ExecuteEvents.Execute (UnityEngine.EventSystems.IPointerClickHandler handler, UnityEngine.EventSystems.BaseEventData eventData) (at ./Library/PackageCache/com.unity.ugui@1.0.0/Runtime/EventSystem/ExecuteEvents.cs:57)
// UnityEngine.EventSystems.ExecuteEvents.Execute[T] (UnityEngine.GameObject target, UnityEngine.EventSystems.BaseEventData eventData, UnityEngine.EventSystems.ExecuteEvents+EventFunction`1[T1] functor) (at ./Library/PackageCache/com.unity.ugui@1.0.0/Runtime/EventSystem/ExecuteEvents.cs:272)
// UnityEngine.EventSystems.EventSystem:Update() (at ./Library/PackageCache/com.unity.ugui@1.0.0/Runtime/EventSystem/EventSystem.cs:530)
    private static string GetNumbers(string input)
    {
        return new string(input.Where(c => char.IsDigit(c)).ToArray());
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
