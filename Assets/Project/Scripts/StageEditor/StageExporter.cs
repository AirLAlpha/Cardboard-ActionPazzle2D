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
using static UnityEditor.PlayerSettings;
using UnityEngine.Events;

public class StageExporter : MonoBehaviour
{
	[Header("データベース")]
	[SerializeField]
	private StageObjectDatabase objectDatabase;

	[Header("ターゲット")]
	[SerializeField]
	private Transform	objectRoot;             //	敵オブジェクトの親
	[SerializeField]
	private Transform	gimmickRoot;
	[SerializeField]
	private Tilemap		tilemap;				//	出力するタイルマップ

	[Header("出力")]
	[SerializeField]
	private string		exportDirectory;		//	書き出し先のディレクトリを指定
	[SerializeField]
	private string		exportName;             //	書き出しファイルの名前を指定

	//	書き出し先のファイルパス
	private string ExportPath => Path.Combine(exportDirectory + exportName);

	/*--------------------------------------------------------------------------------
	|| ステージデータの書き出し
	--------------------------------------------------------------------------------*/
	[ContextMenu("ExportStage")]
	public void ExportStage()
	{
		//	書き出し用の構造体を作成
		var exportStruct = new StageData();
		exportStruct.objectDatas = new List<StageObjectData>();
		exportStruct.gimmickDatas = new List<GimmickObjectData>();

		//	タイルマップが設定されている時
		if (tilemap != null)
		{
			//	タイルマップのタイル情報を書き出し用のリストに含める
			var tilemapList = TilemapToTileList(tilemap);
			exportStruct.objectDatas.AddRange(tilemapList);
		}
		//	オブジェクトのリストを書き出し用に追加する
		exportStruct.objectDatas.AddRange(EnemyToEnemyList(objectRoot));

		//	ギミックを設定する
		exportStruct.gimmickDatas.AddRange(GimmickToGimmickList(gimmickRoot));


		//	構造体をJsonにする
		string json = JsonUtility.ToJson(exportStruct, true);
		//	書き出し
		File.WriteAllText(ExportPath, json);

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
				var tile = tilemap.GetTile(new Vector3Int(x, y));
				//	指定座標がnullなら処理しない
				if (tile == null)
					continue;

				//	データベースからタイルのオブジェクトを検索する
				int objectIndex = objectDatabase.FindObject(tile);
				//	見つからなければこれ以上処理しない
				if (objectIndex == -1)
					continue;

				//	オブジェクトを作成し、座標とデータベースのインデックス番号を設定する
				StageObjectData newData = new StageObjectData(objectIndex, new Vector3(x, y));
				//	リストに追加する
				ret.Add(newData);
			}
		}

		return ret;
	}

	/*--------------------------------------------------------------------------------
	|| ステージ上の敵の出力処理
	--------------------------------------------------------------------------------*/
	public List<StageObjectData> EnemyToEnemyList(Transform root)
	{
		List<StageObjectData> ret = new List<StageObjectData>();

		//	子オブジェクトでデータベースに登録されているものをリスト化し保持する
		foreach (Transform child in root)
		{
			//	インスタンスの名前の" _(数値) "を取り除く
			string name = Regex.Replace(child.name, @"_[0-9]{0,3}", string.Empty);
			//	名前の" (clone) "を取り除く
			name = name.Replace("(Clone)", "");

			//	データベースからタイルのオブジェクトを検索する
			int findIndex = objectDatabase.FindObject(name);
			//	見つからなければこれ以上処理しない
			if (findIndex == -1)
				continue;

			//	新たなデータを作成し情報を格納
			StageObjectData newObj = new StageObjectData(findIndex, child.position);
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

			//	インスタンスの名前の" _(数値) "を取り除く
			string name = Regex.Replace(child.name, @"_[0-9]{0,3}", string.Empty);
			//	名前の" (clone) "を取り除く
			name = name.Replace("(Clone)", "");

			//	データベースからタイルのオブジェクトを検索する
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

			//	新たなデータを作成し情報を格納
			GimmickObjectData newObj = new GimmickObjectData(findIndex, child.position, gimmick.Type, gimmickTarget);
			//	戻り値のリストに追加する
			ret.Add(newObj);
		}

		return ret;
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
}
