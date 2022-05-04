using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPG_App.Combat.Characters
{
	public class Player : Character
	{

		public Inventory inventory = new Inventory();

		public Player() : base("player", "Player.spr", 1, 0, 200, 50, 50, 10, 10, 10, 10, 11)
		{
			
			Attacks.Add(new Attack("Firebolt", 50, Attack.DamageType.Fire, 80, 10));
			Attacks.Add(new Attack("Heal Opp", -100, Attack.DamageType.Ice, 100));
			Attacks.Add(new Attack("Heal", 30, Attack.DamageType.Heal, 100, 10));
			Attacks.Add(new Attack("Filler3", 0, Attack.DamageType.Physical, 100));
			Attacks.Add(new Attack("Filler4", 0, Attack.DamageType.Physical, 100));
			Attacks.Add(new Attack("Filler5", 0, Attack.DamageType.Physical, 100));
			Attacks.Add(new Attack("Filler6", 0, Attack.DamageType.Physical, 100));
			inventory.addItem(new Inventory.PotionHP(this));
			inventory.addItem(new Inventory.PotionHP(this));
			inventory.addItem(new Inventory.PotionHP(this));
			inventory.addItem(new Inventory.PotionHP(this));
			inventory.addItem(new Inventory.PotionHP(this));
			inventory.addItem(new Inventory.PotionMP(this));
			inventory.addItem(new Inventory.PotionMP(this));
			inventory.addItem(new Inventory.PotionMP(this));
			inventory.addItem(new Inventory.Item("Eat", true));
			inventory.addItem(new Inventory.Item("NoEat"));
			inventory.addItem(new Inventory.Item("Eat2", true));
			//inventory.addItem(new Inventory.Item("fItem5"));
		}

		public class Inventory
		{
			public class Item
			{
				protected string _itemName;
				protected bool _IsConsumable;
				public string itemName
				{
					get { return _itemName; }
				}
				public bool IsConsumable
				{
					get { return _IsConsumable; }
				}

				public Item(string itemName = "NameMissing", bool isConsumable = false)
				{
					this._itemName = itemName;
					this._IsConsumable = isConsumable;
				}
				virtual public string Use()
				{
					
					return "Item Unimplemented";
				}
			}
			public class PotionHP : Item
			{
				int healAmount = 50;
				Player player;
				public PotionHP(Player player)
				{
					_itemName = "HP Pot";
					_IsConsumable = true;
					this.player = player;
				}

				public override string Use()
				{
					int amountToHeal = player.BaseStats.HPMax - player.BaseStats.CurrentHP;
					player.BaseStats.CurrentHP += healAmount;
					return "Player healed " + amountToHeal + "HP";
				}
			}
			public class PotionMP : Item
			{
				int restoreAmount = 25;
				Player player;
				public PotionMP(Player player)
				{
					_itemName = "MP Pot";
					_IsConsumable = true;
					this.player = player;
				}

				public override string Use()
				{
					int amountToRestore = player.BaseStats.MPMax - player.BaseStats.CurrentMP;
					player.BaseStats.CurrentMP += restoreAmount;
					return "Player restored " + amountToRestore + "MP";
				}
			}
			public List<Item> items = new List<Item>();
			public List<int> itemCount = new List<int>();

			public void addItem(Item item)
			{
				if (items.Any(x => x.itemName == item.itemName))
				{
					int i = items.IndexOf(items.Where(p => p.itemName == item.itemName).FirstOrDefault());
					itemCount[i]++;
				}
				else if (items.Count < 99)
				{
					items.Add(item);
					itemCount.Add(1);
				}
				else
				{
					//option to drop other items
				}
			}
			public string useItem(Item item)
			{
				string msg = item.Use();
				if (item.IsConsumable == true)
				{
					int i = items.IndexOf(item);
					itemCount[i]--;
					if (itemCount[i] == 0)
					{
						itemCount.RemoveAt(i);
						items.Remove(item);
					}
				}
				return msg;
			}

		}
	}

}
