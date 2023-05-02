using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorSettings : GimmickSettingBase
{
	private const int SPEED_MIN = 5;
	private const int SPEED_MAX = 20;

	private BeltConveyorGimmick conveyorGimmick;

	//	ターゲットの設定
	public override void SetTarget<T>(T target)
	{
		conveyorGimmick = target as BeltConveyorGimmick;

		//	値の初期化
		values = new int[ValueTextsCount];
		//	数値を取得して設定
		values[0] = (int)(conveyorGimmick.SpeedModifier * 10.0f);
		values[1] = conveyorGimmick.Inverse ? 1 : 0;

		ApplyTextAll();
	}

	//	設定した値の適応
	public override void ApplyValues()
	{
		conveyorGimmick.SpeedModifier = values[0] / 10.0f;
		conveyorGimmick.Inverse = values[1] > 0 ? true : false;
	}

	//	値変更の処理
	public override void ValueChangeUpdate(float inputX)
	{
		values[currentSelect] += (int)inputX;
		//	クランプ処理
		values[0] = Mathf.Clamp(values[0], SPEED_MIN, SPEED_MAX);
		values[1] = (int)Mathf.Clamp01(values[1]);

		//	テキストに反映
		ApplyTextAll();
	}

	//	テキストに適応する処理
	protected override void ApplyTextAll()
	{
		string modifier = (values[0] % 10.0f) == 0 ? ".0 x" : " x";
		valueTexts[0].text = (values[0] / 10.0f).ToString() + modifier;

		valueTexts[1].text = values[1] > 0 ? "TRUE" : "FALSE";
	}
}
