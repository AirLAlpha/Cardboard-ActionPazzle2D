/**********************************************
 * 
 *  BlazingShaderController.cs 
 *  ����V�F�[�_�[�̃R���g�[��
 * 
 *  ����ҁF���� ���m
 *  ������F2023/04/05
 * 
 **********************************************/
using UnityEngine;

public class BlazingShaderController : MonoBehaviour
{
    //  �R���|�[�l���g
    [SerializeField]
    private SpriteRenderer spriteRenderer;  //  SpriteRender

    //	����
    [SerializeField]
    private float burnSpeed;                //	���サ�؂鑬�x
    [SerializeField, Range(0.0f, 1.0f)]
    private float burnProgress;             //	�R����i�s�x

    private bool    isBurning;              //	����t���O
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
    || ����̍X�V����
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
