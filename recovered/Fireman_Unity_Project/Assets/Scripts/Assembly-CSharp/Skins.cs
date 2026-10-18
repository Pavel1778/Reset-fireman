using Spine;

public class Skins
{
	public abstract class Skin
	{
		public enum SkinIndex
		{
			Skin_Helmet = 0,
			Skin_Face = 1,
			Skin_Eyes = 2,
			Skin_Arm = 3,
			Skin_Jacket = 4,
			Skin_Blaster = 5,
			Skin_Hand = 6,
			Skin_Left_Leg = 7,
			Skin_Right_Leg = 8,
			Skin_Left_Ankle = 9,
			Skin_Right_Ankle = 10,
			Skin_Left_Foot = 11,
			Skin_Right_Foot = 12,
			Skin_Jetpack = 13
		}

		private static readonly string[] SKIN_PART = new string[14]
		{
			"Fireman_Helmet", "Fireman_Face", "Fireman_Eyes", "Fireman_Arm", "Fireman_Jacket", "Fireman_Blaster", "Fireman_Hand", "Fireman_Left_Leg", "Fireman_Right_Leg", "Fireman_Left_Ankle",
			"Fireman_Right_Ankle", "Fireman_Left_Foot", "Fireman_Right_Foot", "Fireman_Jetpack"
		};

		protected abstract string[] GetParts();

		public void SetHead(Skeleton skel)
		{
			string[] parts = GetParts();
			for (int i = 0; i <= 2; i++)
			{
				skel.SetAttachment(SKIN_PART[i], parts[i]);
			}
		}

		public void SetBody(Skeleton skel)
		{
			string[] parts = GetParts();
			for (int i = 3; i <= 12; i++)
			{
				skel.SetAttachment(SKIN_PART[i], parts[i]);
			}
		}

		public void SetJetPack(Skeleton skel, int index)
		{
			skel.SetAttachment(SKIN_PART[13], "Fireman_Jetpack" + index);
		}
	}

	private class SkinFireman : Skin
	{
		private static readonly string[] SKIN_NAME = new string[13]
		{
			"Fireman_Helmet0", "Fireman_Face0", "Fireman_Eyes0", "Fireman_Arm0", "Fireman_Jacket0", "Fireman_Blaster0", "Fireman_Hand0", "Fireman_Left_Leg0", "Fireman_Right_Leg0", "Fireman_Left_Ankle0",
			"Fireman_Right_Ankle0", "Fireman_Left_Foot0", "Fireman_Right_Foot0"
		};

		protected override string[] GetParts()
		{
			return SKIN_NAME;
		}
	}

	private class SkinDoc : Skin
	{
		private static readonly string[] SKIN_NAME = new string[13]
		{
			"blank", "Fireman_Face1", "Fireman_Eyes1", "Fireman_Arm1", "Fireman_Jacket1", "Fireman_Blaster1", "Fireman_Hand1", "Fireman_Left_Leg1", "Fireman_Right_Leg1", "Fireman_Left_Ankle1",
			"Fireman_Right_Ankle1", "Fireman_Left_Foot1", "Fireman_Right_Foot1"
		};

		protected override string[] GetParts()
		{
			return SKIN_NAME;
		}
	}

	private class SkinGirl : Skin
	{
		private static readonly string[] SKIN_NAME = new string[13]
		{
			"blank", "Fireman_Face2", "Fireman_Eyes2", "Fireman_Arm2", "Fireman_Jacket2", "Fireman_Blaster2", "Fireman_Hand2", "Fireman_Left_Leg2", "Fireman_Right_Leg2", "Fireman_Left_Ankle2",
			"Fireman_Right_Ankle2", "Fireman_Left_Foot2", "Fireman_Right_Foot2"
		};

		protected override string[] GetParts()
		{
			return SKIN_NAME;
		}
	}

	private class SkinEsquimau : Skin
	{
		private static readonly string[] SKIN_NAME = new string[13]
		{
			"blank", "Esquimau_Face", "Esquimau_Eyes", "Esquimau_Arm", "Esquimau_Body", "Esquimau_Blaster", "Esquimau_Hand", "Esquimau_Left_Leg", "Esquimau_Right_Leg", "Esquimau_Left_Ankle",
			"Esquimau_Right_Ankle", "Esquimau_Left_Foot", "Esquimau_Right_Foot"
		};

		protected override string[] GetParts()
		{
			return SKIN_NAME;
		}
	}

	private class SkinYeti : Skin
	{
		private static readonly string[] SKIN_NAME = new string[13]
		{
			"blank", "Yeti_Face", "Yeti_Eyes", "Yeti_Arm", "Yeti_Body", "Yeti_Blaster", "Yeti_Hand", "Yeti_Left_Leg", "Yeti_Right_Leg", "blank",
			"blank", "Yeti_Left_Foot", "Yeti_Right_Foot"
		};

		protected override string[] GetParts()
		{
			return SKIN_NAME;
		}
	}

	private class SkinPereNoel : Skin
	{
		private static readonly string[] SKIN_NAME = new string[13]
		{
			"blank", "PereNoel_Face", "PereNoel_Eyes", "PereNoel_Arm", "PereNoel_Body", "PereNoel_Blaster", "PereNoel_Hand", "PereNoel_Left_Leg", "PereNoel_Right_Leg", "PereNoel_Left_Ankle",
			"PereNoel_Right_Ankle", "PereNoel_Left_Foot", "PereNoel_Right_Foot"
		};

		protected override string[] GetParts()
		{
			return SKIN_NAME;
		}
	}

	private class SkinTyrolienne : Skin
	{
		private static readonly string[] SKIN_NAME = new string[13]
		{
			"blank", "Tyrolienne_Face", "Tyrolienne_Eyes", "Tyrolienne_Arm", "Tyrolienne_Body", "Tyrolienne_Blaster", "Tyrolienne_Hand", "Tyrolienne_Left_Leg", "Tyrolienne_Right_Leg", "blank",
			"blank", "blank", "blank"
		};

		protected override string[] GetParts()
		{
			return SKIN_NAME;
		}
	}

	public static readonly Skin[] All = new Skin[7]
	{
		new SkinFireman(),
		new SkinDoc(),
		new SkinGirl(),
		new SkinEsquimau(),
		new SkinYeti(),
		new SkinPereNoel(),
		new SkinTyrolienne()
	};
}
