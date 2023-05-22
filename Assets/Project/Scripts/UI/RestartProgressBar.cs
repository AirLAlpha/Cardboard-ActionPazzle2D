using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
[RequireComponent(typeof(Animator))]
public class RestartProgressBar : MonoBehaviour
{
	private CanvasGroup group;
	private Animator	anim;

	[SerializeField]
	private CanvasAlphaController canvasAlphaController;
	[SerializeField]
	private float alphaSpeed;

	private float alpha;

	//	実行前初期化処理
	private void Awake()
	{
		group = GetComponent<CanvasGroup>();
		anim = GetComponent<Animator>();
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
		group.alpha = alpha * canvasAlphaController.TargetAlpha;

		anim.SetFloat("Progress", progress);
	}

	private float Ease(float x)
	{
		return -(Mathf.Cos(Mathf.PI * x) - 1.0f) / 2.0f;
	}
}
