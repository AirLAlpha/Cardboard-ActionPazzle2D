/**********************************************
 * 
 *  StageExporter.cs 
 *  ステージの書き出し処理を行う
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/18
 * 
 **********************************************/
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Text.RegularExpressions;
using System.Windows.Forms;

public class StageExporter : MonoBehaviour
{
	[Header("データベース")]
	[SerializeField]
	private StageObjectDatabase objectDatabase;

	[Header("ターゲット")]
	[SerializeField]
	private Transform	enemyRoot;				//	敵オブジェクトの親
	[SerializeField]
	private Transform	gimmickRoot;			//	ギミックの親
	[SerializeField]
	private Transform	cardboardRoot;			//	段ボールの親
	[SerializeField]
	private Tilemap		tilemap;                //	出力するタイルマップ
	[SerializeField]
	private BackgroundSetter bgSetter;


	[Header("出力")]
	[SerializeField]
	private string		exportDirectory;		//	書き出し先のディレクトリを指定
	[SerializeField]
	private string		exportFileName;         //	書き出しファイルの名前を指定

	//	書き出し先のファイルパス
	private string ExportPath => Path.Combine(exportDirectory + exportFileName);

	/*--------------------------------------------------------------------------------
	|| Jsonの書き出し
	--------------------------------------------------------------------------------*/
	public void ExportJson(string fileName = "")
	{
		//	書き出し先を設定
		if (fileName != string.Empty)
			exportFileName = fileName;

		ExportStage(ExportPath);
	}

	/*--------------------------------------------------------------------------------
	|| ステージデータの書き出し
	--------------------------------------------------------------------------------*/
	public void ExportStage(string exportPath)
	{
		//	書き出し用の構造体を作成
		var exportStruct = new StageData();
		exportStruct.objectDatas = new List<StageObjectData>();
		exportStruct.gimmickDatas = new List<GimmickObjectData>();
		exportStruct.usableBoxCount = StageManager.Instance.UsableBoxCount;
		exportStruct.targetBoxCount = StageManager.Instance.TargetBoxCount;
		exportStruct.backgroundType = bgSetter.Index;

		var playerData = ExportPlayer();
		if(playerData == null)
		{
			Debug.LogError("プレイヤーが存在しないステージは書き出すことができません。");
			return;
		}
		exportStruct.objectDatas.Add(playerData);

		//	タイルマップが設定されている時
		if (tilemap != null)
		{
			//	タイルマップのタイル情報を書き出し用のリストに含める
			var tilemapList = TilemapToTileList(tilemap);
			exportStruct.objectDatas.AddRange(tilemapList);
		}

		//	ギミックを設定する
		exportStruct.gimmickDatas.AddRange(GimmickToGimmickList(gimmickRoot));

		//	ステージ上の段ボールを書き出し用に追加する
		exportStruct.objectDatas.AddRange(ObjectToObjectList(cardboardRoot));

		//	敵のリストを書き出し用に追加する
		exportStruct.objectDatas.AddRange(ObjectToObjectList(enemyRoot));

		//	構造体をJsonにする
		string json = JsonUtility.ToJson(exportStruct, true);
		//	書き出し
		try
		{
			File.WriteAllText(exportPath, json);
		}
		//	例外処理
		catch
		{
			Directory.CreateDirectory(exportDirectory);
			File.WriteAllText(exportPath, json);
		}

		//	ログに出力する
		Debug.Log(exportPath + " にステージをエクスポートしました。");
	}

	/*--------------------------------------------------------------------------------
	|| プレイヤーの出力
	--------------------------------------------------------------------------------*/
	private StageObjectData ExportPlayer()
	{
		//	プレイヤーを検索する
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		if (player == null)
			return null;

		string name = GetNonInstanceNumberName(player.name);
		int index = objectDatabase.FindObject(name);

		StageObjectData data = new StageObjectData(index, player.transform.position, player.transform.rotation);
		return data;
	}

	/*--------------------------------------------------------------------------------
	|| タイルマップの出力
	--------------------------------------------------------------------------------*/
	public List<StageObjectData> TilemapToTileList(Tilemap tilemap)
	{
		//	バウンディボックスの範囲に納める
		tilemap.CompressBounds();
		//	タイルマップの大きさを取得
		BoundsInt tilemapSize = tilemap.cellBounds;
		//	戻り値として使用するリストを作成
		List<StageObjectData> ret = new List<StageObjectData>();

		//	タイルマップを出力していく
		for (int y = tilemapSize.min.y; y < tilemapSize.max.y; y++)
		{
			for (int x = tilemapSize.min.x; x < tilemapSize.max.x; x++)
			{
				//	タイルの取得
				var tile = tilemap.GetTile(new Vector3Int(x, y, 0));
				//	指定座標がnullなら処理しない
				if (tile == null)
					continue;

				//	データベースからタイルのオブジェクトを検索する
				int objectIndex = objectDatabase.FindObject(tile);
				//	見つからなければこれ以上処理しない
				if (objectIndex == -1)
					continue;

				//	オブジェクトを作成し、座標とデータベースのインデックス番号を設定する
				StageObjectData newData = new StageObjectData(objectIndex, new Vector3(x, y), Quaternion.identity);
				//	リストに追加する
				ret.Add(newData);
			}
		}

		return ret;
	}

	/*--------------------------------------------------------------------------------
	|| ステージ上の敵の出力処理
	--------------------------------------------------------------------------------*/
	public List<StageObjectData> ObjectToObjectList(Transform root)
	{
		List<StageObjectData> ret = new List<StageObjectData>();

		//	子オブジェクトでデータベースに登録されているものをリスト化し保持する
		foreach (Transform child in root)
		{
			//	データベースからタイルのオブジェクトを検索する
			string name = GetNonInstanceNumberName(child.name);
			int findIndex = objectDatabase.FindObject(name);
			//	見つからなければこれ以上処理しない
			if (findIndex == -1)
				continue;

			//	新たなデータを作成し情報を格納
			StageObjectData newObj = new StageObjectData(findIndex, child.position, child.rotation);
			//	戻り値のリストに追加する
			ret.Add(newObj);
		}

		return ret;
	}

	/*--------------------------------------------------------------------------------
	|| ギミックの書き出し処理
	--------------------------------------------------------------------------------*/
	public List<GimmickObjectData> GimmickToGimmickList(Transform root)
	{
		List<GimmickObjectData> ret = new List<GimmickObjectData>();

		//	子オブジェクトでデータベースに登録されているものをリスト化し保持する
		foreach (Transform child in root)
		{
			Gimmick gimmick;
			if (!child.TryGetComponent<Gimmick>(out gimmick))
				continue;

			//	データベースからタイルのオブジェクトを検索する
			string name = GetNonInstanceNumberName(child.name);
			int findIndex = objectDatabase.FindObject(name);
			//	見つからなければこれ以上処理しない
			if (findIndex == -1)
				continue;

			int gimmickTarget = -1;
			//	ギミックのターゲットを検索する
			if (gimmick.Type == GimmickType.EVENT_RECEIVE)
			{
				ReceiveGimmick receive;
				if (child.TryGetComponent<ReceiveGimmick>(out receive))
				{
					Transform target = receive.Sender.transform;
					gimmickTarget = FindChildIndex(root, target);
				}
			}

			//	ギミック固有の設定を取得
			string exSetting = gimmick.GetExtraSetting();

			//	新たなデータを作成し情報を格納
			GimmickObjectData newObj = new GimmickObjectData(findIndex, child.position, child.rotation, gimmick.Type, gimmickTarget, exSetting);
			//	戻り値のリストに追加する
			ret.Add(newObj);
		}

		return ret;
	}

	/*--------------------------------------------------------------------------------
	|| ファイルを保存するダイアログを表示する
	--------------------------------------------------------------------------------*/
	public void OpenSaveDialog()
	{
		UnityEngine.Cursor.visible = true;

		SaveFileDialog dialog = new SaveFileDialog();
		dialog.InitialDirectory = exportDirectory;
		dialog.AddExtension = true;
		dialog.CreatePrompt = false;
		dialog.Filter = "jsonファイル|*.json";
		dialog.ShowDialog();

		//	キャンセルを選択したときの処理
		if (dialog.FileName == string.Empty)
		{
			Debug.Log("エクスポートをキャンセルしました。");
			return;
		}

		ExportStage(dialog.FileName);
	}

	/*--------------------------------------------------------------------------------
	|| ステージファイルの削除
	--------------------------------------------------------------------------------*/
	public void DeleteFile(string fileName, string directoryPath = "")
	{
		if (directoryPath == string.Empty)
			directoryPath = exportDirectory;

		string path = Path.Combine(directoryPath + fileName);
		File.Delete(path);

		Debug.Log(path + " を削除しました。");
	}

	/*--------------------------------------------------------------------------------
	|| 子オブジェクトのインデックスを検索する
	--------------------------------------------------------------------------------*/
	private int FindChildIndex(Transform root, Transform target)
	{
		for (int i = 0; i < root.childCount; i++)
		{
			if (target == root.GetChild(i))
				return i;
		}

		//	見つからなければ-1を返す
		return -1;
	}

	/*--------------------------------------------------------------------------------
	|| オブジェクトの名前の添字を削除する
	--------------------------------------------------------------------------------*/
	private string GetNonInstanceNumberName(string name)
	{
		//	インスタンスの名前の" _(数値) "を取り除く
		string ret = Regex.Replace(name, @"_[0-9]{0,3}", string.Empty);
		//	名前の" (clone) "を取り除く
		ret = ret.Replace("(Clone)", "");

		return ret;
	}

}
