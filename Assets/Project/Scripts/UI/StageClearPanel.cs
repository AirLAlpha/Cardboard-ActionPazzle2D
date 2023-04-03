/**********************************************
 * 
 *  StageClearPanel.cs 
 *  ステージクリア時に表示されるパネルの処理
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/04
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageClearPanel : MonoBehaviour
{
	//	コンポーネント
	private RectTransform		rectTransform;

	[SerializeField]
	private Vector2				startOffset;
	[SerializeField]
	private float				positionChangeRate;

	//	実行前初期化処理
	private void Awake()
	{
		rectTransform = transform as RectTransform;

	}

	//	初期化処理
	private void Start()
	{
		StageManager.Instance.OnStageClear += OnStageClear;
		gameObject.SetActive(false);
	}

	//	更新処理
	private void Update()
	{
		EnabledUpdate();
	}

	/*--------------------------------------------------------------------------------
	|| ゴール時処理
	--------------------------------------------------------------------------------*/
	private void OnStageClear()
	{
		gameObject.SetActive(true);
	}

	/*--------------------------------------------------------------------------------
	|| アクティブになった際に表示する処理
	--------------------------------------------------------------------------------*/
	private void EnabledUpdate()
	{
		rectTransform.localPosition = Vector2.Lerp(rectTransform.localPosition, Vector2.zero, Time.deltaTime * positionChangeRate);
	}

	/*--------------------------------------------------------------------------------
	|| 非アクティブになったときの処理
	--------------------------------------------------------------------------------*/
	private void OnDisable()
	{
		rectTransform.localPosition = startOffset;
	}
}
