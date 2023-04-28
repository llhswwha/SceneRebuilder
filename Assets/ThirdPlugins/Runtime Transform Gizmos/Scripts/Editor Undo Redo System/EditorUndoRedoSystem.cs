﻿using UnityEngine;
using System.Collections.Generic;

namespace RTEditor
{
    /// <summary>
    /// This class implements the functionality for the Undo/Redo system. It allows
    /// the client code to register actions which can be undone and redone as needed.
    /// </summary>
    public class EditorUndoRedoSystem : MonoSingletonBase<EditorUndoRedoSystem>
    {
        public delegate void UndoStartHandler();
        public delegate void UndoEndHandler();
        public delegate void RedoStartHandler();
        public delegate void RedoEndHandler();

        public event UndoStartHandler UndoStart;
        public event UndoEndHandler UndoEnd;
        public event RedoStartHandler RedoStart;
        public event RedoEndHandler RedoEnd;

        #region Private Variables
        /// <summary>
        /// Represents the maximum number of actions which can be registered with the undo system.
        /// </summary>
        [SerializeField]
        private int _actionLimit = 50;

        /// <summary>
        /// This is the stack on which actions will be pushed when necessary. For example, when an
        /// action needs to be registered with the undo system, it will be pushed onto the stack.
        /// </summary>
        private List<IUndoableAndRedoableAction> _actionStack = new List<IUndoableAndRedoableAction>();

        /// <summary>
        /// This is an index inside the stack. It will allow us to perform undo and redo operations
        /// accordingly. For example, when actions are being pushed on the stack, this will be incremented.
        /// When an action is redone, it will be decremented.
        /// </summary>
        private int _actionStackPointer = -1;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets/sets the action limit. The minimum value is 1. If a value smaller than that is specified,
        /// it will be clamped accordingly.
        /// </summary>
        public int ActionLimit { get { return _actionLimit; } set { ChangeActionLimit(value); } }
        #endregion

        #region Public Methods
        /// <summary>
        /// Clears all actions. You can call this method when you need to remove
        /// all actions from the undo/redo stack and start anew.
        /// </summary>
        public void ClearActions()
        {
            _actionStack.Clear();
            _actionStackPointer = -1;
        }

        /// <summary>
        /// Registers an action with the undo/redo system. When an action is registered
        /// with the undo/redo system, it can be undone and redone as needed.
        /// </summary>
        public void RegisterAction(IUndoableAndRedoableAction action)
        {
            // Is this the first action registered?
            if(_actionStackPointer < 0)
            {
                // Just add the action to the stack and set the pointer to the first element
                _actionStack.Add(action);
                _actionStackPointer = 0;
            }
            else
            {
                // This is not the first action. Normally, we could just add the action on the top
                // of the stack, but we have to make sure that if the stack pointer does not point
                // at the top of the stack, all actions which follow after the stack pointer are
                // removed. This can happen when the user has performed a series of undo operations
                // because in that case, actions will not be popped from the stack. Only the stack
                // pointer will be moved downwards. This is what allows us to redo actions. However,
                // when a new action is registered, we will remove all actions which follow the one
                // referenced by the current stack pointer.
                if (_actionStackPointer < _actionStack.Count - 1)
                {
                    // Calculate the index of the first action to be removed and the number of actions to remove
                    int removeStartIndex = _actionStackPointer + 1;
                    int removeCount = _actionStack.Count - removeStartIndex;

                    // Remove the action range
                    RemoveActionsRange(removeStartIndex, removeCount);
                }

                // Now we can add the new action on the top of the stack
                _actionStack.Add(action);

                // If the maximum number of actions has been exceeded, remove an action from the beginning of the stack
                if (_actionStack.Count > _actionLimit) RemoveActionsRange(0, 1);

                // Whenever a new action is registered, the stack pointer will be adjusted to point at the top of the stack
                _actionStackPointer = _actionStack.Count - 1;
            }
        }
        public void UndoAction()
        {
            Undo();
        }
        public void RedoAction()
        {
            Redo();
        }
        #endregion

        #region Private Methods
        private void Update()
        {
            if (!Application.isEditor)
            {
                if (Input.GetKeyDown(KeyCode.Z) && InputHelper.IsAnyCtrlOrCommandKeyPressed()) Undo();
                else
                if (Input.GetKeyDown(KeyCode.Y) && InputHelper.IsAnyCtrlOrCommandKeyPressed()) Redo();
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Z) && InputHelper.IsAnyCtrlOrCommandKeyPressed() && InputHelper.IsAnyShiftKeyPressed()) Undo();
                else
                if (Input.GetKeyDown(KeyCode.Y) && InputHelper.IsAnyCtrlOrCommandKeyPressed() && InputHelper.IsAnyShiftKeyPressed()) Redo();
            }
        }

        /// <summary>
        /// Performs an undo operation.
        /// </summary>
        private void Undo()
        {
            // Nothing to undo?
            if (_actionStack.Count == 0 || _actionStackPointer < 0) return;

            if (UndoStart != null) UndoStart();

            // Get the action pointed to by the stack pointer and undo it
            IUndoableAndRedoableAction actionToUndo = _actionStack[_actionStackPointer];
            actionToUndo.Undo();

            // Move the stack pointer backwards
            --_actionStackPointer;

            if (UndoEnd != null) UndoEnd();
        }

        /// <summary>
        /// Performs a redo operation.
        /// </summary>
        private void Redo()
        {
            // Nothing to redo?
            if (_actionStack.Count == 0 || _actionStackPointer == _actionStack.Count - 1) return;

            if (RedoStart != null) RedoStart();

            // Increment the stack pointer to the next command and redo it.
            ++_actionStackPointer;
            IUndoableAndRedoableAction actionToRedo = _actionStack[_actionStackPointer];
            actionToRedo.Redo();

            if (RedoEnd != null) RedoEnd();
        }

        /// <summary>
        /// Changes the action limit to the specified value. If the specified value is smaller than 1,
        /// the limit value will be clamped accordingly.
        /// </summary>
        private void ChangeActionLimit(int newActionLimit)
        {
            // Set the new limit and store the old one because we will need it later.
            // Note: Also make sure that the specified limit is not smaller than 1.
            int oldActionLimit = _actionLimit;
            _actionLimit = Mathf.Max(1, newActionLimit);

            // When the action limit is changed while not running in play mode, there is nothing left to do. However,
            // if we are currently in play mode, we have to make sure that if the new action limit is smaller than the
            // old one, we remove the surplus actions from the stack. For example, if the old action limit is 5, and the
            // new action limit is 4, and the stack points currently points at the last action, we have to remove the
            // first action from the stack (element 0) and adjust the stack pointer.
            if(Application.isPlaying && _actionLimit < oldActionLimit && _actionStackPointer >= _actionLimit)
            {
                // Remove the surplus actions from the stack, but leave the most recent ones intact
                RemoveActionsRange(0, oldActionLimit - _actionLimit);
                
                // Adjust the stack pointer to point to the last action
                _actionStackPointer = _actionStack.Count - 1;
            }
        }

        /// <summary>
        /// Removes the specified range of actions from the stack.
        /// </summary>
        private void RemoveActionsRange(int startIndex, int count)
        {
            // Store a list of actions which will be removed. We'll need this to loop
            // through each action and inform it that it was removed from the stack.
            List<IUndoableAndRedoableAction> actionsToRemove = _actionStack.GetRange(startIndex, count);

            // Remove the actions and then inform them that they were removed
            _actionStack.RemoveRange(startIndex, count);
            foreach (var action in actionsToRemove) action.OnRemovedFromUndoRedoStack();
        }
        #endregion
    }
}
