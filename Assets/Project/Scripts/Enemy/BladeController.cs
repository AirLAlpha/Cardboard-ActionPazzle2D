using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BladeController : ReceiveGimmick
{
	[SerializeField]
	private BladeEnemy enemy;

	System.Action<bool> buttonEvent => (bool pressed) => enemy.Packable = pressed;

	public override string GetExtraSetting()
	{
		return string.Empty;
	}

	public override void SetExtraSetting(string json)
	{
	}

	protected override void AddAction()
	{
		if (Sender == null)
			return;

		Sender.AddAction(buttonEvent);
	}

	protected override void RemoveAction()
	{
		if (Sender == null)
			return;

		Sender.RemoveAction(buttonEvent);
	}
}
