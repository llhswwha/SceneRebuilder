using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
namespace UnityEngine.InputSystem
{
    public class InputSystemManage : MonoBehaviour
    {
        public static Vector2 m_Move;//Keyboard WS-x  AD-y

        public static Vector2 m_Look;//Mouse Delta

        public static Vector2 m_UpDown;//QE

        public void OnMove(InputAction.CallbackContext context)
        {            
            m_Move = context.ReadValue<Vector2>();
            Debug.Log("OnMove:" + m_Move);
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            m_Look = context.ReadValue<Vector2>();
        }

        public void Up(InputAction.CallbackContext context)
        {
            m_UpDown = context.ReadValue<Vector2>();
            Debug.Log("m_UpDown:" + m_UpDown);
        }
    }
}