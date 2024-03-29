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

	[Header("サウンド")]
	[SerializeField]
	private SoundPlayer		soundPlayer;

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
		//	セーブデータの設定
		SaveData saveData = SaveDataLoader.LoadJson();
		//	送り状に設定する
		for (int i = 0; i < stageNumberBoxes.Length; i++)
		{
			if (i >= saveData.stageScores.Length)
				break;

			stageNumberBoxes[i].SetStageScore(saveData.stageScores[i]);
		}
		//	選択されているステージを設定
		selectedTaskData.StageID = saveData.lastSelectStage;

		//	選択されているステージの座標にプレイヤーを移動させる
		if(selectedTaskData.StageID != -1)
		{
			player.TargetPos = stageNumberBoxes[selectedTaskData.StageID - 1].transform.position;
			currentDir = Direction.RIGHT;
			player.TargetOffset = stopOffset * (int)currentDir;
			player.SetPosToTarget();

			titleCamera.CameraTarget = stageNumberBoxes[selectedTaskData.StageID - 1].transform;
			titleCamera.SetPosToTarget();

			selectedStage = selectedTaskData.StageID;
		}

		//	BGMの出力先を設定
		BGMPlayer.Instance.FadeSpeed = 10.0f;
		BGMPlayer.Instance.SoundPlayer.SetMixerGroup("EffectedBGM");
		if (selectedStage <= 0)
			BGMPlayer.Instance.StartTransition(0);
	}

	//	更新処理
	private void Update()
	{
		if (titleManager.IsOpening ||
			titleManager.OpenMenu)
			return;

		InputUpdate();      //	入力処理
		SelectUpdate();
		SelectStage();
		ButtonHintUpdate();
		BGMUpdate();
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

		if (x != 0)
			soundPlayer.PlaySound(5);
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
		if (Transition.Instance.IsTransition)
			return;


		if (!enableSelectTask &&
			inputConfirm &&
			selectedStage != 0)
		{
			stageNumberBoxes[selectedStage - 1].SelectBox();

			enableSelectTask = true;
			titleManager.EnableSelectTask = true;

			//	決定SEの再生
			soundPlayer.PlaySound(2);

			return;
		}

		if (enableSelectTask)
		{
			if (inputCancel)
			{
				stageNumberBoxes[selectedStage - 1].CancelBox();
				enableSelectTask = false;
				titleManager.EnableSelectTask = false;

				//	キャンセルSEの再生
				soundPlayer.PlaySound(3);

			}

			if(inputConfirm)
			{
				selectedTaskData.StageID = selectedStage;
				selectedTaskData.TaskIndex = stageNumberBoxes[selectedStage - 1].SelectedTaskIndex;

				//	決定SEの再生
				soundPlayer.PlaySound(2);

				titleManager.LoadScene();
			}
		}
	}

	/*--------------------------------------------------------------------------------
	|| ボタンヒントの更新
	--------------------------------------------------------------------------------*/
	private void ButtonHintUpdate()
	{
		if(enableSelectTask)
		{
			buttonHint.SetActive("Restart", true);
			buttonHint.SetActive("Menu", false);
		}
		else
		{
			buttonHint.SetActive("Restart", false);
			buttonHint.SetActive("Menu", true);
		}
	}

	/*--------------------------------------------------------------------------------
	|| BGMの変更処理
	--------------------------------------------------------------------------------*/
	private void BGMUpdate()
	{
		var source = BGMPlayer.Instance;

		if(selectedStage != source.CurrentIndex)
		{
			source.StartTransition(selectedStage);
		}

		if(selectedStage <= 0)
		{
			source.SoundPlayer.SetMixerGroup("NormalBGM");
		}
		else
		{
			source.SoundPlayer.SetMixerGroup("EffectedBGM");
		}
	}
}
