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
			inventory.addItem(new Inventory.PotionMP(this));
			inventory.addItem(new Inventory.Item("fItem2"));
			inventory.addItem(new Inventory.Item("fItem3"));
			inventory.addItem(new Inventory.Item("fItem4"));
			//inventory.addItem(new Inventory.Item("fItem5"));
		}

		public class Inventory
		{
			public class Item
			{
				public string itemName;
				public Item(string itemName = "NameMissing")
				{
					this.itemName = itemName;
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
					itemName = "HP Potion";
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
					itemName = "MP Potion";
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
					itemCount[items.IndexOf(item)]++;
				}
				else if (items.Count <= 100)
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
				return item.Use();
			}

		}
	}

}
