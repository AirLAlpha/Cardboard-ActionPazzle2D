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
using UnityEngine.Events;

public class StageManager : SingletonMonoBehaviour<StageManager>
{
	//	ステージデータベース
	[Header("DataBase")]
	[SerializeField]
	private StageDataBase		stageDataBase;
	[SerializeField]
	private SelectedTaskData	selectedStageData;

	//	プレイヤー
	[Header("Player")]
	[SerializeField]
	private PlayerMove				playerMove;

	private PlayerBoxManager		playerBoxManager;

	//	キャンバス
	[Header("キャンバス")]
	[SerializeField]
	private CanvasAlphaController	canvasAlphaController;

	//	シーン
	private Scene	loadedScene;										//	読み込み済みのシーン
	private int		currentStageId;										//	現在のシーンID

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
	public UnityEvent OnChangedRemainingBoxCount	{ get; set; }		//	残りの箱の数が更新されたときに呼び出される
	public UnityEvent OnNonRemaining				{ get; set; }       //	箱の残りがなかったときに呼び出される
	public UnityEvent OnChangedCompleteBoxCount		{ get; set; }       //	ゴールさせた箱の数が更新されたときに呼び出される
	public UnityEvent OnStageClear					{ get; set; }		//	ステージクリア時に呼び出される

	//	実行前初期化処理
	private void Awake()
	{
		OnChangedCompleteBoxCount = new UnityEvent();
		OnChangedRemainingBoxCount = new UnityEvent();
		OnNonRemaining = new UnityEvent();
		OnStageClear = new UnityEvent();

		CheckInstance();
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

	//	終了時処理
	private void OnDisable()
	{
		//	すべてのイベントに登録されたデリゲートを削除する
		OnChangedCompleteBoxCount.RemoveAllListeners();
		OnChangedRemainingBoxCount.RemoveAllListeners();
		OnNonRemaining.RemoveAllListeners();
		OnStageClear.RemoveAllListeners();
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
		const string BASE_SCENE_NAME = "StageBase";
		int loadedSceneCount = SceneManager.sceneCount;
		string stageSceneNamae = BASE_SCENE_NAME;
		for (int i = 0; i < loadedSceneCount; i++)
		{
			string sceneName = SceneManager.GetSceneAt(i).name;
			if (sceneName == BASE_SCENE_NAME)
				continue;

			stageSceneNamae = sceneName;
			break;
		}

		//	タスクデータの取得
		int stageId = selectedStageData.StageID;
		int taskIndex = selectedStageData.TaskIndex;
		TaskData task = stageDataBase.Stages[stageId].Tasks[taskIndex];

		//	変数を保持
		UsableBoxCount = task.UsableBoxCount;
		TargetBoxCount = task.TargetBoxCount;
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
				//	ステージを再読込する
				ResetStage();
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
		canvasAlphaController.TargetAlpha = 0.0f;
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

	/*--------------------------------------------------------------------------------
	|| ステージの再読み込み処理
	--------------------------------------------------------------------------------*/
	public void ResetStage()
	{
		int loadedSceneCount = SceneManager.sceneCount;
		string[] scenes = new string[loadedSceneCount];
		//	すべてのシーン名を取得
		for (int i = 0; i < loadedSceneCount; i++)
		{
			scenes[i] = SceneManager.GetSceneAt(i).name;
		}

		//	再読み込みを実行
		SceneManager.LoadScene(scenes[0]);
		for (int i = 1; i < loadedSceneCount; i++)
		{
			SceneManager.LoadScene(scenes[i], LoadSceneMode.Additive);
		}
	}

}
