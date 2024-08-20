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
                Debug.Log("Current index: " + j + " Device: " + inputDevice.displayName + " Player: " + playerNumber + " ToggleGroup: " + toggleGroup.name);
                var index = j;
                toggle.GetComponent<Toggle>().onValueChanged.AddListener((value) => SelectDevice(index, playerNumber));
                toggle.SetActive(true);
                toggle.transform.localPosition = new Vector2(0, -30 * j);
                if (SelectedDevices[playerNumber - 1] == inputDevice)
                {
                    toggle.GetComponent<Toggle>().isOn = true;
                    Debug.Log("Setting toggle for device: " + inputDevice.displayName + " to true for player: " + playerNumber + "as it was previously selected");
                }
            }
        }
    }
    
    // public void OnDropdownValueChanged(int playerNumber)
    // {
    //     // Check if playerNumber is a valid index for playerDropdowns
    //     if (playerNumber <= 0 || playerNumber > playerDropdowns.Length)
    //     {
    //         Debug.Log("Invalid player number: " + playerNumber);
    //         return;
    //     }
    //
    //     int index = playerDropdowns[playerNumber - 1].value;
    //     SelectDevice(index, playerNumber);
    //     UpdateDropdownOptions(playerNumber);
    // }
    //
    private static string GetNumbers(string input)
    {
        return new string(input.Where(c => char.IsDigit(c)).ToArray());
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
