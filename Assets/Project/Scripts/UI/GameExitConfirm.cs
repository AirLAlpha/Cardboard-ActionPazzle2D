using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameExitConfirm : MenuBase
{
	[Header("ポーズメニュー")]
	[SerializeField]
	private TitleMenu		titleMenu;

	private enum MenuItem
	{
		CANCEL,
		CONFIRM,

		OVER_ID
	}

	private void OnEnable()
	{
		if(MenuCursor.parent.TryGetComponent<Canvas>(out Canvas canvas))
		{
			canvas.sortingOrder = 99;
			canvas.sortingOrder = 100;
		}

	}

	protected override void ConfirmUpdate()
	{
		switch ((MenuItem)CurrentIndex)
		{
			case MenuItem.CANCEL:
				gameObject.SetActive(false);
				titleMenu.DisableInput = false;
				break;

			case MenuItem.CONFIRM:
				#if UNITY_EDITOR
				UnityEditor.EditorApplication.isPlaying = false;	//エディタのプレイモードを解除
				#else
				Application.Quit();									//	終了
				#endif
				break;
		}

		//	SEの再生
		soundPlayer.PlaySound(2);
	}

	protected override void MenuUpdate()
	{
		if (InputConfirm)
			ConfirmUpdate();

		if(InputCancel)
		{
			gameObject.SetActive(false);
			titleMenu.DisableInput = false;

			soundPlayer.PlaySound(3);
		}
	}
}
