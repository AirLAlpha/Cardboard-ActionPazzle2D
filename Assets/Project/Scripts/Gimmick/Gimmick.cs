/**********************************************
 * 
 *  Gimmick.cs 
 *  ギミックの基底クラスを記述
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/19
 * 
 **********************************************/
using UnityEngine;
using UnityEditor;

//	ギミックのタイプ
public enum GimmickType
{
	EVENT_NONE,             //	イベントなし		（バネ、トゲ、etc...）

	EVENT_SEND,             //	イベントを送る		（ボタン、レバー、etc...）
	EVENT_RECEIVE           //	イベントを受け取る	（ドア、ベルトコンベア、etc...）
}

public abstract class Gimmick : MonoBehaviour
{
	[SerializeField]
	private GimmickType type;

	public GimmickType Type { get { return type; }  set { type = value; } }
}

//	メッセージを送る側
public abstract class SendGimmick : Gimmick
{
	//	ギミック作動時のイベント
	private System.Action<bool> gimmickAction;

	/*--------------------------------------------------------------------------------
	|| アクションの追加
	--------------------------------------------------------------------------------*/
	public void AddAction(System.Action<bool> action)
	{
		gimmickAction += action;
	}

	/*--------------------------------------------------------------------------------
	|| アクションの削除
	--------------------------------------------------------------------------------*/
	public void RemoveAction(System.Action<bool> action)
	{
		gimmickAction -= action;
	}

	/*--------------------------------------------------------------------------------
	|| アクションの実行
	--------------------------------------------------------------------------------*/
	protected void InvokeAction(bool value)
	{
		gimmickAction.Invoke(value);
	}
}

//	メッセージを受け取る側
public abstract class ReceiveGimmick : Gimmick
{
	[SerializeField]
	private SendGimmick sender;		//	イベントの登録先
	public SendGimmick Sender { get { return this.sender; } set { this.sender = value; } }

	public void Initialize()
	{
		//	アクションの登録
		AddAction();
	}

	private void OnDisable()
	{
		//	アクションの登録を解除
		RemoveAction();
	}

	protected abstract void AddAction();
	protected abstract void RemoveAction();
}
