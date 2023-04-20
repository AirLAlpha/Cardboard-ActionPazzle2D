/**********************************************
 * 
 *  TaskClear.cs 
 *  タスククリア時の処理を記述
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/09
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TaskClear : MonoBehaviour
{
	private enum State
	{
		NONE,				//	状態なし

		CAM_MOVE,			//	カメラ移動
		INVOICE_MOVE,		//	送り状移動
		CHANGE_TIME,		//	時間の更新
		CHANGE_BOX_COUT,	//	使用した箱の数の更新
		STAMP_MOVE,			//	スタンプの移動
		MENU_MOVE,			//	メニューの移動

		END					//	終了
	}
	//	現在の状態
	private State					currentState;

	//	選択済みのタスクデータ
	[Header("選択済みのタスクデータ")]
	[SerializeField]
	private SelectedTaskData		selectedTask;

	//	コンポーネント
	[Header("コンポーネント")]
	[SerializeField]
	private CameraZoom				cameraZoom;			//	カメラズーム
	[SerializeField]
	private PlayerMove				playerMove;
	[SerializeField]
	private CanvasAlphaController	canvasAlphaController;

	private CameraShake				cameraShake;        //	カメラシェイク
	private PlayerBoxManager		playerBoxManager;
	private Transform				CameraTransform => cameraZoom.transform;        //	カメラのトランスフォーム

	//	カメラの移動
	[Header("カメラ移動")]
	[SerializeField]
	private float					cameraMoveWait;				//	カメラの移動の待機維持感

	//	送り状
	[Header("送り状")]
	[SerializeField]
	private Transform				invoice;					//	送り状のTransform
	[SerializeField]
	private float					invoiceActivateTime;		//	送り状の有効化速度
	[SerializeField]
	private Vector3					invoiceStartPos;			//	送り状の開始位置
	[SerializeField]
	private Vector3					invoiceEndPos;				//	送り状の終了位置
	[SerializeField]
	private float					invoiceActivateWait;        //	送り状をアクティブ化するまでの待機時間
	[Space, SerializeField]
	private Vector3					invoicePanelMoveOffset;		//	パネル移動時のオフセット

	//	送り状テキスト
	[Header("送り状テキスト")]
	[SerializeField]
	private TextMeshPro				stageNumberText;            //	ステージ番号を表示するテキスト1
	[SerializeField]
	private TextMeshPro				timeText;					//	時間のテキスト
	[SerializeField]
	private TextMeshPro				boxCountText;               //	使用ボックスのテキスト
	[SerializeField]
	private float					timeAppliedTime;			//	時間の適応にかかる時間
	[SerializeField]
	private float					boxCountAppliedSpeed;       //	使用したハコの数の適応速度
	[SerializeField]
	private float					textAppliedWaitTime;		//	テキストの適応までの待機時間

	//	表示用
	private float	clearedTime;		//	クリアタイム
	private int		usedBoxCount;       //	使用した箱の数

	//	スタンプ
	[Header("スタンプ")]
	[SerializeField]
	private SpriteRenderer			stamp;					//	スタンプ
	[SerializeField]
	private float					stampStartScale;		//	スタンプの開始サイズ
	[SerializeField]
	private float					stampEndScale;			//	スタンプの終了サイズ
	[SerializeField]
	private float					stampTime;				//	スタンプの適応時間
	[SerializeField]
	private float					stampWaitTime;          //	スタンプの待機時間

	//	メニュー
	[Header("リザルトメニュー")]
	[SerializeField]
	private ResultMenu				resultPanel;
	[SerializeField]
	private Vector2					panelStartPos;
	[SerializeField]
	private Vector2					panelEndPos;
	[SerializeField]
	private float					panelMoveTime;
	[SerializeField]
	private float					panelWaitTime;

	private RectTransform			resultPanelTransform;

	//	カメラ
	[Header("カメラ")]
	[SerializeField]
	private Vector3				clearCameraPos;			//	クリア時のカメラ座標
	[SerializeField]
	private float				cameraPosChangeRate;    //	カメラ座標の変化速度

	//	ボタンヒント
	[Header("ボタンヒント")]
	[SerializeField]
	private ButtonHint				buttonHint;
	[SerializeField]
	private CanvasAlphaController	buttonHintAlpha;

	//	アニメーション
	private float				elapsedTime;			//	経過時間
	private bool				isCleared;				//	クリア済みフラグ



	//	実行前初期化処理
	private void Awake()
	{
		//	コンポーネントの取得
		playerMove?.TryGetComponent<PlayerBoxManager>(out playerBoxManager);
		cameraZoom?.TryGetComponent<CameraShake>(out cameraShake);

		resultPanelTransform = resultPanel.transform as RectTransform;
	}

	//	初期化処理
	private void Start()
	{

	}

	//	更新処理
	private void Update()
	{
		//	アニメーションの処理がないステートの際は処理しない
		if (currentState == State.NONE ||
			currentState == State.END)
			return;

		switch (currentState)
		{
			case State.CAM_MOVE:            //	カメラの移動
				CameraMoveUpdate();
				break;

			case State.INVOICE_MOVE:        //	送り状の移動
				InvoiceMoveUpdate();
				break;

			case State.CHANGE_TIME:
				TimeTextUpdate();			//	時間の表示を更新
				break;

			case State.CHANGE_BOX_COUT:
				BoxCountUpdate();			//	使用した箱の数を更新
				break;


			case State.STAMP_MOVE:
				StampUpdate();				//	スタンプの更新処理
				break;

			case State.MENU_MOVE:
				MenuMoveUpdate();			//	メニューパネルの移動
				break;

			default:
				return;
		}

	}


	/*--------------------------------------------------------------------------------
	|| カメラの移動処理
	--------------------------------------------------------------------------------*/
	private void CameraMoveUpdate()
	{
		//	カメラの座標を目標座標へ移動
		CameraTransform.position = Vector3.Lerp(CameraTransform.position, clearCameraPos, Time.deltaTime * cameraPosChangeRate);

		elapsedTime += Time.deltaTime;
		if(elapsedTime >= cameraMoveWait)
		{
			//	経過時間をリセット
			elapsedTime = 0.0f;
			//	状態を変更
			currentState = State.INVOICE_MOVE;

			//	送り状の座標を初期化
			invoice.position = invoiceStartPos;
			//	ステージ番号の表示を設定
			stageNumberText.text = selectedTask.StageID + " - " + (selectedTask.TaskIndex + 1);
			//	送り状を有効化
			invoice.gameObject.SetActive(true);
		}
	}

	/*--------------------------------------------------------------------------------
	|| 送り状の移動処理
	--------------------------------------------------------------------------------*/
	private void InvoiceMoveUpdate()
	{
		//	経過時間から進行度を作成
		elapsedTime += Time.deltaTime;
		float progress = elapsedTime/invoiceActivateTime;

		//	送り状を目標座標へ移動
		invoice.position = Vector3.LerpUnclamped(invoiceStartPos, invoiceEndPos, EasingFunctions.EaseOutBack(progress));

		//	進行度が100%を超えたら次の状態へ繊維
		if(progress >= 1.0f)
		{
			invoice.position = invoiceEndPos;
			elapsedTime = 0.0f;

			currentState = State.CHANGE_TIME;
		}
	}

	/*--------------------------------------------------------------------------------
	|| 時間の表示を更新する処理
	--------------------------------------------------------------------------------*/
	private void TimeTextUpdate()
	{
		//	経過時間を加算
		elapsedTime += Time.deltaTime;
		//	進行度を計算
		float progress = (elapsedTime - textAppliedWaitTime) / timeAppliedTime;

		//	経過時間から表示テキストを計算
		float t = Mathf.Lerp(0, clearedTime, progress);
		int min = (int)(t / 60);
		int sec = (int)(t % 60);

		//	テキストに適応
		timeText.text = min.ToString("D2") + ":" + sec.ToString("D2");

		if(progress >= 1.0f)
		{
			int finalMin = (int)(clearedTime / 60);
			int finalSec = (int)(clearedTime % 60);
			timeText.text = finalMin.ToString("D2") + ":" + finalSec.ToString("D2");

			elapsedTime = 0.0f;
			currentState = State.CHANGE_BOX_COUT;
		}
	}

	/*--------------------------------------------------------------------------------
	|| 使用したハコの数の更新処理
	--------------------------------------------------------------------------------*/
	private void BoxCountUpdate()
	{
		//	経過時間を加算
		elapsedTime += Time.deltaTime;

		//	進行度から表示テキストを計算
		int n = (int)Mathf.Clamp((elapsedTime - textAppliedWaitTime) * boxCountAppliedSpeed, 0, usedBoxCount);

		//	テキストに適応
		boxCountText.text = n.ToString("D2");

		float progress = n / (float)usedBoxCount;
		if(progress >= 1.0f)
		{
			boxCountText.text = usedBoxCount.ToString("D2");

			//	スタンプの初期化
			stamp.transform.localScale = Vector3.one * stampStartScale;
			stamp.color = new Color(stamp.color.r, stamp.color.g, stamp.color.b, 0.0f);
			stamp.gameObject.SetActive(true);

			elapsedTime = 0.0f;
			currentState = State.STAMP_MOVE;
		}
	}

	/*--------------------------------------------------------------------------------
	|| スタンプの更新処理
	--------------------------------------------------------------------------------*/
	private void StampUpdate()	
	{
		//	経過時間を加算
		elapsedTime += Time.deltaTime;
		float progress = (elapsedTime - stampWaitTime) / stampTime;

		//	スケールを作成
		float scale = Mathf.Lerp(stampStartScale, stampEndScale, progress);
		//	スケールを適応
		stamp.transform.localScale = Vector3.one * scale;
		//	透明度を設定
		Color col = stamp.color;
		col.a = progress;
		stamp.color = col;

		if(progress >= 1.0f)
		{
			//	スケールを補正
			stamp.transform.localScale = Vector3.one * stampEndScale;
			//	透明度を補正
			Color finalCol = stamp.color;
			finalCol.a = 1.0f;
			stamp.color = finalCol;

			elapsedTime = 0.0f;
			currentState = State.MENU_MOVE;

			//	リザルトパネルの有効化
			resultPanel.gameObject.SetActive(true);

			//	ボタンヒントのアクティブを切り替え
			buttonHint.SetActive("Vertical",	true);
			buttonHint.SetActive("Jump",		true);
			buttonHint.SetActive("Horizontal",	false);
			buttonHint.SetActive("Fire1",		false);
			buttonHint.SetActive("Restart",		false);
			buttonHint.SetActive("Menu",		false);
			//	ボタンヒントのテキストを変更
			buttonHint.SetDisplayNameIndex("Jump", 1);
		}

	}

	/*--------------------------------------------------------------------------------
	|| メニューの移動処理
	--------------------------------------------------------------------------------*/
	private void MenuMoveUpdate()
	{
		//	経過時間を加算
		elapsedTime += Time.deltaTime;
		float progress = (elapsedTime - panelWaitTime) / panelMoveTime;

		float t = EasingFunctions.EaseOutCubic(progress);
		//	パネルを移動
		resultPanelTransform.anchoredPosition = Vector2.Lerp(panelStartPos, panelEndPos, t);
		//	送り状を移動
		invoice.localPosition = invoiceEndPos + Vector3.Lerp(Vector3.zero, invoicePanelMoveOffset, t);

		//	ボタンヒントの透明度を設定
		buttonHintAlpha.TargetAlpha = progress;

		if(progress >= 1.0f)
		{
			//	座標を補正
			resultPanelTransform.anchoredPosition = panelEndPos;
			invoice.localPosition = invoiceEndPos + invoicePanelMoveOffset;
			//	透明度の値を補正
			buttonHintAlpha.TargetAlpha = 1.0f;

			//	リザルトパネルの入力を有効化
			resultPanel.DisableInput = false;

			elapsedTime = 0.0f;
			currentState = State.END;
		}

	}

	/*--------------------------------------------------------------------------------
	|| クリア時処理
	--------------------------------------------------------------------------------*/
	public void OnTaskClear()
	{
		//	クリアフラグを有効化
		isCleared = true;

		//	カメラのコンポーネントを無効化
		cameraZoom.enabled = false;
		cameraShake.enabled = false;

		//	ゲームUIを透明に設定
		canvasAlphaController.TargetAlpha = 0.0f;
		//	プレイヤーの入力を無効化
		playerMove.DisableInput = true;
		playerBoxManager.DisableInput = true;

		//	時間を取得
		clearedTime = StageManager.Instance.ElapsedTime;
		//	使用した箱の数を取得
		usedBoxCount = StageManager.Instance.UsedBoxCount;

		//	リザルトパネルの入力を無効化
		resultPanel.DisableInput = true;

		//	ハイスコアの書き出し
		ExportHighScore();

		//	状態を更新
		currentState = State.CAM_MOVE;
	}

	/*--------------------------------------------------------------------------------
	|| ハイスコアの書き出し
	--------------------------------------------------------------------------------*/
	private void ExportHighScore()
	{
		//	セーブデータの読み込み
		SaveData data = SaveDataLoader.LoadJson();

		TaskScore taskScore = data.stageScores[selectedTask.StageID].scores[selectedTask.TaskIndex];

		//	過去にクリアしていない場合 or 使用した箱の数を更新したときは書き換える
		if(taskScore.clearTime <= 0.0f ||														//	過去にクリアしていない
			taskScore.usedBoxCount > usedBoxCount ||											//	使用したハコの数が前回より少ない
			(clearedTime < taskScore.clearTime && taskScore.usedBoxCount >= usedBoxCount))		//	クリアタイムを更新し、使用したハコの数が前回以下
		{
			taskScore.clearTime = clearedTime;
			taskScore.usedBoxCount = usedBoxCount;

			//	書き換えたスコアをデータに格納
			data.stageScores[selectedTask.StageID].scores[selectedTask.TaskIndex] = taskScore;
			//	JSONの書き出し
			SaveDataLoader.ExportJson(data);
		}
	}

}
