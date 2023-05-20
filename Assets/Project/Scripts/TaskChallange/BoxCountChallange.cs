/**********************************************
 * 
 *  BoxCountChallange.cs 
 *  ハコの数に対するチャレンジ
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/05/14
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BoxCountChallange : TaskChallange
{
	private int challangeBoxCount;		//	チャレンジを達成できる箱の数

	public override void InitChallange(int value)
	{
		challangeBoxCount = value;
	}

	public override void CheckCompleteUpdate()
	{
		//	現在使用したハコの数が目標数になったら失敗
		if (challangeBoxCount <= StageManager.Instance.UsedBoxCount)
			IsFaild = true;
	}

	public override float GetChallangeProgress()
	{
		return 1.0f - (float)StageManager.Instance.UsedBoxCount / challangeBoxCount;
	}

	public override int GetChallangeValue()
	{
		return challangeBoxCount;
	}
}
