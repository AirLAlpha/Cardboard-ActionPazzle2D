/**********************************************
 * 
 *  ControllerChecker.cs 
 *  コントローラーの接続確認処理を記述
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/05
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ControllerCecker : SingletonMonoBehaviour<ControllerCecker>
{
	//	接続確認
	[SerializeField]
	private bool		enableCheckUpdate;      //	毎フレーム接続処理を行うフラグ
	[SerializeField]
	private bool		awakeConnectedState;	//	最初の接続状態

	private int			controllerCount;		//	コントローラーの接続数
	private bool		controllerConnected;    //	コントローラーの接続フラグ

	//	イベント
	[SerializeField]
	public UnityEvent			OnControllerConnected;	//	コントローラー接続時
	public UnityEvent			OnControllerReleased;    //	コントローラー接続解除時
	public UnityEvent<bool>		OnControllerChanged;		//	コントローラーの接続数が変化したとき 


	//	プロパティ
	public bool			EnableCheckUpdate	{ get { return enableCheckUpdate; } set { enableCheckUpdate = value; } }
	public int			ControllerCount		{ get { return controllerCount; } }
	public bool			ControllerConnected { get { return controllerConnected; } }

	private bool firstUpdateEnded;  //	最初の更新フレームが終了したフラグ

	//	更新処理
	private void Update()
	{
		if (!firstUpdateEnded)
		{
			if (awakeConnectedState)
			{
				OnControllerConnected.Invoke();
			}
			else
			{
				OnControllerReleased.Invoke();
			}
			firstUpdateEnded = true;
		}
		if (enableCheckUpdate)
			CheckControllerConnected();
	}

	/*--------------------------------------------------------------------------------
	|| コントローラーの接続確認処理
	--------------------------------------------------------------------------------*/
	[ContextMenu("CheckController")]
	private void CheckControllerConnected()
	{
		//	現在接続されているコントローラー名を取得
		string[] joypads = Input.GetJoystickNames();

		//	実際に接続されている数を取得
		int currentConnectedCount = 0;
		for (int i = 0; i < joypads.Length; i++)
		{
			if (joypads[i] == "")
				continue;

			currentConnectedCount++;
		}

		//	接続フラグ
		bool isConnected = currentConnectedCount > 0;
		//	前回処理時とは接続状態が違ったときにイベントを実行
		if (currentConnectedCount != controllerCount)
		{
			if (currentConnectedCount == 0)
			{
				//	コントローラー接続解除時処理
				OnControllerReleased?.Invoke();
			}
			else
			{
				//	コントローラー接続時処理
				OnControllerConnected?.Invoke();
			}

			//	コントローラーの接続数が変化したとき
			OnControllerChanged?.Invoke(isConnected);
		}

		//	各変数を保持
		controllerConnected = isConnected;
		controllerCount = currentConnectedCount;
	}


#if UNITY_EDITOR
	[ContextMenu("InvokeConnected")]
	public void InvokeConnected()
	{
		OnControllerConnected?.Invoke();
	}

	[ContextMenu("InvokeReleased")]
	public void InvokeReleased()
	{
		OnControllerReleased?.Invoke();
	}

#endif
}
