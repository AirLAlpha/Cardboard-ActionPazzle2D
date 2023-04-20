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
	public Vector3			pos;				//	座標

	//	コンストラクタ
	public StageObjectData(int databaseIndex, Vector3 pos)
	{
		this.databaseIndex = databaseIndex;
		this.pos = pos;
	}
}

[System.Serializable]
public class GimmickObjectData : StageObjectData
{
	public GimmickType		type;				//	ギミックタイプ
	public int				targetIndex;        //	イベントの追加対象

	//	コンストラクタ
	public GimmickObjectData(int databaseIndex, Vector3 pos, GimmickType type, int targetIndex) 
		: base(databaseIndex, pos)
	{
		this.type = type;
		this.targetIndex = targetIndex;
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

	//	コンストラクタ
	public StageData(List<StageObjectData> objectDatas, List<GimmickObjectData> gimmickData) 
	{ 
		this.objectDatas = objectDatas;
		this.gimmickDatas = gimmickData;
	}

}
