using UnityEngine;
using UnityEditor;
using System.Collections;

namespace NGS.SuperLevelOptimizer
{
    public enum DrawFunction
    {
        OnSceneView,OnSceneGUI,OnDrawGizmos,OnDrawGizmosSelected
    }
    public class UnityEditorHelper
    {
        public static void SetFunction(DrawFunction func){
            //Gizmo drawing functions can only be used in OnDrawGizmos and OnDrawGizmosSelected.
            if(func==DrawFunction.OnSceneView || func==DrawFunction.OnSceneGUI)
            {
                IsGizmos=false;
            }
            else{
                IsGizmos=true;
            }
        }

        //Handles.Label(pos, t.value.ToString("F1"));

        public static void Label(Vector3 pos,string txt)
        {
            // if(IsGizmos){
            //     Gizmos.(center,size);
            // }
            // else{
                Handles.Label(pos, txt);
            // }
        }

        public static void DrawWireCube(Vector3 center, Vector3 size, Color color)
        {
            SetColor(color);
            DrawWireCube(center,size);
       }
        public static void DrawWireCube(Vector3 center, Vector3 size)
        {
            if(IsGizmos){
                Gizmos.DrawWireCube(center,size);
            }
            else{
                Handles.DrawWireCube(center, size);
                //DebugDrawWireCube(center,size);
           }
        }

        public static void DebugDrawWireCube(Vector3 center, Vector3 size){
                var half = size / 2;
                DrawLine(center + new Vector3(-half.x, -half.y, half.z), center + new Vector3(half.x, -half.y, half.z));
                DrawLine(center + new Vector3(-half.x, -half.y, half.z), center + new Vector3(-half.x, half.y, half.z));
                DrawLine(center + new Vector3(half.x, half.y, half.z), center + new Vector3(half.x, -half.y, half.z));
                DrawLine(center + new Vector3(half.x, half.y, half.z), center + new Vector3(-half.x, half.y, half.z));

                DrawLine(center + new Vector3(-half.x, -half.y, -half.z), center + new Vector3(half.x, -half.y, -half.z));
                DrawLine(center + new Vector3(-half.x, -half.y, -half.z), center + new Vector3(-half.x, half.y, -half.z));
                DrawLine(center + new Vector3(half.x, half.y, -half.z), center + new Vector3(half.x, -half.y, -half.z));
                DrawLine(center + new Vector3(half.x, half.y, -half.z), center + new Vector3(-half.x, half.y, -half.z));

                DrawLine(center + new Vector3(-half.x, -half.y, -half.z), center + new Vector3(-half.x, -half.y, half.z));
                DrawLine(center + new Vector3(half.x, -half.y, -half.z), center + new Vector3(half.x, -half.y, half.z));
                DrawLine(center + new Vector3(-half.x, half.y, -half.z), center + new Vector3(-half.x, half.y, half.z));
                DrawLine(center + new Vector3(half.x, half.y, -half.z), center + new Vector3(half.x, half.y, half.z));
 
        }

        public static bool IsGizmos=true;

        private static void SetColor(Color value)
        {
            _color=value;
            if(IsGizmos){
                Gizmos.color=value;
            }
            else
            {
                Handles.color = value;
            }
        }

        private static Color _color=default(Color);

        public static Color color
        {
            set
            {
                SetColor(value);
            }
        } 

        private static Matrix4x4 _matrix;

        private static void SetMatrix(Matrix4x4 value)
        {
            _matrix=value;
            if(IsGizmos){
                Gizmos.matrix=value;
            }
            else
            {
                Handles.matrix = value;
            }
        }

        public static Matrix4x4 matrix
        {
            set
            {
                SetMatrix(value);
            }
        } 

        public static void DrawLine(Vector3 from,Vector3 to)
        {
            if(IsGizmos){
                Gizmos.DrawLine(from,to);
            }
            else
            {
                Handles.DrawLine(from,to);
                //Debug.DrawLine(from,to,_color,0.1f);
            }
        }
    }
}