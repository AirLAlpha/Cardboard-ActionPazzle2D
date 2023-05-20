/**********************************************
 * 
 *  ChallangeManager.cs 
 *  タスク内のチャレンジをまとめる
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/05/14
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallangeManager : MonoBehaviour
{
	//	タスク
	private TaskChallangeData[]			challangeData;			//	タスクの情報
	private TaskChallange[]				challanges;				//	チャレンジの配列

	public TaskChallangeData[] ChallangeData	{ get { return challangeData; } }
	public TaskChallange[] Challanges			{ get { return challanges; } }

	//	更新処理
	private void Update()
	{
		if (challanges == null)
			return;
		
		//	各チャレンジの更新処理
		foreach (var item in challanges)
		{
			item.CheckCompleteUpdate();
		}
	}


	/*--------------------------------------------------------------------------------
	|| タスクの設定
	--------------------------------------------------------------------------------*/
	public void SetTask(TaskInfo task)
	{
		challangeData = task.TaskChallanges;

		var challangesData = task.TaskChallanges;
		challanges = new TaskChallange[challangesData.Length];
		for (int i = 0; i < challanges.Length; i++)
		{
			challanges[i] = CreateChallange(challangesData[i].type, challangesData[i].value);
		}
	}

	/*--------------------------------------------------------------------------------
	|| 新しいチャレンジの作成
	--------------------------------------------------------------------------------*/
	private TaskChallange CreateChallange(ChallangeType type, int value)
	{
		TaskChallange ret = null;

		//	タイプ別にチャレンジの作成
		switch (type)
		{
			case ChallangeType.BOX_COUNT:
				ret = new BoxCountChallange();
				break;

			case ChallangeType.TIME_LIMIT:
				break;

			case ChallangeType.JUMP_COUNT:
				break;

			default:
				return null;
		}

		//	初期化を行う
		ret.InitChallange(value);

		return ret;
	}

}
