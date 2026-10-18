public interface ISwitchable
{
	bool IsON { get; }

	bool IsOFF { get; }

	int Ident { get; }

	bool Toggle();

	bool SwitchON();

	bool SwitchOFF();
}
