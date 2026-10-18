namespace LSlife.Inventory;

public abstract class ItemObject
{
	public string name;

	public bool stackable;

	public string description;

	public ItemObject(string _name, bool _stack, string _desc)
	{
		Innit(_name, _stack, _desc);
	}

	private void Innit(string _name, bool _stack, string _desc)
	{
		name = _name;
		stackable = _stack;
		description = _desc;
	}
}
