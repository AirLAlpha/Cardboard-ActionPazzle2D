using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleBackgroundManager : MonoBehaviour
{
	[Header("コンポーネント")]
	[SerializeField]
	private StageSelect				stageSelect;
	[SerializeField]
	private SpriteRenderer[]		renderers;
	[SerializeField]
	private SpriteRenderer			logo;

	[Header("シェーダー")]
	[SerializeField]
	private float					waveOffsetX;

	[Header("選択")]
	[SerializeField]
	private int						selectedStage;
	[SerializeField]
	private Color					nonSelectedColor;
	[SerializeField]
	private Color					selectedColor;
	[SerializeField]
	private float					colorChangeRate;

	//	プロパティ
	public int SelectedStage { get { return selectedStage; } set { selectedStage = value; } }


	private MaterialPropertyBlock			propertyBlock;

	//	実行前初期化処理
	private void Awake()
	{
	}

	//	初期化処理
	private void Start()
	{
		propertyBlock = new MaterialPropertyBlock();

		//	オフセットの適応
		for (int i = 0; i < renderers.Length; i++)
		{
			//	プロパティを取得
			renderers[i].GetPropertyBlock(propertyBlock);
			//	時間のオブセットを乱数で設定
			propertyBlock.SetFloat("_TimeOffset", Random.Range(0, 10000.0f));

			if (i != 0)
				propertyBlock.SetFloat("_WaveOffsetX", waveOffsetX);

			//	プロパティを設定
			renderers[i].SetPropertyBlock(propertyBlock);
		}

		//	色の変更
		for (int i = 0; i < renderers.Length; i++)
		{
			Color targetColor = stageSelect.SelectedStage == i ? selectedColor : nonSelectedColor;

			if (stageSelect.SelectedStage == 0 && i == 0)
			{
				targetColor = Color.white;
				renderers[renderers.Length - 1].color = targetColor;
			}
			else
			{
				logo.color = targetColor;
			}

			renderers[i].color = targetColor;
		}

	}

	//	更新処理
	private void Update()
	{
		for (int i = 1; i < renderers.Length; i++)
		{
			//	プロパティを取得
			renderers[i].GetPropertyBlock(propertyBlock);

			propertyBlock.SetFloat("_WaveOffsetX", waveOffsetX);

			//	プロパティを設定
			renderers[i].SetPropertyBlock(propertyBlock);
		}

		SelectUpdate();

	}

	/*--------------------------------------------------------------------------------
	|| 選択
	--------------------------------------------------------------------------------*/
	private void SelectUpdate()
	{
		for (int i = 0; i < renderers.Length; i++)
		{
			Color targetColor = stageSelect.SelectedStage == i ? selectedColor : nonSelectedColor;

			if(stageSelect.SelectedStage == 0 && i == 0)
			{
				targetColor = Color.white;
				renderers[renderers.Length - 1].color = Color.Lerp(renderers[renderers.Length - 1].color, targetColor, Time.deltaTime * colorChangeRate);
			}

			renderers[i].color = Color.Lerp(renderers[i].color, targetColor, Time.deltaTime * colorChangeRate);
		}

		//	ロゴ
		Color logoTargetColor = stageSelect.SelectedStage == 0 ? Color.white : nonSelectedColor;
		logo.color = Color.Lerp(logo.color, logoTargetColor, Time.deltaTime * colorChangeRate);

	}

}
