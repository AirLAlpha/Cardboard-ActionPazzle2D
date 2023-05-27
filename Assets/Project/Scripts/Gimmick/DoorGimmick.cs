/**********************************************
 * 
 *  DoorGimmick.cs 
 *  開閉するドアに関する処理を記述
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/04
 * 
 **********************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class DoorGimmick : ReceiveGimmick, IPauseable
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

	//	コンポーネント
	[Header("コンポーネント")]
	[SerializeField]
	private BoxCollider2D	doorCollider;

	//	開閉
	[Header("開閉")]
	[SerializeField]
	private Transform		doorRoot;
	[SerializeField]
	private Rigidbody2D		doorRootRb;
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

	[Header("乗っているオブジェクト")]
	[SerializeField]
	private LayerMask checkObjectMask;

	System.Action<bool> buttonEvent => (bool press) => { isOpen = press; };

	private float	openProgress;
	private Vector3 savePos;        //	前回処理時の座標（移動量取得用）
	private float saveDiffMagnitude;

	//	ポーズ
	private bool isPause;

	//	更新処理（Physics2Dと合わせるためにFixedUpdateを使用）
	private void FixedUpdate()
	{
		if (isPause)
			return;

		//	座標を保持しておく
		savePos = doorRoot.position;

		//	ドアの開閉状態によって進行度を更新する
		if (isOpen)
		{
			openProgress += Time.deltaTime * openSpeed;
		}
		else
		{
			openProgress -= Time.deltaTime * closeSpeed;
		}
		openProgress = Mathf.Clamp01(openProgress);
		//	進行度から座標を算出する（進行度にはイージングを適応）
		doorRoot.localPosition = Vector3.Lerp(Vector3.zero, openOffset, EasingFunctions.EaseInOutSine(openProgress));

		//Vector4 pos = Vector3.Lerp(Vector3.zero, openOffset, EasingFunctions.EaseInOutSine(openProgress));
		//pos.w = 1.0f;
		//var a = transform.localToWorldMatrix * pos;
		//doorRootRb.MovePosition(a);

		CheckOnRodeObject();
	}

	/*--------------------------------------------------------------------------------
	|| アクションの登録（Awake）
	--------------------------------------------------------------------------------*/
	protected override void AddAction()
	{
		if (Sender == null)
			return;

		Sender.AddAction(buttonEvent);
	}
	/*--------------------------------------------------------------------------------
	|| アクションの登録を解除（OnDisable）
	--------------------------------------------------------------------------------*/
	protected override void RemoveAction()
	{
		if (Sender == null)
			return;

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

	/*--------------------------------------------------------------------------------
	|| 上に乗っているオブジェクトに移動量を適応する処理
	--------------------------------------------------------------------------------*/
	private void CheckOnRodeObject()
	{
		Vector2 diff = doorRoot.position - savePos;							//	移動した差分（移動量）を求める
		float mag = diff.magnitude;											//	移動量の大きさを取得
		Vector3 moveDir = diff.normalized;									//	移動した方向をワールド空間で求める

		float dot = Vector2.Dot(Vector2.right, moveDir);					//	移動した方向と右ベクトルの内積を求める

		Vector3 startPos = savePos;											//	BoxCastの開始座標
		Vector3 checkDir = moveDir;											//	BoxCastの射出方向
		float distance = mag;                                               //	BoxCastの最大距離
		float moveValue = mag;
		//	下向きに移動している時
		if (moveDir.y < 0 &&
			Mathf.Abs(dot) <= Mathf.Cos(Mathf.PI / 4))
		{
			return;

			checkDir *= -1;             //	確認方向を上に向ける

			//	移動開始時と終了時に浮くのが気になるときは以下のコメントアウトを解除する
			//	前回処理時の座標を使用するために前回の差分を距離に加算
			//if (Mathf.Approximately(saveDiffMagnitude, 0.0f))
			//	saveDiffMagnitude = mag;
			//distance += saveDiffMagnitude;

			moveValue = saveDiffMagnitude;
		}


		//	BoxCastを行いすべてのオブジェクトを取得
		var hitResult = Physics2D.BoxCastAll(startPos, doorCollider.size * 0.9f, transform.localEulerAngles.z, checkDir, distance, checkObjectMask);
		//	すべてのオブジェクトに移動量を加算する
		foreach (var item in hitResult)
		{
			item.transform.position += moveDir * moveValue;
			//if(item.transform.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb2d))
			//{
			//	rb2d.MovePosition(item.transform.position + moveDir * moveValue);
			//	rb2d.angularVelocity = 0;
			//}
		}

		//	今回の差分を保持しておく
		saveDiffMagnitude = mag;

		Debug.DrawRay(startPos, checkDir * distance, Color.red);
		Debug.DrawLine(startPos + checkDir * distance + new Vector3(-1.5f, 0.5f), startPos + checkDir * distance + new Vector3(1.5f, 0.5f));
	}

	public void Pause()
	{
		isPause = true;
	}

	public void Resume()
	{
		isPause = false;
	}


	//	キャストを行う
	private bool TryCast<Type>(object obj, out Type type) where Type : class
	{
		//	キャストを行う
		type = obj as Type;

		if (type == null)
			return false;

		return true;
	}
}
