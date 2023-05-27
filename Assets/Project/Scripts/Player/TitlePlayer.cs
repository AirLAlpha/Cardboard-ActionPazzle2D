using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitlePlayer : MonoBehaviour
{
	private Direction currentDir;

	[SerializeField]
	private SpriteRenderer spriteRenderer;

	[Header("移動")]
	[SerializeField]
	private Vector3		targetPos;
	[SerializeField]
	private Vector3		targetOffset;
	[SerializeField]
	private float		moveSpeed;

	private float	moveTime;
	private float	moveProgress;
	private Vector3 startPos;
	private Vector3 saveTargetPos;

	private Vector3 TargetPosition => targetPos + targetOffset;

	[Header("回転アニメーション")]
	[SerializeField]
	private Transform spriteRoot;
	[SerializeField]
	private SpriteRenderer cap;
	[SerializeField]
	private AnimationCurve rotateCurve;
	[SerializeField]
	private float rotateSpeed;
	[SerializeField]
	private float capJumpHeight;

	private float rotateInterval;
	private float currentAngle;
	private bool isRotate;               //	回転中
	private int rotateCount;

	//	プロパティ
	public Vector3	TargetPos 
	{ 
		get { return TargetPosition; } 
		set 
		{ 
			targetPos = value;
			//	向きを更新
			if (TargetPosition.x - transform.position.x > 0)
				currentDir = Direction.RIGHT;
			else if (TargetPosition.x - transform.position.x < 0)
				currentDir = Direction.LEFT;
		} 
	}


	public Vector3	TargetOffset { get { return targetOffset; } set { targetOffset = value; } }
	public bool		IsGoal => currentDir == Direction.NONE;

	//	実行前初期化処理
	private void Awake()
	{
		
	}

	//	初期化処理
	private void Start()
	{
		
	}

	//	更新処理
	private void Update()
	{
		MoveUpdate();
		RotateUpdate();

		//	キャップに向きを適応
		if (currentDir == Direction.LEFT)
			cap.flipX = true;
		else if (currentDir == Direction.RIGHT)
			cap.flipX = false;
	}

	/*--------------------------------------------------------------------------------
	|| 移動処理
	--------------------------------------------------------------------------------*/
	private void MoveUpdate()
	{
		if (saveTargetPos != TargetPosition)
		{
			startPos = transform.position;
			moveProgress = 0.0f;
			moveTime = 0.0f;
		}
		saveTargetPos = TargetPosition;

		if (moveProgress >= 1.0f)
		{
			currentDir = Direction.NONE;
			return;
		}
		//	向きを取得
		if (TargetPosition.x - transform.position.x > 0)
			currentDir = Direction.RIGHT;
		else if (TargetPosition.x - transform.position.x < 0)
			currentDir = Direction.LEFT;

		moveTime += Time.deltaTime;
		float dist = Vector3.Distance(startPos, TargetPosition);
		float targetTime = dist / moveSpeed;
		moveProgress = moveTime / targetTime;

		transform.position = Vector3.Lerp(startPos, TargetPosition, moveProgress);
	}

	/*--------------------------------------------------------------------------------
	|| 回転処理
	--------------------------------------------------------------------------------*/
	private void RotateUpdate()
	{
		if (currentDir != Direction.NONE)
			isRotate = true;

		if (!isRotate)
			return;

		rotateInterval += Time.deltaTime * rotateSpeed;
		if (rotateInterval >= 1.0f)
		{
			rotateInterval = 0.0f;

			isRotate = false;

			rotateCount++;
			if (rotateCount >= 4)
				rotateCount = 0;
		}

		float curvePos = Ease(rotateInterval);
		float rot = (Mathf.PI / 2) * curvePos;

		currentAngle = rot + rotateCount * Mathf.PI / 2;

		//	回転を適応
		spriteRenderer.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -currentAngle * Mathf.Rad2Deg * (int)currentDir));

		//	進行度からキャップを動かす
		float capY = (-Mathf.Cos(rotateInterval * Mathf.PI * 2) + 1.0f) / 2;
		cap.transform.localPosition = Vector2.up + Vector2.up * capY * capJumpHeight;
	}
	private float Ease(float x)
	{
		return x * x;
	}

	/*--------------------------------------------------------------------------------
	|| 帽子の切り替え
	--------------------------------------------------------------------------------*/
	public void SetCapEnable(bool value)
	{
		cap.gameObject.SetActive(value);
	}

	/*--------------------------------------------------------------------------------
	|| 強制的にTargetPosに移動させる処理
	--------------------------------------------------------------------------------*/
	public void SetPosToTarget()
	{
		transform.position = TargetPos;
	}
}
