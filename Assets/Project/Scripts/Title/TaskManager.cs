/**********************************************
 * 
 *  TaskManager.cs 
 *  タスク選択の処理を記述
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/08
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//	TODO : リファクタリング

public class TaskManager : MonoBehaviour
{
	//	送り状
	[System.Serializable]
	private struct Invoice
	{
		public SpriteRenderer	sprite;
		public SpriteRenderer	clearStamp;
		public SpriteRenderer	timeIcon;
		public SpriteRenderer	boxIcon;
		public TextMeshPro		stageNumText;
		public TextMeshPro		clearTimeText;
		public TextMeshPro		boxCountText;
	}
	

	//	選択済みのタスクを保持するためのオブジェクト
	[Header("選択済みタスク")]
	[SerializeField]
	private SelectedTaskData		selectedTask;

	[Header("コンポーネント")]
	[SerializeField]
	private TitleManager			titleManager;
	[SerializeField]
	private SpriteAlphaController	backgroundAlphaControlelr;

	private SoundPlayer				soundPlayer;

	//	ステージインデックスの設定
	public int StageIndex { set { selectedTask.StageID = value; } }

	//	アクティブ
	[Header("アクティブ")]
	[SerializeField]
	private float					activateWaitTime;               //	実際にアクティブになるまでの待機時間
	[SerializeField]
	private float					deactivateWaitTime;				//	実際に非アクティブになるまでの待機時間

	public bool						isActive { get; set; }			//	有効化フラグ

	//	入力
	[Header("入力")]
	[SerializeField]
	private float					inputXInterval;		//	X軸入力のインターバル

	private float					inputX;             //	X軸の入力処理
	private bool					inputConfirm;       //	決定
	private bool					inputCancel;        //	キャンセル

	private float					inputXTimer;        //	X軸入力のインターバル計測用

	//	ルートオブジェクト
	[Header("ルートオブジェクト")]
	[SerializeField]
	private Transform				taskImageRoot;		//	タスクのルートオブジェクト
	[SerializeField]
	private float					rootStartPosY;		//	タスクの初期位置
	[SerializeField]
	private float					rootEndPosY;        //	タスクの移動位置
	[SerializeField]
	private float					rootPosChangeRate;  //	ルートオブジェクトの移動速度

	//	間隔
	[Header("間隔")]
	[SerializeField]
	private float					taskGapX;           //	タスク同士の間隔（X)

	//	送り状
	[Header("送り状")]
	[SerializeField]
	private Invoice[]				invoicies;          //	送り状

	private int						activeInvoiceCount;	//	有効な送り状の数

	//	選択
	[Header("選択")]
	[SerializeField]
	private SpriteRenderer			frameImage;			//	枠の画像
	[SerializeField]
	private int						selectedNum;        //	選択済みの番号

	//	スケール
	[Header("スケール")]
	[SerializeField]
	private float					nonSelectedScale;	//	非選択時スケール
	[SerializeField]
	private float					selectedMinScale;   //	選択時スケール（小）
	[SerializeField]
	private float					selectedMaxScale;   //	選択時スケール（大）
	[SerializeField]
	private float					scaleChangeRate;    //	スケールの変更速度

	//	座標
	[Header("座標")]
	[SerializeField]
	private float					nonSelectedPosY;	//	非選択のY座標
	[SerializeField]
	private float					selectedMinPosY;    //	選択時のY座標（下）
	[SerializeField]
	private float					selectedMaxPosY;    //	選択時のY座標（上）
	[SerializeField]
	private float					posChangeRate;      //	座標の変更速度

	//	色
	[Header("色")]
	[SerializeField]
	private Color					nonSelectedColor;
	[SerializeField]
	private Color					selectedColor;
	[SerializeField]
	private float					colorChangeRate;

	//	枠
	[Header("枠")]
	[SerializeField]
	private float					frameMaxScale;		//	枠の最大スケール
	[SerializeField]
	private float					frameMinAlpha;		//	フレームの透明度（最低）
	[SerializeField]
	private float					frameMaxAlpha;      //	フレームの透明度（最高）

	//	ピックアップ
	[Header("ピックアップ")]
	[SerializeField]
	private float					pickupPosY;			//	ピックアップ時のY座標
	[SerializeField]
	private float					pickupScale;        //	ピックアップ時のスケール
	[SerializeField]
	private float					pickupSpeed;        //	ピックアップの速度
	[SerializeField]
	private float					pickupAmplitude;	//	ピックアップ時の振れ幅（Y）

	private bool					pickupFlag;			//	ピックアップ中フラグ
	private float					pickupStartX;		//	ピックアップ前のX座標

	public bool PickUpFlag { get { return pickupFlag; } }

	//	アニメーション
	[Header("アニメーション")]
	[SerializeField]
	private float					animationSpeed;             //	sin波の変化速度

	private float					sin;						//	sin波用の変数
	private float					animationValue;				//	アニメーション用の変数

	private float					rootAnimationProgress;      //	ルートオブジェクトのアニメーション進行度

	private float					pickupAnimationProgress;    //	タスクを1つ選択したときのアニメーション進行度
	
	//	実行前初期化処理
	private void Awake()
	{
		soundPlayer = GetComponent<SoundPlayer>();
	}

	//	初期化処理
	private void Start()
	{
		
	}

	//	更新処理
	private void Update()
	{
		if (!isActive)
			return;

		InputUpdate();      //	入力処理

		PickupUpdate();		//	ピックアップの更新処理

		AnimationUpdate();  //	アニメーションの更新処理

		if(!pickupFlag)
			SelectUpdate();     //	選択処理

		FrameUpdate();      //	枠の更新処理

		if (inputCancel)
		{
			if (pickupFlag)
				pickupFlag = false;
			else
				Cancel();       //	キャンセル処理
		}
		if (inputConfirm)
		{
			if (pickupFlag)
				Confirm();      //	確定処理
			else
				PickUp();		//	ピックアップ処理
		}
	}

	/*--------------------------------------------------------------------------------
	|| 入力処理
	--------------------------------------------------------------------------------*/
	private void InputUpdate()
	{
		//	左右入力
		float horizontal = Input.GetAxisRaw("Horizontal") == 0 ? 0 : Mathf.Sign(Input.GetAxisRaw("Horizontal"));
		float x = horizontal + Input.GetAxisRaw("D-PadX");
		if(x == 0)
		{
			inputX = 0;
			inputXTimer = 0;
		}
		else if(inputXTimer <= 0)
		{
			inputX = x;
			inputXTimer = inputXInterval;

			//	サウンドの再生
			soundPlayer.Play(1);
		}
		else if(inputXTimer > 0)
		{
			inputX = 0;
			inputXTimer -= Time.deltaTime;
		}

		//	決定
		inputConfirm = Input.GetButtonDown("Jump");
		//	キャンセル
		inputCancel = Input.GetButtonDown("Restart");
	}

	/*--------------------------------------------------------------------------------
	|| 選択処理
	--------------------------------------------------------------------------------*/
	private void SelectUpdate()
	{
		//	入力を選択中のインデックスに反映
		if (pickupAnimationProgress <= 0.0f)
			selectedNum += (int)inputX;
		selectedNum = (int)Mathf.Repeat(selectedNum, activeInvoiceCount);

		//	選択時の変数を用意
		float selectedScale = Mathf.Lerp(selectedMinScale, selectedMaxScale, animationValue);
		float selectedPosY = Mathf.Lerp(selectedMinPosY, selectedMaxPosY, animationValue);

		//	選択中のイメージを拡大、そうでないものを等倍に変化させる
		for (int i = 0; i < invoicies.Length; i++)
		{
			var taskImage = invoicies[i].sprite;
			bool isSelected = i == selectedNum;     //	現在のインデックスが選択中か否か

			float targetScale = isSelected ? selectedScale : nonSelectedScale;  //	スケール
			float targetPosY = isSelected ? selectedPosY : nonSelectedPosY;     //	Y座標
			Color targetColor = isSelected ? selectedColor : nonSelectedColor;  //	色

			//	スケールを適応
			taskImage.transform.localScale = Vector3.Lerp(taskImage.transform.localScale, Vector3.one * targetScale, Time.deltaTime * scaleChangeRate);
			//	Y座標を適応
			float posY = Mathf.Lerp(taskImage.transform.localPosition.y, targetPosY, Time.deltaTime * posChangeRate);
			taskImage.transform.localPosition = new Vector3(taskImage.transform.localPosition.x, posY);
			//	色を適応
			taskImage.color = Color.Lerp(taskImage.color, targetColor, Time.deltaTime * colorChangeRate);
		}
	}

	/*--------------------------------------------------------------------------------
	|| 枠の更新処理
	--------------------------------------------------------------------------------*/
	private void FrameUpdate()
	{
		//	枠の座標を変更
		frameImage.transform.localPosition = invoicies[selectedNum].sprite.transform.localPosition;

		//	枠のスケールを変更
		float absRad = Mathf.Abs(sin / 2);
		float t = 1.0f - Mathf.Cos(Mathf.Repeat(absRad, Mathf.PI / 2));
		frameImage.transform.localScale = Vector3.one * Mathf.Lerp(selectedMinScale, frameMaxScale, t);
		//	枠の透明度を変更
		Color col = Color.white;
		col.a = Mathf.Lerp(frameMinAlpha, frameMaxAlpha, t);
		frameImage.color = col;
	}

	/*--------------------------------------------------------------------------------
	|| アニメーションの更新処理
	--------------------------------------------------------------------------------*/
	private void AnimationUpdate()
	{
		//	ルートオブジェクトのアニメーションを進行
		rootAnimationProgress += Time.deltaTime * rootPosChangeRate;
		rootAnimationProgress = Mathf.Clamp01(rootAnimationProgress);
		//	ルートオブジェクトの移動
		taskImageRoot.transform.localPosition = Vector3.up * Mathf.LerpUnclamped(rootStartPosY, rootEndPosY, PopupEase(rootAnimationProgress));


		//	sin波の進行
		sin += Time.deltaTime * animationSpeed;
		sin = Mathf.Repeat(sin, Mathf.PI * 2);
		//	アニメーション用に正規化
		animationValue = (-Mathf.Cos(sin) + 1.0f) / 2.0f;
	}

	/*--------------------------------------------------------------------------------
	|| ピックアップの更新処理
	--------------------------------------------------------------------------------*/
	private void PickupUpdate()
	{
		if (!pickupFlag && pickupAnimationProgress <= 0.0f)
			return;

		float direction = pickupFlag ? 1 : -1;
		//	ピックアップのアニメーションを進行させる
		pickupAnimationProgress += Time.deltaTime * pickupSpeed * direction;
		pickupAnimationProgress = Mathf.Clamp01(pickupAnimationProgress);

		//	スケールの作成
		float scale = Mathf.Lerp(selectedMaxScale, pickupScale, pickupAnimationProgress);
		float xScale = Mathf.Cos(pickupAnimationProgress * Mathf.PI * 2) * scale;
		//	座標の作成
		float xPos = Mathf.Lerp(pickupStartX, 0.0f, pickupAnimationProgress);
		float yPos = Mathf.Lerp(selectedMaxPosY, pickupPosY, pickupAnimationProgress) + animationValue * pickupAmplitude;

		//	スケールの適応
		invoicies[selectedNum].sprite.transform.localScale = new Vector3(xScale, scale, scale);
		//	座標の適応
		invoicies[selectedNum].sprite.transform.localPosition = new Vector3(xPos, yPos);

		//	背景のアルファを適応
		backgroundAlphaControlelr.Alpha = pickupAnimationProgress;
	}

	/*--------------------------------------------------------------------------------
	|| キャンセル処理
	--------------------------------------------------------------------------------*/
	private void Cancel()
	{
		StartCoroutine(DeactivateCroutine());
	}

	/*--------------------------------------------------------------------------------
	|| ピックアップ処理
	--------------------------------------------------------------------------------*/
	private void PickUp()
	{
		//	X座標を保持
		pickupStartX = selectedNum * taskGapX - Mathf.FloorToInt(invoicies.Length / 2) * taskGapX;

		//	描画順の設定
		for (int i = 0; i < invoicies.Length; i++)
		{
			//	送り状の描画順
			int order = selectedNum == i ? 10 : 3;
			//	送り状に描画するものの描画順
			int childOrder = order + 1;

			//	送り状
			invoicies[i].sprite.sortingOrder = order;
			//	スタンプ
			invoicies[i].clearStamp.sortingOrder = childOrder;
			//	アイコン
			invoicies[i].timeIcon.sortingOrder = childOrder;
			invoicies[i].boxIcon.sortingOrder = childOrder;
			//	テキスト
			invoicies[i].stageNumText.sortingOrder = childOrder;
			invoicies[i].clearTimeText.sortingOrder = childOrder;
			invoicies[i].boxCountText.sortingOrder = childOrder;
		}

		//	ピックアップフラグの有効化
		pickupFlag = true;
	}

	/*--------------------------------------------------------------------------------
	|| テキストの書き換え処理
	--------------------------------------------------------------------------------*/
	private void SetTaskTexts()
	{
		//	ステージ番号の書き換え
		for (int i = 0; i < invoicies.Length; i++)
		{
			invoicies[i].stageNumText.text = selectedTask.StageID + " - " + (i + 1);
		}
	}

	/*--------------------------------------------------------------------------------
	|| クリア状況の反映
	--------------------------------------------------------------------------------*/
	private void SetClearStatus()
	{
		//	セーブデータの読み込み
		SaveData data = SaveDataLoader.LoadJson();

		//	クリアスタンプの設定
		for (int i = 0; i < invoicies.Length; i++)
		{
			TaskScore score = data.stageScores[selectedTask.StageID].scores[i];

			//	クリア時間が0以下のときは”未クリア”として処理
			if(score.clearTime <= 0.0f)
			{
				//	テキストの書き換え
				invoicies[i].clearTimeText.text = "--:--";
				invoicies[i].boxCountText.text = "--";
				//	スタンプの無効化
				invoicies[i].clearStamp.gameObject.SetActive(false);
			}
			//	クリア時間が0以上のときはテキストを書き換え、スタンプを表示
			else
			{
				int min = (int)(score.clearTime / 60.0f);	//	秒
				int sec = (int)(score.clearTime % 60.0f);	//	分

				//	テキストの書き換え
				invoicies[i].clearTimeText.text = min.ToString("D2") + ":" + sec.ToString("D2");
				invoicies[i].boxCountText.text = score.usedBoxCount.ToString("D2");
				//	スタンプの有効化
				invoicies[i].clearStamp.gameObject.SetActive(true);
			}
		}
	}

	/*--------------------------------------------------------------------------------
	|| タスクの確定処理
	--------------------------------------------------------------------------------*/
	private void Confirm()
	{
		selectedTask.TaskIndex = selectedNum;

		//	サウンドの再生
		soundPlayer.Play(0);

		//	シーンの読み込みを実行
		titleManager.LoadScene();
	}

	/*--------------------------------------------------------------------------------
	|| 有効化時処理
	--------------------------------------------------------------------------------*/
	public void Activate(Vector3 pos, int taskCount)
	{
		//	タスクのテキストを設定
		SetTaskTexts();
		//	クリア状況の反映
		SetClearStatus();
		//	送り状の表示を設定
		SetInvoiceActive(taskCount);


		StartCoroutine(ActivateCrountine(pos));
	}

	/*--------------------------------------------------------------------------------
	|| 送り状の有効化処理
	--------------------------------------------------------------------------------*/
	private void SetInvoiceActive(int taskCount)
	{
		//	有効な送り状の数を保持
		activeInvoiceCount = taskCount;

		//	アクティブを切り替える
		for (int i = 0; i < invoicies.Length; i++)
		{
			bool active = false;
			if (i < taskCount)
				active = true;

			invoicies[i].sprite.transform.gameObject.SetActive(active);
		}
	}

	/*--------------------------------------------------------------------------------
	|| アクティブ化コルーチン
	--------------------------------------------------------------------------------*/
	private IEnumerator ActivateCrountine(Vector3 pos)
	{
		//	座標の設定（Xのみ）
		transform.position = new Vector3(pos.x, transform.position.y, transform.position.z);

		yield return new WaitForSeconds(activateWaitTime);

		//	アクティブに設定
		isActive = true;
		//	ルートオブジェクトを有効化
		taskImageRoot.gameObject.SetActive(true);

		//	サウンドの再生
		soundPlayer.Play(2);
	}

	/*--------------------------------------------------------------------------------
	|| 非アクティブ化コルーチン
	--------------------------------------------------------------------------------*/
	private IEnumerator DeactivateCroutine()
	{
		yield return new WaitForSeconds(deactivateWaitTime);

		//	非アクティブに設定
		isActive = false;

		//	アニメーション用の変数を初期化
		rootAnimationProgress = 0.0f;
		//	ルートオブジェクトを無効化
		taskImageRoot.gameObject.SetActive(false);
		//	選択中のインデックスを初期化
		selectedNum = 0;

		//	ルートオブジェクトの座標を初期化
		taskImageRoot.transform.localPosition = Vector3.up * rootStartPosY;
	}

	/*--------------------------------------------------------------------------------
	|| タスク有効化時のイージング関数
	--------------------------------------------------------------------------------*/
	private float PopupEase(float x)
	{
		const float c1 = 1.70158f;
		const float c3 = c1 + 1.0f;

		return 1.0f + c3 * Mathf.Pow(x - 1.0f, 3.0f) + c1 * Mathf.Pow(x - 1.0f, 2.0f);
	}
}
