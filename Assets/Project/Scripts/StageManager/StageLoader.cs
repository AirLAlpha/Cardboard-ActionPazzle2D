using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using System.Windows.Forms;

public class StageLoader : MonoBehaviour
{
	[Header("データベース")]
	[SerializeField]
	private StageDataBase			stageDatabase;
	[SerializeField]
	private StageObjectDatabase		objectDatabase;		//	オブジェクトのデータベース

	[Header("ルートオブジェクト")]
	[SerializeField]
	private Transform	enemyRoot;						//	敵のオブジェクトの親
	[SerializeField]	
	private Transform	gimmickRoot;					//	ギミックの親
	[SerializeField]		
	private Tilemap		tilemap;                        //	ステージのタイルを配置するタイルマップ
	[SerializeField]
	private Transform	cardboardRoot;					//	ハコの親

	[Header("ステージ")]
	[SerializeField]
	private string		stageBundleName;				//	ステージバンドルの名前      
	[SerializeField]
	private string		stageDirectory;					//	ステージファイルの存在するディレクトリ
	[SerializeField]
	private string		stageFilename;					//	ステージのファイル名

	//	ステージのパス
	private string		StagePath => Path.Combine(stageDirectory + stageFilename);

	//	イベントを受け取るギミック
	[SerializeField, HideInInspector]
	private List<int>				senderIndex;                    //	メッセージを送るギミックの子番号
	[SerializeField, HideInInspector]
	private List<ReceiveGimmick>	receives;

	/*--------------------------------------------------------------------------------
	|| Jsonファイルよりステージを読み込む処理
	--------------------------------------------------------------------------------*/
	public void LoadStageFromJson(string fileName = "", string fileDirectory = "", bool pauseEnable = false)
	{
		//	フォルダの名前が指定されていないときは、デフォルトを指定
		if(fileDirectory == string.Empty)
		{
			fileDirectory = stageDirectory;
		}
		//	ファイル名が指定されてないときは、設定済みの値を代入
		if (fileName == string.Empty)
		{
			fileName = stageFilename;
		}

		//	入力よりパスを作成
		string path = Path.Combine(fileDirectory + fileName);
		string json = "";
		try
		{
			json = File.ReadAllText(path);
		}
		catch
		{
			if (!Directory.Exists(fileDirectory))
				Directory.CreateDirectory(fileDirectory);

			//	ファイルの読み込みに失敗したら作成する
			var file = File.Create(path);
			file.Close();
		}

		LoadStage(json, pauseEnable);
		//	ログに読み込み結果を出力
		Debug.Log(path + " を読み込みました。");
	}

	/*--------------------------------------------------------------------------------
	|| アセットバンドルよりステージを読み込む処理
	--------------------------------------------------------------------------------*/
	public void LoadStageFromDatabase(int stageID, int taskIndex, bool pauseEnable = false)
	{
		var asset = stageDatabase.Stages[stageID].Tasks[taskIndex].StageJson;
		string json = asset.ToString();

		LoadStage(json, pauseEnable);
		//	ログに読み込み結果を出力
		Debug.Log("アセットバンドル（" + stageBundleName + ")より " + stageFilename + "を読み込みました。");
	}

	/*--------------------------------------------------------------------------------
	|| ステージの読み込み
	--------------------------------------------------------------------------------*/
	private void LoadStage(string json, bool pauseEnable)
	{
		//	ステージを一度リセットする
		ResetStage(false);

		if (json == string.Empty)
			return;

		var stageData = (StageData)JsonUtility.FromJson(json, typeof(StageData));

		GenerateStage(stageData, pauseEnable);

		//	目標の箱の数が0以下だったら1に設定
		if (stageData.targetBoxCount <= 0)
			stageData.targetBoxCount = 1;

		//	使用可能な箱の数と目標の箱の数を設定
		StageManager.Instance.InitStage(stageData.usableBoxCount, stageData.targetBoxCount, stageData.backgroundType);
	}

	/*--------------------------------------------------------------------------------
	|| ステージの生成処理
	--------------------------------------------------------------------------------*/
	private void GenerateStage(StageData data, bool pauseEnable)
	{
		//	プレイヤーを無敵にしておく
		GameObject player = GameObject.FindGameObjectWithTag("Player");
		PlayerDamageReciver pdr = player.GetComponent<PlayerDamageReciver>();
		pdr.DontDeath = true;

		tilemap.ClearAllTiles();

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

				case ObjectType.ENEMY:              //	敵
					SetObject(enemyRoot, obj, objData.pos, objData.rot, pauseEnable);
					break;

				case ObjectType.PLAYER:				//	プレイヤー
					//	プレイヤーを移動させる
					player.transform.position = objData.pos;
					player.transform.rotation = objData.rot;
					player.GetComponent<PlayerMove>().OnStageReset();
					break;

				case ObjectType.BOX:                //	ハコ
					SetObject(cardboardRoot, obj, objData.pos, objData.rot, pauseEnable);
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

			//	ギミックを設置する
			SetGimmick(obj, gimmick, pauseEnable, out ReceiveGimmick receive);
			//	イベントを受け取るギミックのときはリストに追加する
			if (receive != null)
			{
				senderIndex.Add(gimmick.targetIndex);
				receives.Add(receive);
			}
		}
		//	ギミックの初期化を行う
		InitReveiceGimmicks();

		//	プレイヤーの無敵を解除する
		pdr.DontDeath = false;
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
	private void SetObject(Transform parent, StageObject obj, Vector3 pos, Quaternion rot, bool pauseEnable)
	{
		if (enemyRoot == null)
			return;
		if (obj.type != ObjectType.ENEMY &&
			obj.type != ObjectType.BOX)
			return;

		//	オブジェクトの生成
		var newEnemy = Instantiate(obj.prefab, pos, rot, parent) as GameObject;
		//	ポーズ状態の設定
		if (newEnemy.TryGetComponent<IPauseable>(out IPauseable enemyPause))
		{
			if(pauseEnable)
			{
				enemyPause.Pause();
			}
			else
			{
				enemyPause.Resume();
			}
		}
	}

	/*--------------------------------------------------------------------------------
	|| ギミックオブジェクトの設置処理
	--------------------------------------------------------------------------------*/
	private void SetGimmick(StageObject obj, GimmickObjectData gimmickDatas, bool pauseEnable, out ReceiveGimmick receive)
	{
		receive = null;

		//	オブジェクトが割り当てられていないときは処理しない
		if (enemyRoot == null)
			return;
		//	ギミック以外のものは処理しない
		if (obj.type != ObjectType.GIMMICK)
			return;

		//	オブジェクトの設置
		var newGimmick = Instantiate(obj.prefab, gimmickDatas.pos, gimmickDatas.rot, gimmickRoot) as GameObject;
		//	ギミックの取得
		Gimmick gimmickComponent = newGimmick.GetComponent<Gimmick>();
		//	ギミックタイプを設定
		gimmickComponent.Type = gimmickDatas.type;
		//	ギミック固有の設定を適応
		gimmickComponent.SetExtraSetting(gimmickDatas.extraSetting);
		//	受け取り側の場合は値を設定する
		newGimmick.TryGetComponent<ReceiveGimmick>(out receive);

		//	ポーズ状態の設定
		if (newGimmick.TryGetComponent<IPauseable>(out IPauseable gimmickPause))
		{ 
			if(pauseEnable)
			{
				gimmickPause.Pause();
			}
			else
			{
				gimmickPause.Resume();
			}
		}
	}

	/*--------------------------------------------------------------------------------
	|| イベントを受け取るギミックの初期化
	--------------------------------------------------------------------------------*/
	public void InitReveiceGimmicks()
	{
		if (gimmickRoot.childCount <= 0)
			return;

		//	ギミックのアクションを設定
		for (int i = 0; i < receives.Count; i++)
		{
			int childIndex = senderIndex[i];
			if (childIndex == -1)
				continue;

			var a = gimmickRoot.GetChild(childIndex);
			SendGimmick sender = a.GetComponent<SendGimmick>();
			receives[i].Sender = sender;
			//	ギミックの初期化（アクションの登録）を行う
			receives[i].Initialize();
		}
	}

	/*--------------------------------------------------------------------------------
	|| ステージのリセット処理
	--------------------------------------------------------------------------------*/
	[UnityEngine.ContextMenu("Reset stage")]
	public void ResetStage(bool resetAssetBundle)
	{
		//	フラグが有効な場合アセットバンドルを開放する
		if (resetAssetBundle)
			AssetBundle.UnloadAllAssetBundles(true);

		//	タイルマップのタイルをすべて削除
		tilemap?.ClearAllTiles();

		//	オブジェクトルートが設定されていないときは処理しない
		if (enemyRoot == null)
			return;

		//	子オブジェクトにあるステージオブジェクトをすべて削除する
		while (gimmickRoot.childCount > 0)
		{
			DestroyImmediate(gimmickRoot.GetChild(0).gameObject);
		}
		//	子オブジェクトにあるステージオブジェクトをすべて削除する
		while (enemyRoot.childCount > 0)
		{
			DestroyImmediate(enemyRoot.GetChild(0).gameObject);
		}
		//	段ボールを削除
		while(cardboardRoot.childCount > 0)
		{
			DestroyImmediate(cardboardRoot.GetChild(0).gameObject);
		}


		//	リストを初期化
		senderIndex = new List<int>();
		receives = new List<ReceiveGimmick>();
	}

	/*--------------------------------------------------------------------------------
	|| 読み込み用ウィンドウを表示
	--------------------------------------------------------------------------------*/
	public void OpenLoadDialog()
	{
		UnityEngine.Cursor.visible = true;

		//	ファイルを開くダイアログを表示
		OpenFileDialog dialog = new OpenFileDialog();
		dialog.InitialDirectory = stageDirectory;
		dialog.Filter = "jsonファイル|*.json";
		dialog.ShowDialog();

		//	キャンセルを選択したときの処理
		if (dialog.FileName == string.Empty)
		{
			Debug.Log("ロードをキャンセルしました。");
			return;
		}

		//	ファイル名を取得してロード
		string filename = Path.GetFileName(dialog.FileName);
		string fileDir = dialog.FileName.Replace(filename, "");
		LoadStageFromJson(filename, fileDir, true);
	}
}
