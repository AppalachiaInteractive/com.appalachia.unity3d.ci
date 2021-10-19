using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.EnhancedTouch;
using TouchOld = UnityEngine.Touch;
using TouchNew = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace Appalachia.CI.Integration.InputRemapping
{
    public static class Input
    {
        public static int touchCount
        {
            get
            {
                if (!EnhancedTouchSupport.enabled)
                {
                    EnhancedTouchSupport.Enable();
                }

                return TouchNew.activeTouches.Count;
            }
        }

        public static Vector2 mousePosition => Mouse.current.position.ReadValue();

        public static bool GetKey(KeyCode keyCode)
        {
            var key = keyCode.ToKey(out var checkShift);

            return GetKeyResult(key, KeyEventType.Key, checkShift);
        }

        public static bool GetKeyDown(KeyCode keyCode)
        {
            var key = keyCode.ToKey(out var checkShift);

            return GetKeyResult(key, KeyEventType.KeyDown, checkShift);
        }

        public static bool GetKeyUp(KeyCode keyCode)
        {
            var key = keyCode.ToKey(out var checkShift);

            return GetKeyResult(key, KeyEventType.KeyUp, checkShift);
        }

        public static bool GetMouseButton(int button)
        {
            var buttonControl = GetButtonControlFromMouseButtonIndex(button);
            return buttonControl.isPressed;
        }

        public static bool GetMouseButtonDown(int button)
        {
            var buttonControl = GetButtonControlFromMouseButtonIndex(button);
            return buttonControl.wasPressedThisFrame;
        }

        public static bool GetMouseButtonUp(int button)
        {
            var buttonControl = GetButtonControlFromMouseButtonIndex(button);
            return buttonControl.wasReleasedThisFrame;
        }

        /*public static bool GetButton(string button)
        {
            var buttonControl = GetButtonControlFromMouseButtonIndex(button);
            return buttonControl.isPressed;
        }

        public static bool GetButtonDown(string button)
        {
            var buttonControl = GetButtonControlFromMouseButtonIndex(button);
            return buttonControl.wasPressedThisFrame;
        }

        public static bool GetButtonUp(string button)
        {
            InputSystem.
            var buttonControl = GetButtonControlFromMouseButtonIndex(button);
            return buttonControl.wasReleasedThisFrame;
        }
        */

        public static Key ToKey(this KeyCode keyCode, out bool checkShift)
        {
            checkShift = false;

            switch (keyCode)
            {
                case KeyCode.None:
                    return Key.None;
                case KeyCode.Backspace:
                    return Key.Backspace;
                case KeyCode.Delete:
                    return Key.Delete;
                case KeyCode.Tab:
                    return Key.Tab;
                case KeyCode.Return:
                    return Key.Enter;
                case KeyCode.Pause:
                    return Key.Pause;
                case KeyCode.Escape:
                    return Key.Escape;
                case KeyCode.Space:
                    return Key.Space;
                case KeyCode.Keypad0:
                    return Key.Numpad0;
                case KeyCode.Keypad1:
                    return Key.Numpad1;
                case KeyCode.Keypad2:
                    return Key.Numpad2;
                case KeyCode.Keypad3:
                    return Key.Numpad3;
                case KeyCode.Keypad4:
                    return Key.Numpad4;
                case KeyCode.Keypad5:
                    return Key.Numpad5;
                case KeyCode.Keypad6:
                    return Key.Numpad6;
                case KeyCode.Keypad7:
                    return Key.Numpad7;
                case KeyCode.Keypad8:
                    return Key.Numpad8;
                case KeyCode.Keypad9:
                    return Key.Numpad9;
                case KeyCode.KeypadPeriod:
                    return Key.NumpadPeriod;
                case KeyCode.KeypadDivide:
                    return Key.NumpadDivide;
                case KeyCode.KeypadMultiply:
                    return Key.NumpadMultiply;
                case KeyCode.KeypadMinus:
                    return Key.NumpadMinus;
                case KeyCode.KeypadPlus:
                    return Key.NumpadPlus;
                case KeyCode.KeypadEnter:
                    return Key.NumpadEnter;
                case KeyCode.KeypadEquals:
                    return Key.NumpadEquals;
                case KeyCode.UpArrow:
                    return Key.UpArrow;
                case KeyCode.DownArrow:
                    return Key.DownArrow;
                case KeyCode.RightArrow:
                    return Key.RightArrow;
                case KeyCode.LeftArrow:
                    return Key.LeftArrow;
                case KeyCode.Insert:
                    return Key.Insert;
                case KeyCode.Home:
                    return Key.Home;
                case KeyCode.End:
                    return Key.End;
                case KeyCode.PageUp:
                    return Key.PageUp;
                case KeyCode.PageDown:
                    return Key.PageDown;
                case KeyCode.F1:
                    return Key.F1;
                case KeyCode.F2:
                    return Key.F2;
                case KeyCode.F3:
                    return Key.F3;
                case KeyCode.F4:
                    return Key.F4;
                case KeyCode.F5:
                    return Key.F5;
                case KeyCode.F6:
                    return Key.F6;
                case KeyCode.F7:
                    return Key.F7;
                case KeyCode.F8:
                    return Key.F8;
                case KeyCode.F9:
                    return Key.F9;
                case KeyCode.F10:
                    return Key.F10;
                case KeyCode.F11:
                    return Key.F11;
                case KeyCode.F12:
                    return Key.F12;
                case KeyCode.Alpha0:
                    return Key.Digit0;
                case KeyCode.Alpha1:
                    return Key.Digit1;
                case KeyCode.Alpha2:
                    return Key.Digit2;
                case KeyCode.Alpha3:
                    return Key.Digit3;
                case KeyCode.Alpha4:
                    return Key.Digit4;
                case KeyCode.Alpha5:
                    return Key.Digit5;
                case KeyCode.Alpha6:
                    return Key.Digit6;
                case KeyCode.Alpha7:
                    return Key.Digit7;
                case KeyCode.Alpha8:
                    return Key.Digit8;
                case KeyCode.Alpha9:
                    return Key.Digit9;
                case KeyCode.Exclaim:
                    checkShift = true;
                    return Key.Digit1;
                case KeyCode.DoubleQuote:
                    checkShift = true;
                    return Key.Quote;
                case KeyCode.Hash:
                    checkShift = true;
                    return Key.Digit3;
                case KeyCode.Dollar:
                    checkShift = true;
                    return Key.Digit4;
                case KeyCode.Percent:
                    checkShift = true;
                    return Key.Digit5;
                case KeyCode.Ampersand:
                    checkShift = true;
                    return Key.Digit7;
                case KeyCode.Quote:
                    return Key.Quote;
                case KeyCode.LeftParen:
                    checkShift = true;
                    return Key.Digit9;
                case KeyCode.RightParen:
                    checkShift = true;
                    return Key.Digit0;
                case KeyCode.Asterisk:
                    checkShift = true;
                    return Key.Digit8;
                case KeyCode.Plus:
                    checkShift = true;
                    return Key.Equals;
                case KeyCode.Comma:
                    return Key.Comma;
                case KeyCode.Minus:
                    return Key.Minus;
                case KeyCode.Period:
                    return Key.Period;
                case KeyCode.Slash:
                    return Key.Slash;
                case KeyCode.Colon:
                    checkShift = true;
                    return Key.Semicolon;
                case KeyCode.Semicolon:
                    return Key.Semicolon;
                case KeyCode.Less:
                    checkShift = true;
                    return Key.Comma;
                case KeyCode.Equals:
                    return Key.Equals;
                case KeyCode.Greater:
                    checkShift = true;
                    return Key.Period;
                case KeyCode.Question:
                    checkShift = true;
                    return Key.Slash;
                case KeyCode.At:
                    checkShift = true;
                    return Key.Digit2;
                case KeyCode.LeftBracket:
                    return Key.LeftBracket;
                case KeyCode.Backslash:
                    return Key.Backslash;
                case KeyCode.RightBracket:
                    return Key.RightBracket;
                case KeyCode.Caret:
                    checkShift = true;
                    return Key.Digit6;
                case KeyCode.Underscore:
                    checkShift = true;
                    return Key.Minus;
                case KeyCode.BackQuote:
                    return Key.Backquote;
                case KeyCode.A:
                    return Key.A;
                case KeyCode.B:
                    return Key.B;
                case KeyCode.C:
                    return Key.C;
                case KeyCode.D:
                    return Key.D;
                case KeyCode.E:
                    return Key.E;
                case KeyCode.F:
                    return Key.F;
                case KeyCode.G:
                    return Key.G;
                case KeyCode.H:
                    return Key.H;
                case KeyCode.I:
                    return Key.I;
                case KeyCode.J:
                    return Key.J;
                case KeyCode.K:
                    return Key.K;
                case KeyCode.L:
                    return Key.L;
                case KeyCode.M:
                    return Key.M;
                case KeyCode.N:
                    return Key.N;
                case KeyCode.O:
                    return Key.O;
                case KeyCode.P:
                    return Key.P;
                case KeyCode.Q:
                    return Key.Q;
                case KeyCode.R:
                    return Key.R;
                case KeyCode.S:
                    return Key.S;
                case KeyCode.T:
                    return Key.T;
                case KeyCode.U:
                    return Key.U;
                case KeyCode.V:
                    return Key.V;
                case KeyCode.W:
                    return Key.W;
                case KeyCode.X:
                    return Key.X;
                case KeyCode.Y:
                    return Key.Y;
                case KeyCode.Z:
                    return Key.Z;
                case KeyCode.LeftCurlyBracket:
                    checkShift = true;
                    return Key.LeftBracket;
                case KeyCode.Pipe:
                    checkShift = true;
                    return Key.Backslash;
                case KeyCode.RightCurlyBracket:
                    checkShift = true;
                    return Key.RightBracket;
                case KeyCode.Tilde:
                    checkShift = true;
                    return Key.Backquote;
                case KeyCode.Numlock:
                    return Key.NumLock;
                case KeyCode.CapsLock:
                    return Key.CapsLock;
                case KeyCode.ScrollLock:
                    return Key.ScrollLock;
                case KeyCode.RightShift:
                    return Key.RightShift;
                case KeyCode.LeftShift:
                    return Key.LeftShift;
                case KeyCode.RightControl:
                    return Key.RightCtrl;
                case KeyCode.LeftControl:
                    return Key.LeftCtrl;
                case KeyCode.RightAlt:
                    return Key.RightAlt;
                case KeyCode.LeftAlt:
                    return Key.LeftAlt;
                case KeyCode.LeftMeta:
                    return Key.LeftMeta;
                case KeyCode.LeftWindows:
                    return Key.LeftWindows;
                case KeyCode.RightMeta:
                    return Key.RightMeta;
                case KeyCode.RightWindows:
                    return Key.RightWindows;
                case KeyCode.AltGr:
                    return Key.AltGr;
                case KeyCode.Print:
                    return Key.PrintScreen;

                default:
                    throw new ArgumentOutOfRangeException(nameof(keyCode), keyCode, null);
            }
        }

        public static TouchNew GetTouch(int index)
        {
            if (!EnhancedTouchSupport.enabled)
            {
                EnhancedTouchSupport.Enable();
            }

            var touch = TouchNew.activeTouches[index];

            return touch;
        }

        private static bool GetKeyEvent(Key key, KeyEventType keyEventType)
        {
            var buttonControl = Keyboard.current[key];

            switch (keyEventType)
            {
                case KeyEventType.IsPressed:
                    //case KeyEventType.Key:
                    return buttonControl.isPressed;
                case KeyEventType.WasPressedThisFrame:
                    //case KeyEventType.KeyDown:
                    return buttonControl.wasPressedThisFrame;
                case KeyEventType.WasReleasedThisFrame:
                    //case KeyEventType.KeyUp:
                    return buttonControl.wasReleasedThisFrame;
                default:
                    throw new ArgumentOutOfRangeException(nameof(keyEventType), keyEventType, null);
            }
        }

        private static bool GetKeyResult(Key key, KeyEventType keyEventType, bool checkShift)
        {
            var isPressed = GetKeyEvent(key, keyEventType);

            if (!checkShift)
            {
                return isPressed;
            }

            var isShifted = GetKeyEvent(Key.LeftShift,  keyEventType) ||
                            GetKeyEvent(Key.RightShift, keyEventType);

            return isShifted && isPressed;
        }

        private static ButtonControl GetButtonControlFromMouseButtonIndex(int i)
        {
            return i switch
            {
                0 => Mouse.current.leftButton,
                1 => Mouse.current.rightButton,
                2 => Mouse.current.middleButton,
                _ => null
            };
        }

        private enum KeyEventType
        {
            Key = 0,
            IsPressed = 0,
            KeyDown = 10,
            WasPressedThisFrame = 10,
            KeyUp = 20,
            WasReleasedThisFrame = 20
        }
    }
}
