/**********************************************
 * 
 *  DoorGimmick.cs 
 *  開閉するドアに関する処理を記述
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/04
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DoorGimmick : ReceiveGimmick
{
	//	ドア固有の設定の構造体
	[System.Serializable]
	struct DoorExtraSettings
	{
		public Vector2	openOffset;
		public float	openSpeed;
		public float	closeSpeed;

		//	コンストラクタ
		public DoorExtraSettings(Vector2 openOffset, float openSpeed, float closeSpeed)
		{
			this.openOffset = openOffset;
			this.openSpeed = openSpeed;
			this.closeSpeed = closeSpeed;
		}
	}

	//	開閉
	[Header("開閉")]
	[SerializeField]
	private Transform		doorRoot;
	[SerializeField]
	private Vector2			openOffset;
	[SerializeField]
	private float			openSpeed;		//	開く速さ
	[SerializeField]
	private float			closeSpeed;     //	閉まる速さ

	public Vector2 OpenOffset	{ get { return openOffset; } set { openOffset = value; } }
	public float OpenSpeed		{ get { return openSpeed; } set { openSpeed = value; } }
	public float CloseSpeed		{ get { return closeSpeed; } set { closeSpeed = value; } }

	[Space, SerializeField]
	private bool			isOpen;
	public bool				IsOpen { set { isOpen = value; } get { return isOpen; } }

	[SerializeField]
	private Vector2 size;

	System.Action<bool> buttonEvent => (bool press) => { isOpen = press; };

	private float openProgress;

	//	更新処理
	private void Update()
	{
		if(isOpen)
		{
			openProgress += Time.deltaTime * openSpeed;
		}
		else
		{
			openProgress -= Time.deltaTime * closeSpeed;
		}

		openProgress = Mathf.Clamp01(openProgress);
		doorRoot.localPosition = Vector3.Lerp(Vector3.zero, openOffset, EasingFunctions.EaseInOutSine(openProgress));
	}

	/*--------------------------------------------------------------------------------
	|| アクションの登録（Awake）
	--------------------------------------------------------------------------------*/
	protected override void AddAction()
	{
		Sender.AddAction(buttonEvent);
	}
	/*--------------------------------------------------------------------------------
	|| アクションの登録を解除（OnDisable）
	--------------------------------------------------------------------------------*/
	protected override void RemoveAction()
	{
		Sender.RemoveAction(buttonEvent);
	}

	/*--------------------------------------------------------------------------------
	|| 固有の設定を設定する処理
	--------------------------------------------------------------------------------*/
	public override void SetExtraSetting(string json)
	{
		if (json == null || json == string.Empty)
			return;

		//	JSONを構造体に変換
		DoorExtraSettings exSetting = JsonUtility.FromJson<DoorExtraSettings>(json);

		//	各値を設定
		this.openOffset = exSetting.openOffset;
		this.openSpeed = exSetting.openSpeed;
		this.closeSpeed = exSetting.closeSpeed;
	}
	/*--------------------------------------------------------------------------------
	|| 固有の設定を取得する処理
	--------------------------------------------------------------------------------*/
	public override string GetExtraSetting()
	{
		//	構造体を作成
		DoorExtraSettings exSettings = new DoorExtraSettings(openOffset, openSpeed, closeSpeed);
		//	JSONに変換
		string ret = JsonUtility.ToJson(exSettings);

		return ret;
	}

#if UNITY_EDITOR
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(1, 0, 0, 0.5f);
		Gizmos.DrawCube(transform.position, Vector2.up * size.y + Vector2.right * size.x);
	}
#endif
}
