using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RPG_App.Combat
{
	public class CombatEngine
	{
		public Player player = new Player();
		public Character Enemy = new Character("Bad Dude", "Human.txt", 1, 0, 100, 50, 10, 10, 50, 10, 10, 10);
		
		enum Menu{ ChooseAction, ChooseAbility};
		Menu CurrentMenu = Menu.ChooseAction;
		enum Action{Attack, Abilities, Items, Flee };
		Action ChooseAction = Action.Attack;
		public string CombatStep(char c)
		{
			CombatInput(c);
			return CombatOutput();
		}
		public void CombatInput(char c)
		{
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
					}else if(ChooseAction == Action.Abilities)
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
						Enemy.DamageCalc(player.Attacks.Find(x => x.Name == "Basic"), player, Enemy);
					if (ChooseAction == Action.Abilities)
						Enemy.BaseStats.CurrentHP = Enemy.BaseStats.HPMax;
					if (ChooseAction == Action.Items)
						Enemy.DamageCalc(player.Attacks.Find(x => x.Name == "Firebolt"), player, Enemy);
				}
			}
		}
		//public void testfight()
		//{

		//	//Character enemy = new Character("Bad", "Human.txt");
		//}
		public string CombatOutput()
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
				printedScreen += Enemy.CombatSprite[x];
				printedScreen += "     |\r\n";

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
		/*	if (CurrentMenu == Menu.ChooseAbility)
			{
				printedScreen += "|  __________________________ |\r\n";
				printedScreen += "|  |       Abilities        | |\r\n";
				printedScreen += 
			}
		*/


				//
			//printedScreen += (" |\r\n");
			
			printedScreen += string.Concat(Enumerable.Repeat("‾", screenWidth));
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
			public enum DamageType { Physical, Fire, Ice, Electric, none};
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
		public string Name
		{
			get { return _name; }
		}
		Random rand = new Random();
	
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
			if (rand.Next(100) <= (attack.Accuracy * ((float)user.BaseStats.Dex / target.BaseStats.Agi) ) )//See if hits
			{	//attack hits
				if (attack.Type == Attack.DamageType.Physical)
				{
					dmg = (int)((float)(attack.Dmg * user.BaseStats.Atk) / ((float)user.BaseStats.Atk + target.BaseStats.Def));
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
		public Player() : base("player", "Player.txt", 1, 0, 1000, 50, 50, 10, 10, 10, 10, 10)
		{
			Attacks.Add(new Attack("Basic", 50, Attack.DamageType.Physical, 80));
			Attacks.Add(new Attack("Firebolt", 50, Attack.DamageType.Fire, 80, 10));
			Attacks.Add(new Attack("Filler1", 0, Attack.DamageType.Physical, 100));
			Attacks.Add(new Attack("Filler2", 0, Attack.DamageType.Physical, 100));
			Attacks.Add(new Attack("Filler3", 0, Attack.DamageType.Physical, 100));
			Attacks.Add(new Attack("Filler4", 0, Attack.DamageType.Physical, 100));
			Attacks.Add(new Attack("Filler5", 0, Attack.DamageType.Physical, 100));
			Attacks.Add(new Attack("Filler6", 0, Attack.DamageType.Physical, 100));
			Attacks.Add(new Attack("Filler7", 0, Attack.DamageType.Physical, 100));
			Attacks.Add(new Attack("Filler8", 0, Attack.DamageType.Physical, 100));

		}

		class Inventory
		{
		


		}
	}


	
}
