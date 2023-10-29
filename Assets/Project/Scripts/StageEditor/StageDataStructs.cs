/**********************************************
 * 
 *  StageDataStructs.cs 
 *  ステージの書き出しに使用する構造体を記述
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/18
 * 
 **********************************************/
using JetBrains.Annotations;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental;
using UnityEngine;

//	ステージオブジェクトとして書き出すための構造体
[System.Serializable]
public class StageObjectData
{
	public int				databaseIndex;		//	データベースにおけるインデックス番号
	public Vector3			pos;                //	座標
	public Quaternion		rot;                //	回転

	//	コンストラクタ
	public StageObjectData(int databaseIndex, Vector3 pos, Quaternion rot)
	{
		this.databaseIndex = databaseIndex;
		this.pos = pos;
		this.rot = rot;
	}
}

[System.Serializable]
public class GimmickObjectData : StageObjectData
{
	public GimmickType		type;				//	ギミックタイプ
	public int				targetIndex;        //	イベントの追加対象
	public string			extraSetting;		//	ギミック固有の設定（JSON形式）

	//	コンストラクタ
	public GimmickObjectData(int databaseIndex, Vector3 pos, Quaternion rot, GimmickType type, int targetIndex, string extraSetting = "") 
		: base(databaseIndex, pos, rot)
	{
		this.type = type;
		this.targetIndex = targetIndex;
		this.extraSetting = extraSetting;
	}
}

//	ステージ１つの構造体
[System.Serializable]
public struct StageData
{
	public List<StageObjectData>	objectDatas;			//	ステージオブジェクトの配列
	public int DataCount => objectDatas.Count;              //	ステージオブジェクトの数

	public List<GimmickObjectData>	gimmickDatas;
	public int GimmickCount => gimmickDatas.Count;

	public int usableBoxCount;
	public int targetBoxCount;
	public int backgroundType;

	//	コンストラクタ
	public StageData(List<StageObjectData> objectDatas, 
		List<GimmickObjectData> gimmickDatas, 
		int usableBoxCount, 
		int targetBoxCount,
		int backgroundType) 
	{ 
		this.objectDatas = objectDatas;
		this.gimmickDatas = gimmickDatas;
		this.usableBoxCount = usableBoxCount;
		this.targetBoxCount = targetBoxCount;
		this.backgroundType = backgroundType;
	}

}
