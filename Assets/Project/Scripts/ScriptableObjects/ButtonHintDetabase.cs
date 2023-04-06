/**********************************************
 * 
 *  ButtonHintDataBase.cs 
 *  UIで表示するボタンのデータベース
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/06
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//	ボタンのイメージを1つ分のデータ
[System.Serializable]
public struct ButtonContainor
{
	public string	buttonName;				//	ボタンの名前
	public string[] displayNames;			//	UIへの表示名
	public Sprite	positiveKeySprite;		//	キーボードのスプライト（正）
	public Sprite	negativeKeySprite;		//	キーボードのスプライト（負）
	public Sprite	controllerSprite;		//	コントローラのスプライト
}


[CreateAssetMenu(fileName = "UIButtonDataBase", menuName = "Create ButtonHintDataBase")]
public class ButtonHintDetabase : ScriptableObject
{
	[SerializeField]
	private List<ButtonContainor> buttons;		//	ボタンの配列

	//	プロパティ
	public List<ButtonContainor> Buttons { get { return buttons; } }

	//	ボタン名でインデックスを検索する処理
	public int FindIndex(string buttonName)
	{
		return buttons.FindIndex(a => a.buttonName == buttonName);
	}
}
