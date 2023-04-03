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

public class DoorGimmick : MonoBehaviour
{
	//	開閉
	[Header("開閉")]
	[SerializeField]
	private Transform		doorRoot;
	[SerializeField]
	private Vector2			openOffset;
	[SerializeField]
	private float			openSpeed;

	[SerializeField]
	private bool			isOpen;
	public bool				IsOpen { set { isOpen = value; } get { return isOpen; } }

	//	更新処理
	private void Update()
	{
		if(isOpen)
		{
			doorRoot.localPosition = Vector3.Lerp(doorRoot.localPosition, openOffset, Time.deltaTime * openSpeed);
		}
		else
		{
			doorRoot.localPosition = Vector3.Lerp(doorRoot.localPosition, Vector3.zero, Time.deltaTime * openSpeed);
		}
	}
}
