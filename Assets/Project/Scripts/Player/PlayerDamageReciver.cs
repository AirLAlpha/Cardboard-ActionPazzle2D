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

public class PlayerDamageReciver : MonoBehaviour, IBurnable
{
    //  �R���|�[�l���g
    [Header("�R���|�[�l���g")]
    [SerializeField]
    private SpriteRenderer[]            spriteRenders;              //  SpriteRender

    private BlazingShaderController[]   blazingControllers;          //  BlazingShaderController

    //  ����
    private bool                isDead;                 //  ���S�t���O

    //  �}�e���A��
    [Header("�}�e���A��")]
    [SerializeField]
    private Material            blazingMat;             //  ����

	//  ���s�O����������
	private void Awake()
	{
        blazingControllers = new BlazingShaderController[spriteRenders.Length];
		for (int i = 0; i < blazingControllers.Length; i++)
		{
            blazingControllers[i] = spriteRenders[i].GetComponent<BlazingShaderController>();
		}

	}

    //  �X�V����
	private void Update()
    {
        
    }

	/*--------------------------------------------------------------------------------
    || ���㎞����
    --------------------------------------------------------------------------------*/
    public void Burn()
	{
        //  ���łɎ���ł���Ƃ��͏������Ȃ�
        if (isDead)
            return;

        //  �}�e���A���̕ύX
        foreach (var sr in spriteRenders)
		{
            sr.material = blazingMat;
		}

        //  �A�j���[�V����������
        for (int i = 0; i < blazingControllers.Length; i++)
        {
            blazingControllers[i].IsBurning = true;
        }

        isDead = true;
    }

}
