//using UnityEngine;
//using System.Collections;
//using UnityEditor;
//
//[CustomEditor(typeof(GameLogic))]
//public class GameLogicCustomEditor : Editor {
//	string[] _levelOptions = LevelSettings.LevelOption.OPTION_KEYS.ToArray();
//	int _levelIndex;
//
//	public override void OnInspectorGUI ()
//	{
//		// Draw the default inspector
//		DrawDefaultInspector();
//		_levelIndex = EditorGUILayout.Popup(_levelIndex, _levelOptions);
//		GameLogic gameLogic = target as GameLogic;
//		// Update the selected choice in the underlying object
//		gameLogic.levelOptionIndex = _levelIndex;
//		// Save the changes back to the object
//		EditorUtility.SetDirty(target);
//	}
//}
