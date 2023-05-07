/**********************************************
 * 
 *  SaveDataLoader.cs 
 *  セーブデータの書き出し処理を記述
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/13
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public static class SaveDataLoader
{
	[SerializeField]
	static readonly public string		SAVE_PATH = "SaveData.json";        //	セーブデータのパス

	private const int STAGE_COUNT = 5;
	private const int TASK_COUNT = 5;

	/*--------------------------------------------------------------------------------
	|| セーブデータの書き出し
	--------------------------------------------------------------------------------*/
	public static void ExportJson(SaveData saveData)
	{
		//	クラスをJSONに書き出す
		string json = JsonUtility.ToJson(saveData, true);

		//	パスの入力がされていないときは処理しない
		if (SAVE_PATH == string.Empty)
		{
			Debug.LogError("出力先のパスが指定されていません。");
			return;
		}

		//	書き出し
		File.WriteAllText(SAVE_PATH, json);
	}

	/*--------------------------------------------------------------------------------
	|| セーブデータの読み出し
	--------------------------------------------------------------------------------*/
	public static SaveData LoadJson()
	{
		//	パスの入力がされていないときは処理しない
		if (SAVE_PATH == string.Empty)
		{
			Debug.LogError("読み込み先のパスが指定されていません。");
			return default;
		}

		//	JSONを読み込む
		string json = "";
		try
		{
			json = File.ReadAllText(SAVE_PATH);
		}
		catch
		{
			var file = File.Create(SAVE_PATH);
			file.Close();
			ResetSaveData();
			json = File.ReadAllText(SAVE_PATH);
		}

		//	構造体に変換する
		return (SaveData)JsonUtility.FromJson(json, typeof(SaveData));
	}

	/*--------------------------------------------------------------------------------
	|| 新しいセーブデータファイルの作成
	--------------------------------------------------------------------------------*/
	public static void ResetSaveData()
	{
		StageScore[] stages = new StageScore[STAGE_COUNT];
		for (int i = 0; i < STAGE_COUNT; i++)
		{
			stages[i].scores = new TaskScore[TASK_COUNT];
		}
		SaveData saveData = new SaveData(stages);

		SaveDataLoader.ExportJson(saveData);
	}
}
