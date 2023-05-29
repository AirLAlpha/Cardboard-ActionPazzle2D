using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleTasks : MonoBehaviour
{
	//	選択
	private int				selectedTaskIndex;
	private bool			enableTaskSelect;   //	タスク選択の開始フラグ

	[Header("サウンド")]
	[SerializeField]
	private SoundPlayer soundPlayer;

	[Header("入力")]
	[SerializeField]
	private float inputInterval;

	private float inputWaitTime;

	[Header("送り状")]
	[SerializeField]
	private int						stageNum;				//	送り状に表示するステージ番号
	[SerializeField]
	private Invoice[]				invoices;				//	送り状
	[SerializeField]
	private SpriteRenderer[]		invoiceOverlay;

	[Header("ルートオブジェクトのアニメーション")]
	[SerializeField]
	private float			zoomSpeed;
	[SerializeField]
	private Vector2			idleScale;
	[SerializeField]
	private Vector2			zoomScale;
	[SerializeField]
	private Vector2			idlePos;
	[SerializeField]
	private Vector2			zoomPos;

	[Header("送り状のアニメーション")]
	[SerializeField]
	private Vector2			idleInvoiceScale;
	[SerializeField]
	private Vector2			selectedInvoiceScale;
	[SerializeField]
	private float			invoiceZoomSpeed;
	[SerializeField]
	private Color			idleInvoiceColor;
	[SerializeField]
	private Color			selectedInvoiceColor;

	//	プロパティ
	public int SelectedTaskIndex { get { return selectedTaskIndex; } }


	//	実行前初期化処理
	private void Awake()
	{
		
	}

	//	初期化処理
	private void Start()
	{
		//	送り状のステージ番号を設定
		for (int i = 0; i < invoices.Length; i++)
		{
			invoices[i].SetStageNum(stageNum, i);
		}
	}

	//	更新処理
	private void Update()
	{
		InputUpdate();
		RootAnimationUpdate();
		SelectInvoiceAnimationUpdate();
	}

	/*--------------------------------------------------------------------------------
	|| タスク選択の開始
	--------------------------------------------------------------------------------*/
	public void StartTaskSelect()
	{
		enableTaskSelect = true;

		selectedTaskIndex = 0;
	}

	/*--------------------------------------------------------------------------------
	|| タスク選択の終了
	--------------------------------------------------------------------------------*/
	public void EndTaskSelect()
	{
		enableTaskSelect = false;
	}

	/*--------------------------------------------------------------------------------
	|| 入力処理
	--------------------------------------------------------------------------------*/
	private void InputUpdate()
	{
		if (!enableTaskSelect)
		{
			inputWaitTime = 0;
			return;
		}

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
		if (selectedTaskIndex + x >= invoices.Length ||
			selectedTaskIndex + x < 0)
			return;

		selectedTaskIndex += x;
		inputWaitTime = inputInterval;

		soundPlayer.PlaySound(2);
	}

	/*--------------------------------------------------------------------------------
	|| ルートオブジェクトのアニメーション
	--------------------------------------------------------------------------------*/
	private void RootAnimationUpdate()
	{
		Vector2 targetScale = enableTaskSelect ? zoomScale : idleScale;
		transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * zoomSpeed);

		Vector2 targetPos = enableTaskSelect ? zoomPos : idlePos;
		transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * zoomSpeed);
	}

	/*--------------------------------------------------------------------------------
	|| 送り状の選択
	--------------------------------------------------------------------------------*/
	private void SelectInvoiceAnimationUpdate()
	{
		for (int i = 0; i < invoices.Length; i++)
		{
			Vector2 targetScale = idleInvoiceScale;
			Color targetColor = selectedInvoiceColor;
			if (enableTaskSelect)
			{
				if (i == selectedTaskIndex)
					targetScale = selectedInvoiceScale;
				else
					targetColor = idleInvoiceColor;
			}
			//	スケールを変更
			invoices[i].transform.localScale = Vector2.Lerp(invoices[i].transform.localScale, targetScale, Time.deltaTime * invoiceZoomSpeed);

			//	表示順の変更
			invoices[i].SpriteRenderer.sortingOrder = i == selectedTaskIndex ? 102 : 101;
			//	色の変更
			invoiceOverlay[i].color = Color.Lerp(invoiceOverlay[i].color, targetColor, Time.deltaTime * invoiceZoomSpeed);
		}
	}

	/*--------------------------------------------------------------------------------
	|| セーブデータの設定
	--------------------------------------------------------------------------------*/
	public void SetStageScore(StageScore score)
	{
		for (int i = 0; i < invoices.Length; i++)
		{
			if (i >= score.scores.Length)
				break;

			invoices[i].SetScore(score.scores[i]);
		}
	}
}
