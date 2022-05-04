using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using RPG_App.Map;
using RPG_App.Combat.Characters;

namespace RPG_App.Combat
{
	public class CombatEngine
	{
		public Player player = new Player();
		public Character Enemy = new Character("Bad Dude", "Human.spr", 1, 0, 100, 50, 10, 10, 50, 10, 10, 10);

		enum Menu { ChooseAction, ChooseAbility, ChooseItem };
		Menu CurrentMenu = Menu.ChooseAction;
		enum Action { Attack, Abilities, Items, Flee };
		Action ChooseAction = Action.Attack;
		private int _abilityMenuScroll;
		enum MenuSpot { TopLeft, TopRight, BotLeft, BotRight}
		MenuSpot AbilitySpot = MenuSpot.TopLeft;
		private int _itemMenuScroll;
		MenuSpot ItemSpot = MenuSpot.TopLeft;
		enum CombatState{ won, lost, ran,going}
		CombatState combatState = CombatState.going;
		readonly Random rand = new Random();
		RPG_Form form;
		MapEngine mapEngine;
		int fleeAttempts;

		private int abilityMenuScroll
		{
			get{ return _abilityMenuScroll; }
			set{	if ((value <= (player.Attacks.Count-4) / 2) && value >= 0)
					_abilityMenuScroll = value;
				}
		}
		private int itemMenuScroll
		{
			get { return _itemMenuScroll; }
			set
			{
				if ((value <= (player.inventory.items.Count - 2) / 2) && value >= 0)
					_itemMenuScroll = value;
			}
		}
		public void hookUpCombat(RPG_Form form, MapEngine map)
		{
			this.form = form;
			this.mapEngine = map;
		}
		public string CombatStep(char c)
		{
			string output = CombatInput(c);
			if(combatState != CombatState.going )
			{
				form.switchActiveMode();
			}
			return CombatOutput(output);
		}
		public string CombatInput(char c)
		{
			string msg = "";
			if (CurrentMenu == Menu.ChooseAction)
			{
				if (c == 'w')
				{
					if (ChooseAction == Action.Items)
					{
						ChooseAction = Action.Attack;
					}
					else if (ChooseAction == Action.Flee)
					{
						ChooseAction = Action.Abilities;
					}
				} else if (c == 's')
				{
					if (ChooseAction == Action.Attack)
					{
						ChooseAction = Action.Items;
					} else if (ChooseAction == Action.Abilities)
					{
						ChooseAction = Action.Flee;
					}
				} else if (c == 'a')
				{
					if (ChooseAction == Action.Abilities)
					{
						ChooseAction = Action.Attack;
					}
					else if (ChooseAction == Action.Flee)
					{
						ChooseAction = Action.Items;
					}
				} else if (c == 'd')
				{
					if (ChooseAction == Action.Attack)
					{
						ChooseAction = Action.Abilities;
					}
					else if (ChooseAction == Action.Items)
					{
						ChooseAction = Action.Flee;
					}
				} else if (c == ' ')
				{
					if (ChooseAction == Action.Attack)
					{
						msg = combatRound(player.Attacks.Find(x => x.Name == "Basic"));
					}
					else if (ChooseAction == Action.Abilities)
					{
						CurrentMenu = Menu.ChooseAbility;
					}
					else if (ChooseAction == Action.Items)
					{
						CurrentMenu = Menu.ChooseItem;
					}
					else if (ChooseAction == Action.Flee)
					{
						fleeAttempts++;
						msg = combatRoundFlee();
					}
				} 
			}
			else if (CurrentMenu == Menu.ChooseAbility)
			{
				if (c == 'w')
				{
					if (AbilitySpot == MenuSpot.BotLeft)
					{
						AbilitySpot = MenuSpot.TopLeft;
					}
					else if (AbilitySpot == MenuSpot.BotRight)
					{
						AbilitySpot = MenuSpot.TopRight;
					}
					else if (AbilitySpot == MenuSpot.TopLeft)
					{
						abilityMenuScroll--;
					}
					else if (AbilitySpot == MenuSpot.TopRight)
					{
						abilityMenuScroll--;
					}
				}
				else if (c == 's')
				{
					if (AbilitySpot == MenuSpot.TopLeft)
					{
						AbilitySpot = MenuSpot.BotLeft;
					}
					else if (AbilitySpot == MenuSpot.TopRight)
					{
						AbilitySpot = MenuSpot.BotRight;
					}
					else if (AbilitySpot == MenuSpot.BotLeft)
					{
						abilityMenuScroll++;
					}
					else if (AbilitySpot == MenuSpot.BotRight)
					{
						abilityMenuScroll++;
					}

				}
				else if (c == 'a')
				{
					if(AbilitySpot == MenuSpot.TopRight)
					{
						AbilitySpot = MenuSpot.TopLeft;
					}else if(AbilitySpot == MenuSpot.BotRight)
					{
						AbilitySpot = MenuSpot.BotLeft;
					}
				}
				else if (c == 'd')
				{
					if (AbilitySpot == MenuSpot.TopLeft)
					{
						AbilitySpot = MenuSpot.TopRight;
					}
					else if (AbilitySpot == MenuSpot.BotLeft)
					{
						AbilitySpot = MenuSpot.BotRight;
					}
				}
				else if (c == ' ')
				{
					if (player.Attacks.Count > 1 + (int)AbilitySpot + abilityMenuScroll * 2)
					{
						msg = combatRound(player.Attacks[1 + (int)AbilitySpot + abilityMenuScroll * 2]);
					}
				}
				else if (c == 'q')
				{
					if (CurrentMenu != Menu.ChooseAction)
					{
						CurrentMenu = Menu.ChooseAction;
					}
				}
			}
			else if (CurrentMenu == Menu.ChooseItem)
			{
				if (c == 'w')
				{
					if (ItemSpot == MenuSpot.BotLeft)
					{
						ItemSpot = MenuSpot.TopLeft;
					}
					else if (ItemSpot == MenuSpot.BotRight)
					{
						ItemSpot = MenuSpot.TopRight;
					}
					else if (ItemSpot == MenuSpot.TopLeft)
					{
						itemMenuScroll--;
					}
					else if (ItemSpot == MenuSpot.TopRight)
					{
						itemMenuScroll--;
					}
				}
				else if (c == 's')
				{
					if (ItemSpot == MenuSpot.TopLeft)
					{
						ItemSpot = MenuSpot.BotLeft;
					}
					else if (ItemSpot == MenuSpot.TopRight)
					{
						ItemSpot = MenuSpot.BotRight;
					}
					else if (ItemSpot == MenuSpot.BotLeft)
					{
						itemMenuScroll++;
					}
					else if (ItemSpot == MenuSpot.BotRight)
					{
						itemMenuScroll++;
					}

				}
				else if (c == 'a')
				{
					if (ItemSpot == MenuSpot.TopRight)
					{
						ItemSpot = MenuSpot.TopLeft;
					}
					else if (ItemSpot == MenuSpot.BotRight)
					{
						ItemSpot = MenuSpot.BotLeft;
					}
				}
				else if (c == 'd')
				{
					if (ItemSpot == MenuSpot.TopLeft)
					{
						ItemSpot = MenuSpot.TopRight;
					}
					else if (ItemSpot == MenuSpot.BotLeft)
					{
						ItemSpot = MenuSpot.BotRight;
					}
				}
				else if (c == ' ')
				{
					//Use Item
					if (player.inventory.items.Count > (int)ItemSpot + itemMenuScroll * 2)
					{
						msg = combatRound(player.inventory.items[(int)ItemSpot + itemMenuScroll * 2]);
					}
					
				}
				else if (c == 'q')
				{
					if (CurrentMenu != Menu.ChooseAction)
					{
						CurrentMenu = Menu.ChooseAction;
					}
				}
			}
				return msg;	//return message to print
		}
		
		public void EncounterStart(Character enemy)
		{
			Enemy = enemy;
			Enemy.BaseStats.CurrentHP = Enemy.BaseStats.HPMax;
			CurrentMenu = Menu.ChooseAction;
			ChooseAction = Action.Attack;
			AbilitySpot = MenuSpot.TopLeft;
			abilityMenuScroll = 0;
			ItemSpot = MenuSpot.TopLeft;
			itemMenuScroll = 0;
			combatState = CombatState.going;
			fleeAttempts = 0;
		}
		private string combatRound(Player.Attack pAttack)
		{
			string msg = "";
			if(pAttack.MPCost > player.BaseStats.CurrentMP)
			{
				return "Not enough MP";
			}
			bool pFirst = true;	//default player goes first
			if (player.BaseStats.Agi < Enemy.BaseStats.Agi)	//if enemy is faster they go first
				pFirst = false;
			else if( player.BaseStats.Agi == Enemy.BaseStats.Agi)
			{
				if(rand.Next(2) == 1)	//if player and enemy are same speed coinflip on order
				{
					pFirst = false;
				}
			}
			if(pFirst)
			{
				msg = Enemy.DamageCalc(pAttack, player, Enemy);
				if(Enemy.BaseStats.CurrentHP <= 0)
				{
					combatState = CombatState.won;
					return msg + "\r\n" + combatWon();
				}
				//enemy turn
				enemyTurn();
				if (player.BaseStats.CurrentHP <= 0)
				{
					combatState = CombatState.lost;
					return msg + "\r\nplayer lost";
				}
			}
			else
			{
				//enemy turn
				enemyTurn();
				if (player.BaseStats.CurrentHP <= 0)
				{
					combatState = CombatState.lost;
					return msg + "\r\nplayer lost";
				}
				msg = Enemy.DamageCalc(pAttack, player, Enemy);
				if (Enemy.BaseStats.CurrentHP <= 0)
				{
					combatState = CombatState.won;
					return msg + "\r\n" + combatWon();
				}
			}
			fleeAttempts = 0;
			return msg;
		}

		private string combatRound(Player.Inventory.Item item)
		{
			string msg = "";
			bool pFirst = true; //default player goes first
			if (player.BaseStats.Agi < Enemy.BaseStats.Agi) //if enemy is faster they go first
				pFirst = false;
			else if (player.BaseStats.Agi == Enemy.BaseStats.Agi)
			{
				if (rand.Next(1) == 2)  //if player and enemy are same speed coinflip on order
				{
					pFirst = false;
				}
			}
			if (pFirst)
			{
				msg = player.inventory.useItem(item);
				if (Enemy.BaseStats.CurrentHP <= 0)
				{
					combatState = CombatState.won;
					return msg + "\r\n" + combatWon();
				}
				//enemy turn
				enemyTurn();
				if (player.BaseStats.CurrentHP <= 0)
				{
					combatState = CombatState.lost;
					return msg + "\r\nplayer lost";
				}
			}
			else
			{
				//enemy turn
				enemyTurn();
				if (player.BaseStats.CurrentHP <= 0)
				{
					combatState = CombatState.lost;
					return msg + "\r\nplayer lost";
				}
				msg = player.inventory.useItem(item);
				if (Enemy.BaseStats.CurrentHP <= 0)
				{
					combatState = CombatState.won;
					return msg + "\r\n" + combatWon();
				}
			}
			fleeAttempts = 0;
			return msg;
		}
		private string combatRoundFlee()
		{
			string msg = "";
			float speedRelation = (float)player.BaseStats.Agi / (float)Enemy.BaseStats.Agi;
			if (speedRelation * 30 * fleeAttempts > rand.Next(100))
			{
				combatState = CombatState.ran;
				msg = "Successfully ran away";
			}else{
				enemyTurn();
				msg = "Failled to escape";
			}

			return msg;
		}
		private string combatWon()
		{
			string msg = "Player won";
			player.BaseStats.XP += Enemy.BaseStats.XP;
			//drop items
			//drop gold


			return msg;
		}
		private string enemyTurn()
		{
			
			return player.DamageCalc(Enemy.Attacks[rand.Next(Enemy.Attacks.Count)], Enemy, player);
		}
		public string CombatOutput(string msg)
		{
			string printedScreen;
			int screenWidth = 31;
			/*	"                      O   ");
				"                     /|\\  ");
				"                      /\\  ");
				"                    Enemy");
				"");
				"   You");
				"    O");
				"   /|\\");
				"   /\\");
				"  __________________________");
				"  |     Your Actions       |");
				"  | >Attack      Abilities |");
				"  |  Items       Flee      |");
				"  |________________________|"
				*/
			printedScreen = string.Concat(Enumerable.Repeat("_", screenWidth)) + "\r\n";
			
			//Stuff inside box
			for (int x = 0; x < player.CombatSprite.Length; x++)    //Print Enemy
			{
				printedScreen += "|                     ";
				printedScreen += Enemy.CombatSprite[x].PadRight(4);
				printedScreen += "    |\r\n";

			}
			printedScreen += "|" + String.Concat(Enumerable.Repeat(" ", 22 - (Enemy.Name.Length/2)));
			printedScreen += Enemy.Name;
			printedScreen += String.Concat(Enumerable.Repeat(" ", 7 - ((Enemy.Name.Length+1)/2))) + "|\r\n";//Print Enemy Name then HP
			printedScreen += "|" + string.Concat(Enumerable.Repeat(" ", 22 - Convert.ToString(Enemy.BaseStats.CurrentHP).Length));
			printedScreen += Enemy.BaseStats.CurrentHP + "/" + Enemy.BaseStats.HPMax;
			printedScreen += string.Concat(Enumerable.Repeat(" ", 6 - Convert.ToString(Enemy.BaseStats.HPMax).Length)) + "|\r\n";
			
			printedScreen += "|                             |\r\n";
			printedScreen += "|   You                       |\r\n";	// V V Print Player HP and MP V V
			printedScreen += "|" + string.Concat(Enumerable.Repeat(" ", 4 - Convert.ToString(player.BaseStats.CurrentHP).Length));
			printedScreen += player.BaseStats.CurrentHP + "/" + player.BaseStats.HPMax;
			printedScreen += string.Concat(Enumerable.Repeat(" ", 24 - Convert.ToString(player.BaseStats.HPMax).Length)) + "|\r\n";
			printedScreen += "|" + string.Concat(Enumerable.Repeat(" ", 4 - Convert.ToString(player.BaseStats.CurrentMP).Length));
			printedScreen += player.BaseStats.CurrentMP + "/" + player.BaseStats.MPMax;
			printedScreen += string.Concat(Enumerable.Repeat(" ", 24 - Convert.ToString(player.BaseStats.MPMax).Length)) + "|\r\n";
			for (int x = 0; x < player.CombatSprite.Length; x++)	//Print Player sprite
			{
				printedScreen += "|   ";
				printedScreen += player.CombatSprite[x];
				printedScreen += "                       |\r\n";

			}
			//Print Main Action menu
			if (CurrentMenu == Menu.ChooseAction)
			{
				printedScreen +=	 "| ___________________________ |\r\n";
				printedScreen +=	 "| |      Your Actions       | |\r\n";
				if (ChooseAction == Action.Attack)
					printedScreen += "| | >Attack      Abilities  | |\r\n";
				else if (ChooseAction == Action.Abilities)
					printedScreen += "| |  Attack     >Abilities  | |\r\n";
				else
					printedScreen += "| |  Attack      Abilities  | |\r\n";
				if (ChooseAction == Action.Items)
					printedScreen += "| | >Items       Flee       | |\r\n";
				else if (ChooseAction == Action.Flee)
					printedScreen += "| |  Items      >Flee       | |\r\n";
				else
					printedScreen += "| |  Items       Flee       | |\r\n";
				printedScreen +=	 "| |_________________________| |\r\n";
			}
			else if (CurrentMenu == Menu.ChooseAbility)
			{
				string Ability1, Ability2 = "", Ability3 = "", Ability4 = "";
				Ability1 = player.Attacks[1 + 2 * abilityMenuScroll].Name;
				if (player.Attacks.Count - 1 >= 2 + abilityMenuScroll * 2)
				{
					Ability2 = player.Attacks[2 + 2 * abilityMenuScroll].Name;
					if (player.Attacks.Count - 1 >= 3 + abilityMenuScroll * 2)
					{
						Ability3 = player.Attacks[3 + 2 * abilityMenuScroll].Name;
						if (player.Attacks.Count - 1 >= 4 + abilityMenuScroll * 2)
							Ability4 = player.Attacks[4 + 2 * abilityMenuScroll].Name;
					}
				}
				printedScreen += "| ___________________________ |\r\n";
				printedScreen += "| |        Abilities        | |\r\n";
				if(AbilitySpot == MenuSpot.TopLeft)
					printedScreen += "| | >" + Ability1 + string.Concat(Enumerable.Repeat(" ", 11 - Ability1.Length))
							  + Ability2 + string.Concat(Enumerable.Repeat(" ", 11 - Ability2.Length)) + " | |\r\n";
				else if(AbilitySpot == MenuSpot.TopRight)
					printedScreen += "| |  " + Ability1 + string.Concat(Enumerable.Repeat(" ", 10 - Ability1.Length))
							  + ">" + Ability2 + string.Concat(Enumerable.Repeat(" ", 11 - Ability2.Length)) + " | |\r\n";
				else
					printedScreen += "| |  " + Ability1 + string.Concat(Enumerable.Repeat(" ", 11-Ability1.Length))
							  + Ability2 + string.Concat(Enumerable.Repeat(" ", 11 - Ability2.Length)) + " | |\r\n";
				if(AbilitySpot == MenuSpot.BotLeft)
					printedScreen += "| | >" + Ability3 + string.Concat(Enumerable.Repeat(" ", 11 - Ability3.Length))
								  + Ability4 + string.Concat(Enumerable.Repeat(" ", 11 - Ability4.Length)) + " | |\r\n";
				else if(AbilitySpot == MenuSpot.BotRight)
					printedScreen += "| |  " + Ability3 + string.Concat(Enumerable.Repeat(" ", 10 - Ability3.Length))
								  + ">" + Ability4 + string.Concat(Enumerable.Repeat(" ", 11 - Ability4.Length)) + " | |\r\n";
				else
				printedScreen += "| |  " + Ability3 + string.Concat(Enumerable.Repeat(" ", 11 - Ability3.Length))
							  + Ability4 + string.Concat(Enumerable.Repeat(" ", 11 - Ability4.Length)) + " | |\r\n";
				printedScreen += "| |_________________________| |\r\n";
			}else if(CurrentMenu == Menu.ChooseItem)
			{
				string Item1 = "No items", Item2 = "", Item3 = "", Item4 = "";
				if(player.inventory.items.Count >= 1 + itemMenuScroll * 2)
				Item1 = player.inventory.items[itemMenuScroll * 2].itemName + "x" + player.inventory.itemCount[itemMenuScroll * 2];
				if (player.inventory.items.Count>= 2 + itemMenuScroll * 2)
				{
					Item2 = player.inventory.items[1 + 2 * itemMenuScroll].itemName + "x" + player.inventory.itemCount[1 + itemMenuScroll * 2];
					if (player.inventory.items.Count>= 3 + itemMenuScroll * 2)
					{
						Item3 = player.inventory.items[2 + 2 * itemMenuScroll].itemName + "x" + player.inventory.itemCount[2 + itemMenuScroll * 2];
						if (player.inventory.items.Count >= 4 + itemMenuScroll * 2)
							Item4 = player.inventory.items[3 + 2 * itemMenuScroll].itemName + "x" + player.inventory.itemCount[3 + itemMenuScroll * 2];
					}
				}
				printedScreen += "| ___________________________ |\r\n";
				printedScreen += "| |          Items          | |\r\n";
				if (ItemSpot == MenuSpot.TopLeft)
					printedScreen += "| | >" + Item1 + string.Concat(Enumerable.Repeat(" ", 11 - Item1.Length))
							  + Item2 + string.Concat(Enumerable.Repeat(" ", 11 - Item2.Length)) + " | |\r\n";
				else if (ItemSpot == MenuSpot.TopRight)
					printedScreen += "| |  " + Item1 + string.Concat(Enumerable.Repeat(" ", 10 - Item1.Length))
							  + ">" + Item2 + string.Concat(Enumerable.Repeat(" ", 11 - Item2.Length)) + " | |\r\n";
				else
					printedScreen += "| |  " + Item1 + string.Concat(Enumerable.Repeat(" ", 11 - Item1.Length))
							  + Item2 + string.Concat(Enumerable.Repeat(" ", 11 - Item2.Length)) + " | |\r\n";
				if (ItemSpot == MenuSpot.BotLeft)
					printedScreen += "| | >" + Item3 + string.Concat(Enumerable.Repeat(" ", 11 - Item3.Length))
								  + Item4 + string.Concat(Enumerable.Repeat(" ", 11 - Item4.Length)) + " | |\r\n";
				else if (ItemSpot == MenuSpot.BotRight)
					printedScreen += "| |  " + Item3 + string.Concat(Enumerable.Repeat(" ", 10 - Item3.Length))
								  + ">" + Item4 + string.Concat(Enumerable.Repeat(" ", 11 - Item4.Length)) + " | |\r\n";
				else
					printedScreen += "| |  " + Item3 + string.Concat(Enumerable.Repeat(" ", 11 - Item3.Length))
								  + Item4 + string.Concat(Enumerable.Repeat(" ", 11 - Item4.Length)) + " | |\r\n";
				printedScreen += "| |_________________________| |\r\n";
			}

			printedScreen += string.Concat(Enumerable.Repeat("‾", screenWidth));
			printedScreen += "\r\n" + msg;
			printedScreen = printedScreen.Replace("\0", string.Empty);
			printedScreen += "";
			return printedScreen;
		}
	}

	
	

	
}
