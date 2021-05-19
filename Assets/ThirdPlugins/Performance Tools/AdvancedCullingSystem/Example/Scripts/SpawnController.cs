using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AdvancedCullingSystem.DynamicCullingCore;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace AdvancedCullingSystem.Examples
{
    public class SpawnController : MonoBehaviour
    {
        [SerializeField]
        private Transform[] _spawnObjects;

        [SerializeField]
        private Text _objectsCountField;

        [SerializeField]
        private int _objectsPerSpawnCount = 50;

        [SerializeField]
        private Vector3 _minSpawnPos;

        [SerializeField]
        private Vector3 _maxSpawnPos;

        public List<GameObject> newGos = new List<GameObject>();

        public void Spawn()
        {

            Transform t = this.transform;
            for (int i = 0; i < _objectsPerSpawnCount; i++)
            {
                Transform obj = Instantiate(_spawnObjects[Random.Range(0, _spawnObjects.Length)]);

                obj.SetParent(t);

                Vector3 position = new Vector3(
                    Random.Range(_minSpawnPos.x, _maxSpawnPos.x),
                    Random.Range(_minSpawnPos.y, _maxSpawnPos.y),
                    Random.Range(_minSpawnPos.z, _maxSpawnPos.z));

                Vector3 rotation = new Vector3(
                    obj.eulerAngles.x,
                    Random.Range(0, 360),
                    obj.eulerAngles.z);

                RaycastHit hit;
                if(Physics.Raycast(new Vector3(position.x, 100, position.z), Vector3.down, out hit, 1000, LayerMask.GetMask("Water")))
                    position.y = hit.point.y;

                obj.position = position;
                obj.eulerAngles = rotation;

                MeshRenderer[] array = obj.GetComponentsInChildren<MeshRenderer>();
                for (int i1 = 0; i1 < array.Length; i1++)
                {
                    MeshRenderer renderer = array[i1];
                    DynamicCulling.Instance?.AddObjectForCulling(renderer,i1);
                }

                newGos.Add(obj.gameObject);
            }

            if(_objectsCountField != null)
                _objectsCountField.text = (int.Parse(_objectsCountField.text) + 50).ToString();
        }

        public void Clear()
        {
            foreach(var obj in newGos)
            {
                GameObject.DestroyImmediate(obj);
            }
        }
    }



#if UNITY_EDITOR

    [CustomEditor(typeof(SpawnController))]
    public class SpawnControllerEditor : Editor
    {
        protected new SpawnController target
        {
            get
            {
                return (SpawnController) base.target;
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Spawn"))
                target.Spawn();
            if (GUILayout.Button("Clear"))
                target.Clear();
        }
    }

#endif
}
