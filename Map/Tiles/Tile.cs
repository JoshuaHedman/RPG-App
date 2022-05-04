using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RPG_App.Map.MapEngine;

namespace RPG_App.Map.Tiles
{
	class Tile
	{
		private char _sprite = '⚠';
		private bool _impassable = false;
		//readonly item itemPassable;		thing to add later

		public Tile(string sprite = "⚠", string impass = "f")
		{
			this.Sprite = Convert.ToChar(sprite);
			this.Impassable = impass.Contains("f");
		}

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
		private string _mapName;
		private SpecificSpot _changeType;
		private int _xCoord;
		private int _yCoord;
		public enum SpecificSpot { ToCoord, Directional, SameTile }
		

		public string MapName{	get { return _mapName; }	set { _mapName = value; }	}
		public SpecificSpot ChangeType	{	get { return _changeType; }	}
		public int xCoord	{	get { return _xCoord; }	}
		public int yCoord	{	get { return _yCoord; }	}

		public MapChange(string map, string sprite = "⌂", string impass = "f", string changeType = "SameTile", string Coords = "")
		{
			this.Sprite = Convert.ToChar(sprite);
			this.Impassable = impass.Contains("t");

			this._mapName = map;
			this._changeType = (SpecificSpot)Enum.Parse(typeof(SpecificSpot), changeType, ignoreCase: false);
			string[] coords = Coords.Split('x');
			this._xCoord = Convert.ToInt32(coords[0]);
			this._yCoord = Convert.ToInt32(coords[1]);
		}
		public MapChange(string map, string sprite = "⌂", string impass = "f", string changeType = "SameTile")
		{
			this.Sprite = Convert.ToChar(sprite);
			this.Impassable = impass.Contains("t");
			
			this._mapName = map;
			this._changeType = (SpecificSpot)Enum.Parse(typeof(SpecificSpot), changeType, ignoreCase: false);
		}
		public MapChange(string map, string sprite = "⌂", string impass = "f")
		{
			this.Sprite = Convert.ToChar(sprite);
			this.Impassable = impass.Contains("t");
			
			this._mapName = map;
		}

		public override TileAction OnEnter()
		{
			if (ChangeType == SpecificSpot.ToCoord)
				return TileAction.ToMapWCoords;
			else if (ChangeType == SpecificSpot.Directional)
				return TileAction.ToMapDirection;
			else
				return TileAction.ToMap;

		}


	}

}
