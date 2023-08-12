using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleMenu : MenuBase
{
	private enum MenuItem
	{
		RESUME,
		SETTINGS,
		EXIT,

		OVER_ID
	}

	[SerializeField]
	private ButtonHint buttonHint;
	[SerializeField]
	private GameExitConfirm exitConfirm;
	[SerializeField]
	private SettingMenu		settingMenu;

	protected override void Update()
	{
		if (DisableInput)
			return;

		base.Update();
	}

	protected override void MenuUpdate()
	{
		//	メニューキーでポーズを解除
		if (Input.GetButtonDown("Menu"))
		{
			DeactivateMenu();
			soundPlayer.PlaySound(3);
		}

		if (InputConfirm)
			ConfirmUpdate();

		if (InputCancel)
		{
			DeactivateMenu();
			soundPlayer.PlaySound(3);
		}
	}

	protected override void ConfirmUpdate()
	{
		switch ((MenuItem)CurrentIndex)
		{
			case MenuItem.RESUME:
				DeactivateMenu();
				break;
			case MenuItem.SETTINGS:
				settingMenu.Activate(this);
				DisableInput = true;
				break;
			case MenuItem.EXIT:
				exitConfirm.gameObject.SetActive(true);
				DisableInput = true;
				break;
		}

		//	SEの再生
		soundPlayer.PlaySound(2);

	}

	public void ActivateMenu()
	{
		if (gameObject.activeSelf)
			return;

		//	ボタンヒントのアクティブを切り替え
		buttonHint.SetActive("Vertical", true);
		buttonHint.SetActive("Jump", true);
		buttonHint.SetActive("Horizontal", false);
		buttonHint.SetActive("Restart", true);
		buttonHint.SetActive("Menu", false);

		gameObject.SetActive(true);

		soundPlayer.PlaySound(4);
	}

	private void DeactivateMenu()
	{
		if (!gameObject.activeSelf)
			return;

		//	ボタンヒントのアクティブを切り替え
		buttonHint.SetActive("Vertical", false);
		buttonHint.SetActive("Jump", true);
		buttonHint.SetActive("Horizontal", true);
		buttonHint.SetActive("Restart", false);
		buttonHint.SetActive("Menu", true);

		gameObject.SetActive(false);
	}
}
