using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingMenu : MenuBase
{
	private MenuBase parent;

	private enum MenuItem
	{
		BGM,
		SE,
		FULLSCREEN,
		RETURN,

		OVER_ID
	}

	private bool			sliderSelected;
	[SerializeField]
	private VolumeSlider	bgmSlider;
	[SerializeField]
	private VolumeSlider	seSlider;

	[SerializeField]
	private ButtonHint		buttonHint;

	protected override void Update()
	{
		if(DisableInput)
		{
			InputUpdate();
		}
		else
		{
			base.Update();
		}

		if (sliderSelected)
			SliderUpdate();
	}

	protected override void MenuUpdate()
	{
		if (InputConfirm)
			ConfirmUpdate();

		if (InputCancel)
		{
			gameObject.SetActive(false);
			parent.DisableInput = false;

			soundPlayer.PlaySound(3);

			ExportSetting();        //	設定の書き出し
		}
	}

	protected override void ConfirmUpdate()
	{
		switch ((MenuItem)CurrentIndex)
		{
			case MenuItem.BGM:
			case MenuItem.SE:
				sliderSelected = true;
				DisableInput = true;
				SetButtonHint(true);
				break;

			case MenuItem.RETURN:
				gameObject.SetActive(false);
				parent.DisableInput = false;

				ExportSetting();		//	設定の書き出し
				break;

			case MenuItem.FULLSCREEN:
				if (Screen.fullScreen)
				{
					Screen.SetResolution(1280, 720, false);
				}
				else
				{
					Screen.SetResolution(1920, 1080, true);
				}
				break;
		}
		//	SEの再生
		soundPlayer.PlaySound(2);
	}

	/*--------------------------------------------------------------------------------
	|| スライダーの更新処理
	--------------------------------------------------------------------------------*/
	private void SliderUpdate()
	{
		//	BGM
		if ((MenuItem)CurrentIndex == MenuItem.BGM)
		{
			bgmSlider.InputUpdate(InputVec, InputCancel);
			bgmSlider.CursorUpdate();
		}
		//	SE
		else
		{
			seSlider.InputUpdate(InputVec, InputCancel);
			seSlider.CursorUpdate();
		}
	}

	/*--------------------------------------------------------------------------------
	|| スライダーのキャンセル処理
	--------------------------------------------------------------------------------*/
	public void ReturnSlider()
	{
		sliderSelected = false;
		DisableInput = false;

		SetButtonHint(false);

		soundPlayer.PlaySound(3);
	}

	/*--------------------------------------------------------------------------------
	|| 有効化処理
	--------------------------------------------------------------------------------*/
	public void Activate(MenuBase menu = null)
	{
		parent = menu;
		gameObject.SetActive(true);
	}

	/*--------------------------------------------------------------------------------
	|| 設定の適応
	--------------------------------------------------------------------------------*/
	public void InitSettings(Setting setting)
	{
		bgmSlider.SetIndex(setting.bgmVol);
		seSlider.SetIndex(setting.seVol);

		if (setting.fullscreen)
		{
			Screen.SetResolution(1920, 1080, true);
		}
		else
		{
			Screen.SetResolution(1280, 720, false);
		}
	}

	/*--------------------------------------------------------------------------------
	|| 設定の書き出し
	--------------------------------------------------------------------------------*/
	public void ExportSetting()
	{
		Setting setting;
		setting.seVol = seSlider.SelectedIndex;
		setting.bgmVol = bgmSlider.SelectedIndex;
		setting.fullscreen = Screen.fullScreen;

		SettingLoader.ExportSetting(setting);
	}

	/*--------------------------------------------------------------------------------
	|| スライダーを選択したときのボタンUI切り替え
	--------------------------------------------------------------------------------*/
	private void SetButtonHint(bool enable)
	{
		if (enable)
		{
			buttonHint.SetActive("Vertical", false);
			buttonHint.SetActive("Jump", false);
			buttonHint.SetActive("Horizontal", true);
		} 
		else
		{
			buttonHint.SetActive("Vertical", true);
			buttonHint.SetActive("Jump", true);
			buttonHint.SetActive("Horizontal", false);
		}

	}

}
