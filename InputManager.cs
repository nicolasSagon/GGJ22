using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class InputManager: MonoBehaviour
{
    private CustomInputDevice firstController;
    public CustomInputDevice FirstController => firstController;

    private CustomInputDevice secondController;
    public CustomInputDevice SecondController => secondController;
    
    private Boolean isInit = false;
    [CanBeNull] private Action<int> inputManagerInitializedCallback;

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

    public void setInputManagerCallback(Action<int> callback)
    {
        inputManagerInitializedCallback = callback;
    }

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
        inputManagerInitializedCallback?.Invoke(2);
    }

    private void selectController(Boolean isFirst)
    {
        var currentKeyboard = Keyboard.current;
        if (currentKeyboard == null)
        {
            return;
        }
            
        var gamepads = Gamepad.all;
        if (firstController != null)
        {
            gamepads = gamepads.Where(item => item.name != firstController.Controller.name).ToArray();
        }
        

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
                        inputManagerInitializedCallback?.Invoke(1);
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
                    inputManagerInitializedCallback?.Invoke(1);
                }
                else
                {
                    if (firstController is {InputType: CustomInputDevice.InputTypeEnum.KEYBOARD} && firstController.KeyboardKeys.Contains(keyControl))
                    {
                        return;
                    } 
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
                    inputManagerInitializedCallback?.Invoke(1);
                }
                else
                {
                    if (firstController is {InputType: CustomInputDevice.InputTypeEnum.KEYBOARD} && firstController.KeyboardKeys.Contains(keyControl))
                    {
                        return;
                    } 
                    Debug.Log("Keyboard set for player two");
                    secondController = new CustomInputDevice(CustomInputDevice.InputTypeEnum.KEYBOARD, Keyboard.current, secondKeyboardInput, null);
                    setInputManagerInitialized();
                }
            }
        });
    }
}