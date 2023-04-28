﻿using UnityEngine;
using System.Collections.Generic;

namespace RTEditor
{
    /// <summary>
    /// This class can be used to hold a snapshot of the object selection.
    /// </summary>
    /// <remarks>
    /// The class does not contain object selection mask snaphot data.
    /// </remarks>
    public class ObjectSelectionSnapshot
    {
        #region Private Variables
        /// <summary>
        /// Holds all the objects which are marked as selected at the moment the snapshot is taken.
        /// </summary>
        private HashSet<GameObject> _selectedGameObjects = new HashSet<GameObject>();

        /// <summary>
        /// Holds a reference to the last selected game object at the moment the snapshot is taken.
        /// </summary>
        private GameObject _lastSelectedGameObject;
        #endregion

        #region Public Properties
        /// <summary>
        /// Returns the objects marked as selected at the moment the snapshot was taken.
        /// </summary>
        public HashSet<GameObject> SelectedGameObjects { get { return _selectedGameObjects; } }

        /// <summary>
        /// Returns a reference to the last selected game object at the moment the snapshot was taken.
        /// </summary>
        public GameObject LastSelectedGameObject { get { return _lastSelectedGameObject; } }

        /// <summary>
        /// Returns the number of selected objects inside the snapshot.
        /// </summary>
        public int NumberOfSelectedObjects { get { return _selectedGameObjects.Count; } }
        #endregion

        #region Public Methods
        /// <summary>
        /// Takes a snapshot of the current object selection.
        /// </summary>
        public void TakeSnapshot()
        {
            EditorObjectSelection objectSelection = EditorObjectSelection.Instance;
            _selectedGameObjects = new HashSet<GameObject>(objectSelection.SelectedGameObjects);
            _lastSelectedGameObject = objectSelection.LastSelectedGameObject;
        }

        /// <summary>
        /// Checks if the snapshot contains the specified game object.
        /// </summary>
        public bool Contains(GameObject gameObject)
        {
            return _selectedGameObjects.Contains(gameObject);
        }

        /// <summary>
        /// Returns a list of all objects which reside in 'this' snapshot, but not in 'other'.
        /// </summary>
        public List<GameObject> GetDiff(ObjectSelectionSnapshot other)
        {
            if (NumberOfSelectedObjects == 0) return new List<GameObject>();

            var diffObjects = new List<GameObject>();
            foreach(var obj in _selectedGameObjects)
            {
                if (!other.Contains(obj)) diffObjects.Add(obj);
            }

            return diffObjects;
        }
        #endregion
    }
}
