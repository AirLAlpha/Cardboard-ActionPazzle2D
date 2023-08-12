using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
	[SerializeField]
	private AudioMixer	mixer;
	[SerializeField]
	private string		targetName;

	[SerializeField]
	private Sprite selectedSprite;
	[SerializeField]
	private Sprite nonselectedSprite;

	[SerializeField]
	private SettingMenu			parentMenu;		//	親のメニュー
	[SerializeField]
	private Image[]				items;          //	項目
	[SerializeField]
	private RectTransform		cursor;         //	カーソル
	[SerializeField]
	private float				cursorSpeed;    //	カーソル速度
	[SerializeField]
	private Vector2				cursorOffset;

	public UnityEvent			OnValidate;

	private int		selectedItemIndex;
	private int		saveIndex;

	public int SelectedIndex { get { return selectedItemIndex; } }

	public void InputUpdate(Vector2 input, bool inputCancel)
	{
		selectedItemIndex += (int)input.x;
		selectedItemIndex = Mathf.Clamp(selectedItemIndex, 0, items.Length - 1);

		float vol = (float)selectedItemIndex / (float)(items.Length - 1);
		mixer.SetFloat(targetName, Mathf.Lerp(-10.0f, 10.0f, vol));

		if (inputCancel)
			parentMenu.ReturnSlider();

		if(saveIndex != selectedItemIndex)
		{
			OnValidate.Invoke();
			saveIndex = selectedItemIndex;
		}
	}

	public void CursorUpdate()
	{
		Vector3 targetPos = items[selectedItemIndex].rectTransform.position + (Vector3)cursorOffset;
		cursor.position = Vector3.Lerp(cursor.position, targetPos, Time.deltaTime * cursorSpeed);

		for (int i = 0; i < items.Length; i++)
		{
			bool selected = i == selectedItemIndex;

			items[i].sprite = selected ? selectedSprite : nonselectedSprite;
			items[i].transform.localEulerAngles = selected ? Vector3.forward * 5.0f : Vector3.zero;
		}
	}

	public void SetIndex(int index)
	{
		selectedItemIndex = index;
		saveIndex = index;

		//	音量に適応
		float vol = (float)selectedItemIndex / (float)(items.Length - 1);
		mixer.SetFloat(targetName, Mathf.Lerp(-10.0f, 10.0f, vol));
		//	見た目に適応
		for (int i = 0; i < items.Length; i++)
		{
			bool selected = i == selectedItemIndex;

			items[i].sprite = selected ? selectedSprite : nonselectedSprite;
			items[i].transform.localEulerAngles = selected ? Vector3.forward * 5.0f : Vector3.zero;
		}
	}

}
