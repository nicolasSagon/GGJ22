using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class CustomInputDevice
{
    public enum InputTypeEnum
    {
        GAMEPAD,
        KEYBOARD
    }
    
    private InputTypeEnum _inputType;
    public InputTypeEnum InputType => _inputType;
    
    private readonly InputDevice _controller;
    public InputDevice Controller => _controller;

    [CanBeNull] private List<Key> _keyboardKeys;
    [CanBeNull] public List<Key> KeyboardKeys => _keyboardKeys;
    
    [CanBeNull] private List<GamepadButton> _gamepadButtons;
    [CanBeNull] public List<GamepadButton> GamepadButtons => _gamepadButtons;

    public CustomInputDevice(InputTypeEnum inputType, InputDevice controller, [CanBeNull] List<Key> controls, [CanBeNull] List<GamepadButton> gamepadButtons)
    {
        _inputType = inputType;
        _controller = controller;
        _keyboardKeys = controls;
        _gamepadButtons = gamepadButtons;
    }

    public Boolean isAttackPressed()
    {
        switch (InputType)
        {
            case InputTypeEnum.GAMEPAD:
            {
                var gamePad = _controller as Gamepad;
                return gamePad[_gamepadButtons[0]].wasPressedThisFrame;
            }
            case InputTypeEnum.KEYBOARD:
            {
                var keyboard = _controller as Keyboard;
                return keyboard[_keyboardKeys[0]].wasPressedThisFrame;
            }
            default:
                return false;
        }
    }
    
    public Boolean isBlockPressed()
    {
        switch (InputType)
        {
            case InputTypeEnum.GAMEPAD:
            {
                var gamePad = _controller as Gamepad;
                return gamePad[_gamepadButtons[1]].wasPressedThisFrame;
            }
            case InputTypeEnum.KEYBOARD:
            {
                var keyboard = _controller as Keyboard;
                return keyboard[_keyboardKeys[1]].wasPressedThisFrame;
            }
            default:
                return false;
        }
    }
    
    public Boolean isSuperPressed()
    {
        switch (InputType)
        {
            case InputTypeEnum.GAMEPAD:
            {
                var gamePad = _controller as Gamepad;
                return gamePad[_gamepadButtons[2]].wasPressedThisFrame;
            }
            case InputTypeEnum.KEYBOARD:
            {
                var keyboard = _controller as Keyboard;
                return keyboard[_keyboardKeys[2]].wasPressedThisFrame;
            }
            default:
                return false;
        }
    }
}