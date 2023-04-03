/**********************************************
 * 
 *  TitleManager.cs 
 *  タイトルを統括する処理を記述
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/04
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
	//	ステージデータベース
	[SerializeField]
	private StageDataBase		stageDataBase;

	//	コンポーネント
	[Header("コンポーネント")]
	[SerializeField]
	private PlayerMove			playerMove;		//	プレイヤー（移動）

	//	実行前初期化処理
	private void Awake()
	{
		
	}

	//	初期化処理
	private void Start()
	{
		
	}

	//	更新処理
	private void Update()
	{
		
	}


	/*--------------------------------------------------------------------------------
	|| ステージのロード処理
	--------------------------------------------------------------------------------*/
	public void LoadStage(int stageIndex)
	{
		var stage = stageDataBase.StageList[stageIndex];

		var async1 = SceneManager.LoadSceneAsync("StageBase");
		var async2 = SceneManager.LoadSceneAsync("Stage_" + stage.StageID.ToString(),LoadSceneMode.Additive);

		async1.allowSceneActivation = true;
		async2.allowSceneActivation = true;
	}
}
