/**********************************************
 * 
 *  TitleCamera.cs 
 *  タイトルのカメラ処理を記述
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/08
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleCamera : ScrollCamera
{
	[Header("タスクセレクト")]
	[SerializeField]
	private float taskHeight;		//	タスク選択時の高さ
	[SerializeField]
	private float moveSpeed;		//	移動速度
	[SerializeField]
	private float nonSelectTaskY;	//	タスク選択時じゃない時のY座標

	[SerializeField]
	private bool isSelectTask;		//	タスクセレクトの状態


	//	実行前初期化処理
	private void Awake()
	{
		ignoredPosY = nonSelectTaskY;
	}

	//	初期化処理
	protected override void Start()
	{
		base.Start();
	}

	//	更新処理
	private void Update()
	{
		
	}

	protected override void LateUpdate()
	{
		if (!isSelectTask)
		{
			base.LateUpdate();
		}
		else
			SelectTaskUpdate();
	}

	/*--------------------------------------------------------------------------------
	|| タスク選択時更新処理
	--------------------------------------------------------------------------------*/
	private void SelectTaskUpdate()
	{
		Vector3 targetPos = saveNonOffsetPos + new Vector3(trackingOffset.x, trackingOffset.y) + Vector3.up * taskHeight;
		targetPos.z = -10.0f;

		transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * moveSpeed);
	}

	/*--------------------------------------------------------------------------------
	|| タスクセレクトの開始処理
	--------------------------------------------------------------------------------*/
	[ContextMenu("StartTaskSelect")]
	public void StartTaskSelect()
	{
		saveNonOffsetPos = currentTargetPos;
		this.isSelectTask = true;
	}

	/*--------------------------------------------------------------------------------
	|| タスクセレクトの終了処理
	--------------------------------------------------------------------------------*/
	[ContextMenu("EndTaskSelect")]
	public void EndTaskSelect()
	{
		ignoredPosY = nonSelectTaskY;
		base.saveNonOffsetPos = transform.position;
		this.isSelectTask = false;
	}
}
