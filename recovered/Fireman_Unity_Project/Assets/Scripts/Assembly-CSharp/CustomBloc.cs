using System.Collections.Generic;

public class CustomBloc : Decor
{
	public class Property
	{
		public int type;

		public string name;

		public object data;
	}

	public List<Property> properties;

	public CustomBloc()
	{
		properties = new List<Property>();
	}
}
