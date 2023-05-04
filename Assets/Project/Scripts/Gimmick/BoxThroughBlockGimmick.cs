using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxThroughBlockGimmick : Gimmick
{
	public override string GetExtraSetting()
	{
		return string.Empty;
	}

	public override void SetExtraSetting(string json)
	{
	}

	//	実行前初期化処理
	private void Awake()
	{
	}
}
