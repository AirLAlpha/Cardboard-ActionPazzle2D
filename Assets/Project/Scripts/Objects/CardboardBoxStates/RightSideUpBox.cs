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
		private Color		safeLabelColor;	//	安全時のラベルの色
		[SerializeField]
		private Color		attentionColor; //	危険時の色

		[SerializeField]
		private float		breakableTime;	//	傾いて壊れるまでの時間

		MaterialPropertyBlock propertyBlock;	//	マテリアルプロパティブロック

		static readonly float[] ANGLES = { 0.0f, Mathf.PI * 0.5f, Mathf.PI, Mathf.PI * 1.5f, Mathf.PI * 2};
		private float	startRot;       //	最初の角度（90度刻み）

		private float angleProgress;
		private float breakableTimer;

		public RightSideUpBox(RightSideUpBox rightSideUpBox, CardboardBox parent) : 
			base(parent)
		{
			this.stateColor = rightSideUpBox.stateColor;
			this.breakAngle = rightSideUpBox.breakAngle;
			this.safeColor = rightSideUpBox.safeColor;
			this.safeLabelColor = rightSideUpBox.safeLabelColor;
			this.attentionColor = rightSideUpBox.attentionColor;
			this.material = rightSideUpBox.material;
			this.breakableTime = rightSideUpBox.breakableTime;

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
			Parent.LabelSprites[0].material = material;
			Parent.LabelSprites[1].material = material;

			//	角度を取得(90度刻みに補正）
			float a = (int)Parent.transform.localEulerAngles.z / 90;
			float b = Parent.transform.localEulerAngles.z % 90.0f;
			if (b > 45.0f)
				a += 1.0f;
			startRot = a * 90.0f;
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
			if (Parent.IsBurned)
				return;
			
			//	現在の角度を保持
			float angle = Parent.transform.localEulerAngles.z;
			//	開始時の角度との差を求める
			float deltaAngle = Mathf.Abs(Mathf.DeltaAngle(angle, startRot));

			//	差が破壊される角度以上になったら破壊
			if(deltaAngle > breakAngle)
			{
				if (breakableTimer >= breakableTime)
				{
					Parent.SoundPlayer.PlaySound(5);
					Parent.Burn();
				}
				else
				{
					breakableTimer += Time.deltaTime;
				}
			}
			else
			{
				breakableTimer = 0.0f;
			}

			//	角度の割合を保持
			this.angleProgress = deltaAngle / breakAngle;
		}

		/*--------------------------------------------------------------------------------
		|| マテリアルの色の変更処理
		--------------------------------------------------------------------------------*/
		private void MaterialUpdate()
		{
			//	プロパティの取得
			Parent.SpriteRenderer.GetPropertyBlock(propertyBlock);

			Color newColor = HSVLerp(safeColor, attentionColor, angleProgress);
			propertyBlock.SetColor("_InsideColor", newColor);

			//	座標の設定
			propertyBlock.SetVector("_RootPosition", Parent.transform.position);

			Parent.SpriteRenderer.SetPropertyBlock(propertyBlock);

			Color labelColor = HSVLerp(safeLabelColor, attentionColor, angleProgress);
			for (int i = 0; i < Parent.LabelSprites.Length; i++)
			{
				Parent.LabelSprites[i].GetPropertyBlock(propertyBlock);
				propertyBlock.SetColor("_OutsideColor", Color.white);
				propertyBlock.SetColor("_InsideColor", labelColor);
				propertyBlock.SetVector("_RootPosition", Parent.transform.position);
				Parent.LabelSprites[i].SetPropertyBlock(propertyBlock);
			}
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

		Color HSVLerp(Color a, Color b, float t)
		{
			//	各色のHSV値を取得
			Color.RGBToHSV(a, out float aH, out float aS, out float aV);
			Color.RGBToHSV(b, out float bH, out float bS, out float bV);

			//	HSVの値を補間する
			float h = Mathf.Lerp(aH, bH, t);
			float s = Mathf.Lerp(aS, bS, t);
			float v = Mathf.Lerp(aV, bV, t);
			//	色の設定
			return Color.HSVToRGB(h, s, v);
		}
	}
}