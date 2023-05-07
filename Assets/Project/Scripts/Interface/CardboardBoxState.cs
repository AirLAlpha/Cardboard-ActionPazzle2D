/**********************************************
 * 
 *  ICardboardBoxState.cs 
 *  ダンボールの状態を示すインターフェース
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/17
 * 
 **********************************************/
using UnityEngine;

public abstract class CardboardBoxState
{
	private CardboardBox.CardboardBox parent;
	protected CardboardBox.CardboardBox Parent { get { return parent; } }

	//	コンストラクタ
	public CardboardBoxState(CardboardBox.CardboardBox parent)
	{
		this.parent = parent;
	}

	public abstract void OnEnterState();							//	ステートに入ったときの処理
	public abstract void OnExitState();								//	ステートを抜けるときの処理

	public abstract void StateUpdate();								//	各ステートにおける更新処理
	public abstract void OnCollisionEnter(Collision2D collision);	//	衝突判定
}
