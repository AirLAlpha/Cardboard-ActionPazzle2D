using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using UnityEngine;

namespace CardboardBox
{
	[System.Serializable]
	public class RightSideUpBox : CardboardBoxState
	{
		[SerializeField]
		private float		breakAngle;     //	壊れていしまう角度
		[SerializeField]
		private Material	material;       //	マテリアル

		[SerializeField]
		private Color		safeColor;      //	安全なときの色
		[SerializeField]
		private Color		attentionColor;	//	危険時の色

		MaterialPropertyBlock propertyBlock;	//	マテリアルプロパティブロック

		static readonly float[] ANGLES = { 0.0f, Mathf.PI * 0.5f, Mathf.PI, Mathf.PI * 1.5f, Mathf.PI * 2};
		private float	startRot;       //	最初の角度（90度刻み）

		private float angleProgress;

		public RightSideUpBox(RightSideUpBox rightSideUpBox, CardboardBox parent) : 
			base(parent)
		{
			this.stateColor = rightSideUpBox.stateColor;
			this.breakAngle = rightSideUpBox.breakAngle;
			this.safeColor = rightSideUpBox.safeColor;
			this.attentionColor = rightSideUpBox.attentionColor;
			this.material = rightSideUpBox.material;

			propertyBlock = new MaterialPropertyBlock();
		}

		public override void StateUpdate()
		{
			AngleUpdate();
			MaterialUpdate();
		}

		public override void OnEnterState()
		{
			//	状態の色を変更する
			Parent.SpriteRenderer.color = stateColor;
			Parent.SpriteRenderer.material = material;

			//	角度を取得
			float angle = ConvertToRadian(Parent.transform.localEulerAngles.z);

			//	ANGLESの配列から一番近い値を検索
			float saveDiff = Mathf.PI;
			int index = -1;
			for (int i = 0; i < ANGLES.Length; i++)
			{
				float diff = Mathf.Abs(ANGLES[i] - angle);
				if (saveDiff > diff)
				{
					saveDiff = diff;
					index = i;
				}
			}

			//	インデックスが4(2pi)のときは0にする
			if (index == 4)
				index = 0;

			//	最初の角度を保持しておく
			startRot = ANGLES[index];
		}

		public override void OnExitState()
		{
		}

		public override void OnCollisionEnter(Collision2D collision)
		{
		}

		/*--------------------------------------------------------------------------------
		|| 角度による破壊の更新処理
		--------------------------------------------------------------------------------*/
		private void AngleUpdate()
		{
			float angle = Parent.transform.localEulerAngles.z;                      //	現在の角度を取得
			float minus = Mathf.Repeat(startRot - Mathf.PI / 2, Mathf.PI * 2);      //	マイナス側の最低角度を取得
			float plus = Mathf.Repeat(startRot + Mathf.PI / 2, Mathf.PI * 2);       //	プラス側の最大角度を取得

			//	現在の角度との差分を取得
			float minusDelta = Mathf.DeltaAngle(angle, minus * Mathf.Rad2Deg);
			float plusDelta = Mathf.DeltaAngle(angle, plus * Mathf.Rad2Deg);

			//	差が一定未満になったときに破壊する
			if (Mathf.Abs(minusDelta) < breakAngle ||
				Mathf.Abs(plusDelta) < breakAngle)
				Parent.Burn();

			//	角度から割合を計算
			float a = Mathf.InverseLerp(90.0f, breakAngle, Mathf.Abs(plusDelta));
			float b = Mathf.InverseLerp(90.0f, breakAngle, Mathf.Abs(minusDelta));
			//	割合をbreakAngleに向けた進行度として保持
			this.angleProgress= a + b;
		}

		/*--------------------------------------------------------------------------------
		|| マテリアルの色の変更処理
		--------------------------------------------------------------------------------*/
		private void MaterialUpdate()
		{
			//	プロパティの取得
			Parent.SpriteRenderer.GetPropertyBlock(propertyBlock);

			//	各色のHSV値を取得
			Color.RGBToHSV(safeColor, out float safeH, out float safeS, out float safeV);
			Color.RGBToHSV(attentionColor, out float attentionH, out float attentionS, out float attentionV);

			//	HSVの値を補間する
			float h = Mathf.Lerp(safeH, attentionH, angleProgress);
			float s = Mathf.Lerp(safeS, attentionS, angleProgress);
			float v = Mathf.Lerp(safeV, attentionV, angleProgress);
			//	色の設定
			Color newColor = Color.HSVToRGB(h, s, v);
			propertyBlock.SetColor("_InsideColor", newColor);

			//	座標の設定
			propertyBlock.SetVector("_RootPosition", Parent.transform.position);

			Parent.SpriteRenderer.SetPropertyBlock(propertyBlock);
		}

		float ConvertToRadian(float dig)
		{
			float angle = dig * Mathf.Deg2Rad;
			if (angle < 0)
			{
				angle += Mathf.PI * 2;
			}

			return angle;
		}
	}
}