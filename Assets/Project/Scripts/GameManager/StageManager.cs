/**********************************************
 * 
 *  StageManagere.cs 
 *  ステージ全体の管理をする処理を記述
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/02
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
	//	インスタンス
	private static StageManager instance;       //	インスタンス
	public static StageManager Instance			//	インスタンスを取得するプロパティ
	{
		get
		{
			if (instance == null)
			{
				Debug.LogError("StageManagerがアタッチされたオブジェクトはありません。");
				return null;
			}
			else
			{
				return instance;
			}
		}
	}


	//	ステージデータベース
	[Header("DataBase")]
	[SerializeField]
	private StageDataBase stageDataBase;

	//	プレイヤー
	[Header("Player")]
	[SerializeField]
	private PlayerMove				playerMove;

	private PlayerBoxManager		playerBoxManager;

	//	シーン
	private int currentStageId;											//	現在のシーンID

	//	段ボール
	public int UsableBoxCount	{ get; set; }							//	使用可能なハコの数
	public int TargetBoxCount	{ get; set; }							//	ゴールさせるべき箱の数
	public int UsedBoxCount		{ get; set; }							//	使用した箱の数
	public int CompleteBoxCount { get; set; }							//	ゴールさせた箱の数
	public int RemainingBoxCount => UsableBoxCount - UsedBoxCount;      //	残り使用可能な箱の数

	private int saveCompleteBoxCount;                                   //	前回処理時のゴールさせた箱の数
	private int saveRemainingBoxCount;                                  //	前回処理時の残り使用可能な箱の数

	//	リスタート
	[Header("リスタート")]
	[SerializeField]
	private float restartTime;			//	リスタートになる時間

	private float restartPressedTime;   //	リスタートボタンを押している時間

	public float RestartProgress => restartPressedTime / restartTime;

	//	ステージクリア
	public bool IsStageClear	{ get; private set; }					//	ステージクリアフラグ

	//	アクション
	public System.Action OnChangedRemainingBoxCount { get; set; }		//	残りの箱の数が更新されたときに呼び出される
	public System.Action OnNonRemaining				{ get; set; }       //	箱の残りがなかったときに呼び出される
	public System.Action OnChangedCompleteBoxCount	{ get; set; }       //	ゴールさせた箱の数が更新されたときに呼び出される
	public System.Action OnStageClear				{ get; set; }		//	ステージクリア時に呼び出される

	//	実行前初期化処理
	private void Awake()
	{
		InitInstance();     //	シングルトンインスタンスの初期化
		InitStage();        //	ステージの初期化を行う

		//	コンポーネントの取得
		if (playerMove != null)
			playerBoxManager = playerMove.GetComponent<PlayerBoxManager>();
	}

	//	初期化処理
	private void Start()
	{
	}

	//	更新処理
	private void Update()
	{
		RestartUpdate();

		CheckRemainingBoxCount();
		CheckCompleteBoxCount();

		if (IsStageClear &&
			Input.GetButtonDown("Jump"))
			ReturnTitle();
	}

	/*--------------------------------------------------------------------------------
	|| シングルトンインスタンスの初期化
	--------------------------------------------------------------------------------*/
	private void InitInstance()
	{
		//	すでに自分自身がアタッチされているときは処理しない
		if (instance == this)
		{
			return;
		}
		//	インスタンスが設定されていないときは自身を設定する
		else if (instance == null)
		{
			instance = this;
			return;
		}

		//	すでに別のインスタンスが設定されているときは自身を破棄する
		Destroy(this);
	}

	/*--------------------------------------------------------------------------------
	|| ステージの初期化
	--------------------------------------------------------------------------------*/
	private void InitStage()
	{
		//	ステージデータベースのNullチェック
		if (stageDataBase == null)
		{
			Debug.LogError("ステージデータベースが設定されていません。");
			return;
		}

		//	シーン名を取得
		int loadedSceneCount = SceneManager.sceneCount;
		string baseSceneName = SceneManager.GetActiveScene().name;
		string stageSceneNamae = baseSceneName;
		for (int i = 0; i < loadedSceneCount; i++)
		{
			string sceneName = SceneManager.GetSceneAt(i).name;
			if (sceneName == baseSceneName)
				continue;

			stageSceneNamae = sceneName;
			break;
		}

		//	シーン名からステージIDを取得
		int stageId = int.Parse(stageSceneNamae.Replace("Stage_", ""));
		//	データベース内のステージIDから検索
		StageData findResult = stageDataBase.StageList.Find(n => n.StageID == stageId);

		//	変数を保持
		currentStageId = stageId;
		UsableBoxCount = findResult.UsableBoxCount;
		TargetBoxCount = findResult.TargetBoxCount;
		saveRemainingBoxCount = -1;
		saveCompleteBoxCount = -1;

		//	変数の初期化
		IsStageClear = false;
	}

	/*--------------------------------------------------------------------------------
	|| リスタートの更新処理
	--------------------------------------------------------------------------------*/
	private void RestartUpdate()
	{
		if (Input.GetButton("Restart"))
		{
			restartPressedTime += Time.deltaTime;

			if (restartPressedTime >= restartTime)
			{
				var baseScene = SceneManager.LoadSceneAsync("StageBase");
				var stageScene = SceneManager.LoadSceneAsync("Stage_" + currentStageId.ToString(), LoadSceneMode.Additive);
			}
		}
		else
		{
			restartPressedTime = 0;
		}
	}

	/*--------------------------------------------------------------------------------
	|| 残りの箱の数の変化を確認
	--------------------------------------------------------------------------------*/
	private void CheckRemainingBoxCount()
	{
		if(saveRemainingBoxCount != RemainingBoxCount)
		{
			//	処理を呼び出す
			OnChangedRemainingBoxCount?.Invoke();

			//	変数を保持
			saveRemainingBoxCount = RemainingBoxCount;
		}
	}

	/*--------------------------------------------------------------------------------
	|| 箱を置くことが出来るかどうか
	--------------------------------------------------------------------------------*/
	public bool CheckRemainingBoxes()
	{
		//	箱の残数が1以上
		if (RemainingBoxCount > 0)
			return true;

		//	箱の残数が0以下
		OnNonRemaining?.Invoke();
		return false;
	}

	/*--------------------------------------------------------------------------------
	|| ステージクリアの確認処理
	--------------------------------------------------------------------------------*/
	private void CheckCompleteBoxCount()
	{
		if (saveCompleteBoxCount != CompleteBoxCount)
		{
			//	処理を呼び出す
			OnChangedCompleteBoxCount?.Invoke();

			//	変数を保持
			saveCompleteBoxCount = CompleteBoxCount;

			//	目標数に到達したら
			if(CompleteBoxCount >= TargetBoxCount)
			{
				IsStageClear = true;

				StageClear();
				OnStageClear?.Invoke();
			}
		}
	}

	/*--------------------------------------------------------------------------------
	|| ゴール処理
	--------------------------------------------------------------------------------*/
	private void StageClear()
	{
		playerMove.DisableInput = true;
		playerBoxManager.DisableInput = true;
	}

	/*--------------------------------------------------------------------------------
	|| タイトルに戻る処理
	--------------------------------------------------------------------------------*/
	private void ReturnTitle()
	{
		SceneManager.LoadScene("TitleScene");
	}

}
