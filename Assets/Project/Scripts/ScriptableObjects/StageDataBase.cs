/**********************************************
 * 
 *  StageDataBase.cs 
 *  各ステージの情報を保持するデータベース
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/02
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//	シーン1つ分の構造体
[System.Serializable]
public struct StageData
{
	//	シーン情報

	[SerializeField]
	private int stageId;			//	ステージID（シーン名 "Stage_" + stageId）

	//	ステージ情報
	[SerializeField]
	private int usableBoxCount;		//	ステージ内で使用可能な箱の数

	//	プロパティ
	public int StageID			{ get { return stageId; } }				//	ステージID
	public int UsableBoxCount	{ get { return usableBoxCount; } }		//	使用可能な箱の数
	public string SceneName => "Stage_" + stageId.ToString();			//	シーン名
}


[CreateAssetMenu(fileName = "StageDataBase", menuName ="Create StageDataBase")]
public class StageDataBase : ScriptableObject
{
	[SerializeField]
	List<StageData> stageList;							//	ステージデータの配列

	public int StageCount => stageList.Count;		//	ステージの数

	//	プロパティ
	public List<StageData> StageList { get { return stageList; } }
}
