/**********************************************
 * 
 *  IPoseable.cs 
 *  ポーズ可能オブジェクトにつけるインターフェース
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/21
 * 
 **********************************************/
using UnityEngine;

public interface IPoseable
{
	public void Pose();			//	ポーズ処理
	public void Resume();		//	再開処理
}