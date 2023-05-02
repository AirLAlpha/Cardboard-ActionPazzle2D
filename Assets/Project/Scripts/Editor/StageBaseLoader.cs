/**********************************************
 * 
 *  StageBaseLoader.cs 
 *  Stageを読み込む際に自動的にStageBaseを読み込む
 * 
 *  製作者：牛丸 文仁
 *  制作日：2024/04/05
 * 
 **********************************************/
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class StageBaseLoader
{
	//	タイトルシーンの名前
	static readonly string[] NOT_LOAD_MANAGER_NAME =
	{
		"TitleScene",
		"StageBase",
		"PlayScene"
	};

	//	StageBaseシーン
	static readonly string STAGE_BASE_NAME = "StageBase";
	//	Stageのディレクトリ
	static readonly string STAGE_DIRECTORY = "Assets/Project/Scenes/";

	/*--------------------------------------------------------------------------------
	|| 初期化処理
	--------------------------------------------------------------------------------*/
	[InitializeOnLoadMethod]
	private static void Init()
	{
		//EditorSceneManager.sceneOpened += OnSceneLoaded;
	}

	/*--------------------------------------------------------------------------------
	|| シーンが読み込まれたときの処理
	--------------------------------------------------------------------------------*/
	private static void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEditor.SceneManagement.OpenSceneMode mode)
	{
		//	シーン名を取得
		string currentSceneName = scene.name;

		//	ステージベースを読み込むかどうかを判断する
		for (int i = 0; i < NOT_LOAD_MANAGER_NAME.Length; i++)
		{
			if (NOT_LOAD_MANAGER_NAME[i] == currentSceneName)
				return;
		}

		EditorSceneManager.OpenScene(STAGE_DIRECTORY + STAGE_BASE_NAME + ".unity", OpenSceneMode.Additive);
	}
}
#endif