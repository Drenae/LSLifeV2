namespace LSlife.Inventory;

internal class ItemSlot
{
	private bool empty = true;

	private ItemObject item;

	private int itemAmount;

	public bool Empty
	{
		get
		{
			return empty;
		}
		set
		{
			empty = value;
		}
	}

	public ItemObject Item
	{
		get
		{
			return item;
		}
		set
		{
			item = value;
		}
	}

	public int ItemAmount
	{
		get
		{
			return itemAmount;
		}
		set
		{
			itemAmount = value;
		}
	}

	public ItemSlot(ItemObject _item, int _amount)
	{
		item = _item;
		itemAmount = _amount;
		empty = false;
	}
}
