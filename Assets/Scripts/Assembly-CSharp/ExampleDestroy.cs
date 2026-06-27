using UnityEngine;

public class ExampleDestroy : MonoBehaviour
{
	private float mDestroyTime;

	private void Awake()
	{
		mDestroyTime = Time.time + 5f;
	}

	private void Update()
	{
		if (mDestroyTime < Time.time)
		{
			TNManager.Destroy(base.gameObject);
		}
	}

	private void OnClick()
	{
		TNManager.Destroy(base.gameObject);
	}
}
