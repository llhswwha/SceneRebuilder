using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Unity.ComnLib.Utils
{
    public static class ScreenHelper
    {
        public static bool IsOutOfBounder(float padding)
        {
            Vector2 mousePos = Input.mousePosition;
            return (mousePos.x < padding) || (mousePos.y < padding) || (mousePos.y > Screen.height - padding) ||
                   (mousePos.x > Screen.width - padding);
        }
    }
}
