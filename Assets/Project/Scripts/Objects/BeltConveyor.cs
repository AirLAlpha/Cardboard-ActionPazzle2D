/**********************************************
 * 
 *  BeltConveyor.cs 
 *  ベルトコンベアに関する処理を記述
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/04
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeltConveyor : MonoBehaviour
{

	[SerializeField]
	private Vector2 velocity;           //	与える移動量
	[SerializeField]
	private bool	isActive;			//	稼働フラグ
	public bool		IsActive	{ set { isActive = value; } get { return isActive; } }

	[SerializeField]
	private Animator	anim;
	[SerializeField]
	private float		animationSpeed;

	//	更新処理
	private void Update()
	{
		if (anim == null)
			return;

		float speed = 0.0f;
		if (isActive)
			speed = animationSpeed;

		anim.SetFloat("AnimationSpeed", speed);
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (!isActive)
			return;

		if (collision.transform.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
		{
			rb.velocity = velocity * Time.deltaTime;
			rb.angularVelocity = 0.0f;
		}
	}
}
