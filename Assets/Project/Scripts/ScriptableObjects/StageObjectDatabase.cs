/**********************************************
 * 
 *  StageObjectDatabase.cs 
 *  ステージオブジェクトのデータベース
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/17
 * 
 **********************************************/
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//	ステージ上のオブジェクトタイプ
public enum ObjectType
{
	NONE,           //	設定なし

	TILE,           //	タイルマップ
	GIMMICK,        //	ギミックオブジェクト
	ENEMY,          //	敵
}

//	オブジェクト一つ分の構造体
[System.Serializable]
public struct StageObject
{
	public ObjectType	type;				//	オブジェクトタイプ
	public Object		prefab;				//	もととなるオブジェクト
}

[CreateAssetMenu(fileName = "StageObjectDatabase", menuName = "Create StageObjectDatabase")]
public class StageObjectDatabase : ScriptableObject
{
	[SerializeField]
	private List<StageObject>		stageObjects;		//	ステージオブジェクトの配列 

	public List<StageObject> StageObject { get { return stageObjects; } }       //	オブジェクトの配列を取得

	/*--------------------------------------------------------------------------------s
	|| 指定のオブジェクトと同じものを検索する
	--------------------------------------------------------------------------------*/
	public int FindObject(Object obj)
	{
		return stageObjects.FindIndex(stageObj => stageObj.prefab == obj);
	}
	public int FindObject(string name)
	{
		return stageObjects.FindIndex(stageObj => stageObj.prefab.name == name);
	}
}
