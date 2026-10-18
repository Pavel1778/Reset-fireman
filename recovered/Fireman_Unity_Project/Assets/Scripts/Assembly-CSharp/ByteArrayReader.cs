public class ByteArrayReader
{
	private byte[] _data;

	private int _pos;

	public ByteArrayReader(byte[] b)
	{
		if (b != null)
		{
			_data = b;
			_pos = 0;
		}
		else
		{
			_data = null;
			_pos = 0;
		}
	}

	public byte ReadByte()
	{
		byte result = _data[_pos];
		_pos++;
		return result;
	}

	public short ReadInt16()
	{
		int num = _data[_pos] | (_data[_pos + 1] << 8);
		_pos += 2;
		return (short)num;
	}

	public int ReadInt32()
	{
		int result = _data[_pos] | (_data[_pos + 1] << 8) | (_data[_pos + 2] << 16) | (_data[_pos + 3] << 24);
		_pos += 4;
		return result;
	}

	public int Read_Flip(int f)
	{
		if (check_Flip(f))
		{
			return ReadByte();
		}
		return 0;
	}

	public int Read_Angle(int f)
	{
		if (check_Angle(f))
		{
			return ReadInt16();
		}
		return 0;
	}

	public int Read_OfsX(int f)
	{
		if (check_OfsX(f))
		{
			return ReadInt16();
		}
		return 0;
	}

	public int Read_OfsY(int f)
	{
		if (check_OfsY(f))
		{
			return ReadInt16();
		}
		return 0;
	}

	public int Read_Tag(int f)
	{
		if (check_Tag(f))
		{
			return ReadInt16();
		}
		return 0;
	}

	private static bool check_Enabled(int f)
	{
		return (f & 1) == 1;
	}

	private static bool check_IsLoop(int f)
	{
		return (f & 2) == 2;
	}

	private static bool check_Angle(int f)
	{
		return (f & 1) == 1;
	}

	private static bool check_Flip(int f)
	{
		return (f & 2) == 2;
	}

	private static bool check_OfsX(int f)
	{
		return (f & 4) == 4;
	}

	private static bool check_OfsY(int f)
	{
		return (f & 8) == 8;
	}

	private static bool check_Mode(int f)
	{
		return ((f / 16) & 7 & VarsManager.GameMode) != 0;
	}

	private static bool check_Tag(int f)
	{
		return (f & 0x80) == 128;
	}
}
