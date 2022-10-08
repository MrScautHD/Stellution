using UnityEditor;
using UnityEngine;

namespace Mewlist.MassiveClouds
{
    internal class AtmosPadPointer : IAtmosPadDraggable
    {
        private readonly SerializedProperty property;

        public Vector2 Position { get; private set; }
        public string  Name     { get; }

        public AtmosPadPointer(SerializedProperty property)
        {
            Name = "Pointer";
            this.property = property;
            Position = property.vector2Value;
        }

        public bool Intersect(Vector2 point)
        {
            return (point - Position).magnitude < 0.05f;
        }

        public void Update(Vector2 point)
        {
            Position = point;
            property.vector2Value = point;
        }

        private bool Equals(AtmosPadPointer other)
        {
            return Position.Equals(other.Position) && Name == other.Name;
        }

        public bool Equals(IAtmosPadDraggable other)
        {
            var o = (AtmosPadPointer) other;
            if (o == null) return false;
            return Equals(o);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AtmosPadPointer) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (property != null ? property.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Position.GetHashCode();
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}