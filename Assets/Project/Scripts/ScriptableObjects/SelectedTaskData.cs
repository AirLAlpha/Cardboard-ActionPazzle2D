using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//	選択済みのステージを保持するオブジェクト
[CreateAssetMenu(fileName = "SelectedStage", menuName = "Create SelectedStageData")]
public class SelectedTaskData : ScriptableObject
{
	[SerializeField]
	private int stageId;        //	ステージID
	[SerializeField]
	private int taskIndex;      //	タスクのインデックス

	public int StageID { get { return stageId; } set { stageId = value; } }
	public int TaskIndex { get { return taskIndex; } set { taskIndex = value; } }
}
