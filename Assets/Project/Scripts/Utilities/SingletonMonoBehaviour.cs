/**********************************************
 * 
 *  SingletonMonoBehaviour.cs 
 *  MonoBehaviourを継承したシングルトンクラスの
 *  テンプレートクラス
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/06
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
	//	インスタンス
	private static T instance;
	//	プロパティ
	public static T Instance
	{
		get
		{
			//	インスタンスが設定されていないとき
			if(instance == null)
			{
				//	アタッチされたオブジェクトを検索する
				instance = (T)FindObjectOfType(typeof(T));
				//	アタッチしたオブジェクトが見つからないとき
				if (instance == null)
					Debug.LogError(typeof(T) + "のインスタンスは存在しません。");
			}

			//	インスタンスを返す
			return instance;
		}
	}

	//	実行前初期化処理
	private void Awake() 
	{
		CheckInstance();
	}

	/*--------------------------------------------------------------------------------
	|| インスタンスの存在を確認する
	--------------------------------------------------------------------------------*/
	protected void CheckInstance()
	{
		//	インスタンスが設定されていないとき
		if(instance == null)
		{
			//	自身をインスタンスとして登録
			instance = this as T;
			return;
		}
		//	自身がインスタンスとして設定されているとき
		else if(instance == this)
		{
			return;
		}

		//	他のインスタンスが存在しているとき
		Destroy(this);
	}

}
