using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SettingLoader : MonoBehaviour
{
	static readonly string SETTING_PATH = "Settings";

	/*--------------------------------------------------------------------------------
	|| 設定の書き出し
	--------------------------------------------------------------------------------*/
	public static void ExportSetting(Setting setting)
	{
		//	クラスをJSONに書き出す
		string json = JsonUtility.ToJson(setting, true);

		//	パスの入力がされていないときは処理しない
		if (SETTING_PATH == string.Empty)
		{
			Debug.LogError("出力先のパスが指定されていません。");
			return;
		}

		//	書き出し
		File.WriteAllText(SETTING_PATH, json);

		Debug.Log("設定を書き出しました。");
	}

	/*--------------------------------------------------------------------------------
	|| 設定の読み込み
	--------------------------------------------------------------------------------*/
	public static Setting LoadSetting()
	{
		//	JSONを読み込む
		string json = "";
		try
		{
			json = File.ReadAllText(SETTING_PATH);
		}
		catch
		{
			var file = File.Create(SETTING_PATH);
			file.Close();
			ResetSetting();
			json = File.ReadAllText(SETTING_PATH);
		}

		Debug.Log("設定を読み込みました。");
		return JsonUtility.FromJson<Setting>(json);
	}

	/*--------------------------------------------------------------------------------
|| 新しいセーブデータファイルの作成
--------------------------------------------------------------------------------*/
	public static void ResetSetting()
	{
		Setting newSetting = new Setting {bgmVol = 2, seVol = 2};

		SettingLoader.ExportSetting(newSetting);
	}
}
