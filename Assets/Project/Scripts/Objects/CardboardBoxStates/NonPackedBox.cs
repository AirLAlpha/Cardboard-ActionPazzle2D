/**********************************************
 * 
 *  NonPackedBox.cs 
 *  未梱包状態のステート
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/05/07
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CardboardBox
{
	[System.Serializable]
	public class NonPackedBox : CardboardBoxState
	{
		//	コンポーネント
		private SpriteRenderer	spriteRenderer;

		[Header("梱包時のエフェクト")]
		[SerializeField]
		private ParticleSystem packedEffect;        //	梱包時エフェクト

		//	コンストラクタ
		public NonPackedBox(NonPackedBox nonPacked, CardboardBox parent) :
			base(parent)
		{
			this.stateColor = nonPacked.stateColor;
			this.packedEffect = nonPacked.packedEffect;

			//	コンポーネントの取得
			this.spriteRenderer = Parent.SpriteRenderer;
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
			spriteRenderer.color = stateColor;
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
			const float BREAKING_VELOCITY = 13.5f;

			if (collision.transform.TryGetComponent<IPackable>(out var hit))
			{
				//	相手の梱包処理を実行
				CardboardType type = hit.Packing();
				if(type == CardboardType.BREAKABLE)
				{
					float hitVelMag = collision.relativeVelocity.sqrMagnitude;
					if (hitVelMag >= BREAKING_VELOCITY * BREAKING_VELOCITY)
						return;
				}

				//	自身の梱包時処理を実行
				Packing(type, hit);
			}
		}

		/*--------------------------------------------------------------------------------
		|| 梱包処理
		--------------------------------------------------------------------------------*/
		public void Packing(CardboardType type, IPackable packable)
		{
			if (type == CardboardType.NONPACKABLE)
			{
				Parent.Burn();
				return;
			}

			//	ステートの切り替え
			CardboardBoxState nextState = null;
			switch (type)
			{
				case CardboardType.NORMAL:
					nextState = Parent.DefaultBox;
					break;

				case CardboardType.BREAKABLE:
					nextState = Parent.BreakableBox;
					break;

				case CardboardType.RIGHTSIDEUP:
					nextState = Parent.RightSideUpBox;
					break;

				default:
					break;
			}
			//	ステートが割り当てられなかったときは処理しない
			if (nextState == null)
				return;

			//	スケールの初期化
			Parent.transform.localScale = Vector3.one;
			//	ラベルの有効化
			Parent.LabelSprites[0].gameObject.SetActive(true);
			Parent.LabelSprites[1].sprite = packable.LabelSprite;
			//	パーティクルの再生
			packedEffect.Play();

			//	ステートの更新
			Parent.SetState(nextState);
			//	SEの再生
			Parent.SoundPlayer.PlaySound(9);
		}
	}
}
