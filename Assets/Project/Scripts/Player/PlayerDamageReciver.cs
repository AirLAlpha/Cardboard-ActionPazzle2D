/**********************************************
 * 
 *  PlayerDamageReciver.cs 
 *  プレイヤーのダメージを受ける処理を記述
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/05
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageReciver : MonoBehaviour, IBurnable
{
    //  コンポーネント
    [Header("コンポーネント")]
    [SerializeField]
    private SpriteRenderer[]            spriteRenders;              //  SpriteRender

    private BlazingShaderController[]   blazingControllers;          //  BlazingShaderController

    //  生存
    private bool                isDead;                 //  死亡フラグ

    //  マテリアル
    [Header("マテリアル")]
    [SerializeField]
    private Material            blazingMat;             //  炎上

	//  実行前初期化処理
	private void Awake()
	{
        blazingControllers = new BlazingShaderController[spriteRenders.Length];
		for (int i = 0; i < blazingControllers.Length; i++)
		{
            blazingControllers[i] = spriteRenders[i].GetComponent<BlazingShaderController>();
		}

	}

    //  更新処理
	private void Update()
    {
        
    }

	/*--------------------------------------------------------------------------------
    || 炎上時処理
    --------------------------------------------------------------------------------*/
    public void Burn()
	{
        //  すでに死んでいるときは処理しない
        if (isDead)
            return;

        //  マテリアルの変更
        foreach (var sr in spriteRenders)
		{
            sr.material = blazingMat;
		}

        //  アニメーションさせる
        for (int i = 0; i < blazingControllers.Length; i++)
        {
            blazingControllers[i].IsBurning = true;
        }

        isDead = true;
    }

}
