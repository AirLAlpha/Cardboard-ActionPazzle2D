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
		//	SE�̍Đ�
		soundPlayer.PlaySound(2);
	}

	/*--------------------------------------------------------------------------------
	|| �X���C�_�[�̍X�V����
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
	|| �X���C�_�[�̃L�����Z������
	--------------------------------------------------------------------------------*/
	public void ReturnSlider()
	{
		sliderSelected = false;
		DisableInput = false;
	}

	/*--------------------------------------------------------------------------------
	|| �L��������
	--------------------------------------------------------------------------------*/
	public void Activate(MenuBase menu = null)
	{
		parent = menu;
		gameObject.SetActive(true);
	}
}
