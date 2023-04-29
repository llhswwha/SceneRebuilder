using UnityEngine;
#if UNITY_INPUTSYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
#endif

namespace Battlehub.RTCommon
{
    public enum InputAxis
    {
        X,
        Y,
        Z,
        Horizontal,
        Vertical,
    }

    public interface IInput
    {

        bool IsAnyKeyDown();

        float GetAxis(InputAxis axis);
        bool GetKeyDown(KeyCode key);
        bool GetKeyUp(KeyCode key);
        bool GetKey(KeyCode key);
        
        Vector3 GetPointerXY(int pointer);

        bool GetPointerDown(int button);
        bool GetPointerUp(int button);
        bool GetPointer(int button);
    }
    
    public class DisabledInput : IInput
    {
        public float GetAxis(InputAxis axis)
        {
            return 0;
        }

        public bool GetKey(KeyCode key)
        {
            return false;
        }

        public bool GetKeyDown(KeyCode key)
        {
            return false;
        }

        public bool GetKeyUp(KeyCode key)
        {
            return false;
        }

        public bool GetPointer(int button)
        {
            return false;
        }

        public bool GetPointerDown(int button)
        {
            return false;
        }

        public bool GetPointerUp(int button)
        {
            return false;
        }

        public Vector3 GetPointerXY(int pointer)
        {
            if (pointer == 0)
            {
#if UNITY_INPUTSYSTEM
                return Mouse.current.position.ReadValue();
#else
                return Input.mousePosition;
#endif
            }
            else
            {
                Touch touch = Input.GetTouch(pointer);
                return touch.position;
            }
        }

        public bool IsAnyKeyDown()
        {
            return false;
        }
    }

    public class InputLow : IInput
    {
        public virtual bool IsAnyKeyDown()
        {
#if UNITY_INPUTSYSTEM
            if(Keyboard.current!=null)return Keyboard.current.anyKey.wasPressedThisFrame;
            else return Input.anyKeyDown;
#else
            return Input.anyKeyDown;
#endif

        }

        public virtual bool GetKeyDown(KeyCode key)
        {
#if UNITY_INPUTSYSTEM
            KeyControl m = Keyboard.current.FindKeyOnCurrentKeyboardLayout(key.ToString());
            if (m != null) return m.wasPressedThisFrame;
            else return Input.GetKeyDown(key);
#else
            return Input.GetKeyDown(key);
#endif

        }

        public virtual bool GetKeyUp(KeyCode key)
        {
#if UNITY_INPUTSYSTEM
            KeyControl m = Keyboard.current.FindKeyOnCurrentKeyboardLayout(key.ToString());
            if (m != null) return m.wasReleasedThisFrame;
            else return Input.GetKeyUp(key);
#else
            return Input.GetKeyUp(key);
#endif
        }

        public virtual bool GetKey(KeyCode key)
        {
#if UNITY_INPUTSYSTEM
            KeyControl m = Keyboard.current.FindKeyOnCurrentKeyboardLayout(key.ToString());
            if (m != null) return m.isPressed;
            else return Input.GetKey(key);
#else
            return Input.GetKey(key);
#endif
        }

        public virtual float GetAxis(InputAxis axis)
        {
#if UNITY_INPUTSYSTEM
            switch (axis)
            {
                case InputAxis.X:
                    return Mathf.Clamp(Mouse.current.delta.ReadValue().x,-1,1);
                case InputAxis.Y:
                    return Mathf.Clamp(Mouse.current.delta.ReadValue().y,-1,1);
                case InputAxis.Z:
                    return Mathf.Clamp(Mouse.current.scroll.ReadValue().y,-1,1);
                case InputAxis.Horizontal:                   
                    return InputSystemManage.m_Move.x;
                case InputAxis.Vertical:
                    return InputSystemManage.m_Move.y;
                default:
                    return 0;
            }
#else
            switch (axis)
            {
                case InputAxis.X:
                    return Input.GetAxis("Mouse X");
                case InputAxis.Y:
                    return Input.GetAxis("Mouse Y");
                case InputAxis.Z:
                    return Input.GetAxis("Mouse ScrollWheel");
                case InputAxis.Horizontal:
                    return Input.GetAxis("Horizontal");
                case InputAxis.Vertical:
                    return Input.GetAxis("Vertical");
                default:
                    return 0;
            }
#endif
        }

        public virtual Vector3 GetPointerXY(int pointer)
        {
            if (pointer == 0)
            {
#if UNITY_INPUTSYSTEM
                return Mouse.current.position.ReadValue();
#else
                return Input.mousePosition;
#endif
            }
            else
            {
                Touch touch = Input.GetTouch(pointer);
                return touch.position;
            }
        }

        public virtual bool GetPointerDown(int index)
        {
#if UNITY_INPUTSYSTEM
            if(index==0)
            {
                return Mouse.current.leftButton.wasPressedThisFrame;
            }else if(index==1)
            {
                return Mouse.current.rightButton.wasPressedThisFrame;
            }else
            {
                return Mouse.current.middleButton.wasPressedThisFrame;
            }
#else
            bool buttonDown = Input.GetMouseButtonDown(index);
            return buttonDown;
#endif
        }

        public virtual bool GetPointerUp(int index)
        {
#if UNITY_INPUTSYSTEM
            if(index==0)
            {
                return Mouse.current.leftButton.wasReleasedThisFrame;
            }else if(index==1)
            {
                return Mouse.current.rightButton.wasReleasedThisFrame;
            }else
            {
                return Mouse.current.middleButton.wasReleasedThisFrame;
            }
#else
            return Input.GetMouseButtonUp(index);
#endif
        }

        public virtual bool GetPointer(int index)
        {
#if UNITY_INPUTSYSTEM
            if(index==0)
            {
                return Mouse.current.leftButton.isPressed;
            }else if(index==1)
            {
                return Mouse.current.rightButton.isPressed;
            }else
            {
                return Mouse.current.middleButton.isPressed;
            }
#else
            return Input.GetMouseButton(index);
#endif
        }
    }
}
