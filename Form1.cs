using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RPG_App.Combat;


namespace RPG_App
{
	public partial class RPG_Form : Form
	{
		//public Dictionary<string, Map> MapList = new Dictionary<string, Map>();
		//public Map currenMap;
		MapEngine mapEngine;
		CombatEngine combatEngine;
		enum active{ Map, Combat}
		active Active = active.Combat;
		
		public RPG_Form()
		{
			InitializeComponent();
			//Console.OutputEncoding = System.Text.Encoding.UTF8;
			this.KeyPress += new KeyPressEventHandler(RPG_Form_KeyPress);
			//currenMap = new Map("OverWorld.map", 13, 13);
			combatEngine = new CombatEngine();
			mapEngine = new MapEngine();
			OutputTxt.ForeColor = Color.Black;
			OutputTxt.Text = mapEngine.MapOutput();
			//OutputTxt.Text = currenMap.MapString();
		}

		private void RPG_Form_Load(object sender, EventArgs e)
		{
			this.KeyPreview = true;
			
		}

		private void RPG_Form_KeyPress(object sender, KeyPressEventArgs e)
		{
			if(true)
			//if (e.KeyChar >= 48 && e.KeyChar <= 57)//0x30
			{
				//MessageBox.Show("Form.KeyPress: '" + e.KeyChar.ToString() + "' pressed.");

				switch (e.KeyChar)//(int)e.KeyChar)
				{
					case 'w':
					case 'a':
					case 's':
					case 'd':
					case ' ':
						//currenMap.MovePlayer(e.KeyChar);
						//OutputTxt.Text = currenMap.MapString();
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
			}
		}


		//*/
		private void OutputTxt_TextChanged(object sender, EventArgs e)
		{
			//OutputTxt.Text = "working";
		}

		

	}
}
