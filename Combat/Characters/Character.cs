using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RPG_App.Combat.Characters
{
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

			public int HPMax { get { return _HPMax; } }
			public int MPMax { get { return _MPMax; } }
			public int CurrentHP
			{
				get { return _CurrentHP; }
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
						_CurrentMP = _MPMax;
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
			public enum DamageType { Physical, Fire, Ice, Electric, Heal, none };
			readonly string _name;
			readonly int _dmg;
			readonly DamageType _type;
			readonly int _accuracy;
			readonly int _MPCost;
			public string Name { get { return _name; } }
			public int Dmg { get { return _dmg; } }
			public DamageType Type { get { return _type; } }
			public int Accuracy { get { return _accuracy; } }
			public int MPCost { get { return _MPCost; } }
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
			this.CombatSprite = File.ReadAllLines(Environment.CurrentDirectory + "\\..\\..\\Combat\\CombatSprites\\" + fileName);
			BaseStats = new StatBlock(lvl, xp, hp, mp, atk, matk, def, mdef, dex, agi);
			if (weaknesses != null)
			{
				this.Weakness = weaknesses;
			}
			else
				this.Weakness = new Attack.DamageType[] { Attack.DamageType.none };
			Attacks.Add(new Attack("Basic", 50, Attack.DamageType.Physical, 80));
		}

		public string DamageCalc(Attack attack, Character user, Character target)
		{
			// (attack^2) / (attack+defense)
			int dmg;
			string damageMessage;
			user.BaseStats.CurrentMP -= attack.MPCost;
			if (attack.Type == Attack.DamageType.Heal)
			{
				user.BaseStats.CurrentHP += attack.Dmg;
				damageMessage = user.Name + " healed " + attack.Dmg + "hp";
				return damageMessage;
			}
			if (rand.Next(100) <= (attack.Accuracy * ((float)user.BaseStats.Dex / target.BaseStats.Agi)))//See if hits
			{   //attack hits
				if (attack.Type == Attack.DamageType.Physical)
				{
					dmg = (int)((float)(attack.Dmg * user.BaseStats.Atk) / ((float)user.BaseStats.Atk + target.BaseStats.Def) / 2);
					dmg = (dmg * rand.Next(90, 110)) / 100;
				}
				else
				{
					dmg = (attack.Dmg * user.BaseStats.MAtk) / (user.BaseStats.MAtk + target.BaseStats.MDef);
				}
				if (target.Weakness.Contains(attack.Type))
				{
					dmg = dmg * 2;
				}
				target.BaseStats.CurrentHP -= dmg;
				damageMessage = attack.Name + " hit " + target.Name + " for " + Convert.ToString(dmg) + " damage";
			}
			else //attack missed
			{
				damageMessage = attack.Name + " missed";
			}


			return damageMessage;
		}

	}

}
