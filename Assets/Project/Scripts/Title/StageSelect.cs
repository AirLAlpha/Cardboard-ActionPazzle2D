/**********************************************
 * 
 *  StageSelect.cs 
 *  ステージセレクトの処理を記述
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/05/20
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TitleManager))]
public class StageSelect : MonoBehaviour
{
	private TitleManager titleManager;

	[Header("カメラ")]
	[SerializeField]
	private TitleCamera		titleCamera;

	[Header("プレイヤー")]
	[SerializeField]
	private TitlePlayer		player;
	[SerializeField]
	private Vector3			stopOffset;

	[Header("ハコ")]
	[SerializeField]
	private Transform			logoTarget;				//	ロゴ表示時の目標
	[SerializeField]
	private TitleStageBox[]		stageNumberBoxes;		//	ステージの番号が書かれたハコ

	private int StageCount => stageNumberBoxes.Length;  //	ステージの数

	[Header("ステージ選択")]
	[SerializeField]
	private SelectedTaskData selectedTaskData;
	[SerializeField]
	private int				selectedStage;      //	選択中のステージ

	private Direction		currentDir;         //	現在移動中の方向

	[Header("入力")]
	[SerializeField]
	private ButtonHint		buttonHint;
	[SerializeField]
	private float			inputInterval;      //	X軸入力の間隔

	private float			inputWaitTime;
	private bool			inputConfirm;
	private bool			inputCancel;

	//	タスク選択
	private bool			enableSelectTask;


	//	プロパティ
	public int SelectedStage { get { return selectedStage; } }


	//	実行前初期化処理
	private void Awake()
	{
		titleManager = GetComponent<TitleManager>();
	}

	//	初期化処理
	private void Start()
	{
		
	}

	//	更新処理
	private void Update()
	{
		if (titleManager.IsOpening)
			return;

		InputUpdate();      //	入力処理
		SelectUpdate();
		SelectStage();
	}


	/*--------------------------------------------------------------------------------
	|| 入力処理
	--------------------------------------------------------------------------------*/
	private void InputUpdate()
	{
		//	決定
		inputConfirm = Input.GetButtonDown("Jump");
		//	キャンセル
		inputCancel = Input.GetButtonDown("Restart");

		//	タスク選択中は処理しない
		if (enableSelectTask)
			return;

		//	X軸の入力を取得
		float inputX = Input.GetAxis("Horizontal") + Input.GetAxis("D-PadX");
		int x = inputX == 0 ? 0 : (int)Mathf.Sign(inputX);
		//	X軸の入力がなかったときは処理しない
		if (x == 0)
		{
			inputWaitTime = 0;
			return;
		}
		else if (inputWaitTime > 0)
		{
			inputWaitTime -= Time.deltaTime;
			return;
		}

		//	選択の限界のときは処理しない
		if (selectedStage + x > StageCount ||
			selectedStage + x < 0)
			return;

		if (0 < x)
		{
			currentDir = Direction.RIGHT;
		}
		if (x < 0)
		{
			currentDir = Direction.LEFT;
		}
		selectedStage += (int)currentDir;
		titleCamera.CameraTarget =　selectedStage == 0 ? logoTarget : stageNumberBoxes[selectedStage - 1].transform;

		inputWaitTime = inputInterval;
	}

	/*--------------------------------------------------------------------------------
	|| 選択処理
	--------------------------------------------------------------------------------*/
	private void SelectUpdate()
	{
		if (currentDir == Direction.NONE)
			return;


		player.TargetPos = selectedStage == 0 ? logoTarget.position : stageNumberBoxes[selectedStage - 1].transform.position;

		Vector3 offset = stopOffset;
		if (selectedStage != 0)
		{
			offset.x *= (int)currentDir;
		}
		else
		{
			offset.x *= 0;
		}

		player.TargetOffset = offset;

		for (int i = 0; i < stageNumberBoxes.Length; i++)
		{
			bool open = i + 1 == selectedStage;
			stageNumberBoxes[i].IsOpen = open;
		}
	}

	/*--------------------------------------------------------------------------------
	|| 決定時処理
	--------------------------------------------------------------------------------*/
	private void SelectStage()
	{
		if (!enableSelectTask &&
			inputConfirm &&
			selectedStage != 0)
		{
			stageNumberBoxes[selectedStage - 1].SelectBox();

			enableSelectTask = true;
			return;
		}

		if (enableSelectTask)
		{
			if (inputCancel)
			{
				stageNumberBoxes[selectedStage - 1].CancelBox();
				enableSelectTask = false;
			}

			if(inputConfirm)
			{
				selectedTaskData.StageID = selectedStage;
				selectedTaskData.TaskIndex = stageNumberBoxes[selectedStage - 1].SelectedTaskIndex;

				titleManager.LoadScene();
			}
		}
	}

	/*--------------------------------------------------------------------------------
	|| ボタンヒントの更新
	--------------------------------------------------------------------------------*/
	private void ButtonHintUpdate()
	{
		//if()
	}

}
