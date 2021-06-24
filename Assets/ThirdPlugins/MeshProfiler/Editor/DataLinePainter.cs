using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace MeshProfilerNS
{

    public class DataLinePainter
    {
        private static Material _graphMaterial;
        private static Rect _axisRect = new Rect(310, 580, 440, 150);
        private static Rect _graphRect = new Rect(320, 580, 420, 130);
        private static Rect _graphContentRect = new Rect(320, 600, 420, 130);
        private static Color _layerColor = new Color(190f / 255f, 192f / 255f, 41f / 255f, 0.6f);
        private static Vector3[] _points = null;

        private static int _sampleCount;
        private static int maxValue;
        private static List<int> _samples = new List<int>();
        static string[] labelArray = new string[5] { "Low", "Mid Low", "Mid", "Mid High", "High" };
        public static void Init(List<int> data)
        {
            _samples = data;
            maxValue = _samples.Max();
            _sampleCount = _samples.Count;
        }

        public static void Draw()
        {

            if (_graphMaterial == null)
            {
                _graphMaterial = new Material(Shader.Find("Hidden/Internal-Colored"));
                _graphMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                _graphMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                _graphMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
                _graphMaterial.SetInt("_ZWrite", 0);
            }
            DrawGraph();
        }

        private static void DrawGraph()
        {

            if (_points == null || _points.Length != _sampleCount)
            {
                _points = new Vector3[_sampleCount];
            }


            for (int i = 0; i < _samples.Count; ++i)
            {
                _points[i].x = (float)i / _sampleCount * _graphContentRect.width + _graphContentRect.xMin;
                _points[i].y = _graphContentRect.yMax - (float)_samples[i] / maxValue * _graphContentRect.height;

            }
            _graphMaterial.SetPass(0);


            for (int i = 0; i < _samples.Count; ++i)
            {
                if (_graphRect.Contains(_points[i]))
                {
                    GL.Begin(GL.QUADS);
                    GL.Color(_layerColor);
                    GL.Vertex3(_points[i].x, _graphContentRect.yMax, 0);
                    GL.Vertex3(_points[i].x, _points[i].y, 0);
                    GL.Vertex3(_points[i].x + 50f, _points[i].y, 0);
                    GL.Vertex3(_points[i].x + 50f, _graphContentRect.yMax, 0);
                    GL.End();


                }
            }
            for (int i = 0; i < _samples.Count; ++i)
            {
                EditorGUI.LabelField(new Rect(_points[i].x, _points[i].y - 20, 50, 20), _samples[i].ToString(), MPGUIStyles.centerStyle);
                EditorGUI.LabelField(new Rect(_points[i].x, _graphContentRect.yMax + 5, 50, 20), labelArray[i], MPGUIStyles.centerStyle);
            }
            DrawArrow(new Vector2(_axisRect.xMin, _axisRect.yMax), new Vector2(_axisRect.xMin, _axisRect.yMin), EditorGUIUtility.isProSkin ? Color.white : Color.black);
            DrawArrow(new Vector2(_axisRect.xMin, _axisRect.yMax), new Vector2(_axisRect.xMax, _axisRect.yMax), EditorGUIUtility.isProSkin ? Color.white : Color.black);

        }

        private static void DrawArrow(Vector2 from, Vector2 to, Color color)
        {
            Handles.BeginGUI();
            Handles.color = color;
            Handles.DrawAAPolyLine(3, from, to);
            Vector2 v0 = from - to;
            v0 *= 10 / v0.magnitude;
            Vector2 v1 = new Vector2(v0.x * 0.866f - v0.y * 0.5f, v0.x * 0.5f + v0.y * 0.866f);
            Vector2 v2 = new Vector2(v0.x * 0.866f + v0.y * 0.5f, v0.x * -0.5f + v0.y * 0.866f); ;
            Handles.DrawAAPolyLine(3, to + v1, to, to + v2);
            Handles.EndGUI();
        }
    }
}