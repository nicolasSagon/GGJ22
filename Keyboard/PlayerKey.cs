using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.Keyboard
{
    public class PlayerKey
    {
        public Key keyCode { get; }
        public Boolean isPressed;
        public string playerName;

        public PlayerKey(Key keyCode, string playerName)
        {
            this.keyCode = keyCode;
            this.playerName = playerName;
            isPressed = false;
        }
    }
}