/**********************************************
 * 
 *  TaskChallange.cs 
 *  各タスク内のチャレンジに関する基底クラス
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/05/14
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class TaskChallange
{
	private bool isFaild;		//	失敗フラグ
	public bool IsFaild { get { return isFaild; } protected set { isFaild = value; } }

	public abstract void	InitChallange(int value);		//	目標の数値を設定
	public abstract void	CheckCompleteUpdate();			//	タスクの完了確認処理
	public abstract float	GetChallangeProgress();       //	タスクの進行度を取得する
	public abstract int		GetChallangeValue();
}
