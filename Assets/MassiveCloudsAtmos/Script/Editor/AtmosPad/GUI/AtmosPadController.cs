using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Mewlist.MassiveClouds
{
    internal class AtmosPadController : EditorController
    {
        private const float PAD_HEIGHT  = 400f;

        private readonly AtmosPadEditor atmosPadEditor;

        private Rect               controlRect;
        private Rect               padAreaRect;

        private IAtmosPadDraggable dragging;
        private IAtmosPadDraggable selected;

        private AtmosProfile       displayedProfile;
        private AtmosProfileEditor displayedProfileEditor;

        private AtmosPadUI    padUi;

        private AtmosPadPointer          pointer;
        private AtmosPad                 atmosPad;
        private List<AtmosPadProfile>    atmosPadProfiles;
        
        internal AtmosPadController(AtmosPadEditor atmosPadEditor)
        {
            this.atmosPadEditor = atmosPadEditor;
        }

        internal void Update(SerializedObject serializedObject)
        {
            atmosPad         = serializedObject.targetObject as AtmosPad;
            atmosPad.UpdateGroup();

            var pointerProperty  = serializedObject.FindProperty("pointer");
            var profilesProperty = serializedObject.FindProperty("profiles");

            pointer = new AtmosPadPointer(pointerProperty);

            atmosPadProfiles = new List<AtmosPadProfile>();
            for (var i = 0; i < profilesProperty.arraySize; i++)
            {
                var profile = new AtmosPadProfile(profilesProperty.GetArrayElementAtIndex(i));
                atmosPadProfiles.Add(profile);
            }
        }

        internal void Inspector(SerializedObject serializedObject)
        {
            var enableSunControl = serializedObject.FindProperty("enableSunControl");
            var enableSkyControl = serializedObject.FindProperty("enableSkyControl");

            DrawPad(serializedObject);

            HandleEvents();

            if (displayedProfile == null && atmosPad.Groups.Any())
            {
                var profile = atmosPad.Groups.First().Profiles.First();
                if (displayedProfile != profile)
                {
                    displayedProfile = profile;
                    displayedProfileEditor = Editor.CreateEditor(profile) as AtmosProfileEditor;
                }
            }

            if (displayedProfile != null)
            {
                if (selected != null)
                {
                    var profile = (selected as AtmosPadProfile).Reference;
                    if (displayedProfile != profile)
                    {
                        displayedProfile = profile;
                        displayedProfileEditor = Editor.CreateEditor(profile) as AtmosProfileEditor;
                    }
                }
            }

            if (displayedProfileEditor != null)
                displayedProfileEditor.OnInspectorGUIInAtmosPad(enableSunControl.boolValue, enableSkyControl.boolValue);
        }


        private void DrawPad(SerializedObject serializedObject)
        {
            controlRect = EditorGUILayout.GetControlRect(true, PAD_HEIGHT, GUIStyle.none);
            padUi = new AtmosPadUI(controlRect);
            padUi.DrawPad();

            var targetGroupList = atmosPad.TargetGroups();

            foreach (var group in atmosPad.Groups)
            {
                var targetGroup = targetGroupList.FirstOrDefault(x => x.atmosGroup == group);
                var color = Color.HSVToRGB(0.2f, 0.2f, (targetGroup != null) ? 0.2f + 0.5f * targetGroup.Weight : 0.2f);
                var groupAlpha = Mathf.Lerp(0.3f, 1f, (targetGroup != null) ? targetGroup.Weight : 0f);
                color.a = groupAlpha;

                padUi.DrawGroup(group, color);

                foreach (var profile in group.Profiles)
                {
                    var weight = 0f;
                    AtmosWeightedProfile atmosWeightedProfile = null;

                    if (targetGroup != null)
                        atmosWeightedProfile = targetGroup.TargetProfiles(pointer.Position)
                            .FirstOrDefault(wp => wp.Profile == profile);

                    if (atmosWeightedProfile != null) weight = atmosWeightedProfile.Weight * groupAlpha;

                    var baseColor  = profile.LabelColor * Mathf.Lerp(0.3f, 1f, weight);
                    var labelColor = baseColor;
                    var nameString = profile.DisplayName;

                    var draggingProfile = (dragging as AtmosPadProfile);
                    var selectedProfile = (selected as AtmosPadProfile);

                    if (draggingProfile != null && draggingProfile.Reference == profile)
                    {
                        labelColor = profile.LabelColor;
                        nameString = profile.Position.x.ToTimeString();
                    }

                    if (selectedProfile != null && profile == selectedProfile.Reference)
                        labelColor = Color.yellow;

                    padUi.DrawProfile(profile, nameString, baseColor, labelColor);
                }
            }

            padUi.DrawCursor(pointer.Position);
        }

        protected override void OnMouseDown(Event evt)
        {
            var position = padUi.ToLocalPosition(Event.current.mousePosition);

            dragging = null;

            if (!AtmosPadUI.InRange(position)) return;

            var targetList = atmosPadProfiles.Where(x => x.Intersect(position)).ToList();
            if (targetList.Any())
            {
                var target = targetList.First();
                var isSelectedTarget = selected != null && selected.Equals(target);
                dragging = isSelectedTarget ? (IAtmosPadDraggable)target : pointer;
                selected = isSelectedTarget ? selected : target;
            }
            else
            {
                dragging = pointer;
                selected = null;
            }
            Repaint();
        }

        protected override void OnMouseDrag(Event evt)
        {
            var position = padUi.ToLocalPosition(Event.current.mousePosition);

            selected = null;

            if (dragging == null) return;

            var profile = dragging as AtmosPadProfile;
            if (profile != null) Undo.RecordObject(profile.Reference, "Changed Profile Position");

            dragging.Update(new Vector2(Mathf.Clamp01(position.x), Mathf.Clamp01(position.y)));
            Repaint();
        }

        protected override void OnMouseUp(Event evt)
        {
            if (dragging == null) return;

            if (dragging is AtmosPadProfile) selected = dragging;
            dragging = null;
            Repaint();
        }

        protected override void OnDragPerform(Event evt)
        {
            var position = padUi.ToLocalPosition(Event.current.mousePosition);
            var objs = DragAndDrop.objectReferences.OfType<AtmosProfile>().ToArray();

            if (!objs.Any() || !AtmosPadUI.InRange(position)) return;

            Undo.RecordObject(atmosPad, "Add Profile");

            foreach (var obj in objs)
            {
                if (atmosPad.Contains(obj))
                {
                    EditorUtility.DisplayDialog("Error", obj.name + " is already placed on AtmosPad.", "OK");
                }
                else
                {
                    atmosPad.Add(obj);
                }
            }

        }

        protected override void OnDragUpdated(Event evt)
        {
            var position = padUi.ToLocalPosition(Event.current.mousePosition);

            DragAndDrop.AcceptDrag();
            if (AtmosPadUI.InRange(position))
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

        }

        protected override void OnContextClick(Event evt)
        {
            var position = padUi.ToLocalPosition(Event.current.mousePosition);
            var contextTargets = atmosPadProfiles
                .Where(x => x.Intersect(position))
                .ToArray();

            if (contextTargets.Any())
            {
                Undo.RecordObject(atmosPad, "Remove Profile");
                var contextTarget = contextTargets.First() as AtmosPadProfile;
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Remove"), false, _ => atmosPad.Remove(contextTarget.Reference), "item 1");
                menu.AddItem(new GUIContent("RemoveAll"), false, _ => atmosPad.RemoveAll(), "item All");
                menu.ShowAsContext();
            }
            evt.Use();
        }

        private void Repaint()
        {
            atmosPadEditor.Repaint();
        }
    }
}