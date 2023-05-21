using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//	タスク内のチャレンジタイプ
public enum ChallangeType
{
	NONE,

	BOX_COUNT,		//	箱の数
	TIME_LIMIT,     //	時間制限
	JUMP_COUNT,		//	ジャンプ回数

	OVER_ID
}
//	タスク内のチャレンジを定義する構造体
[System.Serializable]
public struct TaskChallangeData
{
	public ChallangeType	type;		//	タイプ
	public int				value;		//	数値
}


//	シーン1つ分の構造体
[System.Serializable]
public struct TaskInfo
{
	//	シーン情報
	[SerializeField]
	private TextAsset stageJson;

	//	タスク内のチャレンジ
	[SerializeField]
	private TaskChallangeData[] challanges;

	//	プロパティ
	public TaskChallangeData[]	TaskChallanges	{ get { return challanges; } }		//	タスク内のチャレンジ
	public TextAsset StageJson { get { return stageJson; } }
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