/**********************************************
 * 
 *  ResultMenu.cs 
 *  リザルトメニューに関する処理を記述
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/10
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultMenu : MenuBase
{
	private enum MenuItems
	{
		NEXT_TASK,
		RETRY,
		RETURN_TITLE,

		OVER_ID
	}

	/*--------------------------------------------------------------------------------
	|| メニューの更新処理
	--------------------------------------------------------------------------------*/
	protected override void MenuUpdate()
	{
		if (InputConfirm)
			ConfirmUpdate();
	}

	/*--------------------------------------------------------------------------------
	|| メニューの決定処理
	--------------------------------------------------------------------------------*/
	protected override void ConfirmUpdate()
	{
		switch ((MenuItems)CurrentIndex)
		{
			case MenuItems.NEXT_TASK:
				StageManager.Instance.LoadNextStage();
				break;
			case MenuItems.RETRY:
				StageManager.Instance.ResetStage();
				break;
			case MenuItems.RETURN_TITLE:
				StageManager.Instance.ReturnTitle();
				break;
		}
	}

	/*--------------------------------------------------------------------------------
	|| 「つぎへ」の有効化を設定
	--------------------------------------------------------------------------------*/
	public void SetEnableNext(bool active)
	{
		SetItemActive(0, active);
	}

}
