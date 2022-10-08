using System;
using UnityEngine;

namespace Mewlist.MassiveClouds
{
    internal interface IAtmosPadDraggable : IEquatable<IAtmosPadDraggable>
    {
        Vector2 Position { get; }
        string Name { get; }
        bool Intersect(Vector2 point);
        void Update(Vector2 point);
    }
}