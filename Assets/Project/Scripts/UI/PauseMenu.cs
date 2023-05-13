/**********************************************
 * 
 *  PauseMenu.cs 
 *  ポーズメニューの処理を記述
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/05/11
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MenuBase
{
	//	ポーズメニューの項目
	private enum PauseMenuItem
	{
		RESUME,		//	再開
		RETRY,		//	リトライ
		//CONFIG,		//	設定
		EXIT,		//	タイトルへ戻る
	}
	//	現在選択中の項目
	private PauseMenuItem		CurrentSelectItem => (PauseMenuItem)CurrentIndex;

	[Header("コンポーネント")]
	[SerializeField]
	private PauseManager		pauseManager;		//	ポーズマネージャー


	//	実行前初期化処理
	private void Awake()
	{
		
	}

	//	初期化処理
	private void Start()
	{
		
	}

	//	更新処理
	protected override void MenuUpdate()
	{
		//	メニューキーでポーズを解除
		if (Input.GetButtonDown("Menu"))
			DeactivatePauseMenu();

		//	確定処理
		if (InputConfirm)
			ConfirmUpdate();
	}

	/*--------------------------------------------------------------------------------
	|| 決定処理
	--------------------------------------------------------------------------------*/
	protected override void ConfirmUpdate()
	{
		switch (CurrentSelectItem)
		{
			case PauseMenuItem.RESUME:
				DeactivatePauseMenu();					//	ポーズの解除
				break;

			case PauseMenuItem.RETRY:
				StageManager.Instance.ResetStage();		//	ステージのリセット
				break;

			//case PauseMenuItem.CONFIG:
			//	break;

			case PauseMenuItem.EXIT:
				StageManager.Instance.ReturnTitle();	//	タイトルへ戻る
				break;
		}
	}

	/*--------------------------------------------------------------------------------
	|| ポーズメニューの有効化
	--------------------------------------------------------------------------------*/
	public void ActivatePauseMenu()
	{
		gameObject.SetActive(true);		//	オブジェクトを有効化
	}

	/*--------------------------------------------------------------------------------
	|| ポーズメニューの無効化
	--------------------------------------------------------------------------------*/
	private void DeactivatePauseMenu()
	{
		gameObject.SetActive(false);	//	オブジェクトを無効化

		pauseManager.Resume();			//	ポーズの解除
	}
}