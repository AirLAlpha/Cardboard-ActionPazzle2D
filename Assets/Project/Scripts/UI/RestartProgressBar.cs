using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class RestartProgressBar : MonoBehaviour
{
	private CanvasGroup group;

	[SerializeField]
	private Slider progressSlider;
	[SerializeField]
	private float alphaSpeed;

	private float alpha;

	//	実行前初期化処理
	private void Awake()
	{
		group = GetComponent<CanvasGroup>();	
	}

	//	初期化処理
	private void Start()
	{
		
	}

	//	更新処理
	private void Update()
	{
		float progress = StageManager.Instance.RestartProgress;

		if(progress > 0.0f)
		{
			alpha += Time.deltaTime * alphaSpeed;
		}
		else
		{
			alpha -= Time.deltaTime * alphaSpeed;
		}
		alpha = Mathf.Clamp01(alpha);
		group.alpha = alpha;

		progressSlider.value = Ease(progress);
		
	}

	private float Ease(float x)
	{
		return -(Mathf.Cos(Mathf.PI * x) - 1.0f) / 2.0f;
	}
}
