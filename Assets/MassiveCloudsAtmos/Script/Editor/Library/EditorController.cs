using System;
using UnityEngine;

namespace Mewlist.MassiveClouds
{
    public abstract class EditorController
    {
        protected void HandleEvents()
        {
            var evt = Event.current;
            switch (Event.current.type)
            {
                case EventType.MouseDown:
                    OnMouseDown(evt);
                    break;
                case EventType.MouseUp:
                    OnMouseUp(evt);
                    break;
                case EventType.MouseMove:
                    OnMouseMove(evt);
                    break;
                case EventType.MouseDrag:
                    OnMouseDrag(evt);
                    break;
                case EventType.KeyDown:
                    break;
                case EventType.KeyUp:
                    break;
                case EventType.ScrollWheel:
                    break;
                case EventType.Repaint:
                    OnRepaint(evt);
                    break;
                case EventType.Layout:
                    break;
                case EventType.DragUpdated:
                    OnDragUpdated(evt);
                    break;
                case EventType.DragPerform:
                    OnDragPerform(evt);
                    break;
                case EventType.DragExited:
                    break;
                case EventType.Ignore:
                    break;
                case EventType.Used:
                    break;
                case EventType.ValidateCommand:
                    break;
                case EventType.ExecuteCommand:
                    break;
                case EventType.ContextClick:
                    OnContextClick(evt);
                    break;
                case EventType.MouseEnterWindow:
                    break;
                case EventType.MouseLeaveWindow:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected virtual void OnMouseDown(Event evt) { }
        protected virtual void OnMouseUp(Event evt) { }
        protected virtual void OnMouseMove(Event evt) { }
        protected virtual void OnMouseDrag(Event evt) { }
        protected virtual void OnRepaint(Event evt) { }
        protected virtual void OnDragUpdated(Event evt) { }
        protected virtual void OnDragPerform(Event evt) { }
        protected virtual void OnContextClick(Event evt) { }
    }
}