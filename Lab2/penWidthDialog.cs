using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Lab2
{	//форма для выбора ширны пера 
	public partial class penWidthDialog : Form
	{

		public int Val = 1;

		public penWidthDialog()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if(comboBox1.SelectedItem==null)
				Val = 1;
			else
				Val = Int32.Parse((string)comboBox1.SelectedItem);
		}
	}
}
