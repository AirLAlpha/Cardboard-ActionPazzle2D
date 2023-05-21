using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleTasks : MonoBehaviour
{
	//	選択
	private int				selectedTaskIndex;
	private bool			enableTaskSelect;   //	タスク選択の開始フラグ

	[Header("入力")]
	[SerializeField]
	private float inputInterval;

	private float inputWaitTime;

	[Header("送り状")]
	[SerializeField]
	private SpriteRenderer[]		invoices;           //	送り状

	private Transform		InvoiceRoot => transform;       //	ルートオブジェクト

	[Header("ルートオブジェクトのアニメーション")]
	[SerializeField]
	private float			zoomSpeed;
	[SerializeField]
	private Vector2			idleScale;
	[SerializeField]
	private Vector2			zoomScale;

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
		if (selectedTaskIndex + x > invoices.Length ||
			selectedTaskIndex + x < 0)
			return;

		selectedTaskIndex += x;
		inputWaitTime = inputInterval;
	}

	/*--------------------------------------------------------------------------------
	|| ルートオブジェクトのアニメーション
	--------------------------------------------------------------------------------*/
	private void RootAnimationUpdate()
	{
		Vector2 targetScale = enableTaskSelect ? zoomScale : idleScale;
		InvoiceRoot.localScale = Vector3.Lerp(InvoiceRoot.localScale, targetScale, Time.deltaTime * zoomSpeed);
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
			invoices[i].sortingOrder = i == selectedTaskIndex ? 102 : 101;
			//	色の変更
			invoices[i].color = Color.Lerp(invoices[i].color, targetColor, Time.deltaTime * invoiceZoomSpeed);
		}
	}


}
