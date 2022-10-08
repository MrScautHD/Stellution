using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Mewlist.MassiveClouds
{
    public class MassiveCloudsEditorTab
    {
        private List<string> sections = new List<string>();
        private List<Action<SerializedObject>> actions = new List<Action<SerializedObject>>();
        private float height = 36f;

        public MassiveCloudsEditorTab(float height = 36f)
        {
            this.height = height;
        }

        public int Count { get { return sections.Count;  } }

        public void AddSection(string name, Action<SerializedObject> action)
        {
            sections.Add(name);
            actions.Add(action);
        }

        public void AddSection(string name, Action action)
        {
            sections.Add(name);
            actions.Add(_ => action());
        }

        public int Inspector(int currentSection, SerializedObject serializedObject = null)
        {
            currentSection = GUILayout.Toolbar(currentSection, sections.ToArray(), GUILayout.Height(height));

            using (new EditorGUILayout.VerticalScope(MassiveCloudsEditorGUI.GroupStyle(), GUILayout.Height(height)))
            {
                actions[currentSection].Invoke(serializedObject);
            }

            return currentSection;
        }
    }
}