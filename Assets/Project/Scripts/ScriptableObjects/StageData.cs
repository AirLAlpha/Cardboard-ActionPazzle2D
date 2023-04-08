using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//	シーン1つ分の構造体
[System.Serializable]
public struct TaskData
{
	//	シーン情報
	[SerializeField]
	private SceneObject scene;

	//	ステージ情報
	[SerializeField]
	private int usableBoxCount;     //	ステージ内で使用可能な箱の数
	[SerializeField]
	private int targetBoxCount;     //	ゴールさせるべき箱の数


	//	プロパティ
	public SceneObject Scene { get { return scene; } }              //	ステージID
	public int UsableBoxCount { get { return usableBoxCount; } }        //	使用可能な箱の数
	public int TargetBoxCount { get { return targetBoxCount; } }        //	使用可能な箱の数
}

//	1ステージ分のタスクをまとめるオブジェクト
[CreateAssetMenu(fileName = "StageData", menuName = "Create StageData")]
public class StageData : ScriptableObject
{
	[SerializeField]
	private int stageId;
	[SerializeField]
	private TaskData[] tasks;       //	タスクの配列

	public int StageID { get { return stageId; } }
	public TaskData[] Tasks { get { return tasks; } }
}