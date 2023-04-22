using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using System;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.Rendering.UI;

public class StageLoader : MonoBehaviour
{
	//	アセットバンドル（ステージデータ）
	static private AssetBundle		stagedataBundle;	//	ステージデータのアセットバンドル

	[Header("データベース")]
	[SerializeField]
	private StageObjectDatabase		objectDatabase;		//	オブジェクトのデータベース

	[Header("ルートオブジェクト")]
	[SerializeField]
	private Transform	objectRoot;                     //	敵のオブジェクトの親
	[SerializeField]	
	private Transform	gimmickRoot;					//	ギミックの親
	[SerializeField]		
	private Tilemap		tilemap;						//	ステージのタイルを配置するタイルマップ

	[Header("ステージ")]
	[SerializeField]
	private string		stageBundleName;				//	ステージバンドルの名前      
	[SerializeField]
	private string		stageDirectory;					//	ステージファイルの存在するディレクトリ
	[SerializeField]
	private string		stageFilename;					//	ステージのファイル名

	//	ステージのパス
	private string		StagePath => Path.Combine(stageDirectory + stageFilename);

	//	有効化時処理
	private void OnEnable()
	{
		//	アセットバンドルが読み込まれていないときは読み込む
		if (stagedataBundle == null)
		{
			string path = Application.streamingAssetsPath;
			path = path.Replace("/", "\\");
			path = Path.Combine(path, stageBundleName);
			stagedataBundle = AssetBundle.LoadFromFile(path);
		}
	}

	//	無効化時処理
	private void OnDisable()
	{
		if (stagedataBundle != null)
			AssetBundle.UnloadAllAssetBundles(true);
	}


	/*--------------------------------------------------------------------------------
	|| Jsonファイルよりステージを読み込む処理
	--------------------------------------------------------------------------------*/
	public void LoadStageFromJson(string filePath = "")
	{
		if (filePath == string.Empty)
			filePath = StagePath;

		string json = File.ReadAllText(filePath);

		LoadStage(json);
	}

	/*--------------------------------------------------------------------------------
	|| アセットバンドルよりステージを読み込む処理
	--------------------------------------------------------------------------------*/
	public void LoadStageFromAssetBundle(string fileName = "")
	{
		if(stagedataBundle == null)
		{
			Debug.LogError("ステージデータのアセットバンドルが読み込まれていません。");
			return;
		}

		if (fileName == string.Empty)
			fileName = stageFilename;

		var asset = stagedataBundle.LoadAsset<TextAsset>(fileName);
		string json = asset.ToString();

		LoadStage(json);
	}

	/*--------------------------------------------------------------------------------
	|| ステージの読み込み
	--------------------------------------------------------------------------------*/
	private void LoadStage(string json)
	{
		var stageData = (StageData)JsonUtility.FromJson(json, typeof(StageData));

		GenerateStage(stageData);
	}

	/*--------------------------------------------------------------------------------
	|| ステージの生成処理
	--------------------------------------------------------------------------------*/
	private void GenerateStage(StageData data)
	{
		//	ステージを一度リセットする
		ResetStage(false);

		List<GimmickObjectData> receivesData = new List<GimmickObjectData>();
		List<ReceiveGimmick> receives = new List<ReceiveGimmick>();     //	アクションを登録するオブジェクト


		

		//	リストにあるオブジェクトを１つずつ生成していく
		foreach (var objData in data.objectDatas)
		{
			//	オブジェクトをデータベースより取得する
			StageObject obj = objectDatabase.StageObject[objData.databaseIndex];

			//	オブジェクトによって生成のタイプを変える
			switch (obj.type)
			{
				case ObjectType.TILE:				//	タイルマップ
					SetTile(obj, objData.pos);
					break;

				case ObjectType.ENEMY:				//	敵
					SetEnemy(obj, objData.pos, objData.rot);
					break;

				case ObjectType.PLAYER:				//	プレイヤー
					//	プレイヤーを移動させる
					GameObject player = GameObject.FindGameObjectWithTag("Player");
					player.transform.position = objData.pos;
					player.transform.rotation = objData.rot;
					break;

				default:							//	それ以外が指定されたら処理しない
					continue;
			}
		}
		//	ギミックの生成
		foreach (var gimmick in data.gimmickDatas)
		{
			//	オブジェクトをデータベースより取得する
			StageObject obj = objectDatabase.StageObject[gimmick.databaseIndex];

			SetGimmick(obj, gimmick, out ReceiveGimmick receive);
			if (receive != null)
			{
				receivesData.Add(gimmick);
				receives.Add(receive);
			}
		}
		//	ギミックのアクションを設定
		for (int i = 0; i < receives.Count; i ++)
		{
			int childIndex = receivesData[i].targetIndex;
			if (childIndex == -1)
				continue;

			SendGimmick sender = gimmickRoot.GetChild(childIndex).GetComponent<SendGimmick>();
			receives[i].Sender = sender;
			//	ギミックの初期化（アクションの登録）を行う
			receives[i].Initialize();
		}


		//	ログに読み込み結果を出力
		Debug.Log(StagePath + " を読み込みました。");
	}

	/*--------------------------------------------------------------------------------
	|| タイルの設置処理
	--------------------------------------------------------------------------------*/
	private void SetTile(StageObject obj, Vector3 pos)
	{
		//	タイルマップが設定されていないときは処理しない
		if (tilemap == null)
			return;
		//	オブジェクトのタイプが”タイル”ではないときは処理しない
		if (obj.type != ObjectType.TILE)
			return;

		Vector3Int tilePos = new Vector3Int((int)pos.x, (int)pos.y);
		tilemap.SetTile(tilePos, obj.prefab as TileBase);
	}

	/*--------------------------------------------------------------------------------
	|| 敵オブジェクトの設置処理
	--------------------------------------------------------------------------------*/
	private void SetEnemy(StageObject obj, Vector3 pos, Quaternion rot)
	{
		if (objectRoot == null)
			return;
		if (obj.type != ObjectType.ENEMY)
			return;

		//	オブジェクトの生成
		Instantiate(obj.prefab, pos, rot, objectRoot);
	}

	/*--------------------------------------------------------------------------------
	|| ギミックオブジェクトの設置処理
	--------------------------------------------------------------------------------*/
	private void SetGimmick(StageObject obj, GimmickObjectData gimmickData, out ReceiveGimmick receive)
	{
		receive = null;

		//	オブジェクトが割り当てられていないときは処理しない
		if (objectRoot == null)
			return;
		//	ギミック以外のものは処理しない
		if (obj.type != ObjectType.GIMMICK)
			return;

		//	オブジェクトの設置
		var newGimmick = Instantiate(obj.prefab, gimmickData.pos, gimmickData.rot, gimmickRoot) as GameObject;
		//	ギミックタイプを設定
		newGimmick.GetComponent<Gimmick>().Type = gimmickData.type;
		//	受け取り側の場合は値を設定する
		newGimmick.TryGetComponent<ReceiveGimmick>(out receive);
	}


	/*--------------------------------------------------------------------------------
	|| ステージのリセット処理
	--------------------------------------------------------------------------------*/
	[ContextMenu("Reset stage")]
	public void ResetStage(bool resetAssetBundle)
	{
		//	フラグが有効な場合アセットバンドルを開放する
		if (resetAssetBundle)
			AssetBundle.UnloadAllAssetBundles(true);

		//	タイルマップのタイルをすべて削除
		tilemap?.ClearAllTiles();

		//	オブジェクトルートが設定されていないときは処理しない
		if (objectRoot == null)
			return;

		//	子オブジェクトにあるステージオブジェクトをすべて削除する
		foreach (Transform item in objectRoot.transform)
		{
			DestroyImmediate(item.gameObject);
		}
		//	ひとつ残る場合があるため、残っていたら削除を実行する
		if (objectRoot.childCount > 0)
			DestroyImmediate(objectRoot.GetChild(0).gameObject);

		//	子オブジェクトにあるステージオブジェクトをすべて削除する
		foreach (Transform item in gimmickRoot.transform)
		{
			DestroyImmediate(item.gameObject);
		}
		//	ひとつ残る場合があるため、残っていたら削除を実行する
		if (gimmickRoot.childCount > 0)
			DestroyImmediate(gimmickRoot.GetChild(0).gameObject);
	}

#if UNITY_EDITOR
	/*--------------------------------------------------------------------------------
	|| アセットバンドルの読み込み処理（デバッグ用）
	--------------------------------------------------------------------------------*/
	[ContextMenu("Load AssetBundle")]
	private void LoadAsestBundle()
	{
		//	アセットバンドルが読み込まれていないときは読み込む
		if (stagedataBundle == null)
		{
			string path = Application.streamingAssetsPath;
			path = path.Replace("/", "\\");
			path = Path.Combine(path, stageBundleName);
			stagedataBundle = AssetBundle.LoadFromFile(path);
		}
	}
#endif
}
