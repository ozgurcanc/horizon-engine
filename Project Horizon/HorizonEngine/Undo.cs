using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System.Reflection;

namespace HorizonEngine
{
    internal static class Undo
    {
        private abstract class UndoAction
        {
            internal abstract void Undo();
            internal abstract void Redo();
        }
        private class PropertyAction : UndoAction
        {
            public object target;
            public object undoValue;
            public object redoValue;
            public string propertyName;

            internal override void Redo()
            {
                var prop = target.GetType().GetProperty(propertyName);
                prop.SetValue(target, redoValue);
            }

            internal override void Undo()
            {
                var prop = target.GetType().GetProperty(propertyName);
                prop.SetValue(target, undoValue);
            }
        }

        private class GameObjectAction : UndoAction
        {
            public GameObject parent;
            public GameObject gameObject;
            public bool isCreateAction;
            internal override void Redo()
            {
                if(isCreateAction)
                {
                    gameObject.OnLoad();
                    gameObject.parent = parent;
                }
                else
                {
                    Scene.Destroy(gameObject);
                }
            }

            internal override void Undo()
            {
                if (isCreateAction)
                {
                    Scene.Destroy(gameObject);
                }
                else
                {
                    gameObject.OnLoad();
                    gameObject.parent = parent;
                }
            }
        }

        private class ComponentAction : UndoAction
        {
            public GameObject gameObject;
            public Component component;
            public bool isCreateAction;

            internal override void Redo()
            {
                if(isCreateAction)
                {
                    gameObject.UndoComponent(component);
                }
                else
                {
                    MethodInfo method = typeof(GameObject).GetMethod(nameof(GameObject.RemoveComponent));
                    MethodInfo generic = method.MakeGenericMethod(component.GetType());
                    generic.Invoke(gameObject, null);
                }
            }

            internal override void Undo()
            {
                if (isCreateAction)
                {
                    MethodInfo method = typeof(GameObject).GetMethod(nameof(GameObject.RemoveComponent));
                    MethodInfo generic = method.MakeGenericMethod(component.GetType());
                    generic.Invoke(gameObject, null);                 
                }
                else
                {
                    gameObject.UndoComponent(component);
                }
            }
        }

        private static Stack<UndoAction> _undoStack;
        private static Stack<UndoAction> _redoStack;
        private static int _sceneSavePoint;

        static Undo()
        {
            _undoStack = new Stack<UndoAction>();
            _redoStack = new Stack<UndoAction>();
        }

        internal static bool canUndo
        {
            get
            {
                return _undoStack.Count > 0 && !GameWindow.isPlaying;
            }
        }

        internal static bool canRedo
        {
            get
            {
                return _redoStack.Count > 0 && !GameWindow.isPlaying;
            }
        }

        internal static bool isSceneModified
        {
            get
            {
                return _sceneSavePoint != _undoStack.Count;
            }
        }

        internal static void RegisterAction(object target, object undoValue, object redoValue, string propertyName)
        {
            if (GameWindow.isPlaying) return;

            if (_sceneSavePoint > _undoStack.Count) _sceneSavePoint = -1;

            PropertyAction undoAction = new PropertyAction();
            undoAction.target = target;
            undoAction.undoValue = undoValue;
            undoAction.redoValue = redoValue;
            undoAction.propertyName = propertyName;
            _undoStack.Push(undoAction);
            _redoStack.Clear();
        }

        internal static void RegisterAction(GameObject gameObject, bool isCreateAction)
        {
            if (GameWindow.isPlaying) return;

            if (_sceneSavePoint > _undoStack.Count) _sceneSavePoint = -1;

            GameObjectAction undoAction = new GameObjectAction();
            undoAction.parent = gameObject.parent;
            undoAction.gameObject = gameObject;
            undoAction.isCreateAction = isCreateAction;
            _undoStack.Push(undoAction);
            _redoStack.Clear();
        }

        internal static void RegisterAction(Component component, bool isCreateAction)
        {
            if (GameWindow.isPlaying) return;

            if (_sceneSavePoint > _undoStack.Count) _sceneSavePoint = -1;

            ComponentAction undoAction = new ComponentAction();
            undoAction.gameObject = component.gameObject;
            undoAction.component = component;
            undoAction.isCreateAction = isCreateAction;
            _undoStack.Push(undoAction);
            _redoStack.Clear();
        }

        internal static void PerformRedo()
        {
            UndoAction action = _redoStack.Pop();
            action.Redo();
            _undoStack.Push(action);
        }

        internal static void PerformUndo()
        {
            UndoAction action = _undoStack.Pop();
            action.Undo();
            _redoStack.Push(action);
        }

        internal static void Reset()
        {
            _undoStack.Clear();
            _redoStack.Clear();
            _sceneSavePoint = 0;
        }

        internal static void SceneSaved()
        {
            _sceneSavePoint = _undoStack.Count;
        }
    }
}
