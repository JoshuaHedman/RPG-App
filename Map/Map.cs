using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using RPG_App.Combat;

namespace RPG_App.Map
{
	public class MapEngine
	{
		private Dictionary<string, Map> MapList = new Dictionary<string, Map>();
		private Map currentMap;
		private Random rand = new Random();
		private CombatEngine combatEngine;
		private RPG_Form form;
		

		public enum TileAction{Move, MoveIfHasItem, ToMap, ToMapWCoords,ToMapDirection, none}
		public MapEngine()
		{
			currentMap = new Map("OverWorld.map", "OverWorldEnemies.txt");
			MapList.Add("OverWorld.map", currentMap);

		}

		public void hookUpMap(RPG_Form form, CombatEngine combat)
		{
			this.form = form;
			this.combatEngine = combat;
		}

		public string Mapstep(char c)
		{
			MapInput(c);
			if (rand.Next(100) < 20)    // encounter chance, currently 20%
			{
				RandEncounter();
			}
			else if (c == 'f')
				RandEncounter();
			return MapOutput();
		}
		public void MapInput(char c)
		{
			if (currentMap.MovePlayer(c))
			{
				if (currentMap.IsPlayerOnTile)
				{
					Tile tile = currentMap.PlayerOnTile;
					TileAction action = tile.OnEnter();
					if(action == TileAction.ToMap || action == TileAction.ToMapWCoords || action == TileAction.ToMapDirection)
					{
						string mapName = ((MapChange)tile).MapName;
						if (MapList.ContainsKey(mapName))
						{
							currentMap = MapList[((MapChange)tile).MapName];
						}
						else
						{
							Map map = new Map(mapName);
							MapList.Add(mapName, map);
							currentMap = MapList[((MapChange)tile).MapName];
						}
						if(action == TileAction.ToMapWCoords)
						{
							currentMap.pXPos = ((MapChange)tile).xCoord;
							currentMap.pYPos = ((MapChange)tile).yCoord;
						}else if(action == TileAction.ToMapDirection)
						{
							MapInput(c);
						}
					}
				}
			}
		}
		public string MapOutput()
		{
			return currentMap.MapString();
		}

		public void RandEncounter()
		{
			combatEngine.EncounterStart(currentMap.enemyList[rand.Next(currentMap.enemyList.Count)]);
			form.switchActiveMode();
		}

		private class Map
		{
			public char[,] grid;
			readonly private int xSize;
			readonly private int ySize;
			public int _pXPos;
			public int _pYPos;
			//private List<Tile> tileSet = new List<Tile>;
			public Dictionary<char, Tile> tileSet = new Dictionary<char, Tile>();
			public List<Combat.Character> enemyList = new List<Character>();

			public int pXPos
			{
				get {return _pXPos;}
				set { _pXPos = value; }
			}
			public int pYPos
			{
				get { return _pYPos; }
				set { _pYPos = value; }
			}

			public Tile PlayerOnTile
			{
				get { return tileSet[this.grid[pXPos, pYPos]]; }
			}

			public bool IsPlayerOnTile
			{
				get { return tileSet.ContainsKey(grid[pXPos, pYPos]); }
			}

			public Map(string fileName, string enemyFileName = null)
			{

				string[] lines = File.ReadAllLines(System.Environment.CurrentDirectory + "\\..\\..\\Map\\Maps\\" + fileName);
				//xSize = lines.Length-1;
				//ySize = xSize;
				string[] mapParams = lines[0].Split(';');
				string[] mapSize = mapParams[0].Split('x');
				string[] playerStart = mapParams[1].Split('x');

				xSize = Convert.ToInt32(mapSize[0]);
				ySize = Convert.ToInt32(mapSize[1]);
				grid = new char[xSize + 1, ySize + 1];
				
				pXPos = Convert.ToInt32(playerStart[0]);
				pYPos = Convert.ToInt32(playerStart[1]);

				//Read in tileset from file
				string[] tileSet = lines[lines.Count() - 1].Split(';');
				for (int i = 0; i < tileSet.Count(); i++)
				{
					string[] tileParams = tileSet[i].Split(',');
					if (tileParams.Length >= 2)
					{
						Type t = Type.GetType("RPG_App.MapEngine" + tileParams[0]);
						//Tile tiletest = (Tile)Activator.CreateInstance(Type.GetType("RPG_App."+ tileParams[0]), (tileParams[2]), tileParams[3]);
						Tile tiletest = (Tile)Activator.CreateInstance(Type.GetType("RPG_App.Map.MapEngine+" + tileParams[0]), args: tileParams.Skip(2).ToArray()); //1st is tile name,  2nd is tile in mapfile, then params
						this.tileSet.Add(Convert.ToChar(tileParams[1]), tiletest);
					}
				}

				//Read map text to grid
				for (int y = 1; y < ySize + 1; y++)
				{
					int x = 1;
					char[] line = lines[y].ToCharArray();
					while (x < xSize)
					{
						this.grid[x + 1, y] = line[x];
						//Console.Write(grid[x, y] + " ");
						x++;
					}
					//Console.Write("\n");
				}

				//Read enemy list
				if(enemyFileName != null)
				{
					string[] eListLines = File.ReadAllLines(System.Environment.CurrentDirectory + "\\..\\..\\Map\\Maps\\" + enemyFileName);
					for(int i = 0; i < eListLines.Length; i++)
					{
						string[] enemyData = eListLines[i].Split(',');
						List<Character.Attack.DamageType> weakness = new List<Character.Attack.DamageType>();
						if (enemyData[12] == "fire")
						{
							weakness.Add(Character.Attack.DamageType.Fire);
						}else if (enemyData[12] == "none")
						{
							weakness.Add(Character.Attack.DamageType.Ice);
						}else if (enemyData[12] == "none")
						{
							weakness.Add(Character.Attack.DamageType.Electric);
						}else if (enemyData[12] == "none")
						{
							weakness.Add(Character.Attack.DamageType.Physical);
						}else
						{
							weakness.Add(Character.Attack.DamageType.none);
						}
						//Slime,Slime.spr,1,50,50,0,10,10,5,5,10,10,fire  ; Example of data
						enemyList.Add(new Character(enemyData[0], enemyData[1], Convert.ToInt32(enemyData[2]), Convert.ToInt32(enemyData[3]), Convert.ToInt32(enemyData[4])
						, Convert.ToInt32(enemyData[5]), Convert.ToInt32(enemyData[6]), Convert.ToInt32(enemyData[7]), Convert.ToInt32(enemyData[8]),
						Convert.ToInt32(enemyData[9]), Convert.ToInt32(enemyData[10]), Convert.ToInt32(enemyData[11]), weakness.ToArray()));
					}
				}

			}


			public string MapString()
			{
				string PrintedMap;

				//Console.Clear();
				PrintedMap = string.Concat(Enumerable.Repeat("_", xSize * 2 + 1)) + "\r\n";
				for (int y = 1; y <= ySize; y++)
				{
					PrintedMap += ("|");
					for (int x = 2; x <= xSize; x++)
					{
						if (x == pXPos && y == pYPos)
						{
							PrintedMap += (" " + "x");
						}
						else
						{

							//Console.Write(" " + grid[x, y]);
							if (tileSet.ContainsKey(grid[x, y]))
							{
								PrintedMap += (" " + tileSet[grid[x, y]].Sprite);
							}
							else
							{
								PrintedMap += (" " + grid[x, y]);
							}
						}
					}
					PrintedMap += (" |\r\n");
				}
				PrintedMap += string.Concat(Enumerable.Repeat("‾", xSize * 2 + 1));
				PrintedMap = PrintedMap.Replace("\0", string.Empty);
				PrintedMap += "";
				return PrintedMap;
			}

			public bool MovePlayer(char d)
			{
				bool moved = false;
				switch (d)
				{
					case 'w':
						if (pYPos > 1 && TryEnterTile(pXPos, pYPos - 1))
						{
							//if(OnTile(pXPos, pYPos))
							pYPos--;
							moved = true;
						}
						break;
					case 's':
						if (pYPos < ySize && TryEnterTile(pXPos, pYPos + 1))
						{
							pYPos++;
							moved = true;
						}
						break;
					case 'a':
						if (pXPos > 2 && TryEnterTile(pXPos - 1, pYPos))
						{
							pXPos--;
							moved = true;
						}
						break;
					case 'd':
						if (pXPos < xSize && TryEnterTile(pXPos + 1, pYPos))
						{
							pXPos++;
							moved = true;
						}
						break;
				
						
						
				}
				return moved;
			}

			private bool TryEnterTile(int x, int y)
			{
				if (tileSet.ContainsKey(grid[x, y]))
				{
					//Tile t = tileSet[grid[x, y]].Impassable;
					if (tileSet[grid[x, y]].Impassable == true)
					{
						return false;
					}
					else
						return true;
				}
				else
					return true;
			}

		}

		class Tile
		{
			private char _sprite = '⚠';
			private bool _impassable = false;
			//readonly item itemPassable;

			public char Sprite
			{
				get { return _sprite; }
				protected set { _sprite = value; }
			}

			public bool Impassable
			{
				get { return _impassable; }
				protected set { _impassable = value; }
			}
			virtual public TileAction OnEnter()
			{
				return TileAction.none;
			}
		}

		class Water : Tile
		{

			//public Water(char sprite = '≈', bool impass = true)
			//{
			//	this.Sprite = sprite;
			//	this.Impassable = impass;
			//}
			public Water(string sprite = "≈", string impass = "t")
			{
				this.Sprite = Convert.ToChar(sprite);
				this.Impassable = impass.Contains("t");
			}

			public Water()
			{
				this.Sprite = '≈';
				this.Impassable = true;
			}

			public override TileAction OnEnter()
			{
				return TileAction.none;
			}
		}

		class Mountain : Tile
		{

			public Mountain(string sprite = "Ʌ", string impass = "t")
			{
				this.Sprite = Convert.ToChar(sprite);
				this.Impassable = impass.Contains("t");
			}

			public Mountain()
			{
				this.Sprite = 'Ʌ';
				this.Impassable = true;
			}

			public override TileAction OnEnter()
			{
				return TileAction.none;
			}
		}

		class MapChange : Tile
		{
			//private Map _toMap;
			private string _mapName;
			private SpecificSpot _changeType;
			private int _xCoord;
			private int _yCoord;
			public enum SpecificSpot { ToCoord, Directional, SameTile}
			//public Map ToMap
			//{
			//	get { return _toMap; }
			//	set { _toMap = value; }
			//}

			public string MapName
			{
				get { return _mapName; }
				set { _mapName = value; }
			}
			public SpecificSpot ChangeType
			{
				get { return _changeType; }
			}
			public int xCoord
			{
				get { return _xCoord; }
			}
			public int yCoord
			{
				get { return _yCoord; }
			}

			public MapChange(string map, string sprite = "⌂", string impass = "f", string  changeType= "SameTile", string Coords = "")
			{
				this.Sprite = Convert.ToChar(sprite);
				this.Impassable = impass.Contains("t");
				//this._toMap = new Map(map, 10, 10);
				this._mapName = map;
				this._changeType = (SpecificSpot)Enum.Parse(typeof(SpecificSpot), changeType, ignoreCase:false);
				string[] coords = Coords.Split('x');
				this._xCoord = Convert.ToInt32(coords[0]);
				this._yCoord = Convert.ToInt32(coords[1]);
			}
			public MapChange(string map, string sprite = "⌂", string impass = "f", string changeType = "SameTile")
			{
				this.Sprite = Convert.ToChar(sprite);
				this.Impassable = impass.Contains("t");
				//this._toMap = new Map(map, 10, 10);
				this._mapName = map;
				this._changeType = (SpecificSpot)Enum.Parse(typeof(SpecificSpot), changeType, ignoreCase: false);
			}
			public MapChange(string map, string sprite = "⌂", string impass = "f")
			{
				this.Sprite = Convert.ToChar(sprite);
				this.Impassable = impass.Contains("t");
				//this._toMap = new Map(map, 10, 10);
				this._mapName = map;
			}

			public override TileAction OnEnter()
			{
				if (ChangeType == SpecificSpot.ToCoord)
					return TileAction.ToMapWCoords;
				else if(ChangeType == SpecificSpot.Directional)
					return TileAction.ToMapDirection;
				else
					return TileAction.ToMap;
				
			}

			 
		}

		//class MapList
		//{
		//	public Dictionary<string, Map> List = new Dictionary<string, Map>();
		//	private MapList()
		//	{

		//	}

		//	public bool MapListContains(string mapName)
		//	{
		//		return List.ContainsKey(mapName);
		//	}
		//}
	}
}
