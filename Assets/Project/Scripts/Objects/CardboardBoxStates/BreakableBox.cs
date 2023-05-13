using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardboardBox
{
	[System.Serializable]
	public class BreakableBox : CardboardBoxState
	{
		[SerializeField]
		private float	breakingVelocity;		//	破壊されてしまう速度

		//	コンストラクタ
		public BreakableBox(BreakableBox breakable, CardboardBox parent) :
			base(parent)
		{
			this.stateColor = breakable.stateColor;
			this.breakingVelocity = breakable.breakingVelocity;
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
			Vector2 hitVel = collision.relativeVelocity;

			//	プレイヤーのときのみの処理
			if (collision.transform.tag == "Player")
			{
				const float IGNORE_RAD = 45.0f;		//	無視するとする角度

				//	プレイヤーとの角度を求める
				Vector2 dir = (collision.transform.position - Parent.transform.position).normalized;
				float rad = Mathf.Abs(Mathf.Atan2(dir.y, dir.x));

				//	プレイヤーとの角度が一定の範囲でなければ処理しない
				if (rad < Mathf.Deg2Rad * IGNORE_RAD ||
					rad > Mathf.Deg2Rad * (180.0f - IGNORE_RAD))
					return;
			}

			float hitVelMag = hitVel.sqrMagnitude;
			if (hitVelMag >= breakingVelocity * breakingVelocity)
				Parent.Burn();
		}
	}
}