using TNet;
using UnityEngine;

[RequireComponent(typeof(TNObject))]
public class ColoredObject : MonoBehaviour
{
	[RFC]
	private void OnColor(Color c)
	{
		base.GetComponent<Renderer>().material.color = c;
	}

	private void OnClick()
	{
		Color color = Color.red;
		if (base.GetComponent<Renderer>().material.color == Color.red)
		{
			color = Color.green;
		}
		else if (base.GetComponent<Renderer>().material.color == Color.green)
		{
			color = Color.blue;
		}
		TNObject component = GetComponent<TNObject>();
		component.Send("OnColor", Target.AllSaved, color);
	}
}
