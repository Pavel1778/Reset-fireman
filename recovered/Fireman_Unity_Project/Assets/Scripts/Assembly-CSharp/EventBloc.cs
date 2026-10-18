using UnityEngine;

public class EventBloc : Decor
{
	private const int TRIGGER_NONE = 0;

	private const int TRIGGER_SWITCH_ON = 1;

	private const int TRIGGER_SWITCH_OFF = 2;

	private const int TRIGGER_TOGGLE = 3;

	public int eventID;

	private int target;

	private bool hit;

	public void Start()
	{
		sprite.atlas = GameManager.getAtlas(-1);
		sprite.spriteName = "comp";
		sprite.depth = 500;
	}

	public void Update()
	{
		if (GameManager.Instance.Status != GameManager.RUNNING)
		{
			return;
		}
		Transform transform = GameManager.mainCamera.transform;
		if (Mathf.Abs(base.transform.position.x - transform.position.x) > (float)GameManager.tailleZoneX || Mathf.Abs(base.transform.position.y - transform.position.y) > (float)GameManager.tailleZoneY)
		{
			sprite.enabled = false;
			return;
		}
		sprite.enabled = true;
		Pompier player = GameManager.Instance.Player1;
		Vector3 localPosition = base.transform.localPosition;
		Vector3 localPosition2 = player.transform.localPosition;
		if (Mathf.Abs(localPosition.x - localPosition2.x) < 50f && Mathf.Abs(localPosition.y - localPosition2.y) < 50f)
		{
			if (!hit)
			{
				hit = true;
				OnEnterTrigger(player.gameObject);
			}
		}
		else if (hit)
		{
			hit = false;
			OnLeaveTrigger(player.gameObject);
		}
		if (hit)
		{
			GameManager.Instance.mTalky.GetComponent<Talky>().showTalky(base.transform.position, eventID);
		}
	}

	public void OnEnterTrigger(GameObject tg)
	{
	}

	public void OnLeaveTrigger(GameObject tg)
	{
	}
}
