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
	//[SerializeField]
	//private bool	invert;


	private void OnTriggerStay2D(Collider2D collision)
	{
		if (collision.transform.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
		{
			rb.velocity = velocity * Time.deltaTime;
			rb.angularVelocity = 0.0f;
		}
	}
}
