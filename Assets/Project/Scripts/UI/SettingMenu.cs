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

			ExportSetting();        //	�ݒ�̏����o��
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

				ExportSetting();		//	�ݒ�̏����o��
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
		//	SE�̍Đ�
		soundPlayer.PlaySound(2);
	}

	/*--------------------------------------------------------------------------------
	|| �X���C�_�[�̍X�V����
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
	|| �X���C�_�[�̃L�����Z������
	--------------------------------------------------------------------------------*/
	public void ReturnSlider()
	{
		sliderSelected = false;
		DisableInput = false;

		SetButtonHint(false);

		soundPlayer.PlaySound(3);
	}

	/*--------------------------------------------------------------------------------
	|| �L��������
	--------------------------------------------------------------------------------*/
	public void Activate(MenuBase menu = null)
	{
		parent = menu;
		gameObject.SetActive(true);
	}

	/*--------------------------------------------------------------------------------
	|| �ݒ�̓K��
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
	|| �ݒ�̏����o��
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
	|| �X���C�_�[��I�������Ƃ��̃{�^��UI�؂�ւ�
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
