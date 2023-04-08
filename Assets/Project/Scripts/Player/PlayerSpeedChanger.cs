using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerMove))]
public class PlayerSpeedChanger : MonoBehaviour
{
	//	コンポーネント
	private PlayerMove			playerMove;

	//	入力
	private bool				inputDash;

	//	速度
	[SerializeField]
	private float				normalSpeed;
	[SerializeField]
	private float				dashSpeed;


	//	実行前初期化処理
	private void Awake()
	{
		//	コンポーネントの取得
		playerMove = GetComponent<PlayerMove>();
	}

	//	初期化処理
	private void Start()
	{
		normalSpeed = playerMove.MoveSpeed;
	}

	//	更新処理
	private void Update()
	{
		InputUpdate();
		SpeedChangeUpdate();
	}


	/*--------------------------------------------------------------------------------
	|| 入力処理
	--------------------------------------------------------------------------------*/
	private void InputUpdate()
	{
		inputDash = Input.GetButton("Fire1");
	}

	/*--------------------------------------------------------------------------------
	|| スピードの更新処理
	--------------------------------------------------------------------------------*/
	private void SpeedChangeUpdate()
	{
		float targetSpeed = normalSpeed;
		if (inputDash)
			targetSpeed = dashSpeed;

		playerMove.MoveSpeed = Mathf.Lerp(playerMove.MoveSpeed, targetSpeed, playerMove.SpeedChangeRate);
	}
}
