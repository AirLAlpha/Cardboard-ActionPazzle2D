/**********************************************
 * 
 *  Enemy.cs 
 *  敵の基底クラス
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/02
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class Enemy : MonoBehaviour
{
	//	コンポーネント
	protected Rigidbody2D rb;			//	Rigidbody2D

	//	移動
	protected Vector2	acce;			//	加速度
	protected Direction	currentDir;		//	現在向いている方向


}
