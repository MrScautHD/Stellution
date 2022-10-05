using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;


[CustomEditor(typeof(EnviroZone))]
public class EnviroEnviroZoneEditor : Editor {

	GUIStyle boxStyle;
    GUIStyle boxStyleModified;
    GUIStyle wrapStyle;
	GUIStyle clearStyle;
    GUIStyle headerStyle;
    GUIStyle headerFoldout;
    GUIStyle popUpStyle;

    EnviroZone myTarget;

	bool showGizmo = true;
	bool showGeneral = true;
	bool showWeather = true;

    private Color boxColor1;

    ReorderableList weatherList;


	void OnEnable()
	{
		myTarget = (EnviroZone)target;

		weatherList = new ReorderableList(serializedObject,serializedObject.FindProperty("zoneWeatherPresets"),true, true, true, true);
		weatherList.drawHeaderCallback = (Rect rect) =>
		{
			EditorGUI.LabelField(rect, "Weather Presets:");
		};

		weatherList.drawElementCallback =  
			(Rect rect, int index, bool isActive, bool isFocused) => {
			var element = weatherList.serializedProperty.GetArrayElementAtIndex(index);
			rect.y += 2;
			EditorGUI.PropertyField(new Rect(rect.x, rect.y, Screen.width * 0.8f, EditorGUIUtility.singleLineHeight),element,GUIContent.none);
		};

		weatherList.onAddCallback = (ReorderableList l) =>
		{
			var index = l.serializedProperty.arraySize;
			l.serializedProperty.arraySize++;
			l.index = index;
			//var element = l.serializedProperty.GetArrayElementAtIndex(index);
		};

#if UNITY_2019_3_OR_NEWER
        boxColor1 = new Color(0.95f, 0.95f, 0.95f,1f);
#else
        boxColor1 = new Color(0.85f, 0.85f, 0.85f, 1f);
#endif
    }

    public override void OnInspectorGUI ()
	{
		
		myTarget = (EnviroZone)target;

		//Set up the box style
		if (boxStyle == null)
		{
			boxStyle = new GUIStyle(GUI.skin.box);
			boxStyle.normal.textColor = GUI.skin.label.normal.textColor;
			boxStyle.fontStyle = FontStyle.Bold;
			boxStyle.alignment = TextAnchor.UpperLeft;
		}

        if (boxStyleModified == null)
        {
            boxStyleModified = new GUIStyle(EditorStyles.helpBox);
            boxStyleModified.normal.textColor = GUI.skin.label.normal.textColor;
            boxStyleModified.fontStyle = FontStyle.Bold;
            boxStyleModified.fontSize = 11;
            boxStyleModified.alignment = TextAnchor.UpperLeft;
        }

        //Setup the wrap style
        if (wrapStyle == null)
		{
			wrapStyle = new GUIStyle(GUI.skin.label);
			wrapStyle.fontStyle = FontStyle.Normal;
			wrapStyle.wordWrap = true;
		}

		if (clearStyle == null) {
			clearStyle = new GUIStyle(GUI.skin.label);
			clearStyle.normal.textColor = GUI.skin.label.normal.textColor;
			clearStyle.fontStyle = FontStyle.Bold;
			clearStyle.alignment = TextAnchor.UpperRight;
		}

        if (headerStyle == null)
        {
            headerStyle = new GUIStyle(GUI.skin.label);
            headerStyle.fontStyle = FontStyle.Bold;
            headerStyle.alignment = TextAnchor.UpperLeft;
        }

        if (headerFoldout == null)
        {
            headerFoldout = new GUIStyle(EditorStyles.foldout);
            headerFoldout.fontStyle = FontStyle.Bold;
        }

        if (popUpStyle == null)
        {
            popUpStyle = new GUIStyle(EditorStyles.popup);
            popUpStyle.alignment = TextAnchor.MiddleCenter;
            popUpStyle.fixedHeight = 20f;
            popUpStyle.fontStyle = FontStyle.Bold;
        }

        EditorGUI.BeginChangeCheck();

        // Begin
        GUILayout.BeginVertical("", boxStyle);
        EditorGUILayout.LabelField("Enviro - Weather Zone", headerStyle);
        EditorGUILayout.LabelField("Add your weather presets for this biome here. You can have multiple zones and each zone will have its own current weather state. Once your player enters the zone weather will change.", wrapStyle);

        // General Setup
        GUI.backgroundColor = boxColor1;
        GUILayout.BeginVertical("", boxStyleModified);
        GUI.backgroundColor = Color.white;
        showGeneral = GUILayout.Toggle(showGeneral, "General Configs", headerFoldout);
        if (showGeneral) {
            myTarget.zoneName = EditorGUILayout.TextField("Zone Name", myTarget.zoneName);
            GUILayout.Space(5);
            myTarget.useMeshZone = EditorGUILayout.Toggle("Use Custom Mesh", myTarget.useMeshZone);
            if(myTarget.useMeshZone)
            myTarget.zoneMesh = (Mesh)EditorGUILayout.ObjectField("Mesh", myTarget.zoneMesh,typeof(Mesh),false);
			myTarget.zoneScale = EditorGUILayout.Vector3Field ("Zone Scale", myTarget.zoneScale);
            GUILayout.Space(5);
            myTarget.ExitToDefault = EditorGUILayout.Toggle("Exit to Default Zone", myTarget.ExitToDefault);
        }
		EditorGUILayout.EndVertical ();

        // Weather Setup
        GUI.backgroundColor = boxColor1;
        GUILayout.BeginVertical("", boxStyleModified);
        GUI.backgroundColor = Color.white;
        showWeather = GUILayout.Toggle(showWeather, "Weather Configs", headerFoldout);
		if (showWeather) {

			//GUILayout.Space(15);
			myTarget.updateMode = (EnviroZone.WeatherUpdateMode)EditorGUILayout.EnumPopup ("Weather Update Mode", myTarget.updateMode);
			myTarget.WeatherUpdateIntervall = EditorGUILayout.FloatField ("Weather Update Interval", myTarget.WeatherUpdateIntervall);
            myTarget.forceWeatherChange = EditorGUILayout.Toggle("Force Weather Changes", myTarget.forceWeatherChange);

            GUILayout.Space(10);
			weatherList.DoLayoutList();
			serializedObject.ApplyModifiedProperties();
		}
		EditorGUILayout.EndVertical ();


        // Gizmo Setup
        GUI.backgroundColor = boxColor1;
        GUILayout.BeginVertical("", boxStyleModified);
        GUI.backgroundColor = Color.white;
        showGizmo = GUILayout.Toggle(showGizmo, "Gizmo", headerFoldout);
		if (showGizmo) {
			myTarget.zoneGizmoColor = EditorGUILayout.ColorField ("Gizmo Color", myTarget.zoneGizmoColor);
		}
		EditorGUILayout.EndVertical ();


		// END
		EditorGUILayout.EndVertical ();

        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(target);
        }
    }
}
