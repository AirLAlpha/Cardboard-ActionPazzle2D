using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneSettings : GimmickSettingBase
{
	private CraneGimmick		craneGimmick;

	public override void SetTarget<T>(T target)
	{
		craneGimmick = target as CraneGimmick;

		//	初期化
		values = new int[ValueTextsCount];
		//	数値を取得して設定
		values[0] = (int)craneGimmick.Speed;
		values[1] = (int)craneGimmick.RangeY;

		ApplyTextAll();
	}

	public override void ApplyValues()
	{
		craneGimmick.Speed = values[0];
		craneGimmick.RangeY = values[1];
	}
}
