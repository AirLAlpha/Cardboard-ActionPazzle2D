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
		values[2] = (int)this.target.OpenSpeed;
		values[3] = (int)this.target.CloseSpeed;
		//	テキストに反映
		ApplyTextAll();
	}

	public override void ApplyValues()
	{
		Vector2 openOffset = new Vector2(values[0], values[1]);
		this.target.OpenOffset = openOffset;
		this.target.OpenSpeed = values[2];
		this.target.CloseSpeed = values[3];
	}

}
