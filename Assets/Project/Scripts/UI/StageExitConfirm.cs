using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageExitConfirm : MenuBase
{
	[Header("ポーズメニュー")]
	[SerializeField]
	private PauseMenu		pauseMenu;

	private enum MenuItem
	{
		CANCEL,
		CONFIRM,

		OVER_ID
	}


	protected override void ConfirmUpdate()
	{
		switch ((MenuItem)CurrentIndex)
		{
			case MenuItem.CANCEL:
				gameObject.SetActive(false);
				pauseMenu.DisableUpdate= false;
				break;

			case MenuItem.CONFIRM:

				break;
		}
	}

	protected override void MenuUpdate()
	{
		if (InputConfirm)
			ConfirmUpdate();
	}
}
