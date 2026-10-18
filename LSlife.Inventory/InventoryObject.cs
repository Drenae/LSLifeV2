using System.Collections.Generic;
using System.Linq;

namespace LSlife.Inventory;

internal class InventoryObject
{
	private List<ItemSlot> Inventory = new List<ItemSlot>();

	public bool AddItem(ItemObject _item, int _amount)
	{
		bool returnValue = false;
		if (_item.stackable && Inventory.Count > 0)
		{
			foreach (ItemSlot slot in Inventory)
			{
				if (slot.Item.GetType() == _item.GetType())
				{
					slot.ItemAmount += _amount;
					returnValue = true;
				}
			}
		}
		else
		{
			returnValue = true;
		}
		return returnValue;
	}

	public bool RemoveItem(ItemObject _item, int _amount)
	{
		bool returnValue = false;
		if (Inventory.Count > 0)
		{
			foreach (ItemSlot slot in Inventory.ToList())
			{
				if (slot.Item.name == _item.name)
				{
					if (slot.ItemAmount > _amount)
					{
						slot.ItemAmount -= _amount;
						returnValue = true;
						break;
					}
					if (slot.ItemAmount == _amount)
					{
						Inventory.Remove(slot);
						returnValue = true;
						break;
					}
				}
			}
		}
		return returnValue;
	}
}
