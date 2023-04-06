/**********************************************
 * 
 *  BlazingShaderController.cs 
 *  炎上シェーダーのコントール
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/05
 * 
 **********************************************/
using UnityEngine;

public class BlazingShaderController : MonoBehaviour
{
    //  コンポーネント
    [SerializeField]
    private SpriteRenderer spriteRenderer;  //  SpriteRender

    //	炎上
    [SerializeField]
    private float burnSpeed;                //	炎上し切る速度
    [SerializeField, Range(0.0f, 1.0f)]
    private float burnProgress;             //	燃える進行度

    private bool    isBurning;              //	炎上フラグ
    public bool     IsBurning { get { return isBurning; } set { isBurning = value; } }

    private MaterialPropertyBlock propertyBlock;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

		propertyBlock = new MaterialPropertyBlock();
    }

    // Update is called once per frame
    void Update()
    {
        if (isBurning)
        {
            BurnUpdate();
            return;
        }
    }

    /*--------------------------------------------------------------------------------
    || 炎上の更新処理
    --------------------------------------------------------------------------------*/
    private void BurnUpdate()
    {
        burnProgress = Mathf.Clamp01(burnProgress + Time.deltaTime * burnSpeed);

        spriteRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetFloat("_Progress", burnProgress);
        spriteRenderer.SetPropertyBlock(propertyBlock);

        if (burnProgress >= 1.0f)
            Destroy(gameObject);
    }
}
