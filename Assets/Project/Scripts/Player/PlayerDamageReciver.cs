/**********************************************
 * 
 *  PlayerDamageReciver.cs 
 *  �v���C���[�̃_���[�W���󂯂鏈�����L�q
 * 
 *  ����ҁF���� ���m
 *  ������F2023/04/05
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class PlayerDamageReciver : MonoBehaviour, IBurnable
{
    //  �R���|�[�l���g
    [Header("�R���|�[�l���g")]
    [SerializeField]
    private SpriteRenderer[]            spriteRenders;              //  SpriteRender
    [SerializeField]
    private Animator                    anim;

    private BlazingShaderController[]   blazingControllers;          //  BlazingShaderController
    private Rigidbody2D                 rb;

    private PlayerMove                  playerMove;
    private PlayerBoxManager            playerBoxManager;

    private CardboardBox.CardboardBox playerGrabBox;              //  �v���C���[�������Ă���n�R

    //  ����
    private bool                isDead;                 //  ���S�t���O

    [SerializeField]
    private bool                dontDeath;               //  ���G�t���O
    public bool                 DontDeath { get { return dontDeath; } set { dontDeath = value; } } 

    //  �C�x���g
    public UnityEvent           OnDead;                 //  ���S������
    private bool                executedEvent;          //  �C�x���g�̎��s�ς݃t���O

    //  �}�e���A��
    [Header("�}�e���A��")]
    [SerializeField]
    private Material            blazingMat;             //  ����
    [SerializeField]
    private LayerMask           mask;


	//  ���s�O����������
	private void Awake()
	{
        //  �R���|�[�l���g���擾
        rb                  = GetComponent<Rigidbody2D>();
        playerMove          = GetComponent<PlayerMove>();
        playerBoxManager    = GetComponent<PlayerBoxManager>();

        //  �eSprite����R���|�[�l���g���擾����
        blazingControllers = new BlazingShaderController[spriteRenders.Length];
		for (int i = 0; i < blazingControllers.Length; i++)
		{
            blazingControllers[i] = spriteRenders[i].GetComponent<BlazingShaderController>();
		}

        //  �ϐ��̏�����
        isDead = false;
	}

    //  �X�V����
    private void Update()
    {
        //  ���S���ăC�x���g�����s����Ă��Ȃ��Ƃ�
        if (isDead && !executedEvent)
        {
            //  �C�x���g�����s
            OnDead?.Invoke();

            //  �C�x���g�����s�ς݂ɂ���
            executedEvent = true;
        }

        //	�����ɏՓ˂�����ׂ���Ă���Ɣ��肷��
        var hit = Physics2D.OverlapBox(transform.position, Vector2.one * 0.1f, 0.0f, mask);
        if (hit != null)
        {
            Burn();
        }
    }

	//  �I��������
	private void OnDisable()
	{
        OnDead.RemoveAllListeners();
	}

	/*--------------------------------------------------------------------------------
    || ���㎞����
    --------------------------------------------------------------------------------*/
	public void Burn()
	{
        //  ���G�̍ۂ͏������Ȃ�
        if (DontDeath)
            return;

        //  ���łɎ���ł���Ƃ��͏������Ȃ�
        if (isDead)
            return;

        playerMove.DisableInput = true;

        //  �}�e���A���̕ύX
        foreach (var sr in spriteRenders)
		{
            sr.material = blazingMat;
		}
        playerGrabBox = playerBoxManager.CurrentBox;

        //  ���S�t���O�̗L����
        isDead = true;
        //  �A�j���[�V�����̒�~
        anim.enabled = false;
        //  �������Z�𖳌�������
        rb.simulated = false;
    }

	/*--------------------------------------------------------------------------------
	|| ����̃A�j���[�V�����̊J�n
	--------------------------------------------------------------------------------*/
    public void StartBurnAnimation()
	{
        //  �A�j���[�V����������
        for (int i = 0; i < blazingControllers.Length; i++)
        {
            blazingControllers[i].IsBurning = true;
        }
        if (playerGrabBox != null)
            playerGrabBox.Burn();
    }

}
