using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.InputSystem.DualShock;

[ExecuteAlways]

public class LocalMultiplayerControllerSelection : MonoBehaviour
{
    public static InputDevice[] AvailableInputDevices;
    public TMP_Dropdown[] playerDropdowns;
    public static InputDevice[] SelectedDevices;
    
    public void SelectDevice(int index, int playerNumber)
    {
        if (index >= 0 && index < AvailableInputDevices.Length)
        {
            SelectedDevices[playerNumber-1] = AvailableInputDevices[index];
            Debug.Log("Player " + playerNumber + " selected device: " + AvailableInputDevices[index].displayName);
        }
    }
    
    private void OnEnable()
    {
        InputSystem.onDeviceChange += OnDeviceChange;
        UpdateInputDeviceList(); // Initialize the list on enable
        SelectedDevices = new InputDevice[playerDropdowns.Length];
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
        UpdateDropdownOptions();
    }
    
    private void UpdateDropdownOptions()
    {
        for (int i = 0; i < playerDropdowns.Length; i++)
        {
            var dropdown = playerDropdowns[i];
            dropdown.ClearOptions();
            List<string> options = new List<string>();
            foreach (var device in AvailableInputDevices)
            {
                if (device is Keyboard)
                {
                    options.Add("Klawiatura i mysz");
                }
                else if (device is Gamepad || device is Joystick)
                {
                    options.Add(device.displayName);
                }
            }
            dropdown.AddOptions(options);
            dropdown.onValueChanged.AddListener((value) => OnDropdownValueChanged(int.Parse(GetNumbers(dropdown.name))));
        }
    }
    
    public void OnDropdownValueChanged(int playerNumber)
    {
        // Check if playerNumber is a valid index for playerDropdowns
        if (playerNumber <= 0 || playerNumber > playerDropdowns.Length)
        {
            Debug.Log("Invalid player number: " + playerNumber);
            return;
        }

        int index = playerDropdowns[playerNumber - 1].value;
        SelectDevice(index, playerNumber);
    }
    
    private static string GetNumbers(string input)
    {
        return new string(input.Where(c => char.IsDigit(c)).ToArray());
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
