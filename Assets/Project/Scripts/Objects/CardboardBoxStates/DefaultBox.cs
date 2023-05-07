using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardboardBox
{
	[System.Serializable]
	public class DefaultBox : CardboardBoxState
	{
		//	コンストラクタ
		public DefaultBox(DefaultBox defaultBox, CardboardBox parent) :
			base(parent)
		{
			this.stateColor = defaultBox.stateColor;
		}

		/*--------------------------------------------------------------------------------
		|| 状態の更新処理
		--------------------------------------------------------------------------------*/
		public override void StateUpdate()
		{
		}

		/*--------------------------------------------------------------------------------
		|| ステートに入ったときの処理
		--------------------------------------------------------------------------------*/
		public override void OnEnterState()
		{
			//	状態の色を変更する
			Parent.SpriteRenderer.color = stateColor;
		}

		/*--------------------------------------------------------------------------------
		|| ステートを抜けたときの処理
		--------------------------------------------------------------------------------*/
		public override void OnExitState()
		{
		}

		/*--------------------------------------------------------------------------------
		|| 衝突判定
		--------------------------------------------------------------------------------*/
		public override void OnCollisionEnter(Collision2D collision)
		{
		}
	}
}