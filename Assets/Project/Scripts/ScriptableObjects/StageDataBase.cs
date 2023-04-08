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

//	ステージをまとめるオブジェクト
[CreateAssetMenu(fileName = "StageDataBase", menuName ="Create StageDataBase")]
public class StageDataBase : ScriptableObject
{
	[SerializeField]
	StageData[] stages;								//	ステージデータの配列

	public int StageCount => stages.Length;		//	ステージの数

	//	プロパティ
	public StageData[] Stages { get { return stages; } }
}