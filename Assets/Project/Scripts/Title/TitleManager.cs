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
using UnityEngine.Playables;
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
	private int					saveSelectedStage;

	//	ボタンヒント
	[Header("ボタンヒント")]
	[SerializeField]
	private ButtonHint			buttonHit;
	[SerializeField]
	private RectTransform		cancelHint;

	//	入力
	private bool				inputConfirm;					//	確定ボタン
	private bool				inputCancel;                    //	キャンセルボタン

	//	オープニング
	[Header("オープニング")]
	[SerializeField]
	private PlayableDirector	openingDirector;
	[SerializeField]
	private float				playerSpawnPosY;


	//	実行前初期化処理
	private void Awake()
	{
		//	変数の初期化
		SelectedStage = -1;

		if (selectedData.StageID == -1)
			openingDirector.Play();
		else
		{
			Vector3 pos = stageNumberBoxes[selectedData.StageID].transform.position; ;
			pos.y = playerSpawnPosY;
			playerMove.transform.position = pos;
			titleCamera.SkipScroll();
		}

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

		ButtonHintUpdate();

		//	値を保持する
		saveSelectedStage = SelectedStage;
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
		//	タスクの表示を有効化
		taskManager.Activate(
			stageNumberBoxes[SelectedStage].position,			//	座標
			stageDataBase.Stages[SelectedStage].TaskCount		//	タスクの数
			);

		//	キャンセルボタンのヒントを表示
		cancelHint.gameObject.SetActive(true);
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

		//	キャンセルボタンのヒントを非表示
		cancelHint.gameObject.SetActive(false);
	}

	/*--------------------------------------------------------------------------------
	|| ボタンヒントの更新処理
	--------------------------------------------------------------------------------*/
	private void ButtonHintUpdate()
	{
		if (SelectedStage == saveSelectedStage)
			return;

		if(SelectedStage != -1)
		{
			buttonHit.SetDisplayNameIndex("Jump", 1);
		}
		else
		{
			buttonHit.SetDisplayNameIndex("Jump", 0);
		}
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

		StageInfo stage = stageDataBase.Stages[selectedData.StageID];
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

		TaskInfo task = stage.Tasks[selectedData.TaskIndex];
		if(task.Scene == null)
		{
			Debug.LogError("ステージID : " + selectedData.StageID + " タスクID : " + selectedData.TaskIndex + " は設定されていません。");
			return;
		}

		//	シーン名を配列にする
		string[] scenes =
		{
			"StageBase",
			task.Scene.SceneName
		};
		//	シーン遷移の開始
		Transition.Instance.StartTransition(scenes);
	}
}
