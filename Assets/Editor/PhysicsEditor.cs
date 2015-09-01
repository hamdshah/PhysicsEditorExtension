using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;


[CustomEditor(typeof(Polygon))]
public class PhysicsEditor : Editor {
	
	SerializedProperty polygonCollider;
	SerializedProperty plistPath;
	SerializedProperty pixelsPerUnit;
	SerializedProperty totalPolygonsinFile;
	SerializedProperty selectedIndex;

	public void OnEnable () {
		polygonCollider = serializedObject.FindProperty("_polygonCollider");
		plistPath = serializedObject.FindProperty("PlistPath");
		pixelsPerUnit = serializedObject.FindProperty("PixelsPerUnit");
		totalPolygonsinFile = serializedObject.FindProperty("_totalPolygonsinFile");
		selectedIndex = serializedObject.FindProperty("selectedIndex");
	}

	public override void OnInspectorGUI ()
	{

		serializedObject.Update();

		Polygon currentPolygonObject = (Polygon) target;

		GUI.changed = false;

		EditorGUILayout.PropertyField(polygonCollider, new GUIContent("Polygon Collider"));
		EditorGUILayout.PropertyField(plistPath, new GUIContent("Plist Path"));

		/*If file path is changed*/
		if(GUI.changed) {
//			Debug.Log("file property is changed");
			serializedObject.ApplyModifiedProperties();
			currentPolygonObject.ParsePolygonsFromFile();

		}

		//pixelsPerUnit =  EditorGUILayout.FloatField("Pixels Per Unit",80.0f);

//		EditorGUILayout.BeginVertical();
//
//		for(int i = 0; i<polygons.arraySize; i++) {
//			EditorGUILayout.PropertyField(polygons.GetArrayElementAtIndex(i));
//		}
//
//		EditorGUILayout.EndVertical();


		/*Parse Bodies Name and show in Popup*/

			string[] bodiesNames = new string[totalPolygonsinFile.arraySize];
			for(int i= 0; i<totalPolygonsinFile.arraySize; i++){
				SerializedProperty temppolygonObject = totalPolygonsinFile.GetArrayElementAtIndex(i);
				SerializedProperty tempbodyname = temppolygonObject.FindPropertyRelative("bodyname");
				if(tempbodyname != null) {
					bodiesNames[i] = "" + tempbodyname.stringValue;
				}

			}	

			GUI.changed = false;
			selectedIndex.intValue = EditorGUILayout.Popup("Bodies",selectedIndex.intValue,bodiesNames);

			/*If Popup  is changed*/
			
			if(GUI.changed) {
//				Debug.Log("Popup is changed");
				serializedObject.ApplyModifiedProperties();
				currentPolygonObject.setPolygonOfIndex(selectedIndex.intValue);
			}
			



		GUI.changed = false;

		EditorGUILayout.PropertyField(pixelsPerUnit,new GUIContent("Pixels Per Unit"));

		/*If Popup  is changed*/
		
		if(GUI.changed) {
//			Debug.Log("Pixels changed is changed");
			serializedObject.ApplyModifiedProperties();
			currentPolygonObject.setPolygonOfIndex(selectedIndex.intValue);
		}


//		if(GUILayout.Button("Update Mesh")) {
//
//			Polygon testing = (Polygon) target;
//
//			testing.ParsePolygonsFromFile();
//			return;
//		}

		serializedObject.ApplyModifiedProperties();
	}
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
