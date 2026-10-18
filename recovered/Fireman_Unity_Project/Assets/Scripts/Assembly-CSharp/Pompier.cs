using UnityEngine;

public class Pompier : MonoBehaviour
{
	public const float SP_GOUTTE = 33.333332f;

	public const float FORCE_FORWARD = 2f;

	public const float FORCE_JETPACK = 0.1f;

	public const float DEFAULT_ANGLE = 0.2f;

	public Vector2 shotDir = default(Vector2);

	public FootCollider footCollider;

	public GameObject mAnim2;

	public float upDownMoveX;

	public int playerIndex;

	public BoxCollider mainCollider;

	private SkeletonAnimation mAnim;

	private float timeGoutteJet;

	private bool activeJetPack;

	private float reserveWater;

	private bool useTheWater;

	private bool useTheJet;

	private Vector3 lastBorne;

	private float timeBlink;

	private float timeEffetBlink;

	private bool dead;

	private float timeBlinkWater;

	private Vector3 posTampon;

	private bool canTp;

	private Vector3 lastPosTp;

	private float padMoveX;

	private float regardCam;

	private float timeDead;

	private float timeCanReuseJetpack;

	private int mJetPackLevel;

	private static readonly float[] WATER_REUSE = new float[4] { 0.2f, 0.21f, 0.25f, 0.35f };

	private static readonly float[] WATER_REFILL = new float[4]
	{
		1f / 90f,
		1f / 60f,
		1f / 45f,
		1f / 30f
	};

	private static readonly float[] WATER_POWER_GUN = new float[4]
	{
		0.15396824f,
		0.12777771f,
		19f / 180f,
		0.1f
	};

	private static readonly float[] WATER_POWER_JET = new float[4]
	{
		0.15396824f,
		0.12777771f,
		19f / 180f,
		0.1f
	};

	public static float lastGoutte;

	public static float nextShoot = 0f;

	public static float timeShoot = 0.03f;

	public static float rndShoot = 0.1f;

	public static float ofsShootX = 35f;

	public static float ofsShootY = 1f;

	public void setPompier(Vector3 pos)
	{
		mainCollider = GetComponent<BoxCollider>();
		base.transform.parent = GameManager.Instance.panelGame.transform;
		base.transform.localScale = Vector3.one;
		base.transform.localPosition = pos;
		shotDir.x = 1f;
		shotDir.y = 0.2f;
		reserveWater = 1f;
		mAnim = mAnim2.GetComponent<SkeletonAnimation>();
		mAnim.skeletonDataAsset = SpineData.Instance.dataFireman;
		mAnim.skeletonDataAsset.scale = 0.6f;
		mAnim.Initialize();
		mAnim.state.SetAnimation("Idle", true);
		mAnim.GetComponent<Renderer>().enabled = true;
		lastPosTp = base.transform.localPosition;
		lastBorne = base.transform.localPosition;
		if (VarsManager.Instance != null)
		{
			mJetPackLevel = VarsManager.Instance.getObjetShopStep("upgrade_jetpack");
		}
		int skinHead = VarsManager.Instance.getSkinHead();
		int skinJacket = VarsManager.Instance.getSkinJacket();
		if (playerIndex == 0)
		{
			VarsManager.Instance.commonPompierSkinUpdater(mAnim2.GetComponent<SkeletonAnimation>(), skinHead, skinJacket);
		}
		else
		{
			VarsManager.Instance.commonPompierSkinUpdater(mAnim2.GetComponent<SkeletonAnimation>(), 1, 1);
		}
	}

	private void Update()
	{
		bool flag = false;
		useTheJet = false;
		if (GameManager.Instance.Status != GameManager.RUNNING)
		{
			GetComponent<Rigidbody>().velocity = new Vector3(0f, GetComponent<Rigidbody>().velocity.y, GetComponent<Rigidbody>().velocity.z);
			return;
		}
		if (shotDir.y < 0.2f)
		{
			shotDir.y += Time.deltaTime;
			if (shotDir.y > 0.2f)
			{
				shotDir.y = 0.2f;
			}
		}
		if (shotDir.y > 0.2f)
		{
			shotDir.y -= Time.deltaTime;
			if (shotDir.y < 0.2f)
			{
				shotDir.y = 0.2f;
			}
		}
		if (Mathf.Abs(base.transform.localPosition.x - lastPosTp.x) > 50f || Mathf.Abs(base.transform.localPosition.y - lastPosTp.y) > 50f)
		{
			canTp = true;
		}
		if (upDownMoveX != 0f)
		{
			base.transform.localPosition = new Vector3(base.transform.localPosition.x + upDownMoveX * Time.deltaTime, base.transform.localPosition.y, base.transform.localPosition.z);
		}
		if (footCollider.isTouch)
		{
			GetComponent<Rigidbody>().velocity = new Vector3(0f, GetComponent<Rigidbody>().velocity.y, GetComponent<Rigidbody>().velocity.z);
		}
		useTheWater = false;
		if (!dead)
		{
			GameManager.Instance.uiLeft.alpha = 0.7f;
			GameManager.Instance.uiRight.alpha = 0.7f;
			GameManager.Instance.uiShoot.alpha = 0.7f;
			GameManager.Instance.uiJump.alpha = 0.7f;
			GameManager.Instance.uiPad.alpha = 1f;
			GameManager.Instance.uiPadBackground.alpha = 0.2f;
			if (playerIndex == 0)
			{
				if (Input.GetKey(KeyCode.LeftArrow))
				{
					GameManager.Instance.uiLeft.alpha = 1f;
					MoveLeft(1f);
				}
				if (Input.GetKey(KeyCode.RightArrow))
				{
					GameManager.Instance.uiRight.alpha = 1f;
					MoveRight(1f);
				}
				if (Input.GetKey(KeyCode.UpArrow))
				{
					shotDir.y += 3f * Time.deltaTime;
					if (shotDir.y > 1f)
					{
						shotDir.y = 1f;
					}
				}
				if (Input.GetKey(KeyCode.DownArrow))
				{
					shotDir.y -= 3f * Time.deltaTime;
					if (shotDir.y < 0f)
					{
						shotDir.y = 0f;
					}
				}
				if (Input.GetKey(KeyCode.W))
				{
					GameManager.Instance.uiJump.alpha = 1f;
					Jump();
				}
				if (Input.GetKey(KeyCode.X))
				{
					GameManager.Instance.uiShoot.alpha = 1f;
					flag = true;
				}
			}
			else if (playerIndex == 1)
			{
				if (Input.GetKey(KeyCode.Q))
				{
					GameManager.Instance.uiLeft.alpha = 1f;
					MoveLeft(1f);
				}
				if (Input.GetKey(KeyCode.D))
				{
					GameManager.Instance.uiRight.alpha = 1f;
					MoveRight(1f);
				}
				if (Input.GetKey(KeyCode.Z))
				{
					shotDir.y += 3f * Time.deltaTime;
					if (shotDir.y > 1f)
					{
						shotDir.y = 1f;
					}
				}
				if (Input.GetKey(KeyCode.S))
				{
					shotDir.y -= 3f * Time.deltaTime;
					if (shotDir.y < 0f)
					{
						shotDir.y = 0f;
					}
				}
				if (Input.GetKey(KeyCode.A))
				{
					GameManager.Instance.uiJump.alpha = 1f;
					Jump();
				}
				if (Input.GetKey(KeyCode.E))
				{
					GameManager.Instance.uiShoot.alpha = 1f;
					flag = true;
				}
			}
			int touchCount = Input.touchCount;
			float num = -1f;
			float num2 = -1f;
			float num3 = -1f;
			bool flag2 = false;
			for (int i = 0; i < touchCount; i++)
			{
				bool flag3 = false;
				Vector2 vector = Vector2.zero;
				Touch touch = Input.GetTouch(i);
				flag3 = touch.phase != TouchPhase.Canceled && touch.phase != TouchPhase.Ended;
				if (flag3)
				{
					vector = touch.position;
				}
				if (!flag3)
				{
					continue;
				}
				if (vector.y / (float)Screen.height < 0.5f && VarsManager.Instance.useArrows)
				{
					if (vector.x / (float)Screen.width < 0.16f)
					{
						GameManager.Instance.uiLeft.alpha = 1f;
						MoveLeft(1f);
					}
					if ((double)(vector.x / (float)Screen.width) > 0.16 && (double)(vector.x / (float)Screen.width) < 0.32)
					{
						GameManager.Instance.uiRight.alpha = 1f;
						MoveRight(1f);
					}
				}
				if (vector.y / (float)Screen.height < 0.9f && vector.x / (float)Screen.width < 0.5f && !VarsManager.Instance.useArrows)
				{
					flag2 = true;
					if (GameManager.Instance.padInitX == -1f)
					{
						GameManager.Instance.padInitX = vector.x / (float)Screen.width;
					}
					if (GameManager.Instance.padInitY == -1f)
					{
						GameManager.Instance.padInitY = vector.y / (float)Screen.height;
					}
					GameManager.Instance.padX = vector.x / (float)Screen.width;
					GameManager.Instance.padY = GameManager.Instance.padInitY;
					float num4 = Mathf.Sqrt(Mathf.Pow(GameManager.Instance.padX - GameManager.Instance.padInitX, 2f) + Mathf.Pow(GameManager.Instance.padY - GameManager.Instance.padInitY, 2f));
					float f = Mathf.Atan2(GameManager.Instance.padY - GameManager.Instance.padInitY, GameManager.Instance.padX - GameManager.Instance.padInitX);
					if (num4 > 0.04f)
					{
						GameManager.Instance.padX = GameManager.Instance.padInitX + Mathf.Cos(f) * 0.04f;
						GameManager.Instance.padY = GameManager.Instance.padInitY;
					}
					float num5 = (GameManager.Instance.padInitX - GameManager.Instance.padX) / 0.04f;
					if (num5 > 0f)
					{
						MoveLeft(num5);
					}
					if (num5 < 0f)
					{
						MoveRight(0f - num5);
					}
				}
				if (!(vector.y / (float)Screen.height < 0.9f))
				{
					continue;
				}
				float num6 = 0.01f;
				if ((Mathf.Abs(GameManager.Instance.uiJump.GetComponent<UIAnchor>().relativeOffset.x - GameManager.Instance.relPosBtnJumpX) < num6 && (double)(vector.x / (float)Screen.width) > 0.84) || (Mathf.Abs(GameManager.Instance.uiJump.GetComponent<UIAnchor>().relativeOffset.x - GameManager.Instance.relPosBtnJumpX) > num6 && (double)(vector.x / (float)Screen.width) > 0.6 && vector.x / (float)Screen.width < 1f))
				{
					GameManager.Instance.uiJump.alpha = 1f;
					Jump();
					if (Mathf.Abs(GameManager.Instance.uiShoot.GetComponent<UIAnchor>().relativeOffset.x - GameManager.Instance.relPosBtnShootX) < num6)
					{
						num3 = vector.x / (float)Screen.width;
					}
				}
				if ((Mathf.Abs(GameManager.Instance.uiShoot.GetComponent<UIAnchor>().relativeOffset.x - GameManager.Instance.relPosBtnShootX) < num6 && (double)(vector.x / (float)Screen.width) > 0.6 && (double)(vector.x / (float)Screen.width) < 0.84) || ((Mathf.Abs(GameManager.Instance.uiShoot.GetComponent<UIAnchor>().relativeOffset.x - GameManager.Instance.relPosBtnShootX) > num6 || Mathf.Abs(GameManager.Instance.uiShoot.GetComponent<UIAnchor>().relativeOffset.y - GameManager.Instance.relPosBtnShootY) > num6) && (double)(vector.x / (float)Screen.width) > 0.6 && vector.x / (float)Screen.width < 1f && num3 == -1f))
				{
					GameManager.Instance.uiShoot.alpha = 1f;
					num = ((!(Mathf.Abs(GameManager.Instance.uiJump.GetComponent<UIAnchor>().relativeOffset.x - GameManager.Instance.relPosBtnJumpX) < num6)) ? GameManager.Instance.relPosBtnShootX : (vector.x / (float)Screen.width));
					num2 = vector.y / (float)Screen.height;
					float factor = Common.clipf(num2 * 2f, 0f, 1f);
					shotDir.y = Common.lerpLinear(0.1f, 1f, factor);
					flag = true;
				}
			}
			if (num3 > 1f)
			{
				num3 = 1f;
			}
			if (num3 < 0f && num3 != -1f)
			{
				num3 = 0f;
			}
			if (num > 1f)
			{
				num = 1f;
			}
			if (num < 0f && num != -1f)
			{
				num = 0f;
			}
			if (num2 > 1f)
			{
				num2 = 1f;
			}
			if (num2 < 0f && num2 != -1f)
			{
				num2 = 0f;
			}
			if (num != -1f || num2 != -1f)
			{
				GameManager.Instance.uiShoot.GetComponent<UIAnchor>().relativeOffset.x += (num - GameManager.Instance.uiShoot.GetComponent<UIAnchor>().relativeOffset.x) * 0.4f;
				GameManager.Instance.uiShoot.GetComponent<UIAnchor>().relativeOffset.y += (num2 - GameManager.Instance.uiShoot.GetComponent<UIAnchor>().relativeOffset.y) * 0.4f;
			}
			else
			{
				GameManager.Instance.uiShoot.GetComponent<UIAnchor>().relativeOffset.x += (GameManager.Instance.relPosBtnShootX - GameManager.Instance.uiShoot.GetComponent<UIAnchor>().relativeOffset.x) * 0.4f;
				GameManager.Instance.uiShoot.GetComponent<UIAnchor>().relativeOffset.y += (GameManager.Instance.relPosBtnShootY - GameManager.Instance.uiShoot.GetComponent<UIAnchor>().relativeOffset.y) * 0.4f;
			}
			if (num3 != -1f)
			{
				GameManager.Instance.uiJump.GetComponent<UIAnchor>().relativeOffset.x += (num3 - GameManager.Instance.uiJump.GetComponent<UIAnchor>().relativeOffset.x) * 0.4f;
			}
			else
			{
				GameManager.Instance.uiJump.GetComponent<UIAnchor>().relativeOffset.x += (GameManager.Instance.relPosBtnJumpX - GameManager.Instance.uiJump.GetComponent<UIAnchor>().relativeOffset.x) * 0.4f;
			}
			if (!flag2)
			{
				GameManager.Instance.padInitX = -1f;
				GameManager.Instance.padInitY = -1f;
			}
			if (useTheJet)
			{
				if (!mAnim2.GetComponent<SkeletonAnimation>().state.Animation.Name.Equals("Jetpack"))
				{
					mAnim2.GetComponent<SkeletonAnimation>().state.SetAnimation("Jetpack", true);
				}
			}
			else
			{
				Sounds.snd_fireman_jetpack_use.Stop();
				if (GetComponent<Rigidbody>().velocity.x != 0f && footCollider.isTouch)
				{
					if (!mAnim2.GetComponent<SkeletonAnimation>().state.Animation.Name.Equals("Run"))
					{
						mAnim2.GetComponent<SkeletonAnimation>().state.SetAnimation("Run", true);
					}
				}
				else if (GetComponent<Rigidbody>().velocity.y > 0f && !footCollider.isTouch)
				{
					if (!mAnim2.GetComponent<SkeletonAnimation>().state.Animation.Name.Equals("Jump"))
					{
						mAnim2.GetComponent<SkeletonAnimation>().state.SetAnimation("Jump", true);
					}
				}
				else if (GetComponent<Rigidbody>().velocity.y < 0f && !footCollider.isTouch)
				{
					if (!mAnim2.GetComponent<SkeletonAnimation>().state.Animation.Name.Equals("Land"))
					{
						mAnim2.GetComponent<SkeletonAnimation>().state.SetAnimation("Land", true);
					}
				}
				else if (!mAnim2.GetComponent<SkeletonAnimation>().state.Animation.Name.Equals("Idle"))
				{
					mAnim2.GetComponent<SkeletonAnimation>().state.SetAnimation("Idle", true);
				}
			}
			mAnim2.GetComponent<SkeletonAnimation>().skeleton.FindBone("Blaster").Data.Rotation = 0f + 18f * shotDir.y;
			mAnim2.GetComponent<SkeletonAnimation>().skeleton.UpdateWorldTransform();
			if (flag)
			{
				Pistolet();
			}
		}
		if (reserveWater < 1f && !flag)
		{
			reserveWater += WATER_REFILL[mJetPackLevel] * Time.deltaTime;
			if (reserveWater > 1f)
			{
				reserveWater = 1f;
			}
		}
		if (useTheWater)
		{
			float num7 = 0f;
			if (useTheJet)
			{
				num7 += WATER_POWER_JET[mJetPackLevel];
			}
			if (flag)
			{
				num7 += WATER_POWER_GUN[mJetPackLevel];
			}
			reserveWater -= num7 * Time.deltaTime;
			GameManager.Instance.litre += 2f * Time.deltaTime;
			if (!useTheJet && !Sounds.snd_fireman_shoot_loop.isPlaying)
			{
				Sounds.snd_fireman_shoot_loop.Play();
			}
		}
		else
		{
			Sounds.snd_fireman_shoot_loop.Stop();
		}
		if (timeCanReuseJetpack > 0f)
		{
			timeCanReuseJetpack -= Time.deltaTime;
		}
		if (reserveWater <= 0f)
		{
			reserveWater = 0f;
			timeCanReuseJetpack = WATER_REUSE[mJetPackLevel];
		}
		regardCam += (shotDir.x - regardCam) * 0.05f;
		if (playerIndex == 0)
		{
			Transform transform = GameManager.mainCamera.transform;
			transform.position = new Vector3(transform.position.x + (base.transform.position.x + 0.45f * regardCam - transform.position.x) * 0.1f, transform.position.y + (base.transform.position.y - transform.position.y) * 0.1f, transform.position.z);
		}
		posTampon = new Vector3(posTampon.x + (base.transform.position.x - posTampon.x) * 0.65f, posTampon.y + (base.transform.position.y + 0.035f - posTampon.y) * 0.65f - 0.07f, -2.5f);
		mAnim2.transform.position = posTampon;
		mAnim2.transform.localScale = new Vector3(shotDir.x, 1f, 1f);
		if (timeBlink > 0f)
		{
			timeBlink -= Time.deltaTime;
			if (timeBlink < 0f)
			{
				timeBlink = 0f;
			}
		}
		timeEffetBlink -= 10f * Time.deltaTime;
		while (timeEffetBlink < -1f)
		{
			timeEffetBlink = 1f;
		}
		if ((timeBlink > 0f && timeEffetBlink >= 0f) || dead)
		{
			mAnim2.GetComponent<Renderer>().enabled = false;
		}
		else
		{
			mAnim2.GetComponent<Renderer>().enabled = true;
		}
		timeBlinkWater -= 7f * Time.deltaTime;
		while (timeBlinkWater < -1f)
		{
			timeBlinkWater += 2f;
		}
		if (reserveWater < 0.35f && timeBlinkWater >= 0f)
		{
			GameManager.Instance.waterBarre.fillAmount = 0f;
		}
		else
		{
			GameManager.Instance.waterBarre.fillAmount = reserveWater * 0.76f + 0.12f;
		}
		if (timeDead >= 0f)
		{
			timeDead -= Time.deltaTime;
		}
		if (dead && timeDead < 0f)
		{
			GameManager.Instance.restart();
		}
	}

	private void MoveLeft(float ratio)
	{
		shotDir.x = -1f;
		if (footCollider.isTouch)
		{
			GetComponent<Rigidbody>().velocity = new Vector3(-1f * ratio, GetComponent<Rigidbody>().velocity.y, 0f);
		}
		else
		{
			GetComponent<Rigidbody>().velocity = new Vector3(-0.9f * ratio, GetComponent<Rigidbody>().velocity.y, 0f);
		}
	}

	private void MoveRight(float ratio)
	{
		shotDir.x = 1f;
		if (footCollider.isTouch)
		{
			GetComponent<Rigidbody>().velocity = new Vector3(1f * ratio, GetComponent<Rigidbody>().velocity.y, 0f);
		}
		else
		{
			GetComponent<Rigidbody>().velocity = new Vector3(0.9f * ratio, GetComponent<Rigidbody>().velocity.y, 0f);
		}
	}

	private void Jump()
	{
		float num = GetComponent<Rigidbody>().velocity.y;
		if (!footCollider.isTouch)
		{
			if (activeJetPack)
			{
				if (EffetJetpack())
				{
					num = 0.96f;
				}
			}
			else if (GetComponent<Rigidbody>().velocity.y <= 0f)
			{
				activeJetPack = true;
			}
		}
		else
		{
			if (num < 0.2f)
			{
				Sounds.snd_fireman_jump.Play();
			}
			num = 3f;
			activeJetPack = false;
		}
		GetComponent<Rigidbody>().velocity = new Vector3(GetComponent<Rigidbody>().velocity.x, num, 0f);
	}

	private bool EffetJetpack()
	{
		if (timeCanReuseJetpack <= 0f)
		{
			if (!Sounds.snd_fireman_jetpack_use.isPlaying)
			{
				Sounds.snd_fireman_jetpack_use.Play();
			}
			useTheWater = true;
			useTheJet = true;
			timeGoutteJet -= Time.deltaTime;
			if (timeGoutteJet <= 0f)
			{
				timeGoutteJet = 0.03f;
				float num = Random.value * 0.1f;
				GameManager.newGoutte(base.transform.localPosition.x - 20f * shotDir.x, base.transform.localPosition.y, 0f - num - shotDir.x * 0.1f, 0f, 0.7f);
				num = Random.value * 0.1f;
				GameManager.newGoutte(base.transform.localPosition.x - 26f * shotDir.x, base.transform.localPosition.y, 0f - num - shotDir.x * 0.1f, 0f, 0.7f);
			}
			return true;
		}
		if (!Sounds.snd_fireman_jetpack_end.isPlaying)
		{
			Sounds.snd_fireman_jetpack_end.Play();
		}
		return false;
	}

	public void finish()
	{
		mAnim2.GetComponent<Renderer>().enabled = false;
		Object.Instantiate(GameManager.Instance.prefabPoufAnim, new Vector3(base.transform.position.x, base.transform.position.y, -2f), Quaternion.identity);
	}

	public void laMort()
	{
		if (timeBlink == 0f && !dead)
		{
			Object.Instantiate(GameManager.Instance.prefabPoufAnim, new Vector3(base.transform.position.x, base.transform.position.y, -2f), Quaternion.identity);
			Sounds.snd_fireman_loss_life.Play();
			if (GameManager.Instance.life == 1)
			{
				Sounds.snd_fireman_dead.Play();
			}
			dead = true;
			timeDead = 1f;
			GetComponent<Rigidbody>().useGravity = false;
			GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, GetComponent<Rigidbody>().velocity.z);
		}
	}

	public void refill()
	{
		refill(7f * Time.deltaTime);
	}

	public void refill(float v)
	{
		if (reserveWater < 1f)
		{
			reserveWater += v;
		}
		if (reserveWater > 1f)
		{
			reserveWater = 1f;
		}
	}

	public bool setLastBorne(Vector2 v)
	{
		if (v.x != lastBorne.x || v.y != lastBorne.y)
		{
			lastBorne = v;
			return true;
		}
		return false;
	}

	public float getTimeBlink()
	{
		return timeBlink;
	}

	public void setTimeBlink(float t)
	{
		timeBlink = t;
	}

	public void returnToBorne()
	{
		base.transform.localPosition = lastBorne;
		timeBlink = 1.3f;
		dead = false;
		GetComponent<Rigidbody>().useGravity = true;
		reserveWater = 1f;
	}

	public bool canTpTruc()
	{
		if (footCollider.isTouch)
		{
			return true;
		}
		return false;
	}

	public void stopWalk()
	{
		mAnim2.GetComponent<SkeletonAnimation>().state.SetAnimation("Idle", true);
	}

	public void tpHere(int x, int y)
	{
		if (canTp)
		{
			GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
			base.transform.localPosition = new Vector3(LevelLoader.scaleX(x), LevelLoader.scaleY(y), base.transform.localPosition.z);
			lastPosTp = base.transform.localPosition;
			Sounds.snd_ig_teleport.Play();
			Object.Instantiate(GameManager.Instance.prefabMagicPoufAnim, new Vector3(base.transform.position.x, base.transform.position.y, -4f), Quaternion.identity);
			canTp = false;
		}
	}

	private void addGoutte(int loop)
	{
		float num = Random.value * rndShoot;
		Goutte goutte = GameManager.newGoutte(base.transform.localPosition.x + shotDir.x * ofsShootX, base.transform.localPosition.y + shotDir.y * ofsShootY, num + shotDir.x * 2f + GetComponent<Rigidbody>().velocity.x, num + shotDir.y * 2f, 1f);
		for (int i = 0; i < loop; i++)
		{
			goutte.Update();
		}
	}

	private void Pistolet()
	{
		if (timeCanReuseJetpack <= 0f)
		{
			useTheWater = true;
			float time = Time.time;
			if (nextShoot < time)
			{
				addGoutte(1);
				addGoutte(0);
				nextShoot = time + timeShoot;
			}
		}
		else if (!Sounds.snd_fireman_shoot_end.isPlaying)
		{
			Sounds.snd_fireman_shoot_end.Play();
		}
	}

	public bool IsFullWater()
	{
		return reserveWater >= 1f;
	}
}
