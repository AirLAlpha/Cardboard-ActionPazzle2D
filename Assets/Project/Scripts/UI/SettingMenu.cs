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
		RETURN,

		OVER_ID
	}

	private bool			sliderSelected;
	[SerializeField]
	private VolumeSlider	bgmSlider;
	[SerializeField]
	private VolumeSlider	seSlider;

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
	}

	protected override void ConfirmUpdate()
	{
		switch ((MenuItem)CurrentIndex)
		{
			case MenuItem.BGM:
			case MenuItem.SE:
				sliderSelected = true;
				DisableInput = true;
				break;

			case MenuItem.RETURN:
				gameObject.SetActive(false);
				parent.DisableInput = false;
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
		if((MenuItem)CurrentIndex == MenuItem.BGM)
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
	}

	/*--------------------------------------------------------------------------------
	|| 有効化処理
	--------------------------------------------------------------------------------*/
	public void Activate(MenuBase menu = null)
	{
		parent = menu;
		gameObject.SetActive(true);
	}
}
