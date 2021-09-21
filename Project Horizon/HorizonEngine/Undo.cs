using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace HorizonEngine
{
    internal static class Undo
    {
        private class UndoAction
        {
            public object target;
            public object undoValue;
            public object redoValue;
            public string propertyName;
        }

        private static Stack<UndoAction> _undoStack;
        private static Stack<UndoAction> _redoStack;

        static Undo()
        {
            _undoStack = new Stack<UndoAction>();
            _redoStack = new Stack<UndoAction>();
        }

        internal static bool canUndo
        {
            get
            {
                return _undoStack.Count > 0;
            }
        }

        internal static bool canRedo
        {
            get
            {
                return _redoStack.Count > 0;
            }
        }

        internal static void RegisterAction(object target, object undoValue, object redoValue, string propertyName)
        {
            UndoAction peek = _undoStack.Count > 0 ? _undoStack.Peek() : null;
            if(peek != null && peek.target == target && peek.propertyName == propertyName)
            {
                peek.redoValue = redoValue;
            }
            else
            {
                UndoAction undoAction = new UndoAction();
                undoAction.target = target;
                undoAction.undoValue = undoValue;
                undoAction.redoValue = redoValue;
                undoAction.propertyName = propertyName;
                _undoStack.Push(undoAction);
            }
            _redoStack.Clear();
        }

        internal static void PerformRedo()
        {
            UndoAction action = _redoStack.Pop();
            var prop = action.target.GetType().GetProperty(action.propertyName);
            prop.SetValue(action.target, action.redoValue);
            _undoStack.Push(action);
        }

        internal static void PerformUndo()
        {
            UndoAction action = _undoStack.Pop();
            var prop = action.target.GetType().GetProperty(action.propertyName);
            prop.SetValue(action.target, action.undoValue);
            _redoStack.Push(action);
        }

        internal static void Reset()
        {
            _undoStack.Clear();
            _redoStack.Clear();
        }
    }
}
