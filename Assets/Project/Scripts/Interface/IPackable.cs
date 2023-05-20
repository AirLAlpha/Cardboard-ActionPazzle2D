/**********************************************
 * 
 *  IPackable.cs 
 *  梱包可能なオブジェクトに付与するインターフェース
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/17
 * 
 **********************************************/
using UnityEngine;

public interface IPackable
{
	public Sprite LabelSprite { get; }
	public abstract CardboardType Packing();		//	梱包時の処理を記述
}
