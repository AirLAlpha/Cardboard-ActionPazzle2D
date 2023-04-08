/**********************************************
 * 
 *  TitleManager.cs 
 *  タイトルを統括する処理を記述
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/04
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
	//	状態
	enum State
	{
		TITLE,				//	タイトル
		TASK_SELECT,		//	タスクセレクト
		CHANGE_SCENE,		//	シーンチェンジ
	}
	private State				state;

	//	ステージ
	[Header("ステージデータ")]
	[SerializeField]
	private StageDataBase		stageDataBase;
	[SerializeField]
	private SelectedTaskData	selectedData;

	//	コンポーネント
	[Header("コンポーネント")]
	[SerializeField]
	private PlayerMove			playerMove;         //	プレイヤー（移動）
	[SerializeField]
	private TitleCamera			titleCamera;        //	タイトルのカメラ
	[SerializeField]
	private TaskManager			taskManager;        //	タスクマネージャー

	//	ステージセレクト
	[Header("ステージ")]
	[SerializeField]
	private Transform[]			stageNumberBoxes;				//	ステージ番号のハコ

	public int					SelectedStage { get; set; }     //	選択中のステージ


	//	入力
	private bool				inputConfirm;					//	確定ボタン
	private bool				inputCancel;					//	キャンセルボタン

	//	実行前初期化処理
	private void Awake()
	{
		//	変数の初期化
		SelectedStage = -1;	
	}

	//	初期化処理
	private void Start()
	{
		
	}

	//	更新処理
	private void Update()
	{
		InputUpdate();      //	入力処理

		if (inputConfirm && state == State.TITLE)
			StageSelect();
		if (inputCancel && state == State.TASK_SELECT)
			CancelTaskSelect();
	}

	/*--------------------------------------------------------------------------------
	|| 入力処理
	--------------------------------------------------------------------------------*/
	private void InputUpdate()
	{
		inputConfirm	= Input.GetButtonDown("Jump");
		inputCancel		= Input.GetButtonDown("Restart");
	}

	/*--------------------------------------------------------------------------------
	|| ステージ選択時処理
	--------------------------------------------------------------------------------*/
	private void StageSelect()
	{
		//	ステージが選択されていないときは処理しない
		if (SelectedStage < 0)
			return;

		//	タスクセレクトへ移行する
		state = State.TASK_SELECT;

		//	プレイヤーの入力を無効化
		playerMove.DisableInput = true;
		//	タスク選択の開始
		titleCamera.StartTaskSelect();
		taskManager.StageIndex = SelectedStage;
		taskManager.Activate(stageNumberBoxes[SelectedStage].position);
	}

	/*--------------------------------------------------------------------------------
	|| タスク選択のキャンセル時処理
	--------------------------------------------------------------------------------*/
	private void CancelTaskSelect()
	{
		if (taskManager.PickUpFlag)
			return;

		//	タイトルへと移行する
		state = State.TITLE;

		//	タスク選択の終了
		titleCamera.EndTaskSelect();
		//	プレイヤーの入力を有効化
		playerMove.DisableInput = false;
	}

	/*--------------------------------------------------------------------------------
	|| シーンの読み込み
	--------------------------------------------------------------------------------*/
	[ContextMenu("LoadScene")]
	public void LoadScene()
	{
		if (stageDataBase.Stages.Length - 1 < selectedData.StageID)
		{
			Debug.LogError("ステージID : " + selectedData.StageID + " は設定されていません。");
			return;
		}

		StageData stage = stageDataBase.Stages[selectedData.StageID];
		if(stage == null)
		{
			Debug.LogError("ステージID : " + selectedData.StageID + " は設定されていません。");
			return;
		}

		if(stage.Tasks.Length - 1 < selectedData.TaskIndex)
		{
			Debug.LogError("ステージID : " + selectedData.StageID + " タスクID : " + selectedData.TaskIndex + " は設定されていません。");
			return;
		}

		TaskData task = stage.Tasks[selectedData.TaskIndex];
		if(task.Scene == null)
		{
			Debug.LogError("ステージID : " + selectedData.StageID + " タスクID : " + selectedData.TaskIndex + " は設定されていません。");
			return;
		}

		string taskSceneName = task.Scene.SceneName;

		SceneManager.LoadSceneAsync("StageBase");
		SceneManager.LoadSceneAsync(taskSceneName, LoadSceneMode.Additive);
	}
}
