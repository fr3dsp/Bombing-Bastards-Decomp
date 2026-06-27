using UnityEngine;

[ExecuteInEditMode]
public class FillText : MonoBehaviour
{
	public string text = "Hello World";

	private TextMesh mMesh;

	private GUIText mText;

	private void Awake()
	{
		mMesh = GetComponent<TextMesh>();
		mText = GetComponent<GUIText>();
		if (Application.isPlaying)
		{
			Update();
			Object.Destroy(this);
		}
	}

	private void Update()
	{
		if (mMesh != null && mMesh.text != text)
		{
			mMesh.text = text;
		}
		if (mText != null && mText.text != text)
		{
			mText.text = text;
		}
	}
}
