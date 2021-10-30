﻿/*************************************************************************
 *  Copyright © 2016-2018 Mogoson. All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  HelpInfo.cs
 *  Description  :  Draw help info in scene.
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  0.1.0
 *  Date         :  4/15/2018
 *  Description  :  Initial development version.
 *************************************************************************/

using UnityEngine;

namespace Mogoson.UI
{
    public class HelpInfo : MonoBehaviour
    {
        #region Field and Property
        [Multiline]
        public string info = "Help info.";

        public float top = 10;
        public float left = 10;
        #endregion

        #region Private Method
        private void OnGUI()
        {
            GUILayout.Space(left);
            GUILayout.BeginHorizontal();
            GUILayout.Space(top);
            GUILayout.Label(info);
            GUILayout.EndHorizontal();
        }
        #endregion
    }
}