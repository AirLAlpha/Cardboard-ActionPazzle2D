using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//	シーン1つ分の構造体
[System.Serializable]
public struct TaskInfo
{
	//	シーン情報
	[SerializeField]
	private string stageFileName;

	//	プロパティ
	public string SceneFileName { get { return stageFileName; } }              //	ステージID
}

//	1ステージ分のタスクをまとめるオブジェクト
[CreateAssetMenu(fileName = "StageData", menuName = "Create StageData")]
public class StageInfo : ScriptableObject
{
	[SerializeField]
	private int stageId;
	[SerializeField]
	private TaskInfo[] tasks;       //	タスクの配列

	public int TaskCount => tasks.Length;
	public int StageID { get { return stageId; } }
	public TaskInfo[] Tasks { get { return tasks; } }
}