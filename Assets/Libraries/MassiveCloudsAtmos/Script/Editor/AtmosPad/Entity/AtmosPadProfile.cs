using UnityEditor;
using UnityEngine;

namespace Mewlist.MassiveClouds
{
    internal class AtmosPadProfile : IAtmosPadDraggable
    {
        private const float RANGE = 0.05f;

        private readonly SerializedObject serializedObject;
        private readonly AtmosProfile     profile;

        public Vector2      Position { get; private set; }
        public string       Name { get; private set; }

        public AtmosProfile Reference { get { return profile; } }

        public AtmosPadProfile(SerializedProperty property)
        {
            profile          = property.objectReferenceValue as AtmosProfile;
            serializedObject = new SerializedObject(profile);
            Position         = profile.Position;
            Name             = profile.DisplayName;
        }

        public bool Intersect(Vector2 point)
        {
            return (point - Position).magnitude < RANGE;
        }

        public void Update(Vector2 point)
        {
            Position         = point;
            profile.Position = point;
            serializedObject.FindProperty("position").vector2Value = point;
            serializedObject.ApplyModifiedProperties();
        }

        private bool Equals(AtmosPadProfile other)
        {
            return Equals(profile, other.profile) && RANGE.Equals(RANGE) && Position.Equals(other.Position) && Name == other.Name;
        }

        public bool Equals(IAtmosPadDraggable other)
        {
            var o = (AtmosPadProfile) other;
            if (o == null) return false;
            return Equals(o);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((AtmosPadProfile) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (serializedObject != null ? serializedObject.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (profile != null ? profile.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Position.GetHashCode();
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}