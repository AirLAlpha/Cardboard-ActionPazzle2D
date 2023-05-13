using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSettings : GimmickSettingBase
{
	private DoorGimmick target;

	public override void SetTarget<T>(T target)
	{
		this.target = target as DoorGimmick;

		//	値の初期化
		values = new int[ValueTextsCount];
		//	数値を取得して設定
		values[0] = (int)this.target.OpenOffset.x;
		values[1] = (int)this.target.OpenOffset.y;
		//	速度は少数第一位が一の位になるよう取得
		values[2] = (int)(this.target.OpenSpeed * 10.0f);
		values[3] = (int)(this.target.CloseSpeed * 10.0f);
		//	テキストに反映
		ApplyTextAll();
	}

	public override void ApplyValues()
	{
		Vector2 openOffset = new Vector2(values[0], values[1]);
		this.target.OpenOffset = openOffset;

		//	速度は少数で設定する
		this.target.OpenSpeed = values[2] / 10.0f;
		this.target.CloseSpeed = values[3] / 10.0f;
	}

	protected override void ApplyTextAll()
	{
		//	オフセットはそのまま設定
		for (int i = 0; i < 2; i++)
		{
			valueTexts[i].text = values[i].ToString("D2");
		}

		//	速度は少数で表示
		for (int i = 2; i < 4; i++)
		{
			valueTexts[i].text = (values[i] / 10.0f).ToString("F1");
		}
	}

}
