/**********************************************
 * 
 *  TitleStageBox.cs 
 *  �X�e�[�W�Z���N�g�̊e�X�e�[�W�̃{�b�N�X�̏������L�q
 * 
 *  ����ҁF���� ���m
 *  ������F2023/04/04
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleStageBox : MonoBehaviour
{
	private bool enableTaskSelect;

	//	�R���|�[�l���g
	[Header("�R���|�[�l���g")]
	[SerializeField]
	private Animator		boxAnim;
	[SerializeField]
	private TitleTasks		tasks;

	//	�n�R�̊J��
	[Header("�J��")]
	[SerializeField]
	private bool			isOpen;			//	�J�t���O
	[SerializeField]
	private float			openSpeed;		//	�n�R�̊J���x
	
	private float			openProgress;

	[Header("�w�i���[�g")]
	[SerializeField]
	private Transform		bgRoot;                         //	�w�i���[�g
	[SerializeField]
	private SpriteAlphaController bgRootAlphaController;	//	�w�i�̓����x
	[SerializeField]
	private float			bgRootAnimSpeed;				//	�w�i�A�j���[�V�����̑��x
	[SerializeField]
	private Vector2			bgRootMoveRange;				//	�w�i�̏㉺�ɗh��镝
	[SerializeField]
	private Vector2			bgRootEnableScale;				//	�w�i���\�������Ƃ��̃X�P�[��
	[SerializeField]
	private Vector2			bgRootOffset;                   //	�w�i���㉺�ɗh���ۂ̂̃I�t�Z�b�g
	[SerializeField]
	private Vector2			bgRootTaskSelectOffset;			//	�^�X�N�I�����̔w�i�̃I�t�Z�b�g

	private float			bgRootSine;						//	�w�i�̏㉺�ɗh���sin�g

	[Header("�w�i")]
	[SerializeField]
	private SpriteRenderer	taskBackground;
	[SerializeField]
	private	SpriteRenderer	bgArrow;
	[SerializeField]
	private float			bgZoomSpeed;
	[SerializeField]
	private Vector2			bgIdleSize;
	[SerializeField]
	private Vector2			bgZoomSize;
	[SerializeField]
	private Vector2			bgIdleOffset;
	[SerializeField]
	private Vector2			bgZoomOffset;
	[SerializeField]
	private Vector2			bgArrowIdleOffset;
	[SerializeField]
	private Vector2			bgArrowZoomOffset;

	[Header("����")]
	[SerializeField]
	private SpriteRenderer stageText;
	[SerializeField]
	private Vector2			textIdleOffset;
	[SerializeField]
	private Vector2			textZoomOffset;
	[SerializeField]
	private Vector2			textIdleScale;
	[SerializeField]
	private Vector2			textZoomScale;


	//	�v���p�e�B
	public bool IsOpen { get { return isOpen; } set { isOpen = value; } }
	public int	SelectedTaskIndex => tasks.SelectedTaskIndex;

	//	�X�V����
	private void Update()
	{
		BoxOpenUpdate();
		BackgroundAnimationUpdate();

		//	�N���A�󋵂̃A�j���[�V�����p�����[�^��ύX
		//stageClearStatusAnim.SetBool("Enable", isOpen);

	}

	/*--------------------------------------------------------------------------------
	|| �n�R�̊J����
	--------------------------------------------------------------------------------*/
	private void BoxOpenUpdate()
	{
		int progressDir = isOpen ? 1 : -1;
		openProgress += Time.deltaTime * openSpeed * progressDir;
		openProgress = Mathf.Clamp01(openProgress);

		boxAnim.SetFloat("OpenProgress", EasingFunctions.EaseInOutCubic(openProgress));
	}

	/*--------------------------------------------------------------------------------
	|| �w�i�̃A�j���[�V��������
	--------------------------------------------------------------------------------*/
	private void BackgroundAnimationUpdate()
	{
		//	�X�P�[��
		Vector2 targetScale = isOpen ? bgRootEnableScale : Vector2.zero;
		bgRoot.localScale = Vector2.Lerp(bgRoot.localScale, targetScale, Time.deltaTime * bgZoomSpeed);
		//	���W
		Vector2 targetPos = Vector2.zero;
		if (enableTaskSelect)
		{
			targetPos = bgRootTaskSelectOffset;
		}
		else if(isOpen)
		{
			bgRootSine += Time.deltaTime * bgRootAnimSpeed;
			bgRootSine = Mathf.Repeat(bgRootSine, Mathf.PI * 2);
			float sin = (Mathf.Sin(bgRootSine) + 1.0f) / 2.0f;
			Vector2 pos = Vector2.Lerp(-bgRootMoveRange, bgRootMoveRange, sin);
			targetPos = pos + bgRootOffset;
		}
		bgRoot.localPosition = Vector2.Lerp(bgRoot.localPosition, targetPos, Time.deltaTime * bgRootAnimSpeed);
		//	�����x
		bgRootAlphaController.TargetAlpha = isOpen ? 1.0f : 0.0f;

		//	SpriteRenderer�̃T�C�Y
		Vector2 targetSize = enableTaskSelect ? bgZoomSize : bgIdleSize;
		taskBackground.size = Vector2.Lerp(taskBackground.size, targetSize, Time.deltaTime * bgZoomSpeed);
		//	Sprite�̃I�t�Z�b�g
		Vector2 targetOffset = enableTaskSelect ? bgZoomOffset : bgIdleOffset;
		taskBackground.transform.localPosition = Vector2.Lerp(taskBackground.transform.localPosition, targetOffset, Time.deltaTime * bgZoomSpeed);
		//	���̍��W
		Vector2 targetArrowPos = enableTaskSelect ? bgArrowZoomOffset : bgArrowIdleOffset;
		bgArrow.transform.localPosition = Vector2.Lerp(bgArrow.transform.localPosition, targetArrowPos, Time.deltaTime * bgZoomSpeed);

		//	����
		//	���W
		Vector2 textTargetPos = enableTaskSelect ? textZoomOffset : textIdleOffset;
		stageText.transform.localPosition = Vector2.Lerp(stageText.transform.localPosition, textTargetPos, Time.deltaTime * bgZoomSpeed);
		//	�X�P�[��
		Vector2 textTargetScale = enableTaskSelect ? textZoomScale : textIdleScale;
		stageText.transform.localScale = Vector2.Lerp(stageText.transform.localScale, textTargetScale, Time.deltaTime * bgZoomSpeed);
	}

	/*--------------------------------------------------------------------------------
	|| �X�e�[�W�̑I������
	--------------------------------------------------------------------------------*/
	public void SelectBox()
	{
		tasks.StartTaskSelect();
		enableTaskSelect = true;
	}

	/*--------------------------------------------------------------------------------
	|| �L�����Z������
	--------------------------------------------------------------------------------*/
	public void CancelBox()
	{
		enableTaskSelect = false;
		tasks.EndTaskSelect();
	}

	/*--------------------------------------------------------------------------------
	|| �Z�[�u�f�[�^�̐ݒ菈��
	--------------------------------------------------------------------------------*/
	public void SetStageScore(StageScore score)
	{
		tasks.SetStageScore(score);
	}

}
