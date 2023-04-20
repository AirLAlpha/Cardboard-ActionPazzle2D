using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GameReset
{
	static string selectedTaskDataPath = "Assets/Project/ScriptableObjects/SelectedStageData.asset";

	[MenuItem("SelectStageData/SelectStageReset")]
	static private void SelectStageReset()
	{
		var task = AssetDatabase.LoadAssetAtPath<SelectedTaskData>(selectedTaskDataPath);

		task.StageID = -1;
		task.TaskIndex = 0;
	}

	[MenuItem("SelectStageData/Ping")]
	static private void Ping()
	{
		var task = AssetDatabase.LoadAssetAtPath<SelectedTaskData>(selectedTaskDataPath);
		EditorGUIUtility.PingObject(task);
	}

	[MenuItem("SelectStageData/SetDebugStage")]
	static private void SetDebugStage()
	{
		var task = AssetDatabase.LoadAssetAtPath<SelectedTaskData>(selectedTaskDataPath);

		task.StageID = 0;
		task.TaskIndex = 0;
	}
}
