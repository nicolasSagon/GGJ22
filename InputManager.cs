using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class InputManager: MonoBehaviour
{
    private CustomInputDevice firstController;
    private CustomInputDevice secondController;
    private Boolean isInit = false;

    private List<Key> firstKeyboardInput = new()
    {
        Key.Q,
        Key.W,
        Key.E
    };
    private List<Key> secondKeyboardInput = new()
    {
        Key.I,
        Key.O,
        Key.P
    };

    private List<GamepadButton> gamePadInput = new()
    {
        GamepadButton.A,
        GamepadButton.B,
        GamepadButton.X
    };

    private void Update()
    {
        if (!isInit && firstController == null)
        {
            selectController(true);
        } else if (!isInit && firstController != null)
        {
            selectController(false);
        }
    }

    private void setInputManagerInitialized()
    {
        isInit = true;
    }

    private void selectController(Boolean isFirst)
    {
        var currentKeyboard = Keyboard.current;
        if (currentKeyboard == null)
        {
            return;
        }
            
        var gamepads = Gamepad.all;
            
        foreach (var gamepad in gamepads)
        {
            foreach (var gamepadButton in gamePadInput)
            {
                if (gamepad[gamepadButton].wasPressedThisFrame)
                {
                    
                    if (isFirst)
                    {
                        Debug.Log($"Manette : {gamepad.name} was selected for player one");
                        firstController = new CustomInputDevice(CustomInputDevice.InputTypeEnum.GAMEPAD, gamepad, null, gamePadInput);
                    }
                    else
                    {
                        Debug.Log($"Manette : {gamepad.name} was selected for player two");
                        secondController = new CustomInputDevice(CustomInputDevice.InputTypeEnum.GAMEPAD, gamepad, null, gamePadInput);
                        setInputManagerInitialized();
                    }
                }
            }
        }

        firstKeyboardInput.ForEach(keyControl =>
        {
            if (currentKeyboard[keyControl].wasPressedThisFrame)
            {
                if (isFirst)
                {
                    Debug.Log("Keyboard set for player one");
                    firstController = new CustomInputDevice(CustomInputDevice.InputTypeEnum.KEYBOARD, Keyboard.current, firstKeyboardInput, null);
                }
                else
                {
                    Debug.Log("Keyboard set for player two");
                    secondController = new CustomInputDevice(CustomInputDevice.InputTypeEnum.KEYBOARD, Keyboard.current, firstKeyboardInput, null);
                    setInputManagerInitialized();
                }
            }
        });
            
        secondKeyboardInput.ForEach(keyControl =>
        {
            if (currentKeyboard[keyControl].wasPressedThisFrame)
            {
                if (isFirst)
                {
                    Debug.Log("Keyboard set for player one");
                    firstController = new CustomInputDevice(CustomInputDevice.InputTypeEnum.KEYBOARD, Keyboard.current, secondKeyboardInput, null);
                }
                else
                {
                    Debug.Log("Keyboard set for player two");
                    secondController = new CustomInputDevice(CustomInputDevice.InputTypeEnum.KEYBOARD, Keyboard.current, secondKeyboardInput, null);
                    setInputManagerInitialized();
                }
            }
        });
    }
}