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

	[SerializeField]
	private StageLoader			stageLoader;

	public int StageID => selectedStageData.StageID;
	public int TaskIndex => selectedStageData.TaskIndex;

	//	段ボール
	public int UsableBoxCount	{ get; set; }							//	使用可能なハコの数
	public int TargetBoxCount	{ get; set; }							//	ゴールさせるべき箱の数
	public int UsedBoxCount		{ get; set; }							//	使用した箱の数
	public int CompleteBoxCount { get; set; }							//	ゴールさせた箱の数
	public int RemainingBoxCount => UsableBoxCount - UsedBoxCount;      //	残り使用可能な箱の数

	private int saveCompleteBoxCount;                                   //	前回処理時のゴールさせた箱の数
	private int saveRemainingBoxCount;                                  //	前回処理時の残り使用可能な箱の数

	//	時間
	private float elapsedTime;											//	スタートからの経過時間
	public float ElapsedTime { get { return elapsedTime; } }

	public bool TimeStopFlag { get; private set; }

	//	リスタート
	[Header("リスタート")]
	[SerializeField]
	private float restartTime;			//	リスタートになる時間

	private float restartPressedTime;   //	リスタートボタンを押している時間

	public float RestartProgress => Mathf.Clamp01(restartPressedTime / restartTime);

	//	ステージクリア
	public bool IsStageClear	{ get; private set; }					//	ステージクリアフラグ

	//	アクション
	public UnityEvent OnChangedRemainingBoxCount;						//	残りの箱の数が更新されたときに呼び出される
	public UnityEvent OnNonRemaining;									//	箱の残りがなかったときに呼び出される
	public UnityEvent OnChangedCompleteBoxCount;						//	ゴールさせた箱の数が更新されたときに呼び出される
	public UnityEvent OnStageClear;										//	ステージクリア時に呼び出される

	//	実行前初期化処理
	protected override void Awake()
	{
		base.Awake();

		InitStage();        //	ステージの初期化を行う

		//	フラグの初期化
		TimeStopFlag = false;
		//	タイマーを止める処理を追加
		OnStageClear.AddListener(() => { TimeStopFlag = true; });
	}

	//	初期化処理
	private void Start()
	{
		if (stageLoader == null)
			return;

		//	ステージの読み込みを行う
		string stageFileName = stageDataBase.Stages[selectedStageData.StageID].Tasks[selectedStageData.TaskIndex].SceneFileName;
		stageLoader.LoadStageFromAssetBundle(stageFileName);
	}

	//	更新処理
	private void Update()
	{
		TimerUpdate();
		RestartUpdate();

		CheckRemainingBoxCount();
		CheckCompleteBoxCount();
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

		//	タスクデータの取得
		int stageId = selectedStageData.StageID;
		int taskIndex = selectedStageData.TaskIndex;
		TaskInfo task = stageDataBase.Stages[stageId].Tasks[taskIndex];

		//	変数を保持
		UsableBoxCount = task.UsableBoxCount;
		TargetBoxCount = task.TargetBoxCount;
		saveRemainingBoxCount = -1;
		saveCompleteBoxCount = -1;

		//	変数の初期化
		IsStageClear = false;
	}

	/*--------------------------------------------------------------------------------
	|| 経過時間の更新処理
	--------------------------------------------------------------------------------*/
	private void TimerUpdate()
	{
		if (TimeStopFlag)
			return;

		elapsedTime += Time.deltaTime;
	}

	/*--------------------------------------------------------------------------------
	|| リスタートの更新処理
	--------------------------------------------------------------------------------*/
	private void RestartUpdate()
	{
		//	クリア済みのときは処理しない
		if (IsStageClear)
			return;

		if (Input.GetButton("Restart"))
		{
			restartPressedTime += Time.deltaTime;

			if (restartPressedTime >= restartTime && 
				!Transition.Instance.IsTransition)
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

				OnStageClear?.Invoke();
			}
		}
	}

	/*--------------------------------------------------------------------------------
	|| タイトルに戻る処理
	--------------------------------------------------------------------------------*/
	public void ReturnTitle()
	{
		string[] scenes = {"TitleScene"};
		Transition.Instance.StartTransition(scenes);
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
		Transition.Instance.StartTransition(scenes);
	}

	/*--------------------------------------------------------------------------------
	|| 次のステージを読み込む処理
	--------------------------------------------------------------------------------*/
	public void LoadNextStage()
	{
		//	タスクインデックをインクリメント
		selectedStageData.TaskIndex ++;

		StageInfo stage = stageDataBase.Stages[selectedStageData.StageID];
		TaskInfo task = stage.Tasks[selectedStageData.TaskIndex];

		string taskSceneName = SceneManager.GetActiveScene().name;

		string[] scenes = { /*"StageBase", */taskSceneName };

		Transition.Instance.StartTransition(scenes);
	}

#if UNITY_EDITOR
	[ContextMenu("Add 12:34")]
	public void AddTime()
	{
		elapsedTime += 12 * 60 + 34;
	}
	[ContextMenu("Set Boxcount 18")]
	public void SetBox()
	{
		UsedBoxCount = 18;
	}
#endif
}
