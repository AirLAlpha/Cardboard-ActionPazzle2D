/**********************************************
 * 
 *  PauseChallangeState.cs 
 *  ポーズメニューのチャレンジのクリア状況を表示するUI
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/05/14
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class PauseChallangeState : MonoBehaviour
{
	//	項目ひとつ分の構造体
	[System.Serializable]
	private struct ChallangeStateItem
	{
		public RectTransform	root;
		public TextMeshProUGUI	aboutText;
		public Slider			progressSlider;
		public RectTransform	faildOverlay;
	}

	[Header("コンポーネント")]
	[SerializeField]
	private ChallangeManager challangeManager;

	[Header("項目")]
	[SerializeField]
	private ChallangeStateItem[] stateItems;    //	項目のUI
	[SerializeField]
	private string[] typeString;

	//	TODO : UIの個数を5つにし、それぞれに設定を行うようにする

	/*--------------------------------------------------------------------------------
	|| チャレンジのクリア状況を設定
	--------------------------------------------------------------------------------*/
	public void UpdateChallangeState()
	{
		int count = challangeManager.ChallangeData.Length;
		for (int i = 0; i < stateItems.Length; i++)
		{
			//	アクティブの切り替え
			bool active = count > i ? true : false;
			stateItems[i].root.gameObject.SetActive(active);

			if (!active)
				continue;

			//	文字列の設定
			string text = typeString[(int)challangeManager.ChallangeData[i].type];
			text = text.Replace("(value)", challangeManager.Challanges[i].GetChallangeValue().ToString());
			stateItems[i].aboutText.text = text;
			//	進行度を取得
			stateItems[i].progressSlider.value = challangeManager.Challanges[i].GetChallangeProgress();

			//	失敗になったらオーバーレイを有効化
			stateItems[i].faildOverlay.gameObject.SetActive(challangeManager.Challanges[i].IsFaild);
		}
	}

}
