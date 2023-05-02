using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeGimmick : Gimmick
{
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.transform.TryGetComponent<IBurnable>(out IBurnable burnable))
			burnable.Burn();
	}

	public override string GetExtraSetting()
	{
		return "";
	}
	public override void SetExtraSetting(string json)
	{
	}
}
