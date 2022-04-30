using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RPG_App.Map;
using RPG_App.Combat;
using RPG_App.Map.Tiles;

namespace RPG_App
{
	public partial class RPG_Form : Form
	{
		//public Dictionary<string, Map> MapList = new Dictionary<string, Map>();
		//public Map currenMap;
		MapEngine mapEngine;
		CombatEngine combatEngine;
		enum active{ Map, Combat}
		private active Active = active.Map;
		bool switchEngine = false;
		public void switchActiveMode()
		{
			if (Active == active.Map)
				Active = active.Combat;
			else if (Active == active.Combat)
				Active = active.Map;
			switchEngine = true;
		}
		public RPG_Form()
		{
			InitializeComponent();
			//Console.OutputEncoding = System.Text.Encoding.UTF8;
			this.KeyPress += new KeyPressEventHandler(RPG_Form_KeyPress);
			combatEngine = new CombatEngine();
			mapEngine = new MapEngine();
			mapEngine.hookUpMap(this, combatEngine);
			combatEngine.hookUpCombat(this, mapEngine);
			OutputTxt.ForeColor = Color.Black;
			OutputTxt.Text = "WASD to move. In combat, spacebar to select and Q to go back. DEBUG F forces encouter\r\n" + mapEngine.MapOutput();
			//OutputTxt.Text += "\r\nWASD to move. In combat, spacebar to select and Q to go back
		}

		private void RPG_Form_Load(object sender, EventArgs e)
		{
			this.KeyPreview = true;
			
		}

		private void RPG_Form_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (!switchEngine)
			//if (e.KeyChar >= 48 && e.KeyChar <= 57)//0x30
			{

				switch (e.KeyChar)
				{
					case 'w':
					case 'a':
					case 's':
					case 'd':
					case ' ':
					case 'q':
					case 'f':
						if (Active == active.Map)
							OutputTxt.Text = mapEngine.Mapstep(e.KeyChar);
						else if (Active == active.Combat)
							OutputTxt.Text = combatEngine.CombatStep(e.KeyChar);
						break;
					case '4':
						break;
					case '5':
						break;
					case '6':
						break;
					case '7':
						break;
					case '8':
						break;
					case '9':

						break;
					default:
						//MessageBox.Show("Form.KeyPress: '" + e.KeyChar.ToString() + "' consumed.");
						e.Handled = true;
						break;
				}
			} else
			{
				switchEngine = false;
				if (Active == active.Combat)
				{
					OutputTxt.Text = combatEngine.CombatOutput("");

				}else if (Active == active.Map)
				{
					OutputTxt.Text = mapEngine.MapOutput();
					
				}
			}
		}


		//*/
		private void OutputTxt_TextChanged(object sender, EventArgs e)
		{
			//OutputTxt.Text = "working";
		}

		

	}
}
