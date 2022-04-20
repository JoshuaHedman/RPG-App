using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using RPG_App.Map;

namespace RPG_App.Combat
{
	public class CombatEngine
	{
		public Player player = new Player();
		public Character Enemy = new Character("Bad Dude", "Human.spr", 1, 0, 100, 50, 10, 10, 50, 10, 10, 10);

		enum Menu { ChooseAction, ChooseAbility };
		Menu CurrentMenu = Menu.ChooseAction;
		enum Action { Attack, Abilities, Items, Flee };
		Action ChooseAction = Action.Attack;
		private int _abilityMenuScroll;
		enum MenuSpot { TopLeft, TopRight, BotLeft, BotRight}
		MenuSpot AbilitySpot = MenuSpot.TopLeft;
		enum CombatState{ won, lost, going}
		CombatState combatState = CombatState.going;
		readonly Random rand = new Random();
		RPG_Form form;
		MapEngine mapEngine;

		private int abilityMenuScroll
		{
			get{ return _abilityMenuScroll; }
			set{	if ((value <= (player.Attacks.Count-4) / 2) && value >= 0)
					_abilityMenuScroll = value;
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
						msg = combatRound(player.Attacks.Find(x => x.Name == "Basic"));
					//msg = Enemy.DamageCalc(player.Attacks.Find(x => x.Name == "Basic"), player, Enemy);
					else if (ChooseAction == Action.Abilities)
						CurrentMenu = Menu.ChooseAbility;
					else if (ChooseAction == Action.Items)
						msg = "Not Implemented yet";
					else if (ChooseAction == Action.Flee)
						msg = "Not Implemented yet";
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
						//msg = Enemy.DamageCalc(player.Attacks[1 + (int)AbilitySpot + abilityMenuScroll * 2], player, Enemy);
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
				return msg;	//return message to print
		}
		//public void testfight()
		//{

		//	//Character enemy = new Character("Bad", "Human.txt");
		//}
		
		public void EncounterStart(Character enemy)
		{
			Enemy = enemy;
			Enemy.BaseStats.CurrentHP = Enemy.BaseStats.HPMax;
			ChooseAction = Action.Attack;
			AbilitySpot = MenuSpot.TopLeft;
			abilityMenuScroll = 0;
			combatState = CombatState.going;
		}
		private string combatRound(Player.Attack pAttack)
		{
			string msg = "";
			bool pFirst = true;	//default player goes first
			if (player.BaseStats.Agi < Enemy.BaseStats.Agi)	//if enemy is faster they go first
				pFirst = false;
			else if( player.BaseStats.Agi == Enemy.BaseStats.Agi)
			{
				if(rand.Next(1) == 2)	//if player and enemy are same speed coinflip on order
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
					return msg + "\r\nplayer won";
				}
				//enemy turn
				if (player.BaseStats.CurrentHP <= 0)
				{
					combatState = CombatState.lost;
					return msg + "\r\nplayer lost";
				}
			}
			else
			{
				//enemy turn
				if (player.BaseStats.CurrentHP <= 0)
				{
					combatState = CombatState.lost;
					return msg + "\r\nplayer lost";
				}
				msg = Enemy.DamageCalc(pAttack, player, Enemy);
				if (Enemy.BaseStats.CurrentHP <= 0)
				{
					combatState = CombatState.won;
					return msg + "\r\nplayer won";
				}
			}
			return msg;
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
			//Console.Clear();
			printedScreen = string.Concat(Enumerable.Repeat("_", screenWidth)) + "\r\n";
			//printedScreen += ("|");
			//Stuff inside box
			for (int x = 0; x < player.CombatSprite.Length; x++)    //Print Enemy
			{
				printedScreen += "|                     ";
				printedScreen += Enemy.CombatSprite[x].PadRight(4);
				printedScreen += "    |\r\n";

			}
			printedScreen += "|" + String.Concat(Enumerable.Repeat(" ", 22 - (Enemy.Name.Length/2)));
			printedScreen += Enemy.Name;
			printedScreen += String.Concat(Enumerable.Repeat(" ", 7 - ((Enemy.Name.Length+1)/2))) + "|\r\n";//Print Enemy Name then V V HP
			printedScreen += "|" + string.Concat(Enumerable.Repeat(" ", 22 - Convert.ToString(Enemy.BaseStats.CurrentHP).Length));
			printedScreen += Enemy.BaseStats.CurrentHP + "/" + Enemy.BaseStats.HPMax;
			printedScreen += string.Concat(Enumerable.Repeat(" ", 6 - Convert.ToString(Enemy.BaseStats.HPMax).Length)) + "|\r\n";
			//|                    Enemy    |\r\n"
			printedScreen += "|                             |\r\n";
			printedScreen += "|   You                       |\r\n";	// V V Print Player HP and MP V V
			printedScreen += "|" + string.Concat(Enumerable.Repeat(" ", 4 - Convert.ToString(player.BaseStats.CurrentHP).Length));
			printedScreen += player.BaseStats.CurrentHP + "/" + player.BaseStats.HPMax;
			printedScreen += string.Concat(Enumerable.Repeat(" ", 24 - Convert.ToString(player.BaseStats.CurrentHP).Length)) + "|\r\n";
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
			}


				//
			//printedScreen += (" |\r\n");
			
			printedScreen += string.Concat(Enumerable.Repeat("‾", screenWidth));
			printedScreen += "\r\n" + msg;
			printedScreen = printedScreen.Replace("\0", string.Empty);
			printedScreen += "";
			return printedScreen;
			//return "combat";
		}
	}

	public class Character
	{
		public class StatBlock
		{
			public int Lvl;
			public int XP;
			private int _HPMax;
			private int _CurrentHP;
			private int _MPMax;
			private int _CurrentMP;
			public int Atk;
			public int MAtk;
			public int Def;
			public int MDef;
			public int Dex;
			public int Agi;

			public int HPMax{ get { return _HPMax; } }
			public int MPMax{ get { return _MPMax; } }
			public int CurrentHP
			{
				get{ return _CurrentHP; }
				set
				{
					if (value <= _HPMax)
						_CurrentHP = value;
					else
						_CurrentHP = _HPMax;
				}
			}
			public int CurrentMP
			{
				get { return _CurrentMP; }
				set
				{
					if (value <= _MPMax)
						_CurrentMP = value;
					else
						_CurrentMP = _HPMax;
				}
			}

			public StatBlock(int lvl, int xp, int hp, int mp, int atk, int matk, int def, int mdef, int dex, int agi)
			{
				Lvl = lvl;
				XP = xp;
				_HPMax = hp;
				_CurrentHP = hp;
				_MPMax = mp;
				_CurrentMP = mp;
				Atk = atk;
				MAtk = matk;
				Def = def;
				MDef = mdef;
				Dex = dex;
				Agi = agi;
			}


		}
		public class Attack
		{
			public enum DamageType { Physical, Fire, Ice, Electric, Heal, none};
			readonly string _name;
			readonly int _dmg;
			readonly DamageType _type;
			readonly int _accuracy;
			readonly int _MPCost;
			public string Name{ get { return _name; } }
			public int Dmg{ get { return _dmg; } }
			public DamageType Type { get { return _type; } }
			public int Accuracy{ get { return _accuracy; } }
			public int MPCost{ get { return _MPCost; } }
			public Attack(string name, int dmg, DamageType type, int accuracy, int MPCost = 0)
			{
				_name = name;
				_dmg = dmg;
				_type = type;
				_accuracy = accuracy;
				_MPCost = MPCost;
			}
		}
		string _name;
		//public readonly char mapSprite;
		readonly public string[] CombatSprite;
		public List<Attack> Attacks = new List<Attack>();
		public StatBlock BaseStats;
		Attack.DamageType[] Weakness;
		readonly Random rand = new Random();
		public string Name
		{
			get { return _name; }
		}
		
	
		public Character(string name, string fileName, int lvl, int xp, int hp, int mp, int atk, int matk, int def, int mdef, int dex, int agi, Attack.DamageType[] weaknesses = null)
		{
			_name = name;
			this.CombatSprite = File.ReadAllLines(System.Environment.CurrentDirectory + "\\..\\..\\Combat\\CombatSprites\\" + fileName);
			BaseStats = new StatBlock(lvl, xp, hp, mp, atk, matk, def, mdef, dex, agi);
			if (weaknesses != null)
			{
				this.Weakness = weaknesses;
			}
			else
				this.Weakness = new Attack.DamageType[] { Attack.DamageType.none };
		}

		public string DamageCalc(Attack attack, Character user, Character target)
		{
			// (attack^2) / (attack+defense)
			int dmg;
			string damageMessage;
			//rand.Next(100)
			user.BaseStats.CurrentMP -= attack.MPCost;
			if(attack.Type == Attack.DamageType.Heal)
			{
				user.BaseStats.CurrentHP += attack.Dmg;
				damageMessage = user.Name + " healed " + attack.Dmg + "hp";
				return damageMessage;
			}
			if (rand.Next(100) <= (attack.Accuracy * ((float)user.BaseStats.Dex / target.BaseStats.Agi) ) )//See if hits
			{	//attack hits
				if (attack.Type == Attack.DamageType.Physical)
				{
					dmg = (int)((float)(attack.Dmg * user.BaseStats.Atk) / ((float)user.BaseStats.Atk + target.BaseStats.Def) / 2);
					dmg = (dmg * rand.Next(80, 100)) / 100;
				}
				else
				{
					dmg = (attack.Dmg * user.BaseStats.MAtk) / (user.BaseStats.MAtk + target.BaseStats.MDef);
				}
				if(target.Weakness.Contains(attack.Type))
				{
					dmg = dmg * 2;
				}
				target.BaseStats.CurrentHP -= dmg;
				damageMessage = attack.Name + " hit " + target.Name + " for " + Convert.ToString(dmg) + " damage";
			}else //attack missed
			{
				damageMessage = attack.Name + " missed";
			}


			return damageMessage;
		}
		
	}

	public class Player : Character
	{

		/*public Player(string name) 
		{
			string Name = name;
		}*/
		public Player() : base("player", "Player.spr", 1, 0, 1000, 50, 50, 10, 10, 10, 10, 10)
		{
			Attacks.Add(new Attack("Basic", 50, Attack.DamageType.Physical, 80));
			Attacks.Add(new Attack("Firebolt", 50, Attack.DamageType.Fire, 80, 10));
			Attacks.Add(new Attack("Heal Opp", -100, Attack.DamageType.Ice, 100));
			Attacks.Add(new Attack("Heal", 30, Attack.DamageType.Heal, 100, 10));
			//Attacks.Add(new Attack("Filler3", 0, Attack.DamageType.Physical, 100));
			//Attacks.Add(new Attack("Filler4", 0, Attack.DamageType.Physical, 100));
			//Attacks.Add(new Attack("Filler5", 0, Attack.DamageType.Physical, 100));
			//Attacks.Add(new Attack("Filler6", 0, Attack.DamageType.Physical, 100));
			//Attacks.Add(new Attack("Filler7", 0, Attack.DamageType.Physical, 100));
			//Attacks.Add(new Attack("Filler8", 0, Attack.DamageType.Physical, 100));

		}

		class Inventory
		{
		


		}
	}


	
}
