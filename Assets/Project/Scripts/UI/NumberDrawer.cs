using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberDrawer : MonoBehaviour
{
	[SerializeField]
	private NumberSpriteDatabase numberDB;

	[Header("数値")]
	[SerializeField]
	private int		number;     //	数字
	[SerializeField]
	private Color	color;		//	色
	[SerializeField]
	private Image[]	numberImages;	//	数値ひとつを描画するImage（要素数が桁数となる）

	//	プロパティ
	public int Number	{ get { return number; } set { number = value; UpdateNumber(); } }
	public Color Color	{ get { return color; } set { color = value; UpdateNumber(); } }

	/*--------------------------------------------------------------------------------
	|| 画像の更新
	--------------------------------------------------------------------------------*/
	private void UpdateNumber()
	{
		string num = number.ToString("D" + numberImages.Length.ToString());

		for (int i = 0; i < num.Length; i++)
		{
			numberImages[i].sprite = numberDB.NumberSprites[num[i] - '0'];
			numberImages[i].color = color;
		}
	}


}
