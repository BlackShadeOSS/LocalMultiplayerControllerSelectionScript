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
        SelectedDevices = new InputDevice[playerDropdowns.Length];
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
        UpdateDropdownOptions();
    }
    
    private void UpdateDropdownOptions()
    {
        for (int i = 0; i < playerDropdowns.Length; i++)
        {
            var dropdown = playerDropdowns[i];
            var playerNumber = int.Parse(GetNumbers(dropdown.name));
            var PerPlayerDevices = new List<InputDevice>();
            foreach (var device in AvailableInputDevices)
            {
                PerPlayerDevices.Add(device);
            }
            for (var j = 0; j < SelectedDevices.Length; j++)
            {
                if (SelectedDevices[i] != null && PerPlayerDevices.Contains(SelectedDevices[i]) && j != playerNumber - 1)
                {
                    Debug.Log("Removing device: " + SelectedDevices[i].displayName + " from player " + (j + 1));
                    PerPlayerDevices.Remove(SelectedDevices[j]);
                }
            }
            dropdown.ClearOptions();
            List<string> options = new List<string>();
            options.Add("Wybierz urządzenie");
            foreach (var device in PerPlayerDevices)
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
            dropdown.onValueChanged.AddListener((value) => OnDropdownValueChanged(playerNumber));
        }
    }

    private void UpdateDropdownOptions(int playerNumberToSkip)
    {
        for (int i = 0; i < playerDropdowns.Length; i++)
        {
            if (i == playerNumberToSkip - 1)
            {
                Debug.Log("Skipping player " + playerNumberToSkip);
                continue;
            }
            var dropdown = playerDropdowns[i];
            var playerNumber = int.Parse(GetNumbers(dropdown.name));
            var PerPlayerDevices = new List<InputDevice>();
            foreach (var device in AvailableInputDevices)
            {
                PerPlayerDevices.Add(device);
            }
            for (var j = 0; j < SelectedDevices.Length; j++)
            {
                if (SelectedDevices[i] != null && PerPlayerDevices.Contains(SelectedDevices[i]) && j != playerNumber - 1)
                {
                    Debug.Log("Removing device: " + SelectedDevices[i].displayName + " from player " + (j + 1));
                    PerPlayerDevices.Remove(SelectedDevices[j]);
                }
            }
            dropdown.ClearOptions();
            List<string> options = new List<string>();
            options.Add("Wybierz urządzenie");
            foreach (var device in PerPlayerDevices)
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
            dropdown.onValueChanged.AddListener((value) => OnDropdownValueChanged(playerNumber));
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

        int index = playerDropdowns[playerNumber - 1].value + 1;
        SelectDevice(index, playerNumber);
        UpdateDropdownOptions(playerNumber);
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
