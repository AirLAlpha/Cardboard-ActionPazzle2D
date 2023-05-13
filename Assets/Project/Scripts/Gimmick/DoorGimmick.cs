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

	//	更新処理
	private void Update()
	{
		//	座標を保持しておく
<<<<<<< HEAD
		savePos = doorRoot.localPosition;
=======
		//savePos = doorRoot.localPosition;
>>>>>>> 61bb0e1f8d4cd31ec5078bfe6af5c9e89e009668

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
<<<<<<< HEAD
		doorRoot.localPosition = Vector3.Lerp(Vector3.zero, openOffset, EasingFunctions.EaseInOutSine(openProgress));

		//var b = Vector3.Lerp(Vector3.zero, openOffset, EasingFunctions.EaseInOutSine(openProgress));
		//var a = transform.TransformPoint(b);
		//doorRootRb.MovePosition(a);

		CheckOnRodeObject();
=======
		//doorRoot.localPosition = Vector3.Lerp(Vector3.zero, openOffset, EasingFunctions.EaseInOutSine(openProgress));

		var b = Vector3.Lerp(Vector3.zero, openOffset, EasingFunctions.EaseInOutSine(openProgress));
		var a = transform.TransformPoint(b);
		doorRootRb.MovePosition(a);
>>>>>>> 61bb0e1f8d4cd31ec5078bfe6af5c9e89e009668
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
		const float DOWN_OFFSET = 0.05f;		//	下向きに動くときのオフセット

		Vector2 diff = doorRoot.localPosition - savePos;
		Vector3 moveDir = transform.TransformDirection(diff.normalized);
		float mag = diff.magnitude;

		float dot = Vector2.Dot(Vector2.right, moveDir.normalized);

		Vector3 startPos = doorRoot.position;
		Vector3 checkDir = moveDir;
		float distance = mag;
		if (mag > 0.001f &&
			moveDir.y < 0 &&
			Mathf.Abs(dot) <= Mathf.Cos(Mathf.PI / 4))
		{
			checkDir *= -1;
			distance += DOWN_OFFSET;
			//startPos = transform.TransformPoint(savePos);

			print("aaa");
		}

		var a = Physics2D.BoxCastAll(startPos, new Vector2(1, 3), transform.localEulerAngles.z, checkDir, distance, checkObjectMask);
		foreach (var item in a)
		{
			print(item.transform.name);
			item.transform.position += moveDir * mag;
		}
		//print(mag);
		Debug.DrawRay(startPos, checkDir * mag + checkDir, Color.red);
	}


#if UNITY_EDITOR
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(1, 0, 0, 0.5f);
		Gizmos.DrawCube(transform.position, Vector2.up * size.y + Vector2.right * size.x);
	}
#endif
}
