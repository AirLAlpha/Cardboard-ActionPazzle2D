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
using UnityEngine.Events;

public class PlayerDamageReciver : MonoBehaviour, IBurnable
{
    //  コンポーネント
    [Header("コンポーネント")]
    [SerializeField]
    private SpriteRenderer[]            spriteRenders;              //  SpriteRender
    [SerializeField]
    private Animator                    anim;

    private BlazingShaderController[]   blazingControllers;          //  BlazingShaderController
    private Rigidbody2D                 rb;

    private PlayerMove                  playerMove;
    private PlayerBoxManager            playerBoxManager;

    private CardboardBox                playerGrabBox;              //  プレイヤーが持っているハコ

    //  生存
    private bool                isDead;                 //  死亡フラグ

    public bool                 DontDeth { get; set; }  //  無敵フラグ

    //  イベント
    public UnityEvent           OnDead;                 //  死亡時処理
    private bool                executedEvent;          //  イベントの実行済みフラグ

    //  マテリアル
    [Header("マテリアル")]
    [SerializeField]
    private Material            blazingMat;             //  炎上

	//  実行前初期化処理
	private void Awake()
	{
        //  コンポーネントを取得
        rb                  = GetComponent<Rigidbody2D>();
        playerMove          = GetComponent<PlayerMove>();
        playerBoxManager    = GetComponent<PlayerBoxManager>();

        //  各Spriteからコンポーネントを取得する
        blazingControllers = new BlazingShaderController[spriteRenders.Length];
		for (int i = 0; i < blazingControllers.Length; i++)
		{
            blazingControllers[i] = spriteRenders[i].GetComponent<BlazingShaderController>();
		}

        //  変数の初期化
        isDead = false;
	}

    //  更新処理
	private void Update()
    {
        //  死亡してイベントが実行されていないとき
        if(isDead && !executedEvent)
		{
            //  イベントを実行
            OnDead?.Invoke();

            //  イベントを実行済みにする
            executedEvent = true;
		}
    }

	//  終了時処理
	private void OnDisable()
	{
        OnDead.RemoveAllListeners();
	}

	/*--------------------------------------------------------------------------------
    || 炎上時処理
    --------------------------------------------------------------------------------*/
	public void Burn()
	{
        //  無敵の際は処理しない
        if (DontDeth)
            return;

        //  すでに死んでいるときは処理しない
        if (isDead)
            return;

        playerMove.DisableInput = true;

        //  マテリアルの変更
        foreach (var sr in spriteRenders)
		{
            sr.material = blazingMat;
		}
        playerGrabBox = playerBoxManager.CurrentBox;

        //  死亡フラグの有効化
        isDead = true;
        //  アニメーションの停止
        anim.enabled = false;
        //  物理演算を無効化する
        rb.simulated = false;
    }

	/*--------------------------------------------------------------------------------
	|| 炎上のアニメーションの開始
	--------------------------------------------------------------------------------*/
    public void StartBurnAnimation()
	{
        //  アニメーションさせる
        for (int i = 0; i < blazingControllers.Length; i++)
        {
            blazingControllers[i].IsBurning = true;
        }
        playerGrabBox?.Burn();
    }

}
