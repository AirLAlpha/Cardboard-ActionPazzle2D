using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxThroughBlockGimmick : Gimmick
{
	[SerializeField]
	private Animator	anim;

	private int			anim_close;	//	Closeのハッシュ値


	[Header("スプライト")]
	[SerializeField]
	private SpriteRenderer spriteRenderer;
	[SerializeField]
	private Sprite		openSprite;
	[SerializeField]
	private Sprite		closeSprite;

	[SerializeField]
	private Vector2		checkArea;
	[SerializeField]
	private Vector2		checkOffset;
	[SerializeField]
	private LayerMask	checkMask;

	private Vector2 CheckCenter => (Vector2)transform.position + checkOffset;
	private bool	isClose;

	private void Awake()
	{
		anim_close = Animator.StringToHash("Close");
	}

	public override string GetExtraSetting()
	{
		return string.Empty;
	}

	public override void SetExtraSetting(string json)
	{
	}

	private void Update()
	{
		var hitResult = Physics2D.OverlapBox(CheckCenter, checkArea, 0.0f, checkMask);

		if(hitResult != null)
		{
			isClose = true;
		}
		else
		{
			isClose = false;
		}

		spriteRenderer.sprite = isClose ? closeSprite : openSprite;
		anim.SetBool(anim_close, isClose);
	}

#if UNITY_EDITOR
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(1, 0, 0, 0.5f);

		Gizmos.DrawCube(CheckCenter, checkArea);
	}
#endif
}
