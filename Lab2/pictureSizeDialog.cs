using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Lab2
{	
	// форма для выбора размера нового окна
	public partial class pictureSizeDialog : Form
	{
		public int newHeight=10,newWidth=10;

		public pictureSizeDialog()
		{
			InitializeComponent();
		}

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			if(checkBox1.Checked)
			{
				textBox1.Enabled = true;
				textBox2.Enabled = true;
				label1.Enabled = true;
				label2.Enabled = true;
			}else{
				textBox1.Enabled = false;
				textBox2.Enabled = false;
				label1.Enabled = false;
				label2.Enabled = false;
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			//set size
			if(checkBox1.Checked)
			{
				try{
					newWidth = Int32.Parse(textBox1.Text);
				}catch(Exception){
					newWidth = 200; //set default
				}
				try{
					newHeight = Int32.Parse(textBox2.Text);
				}catch(Exception){
					newHeight = 200;
				}
			}else{
				if(radioButton1.Checked)
				{
					newWidth = 320;
					newHeight = 240;
				}else if(radioButton2.Checked)
				{
					newWidth = 640;
					newHeight = 480;
				}else{
					newWidth = 800;
					newHeight = 600;
				}
			}
		}

		private void textBox1_TextChanged(object sender, EventArgs e)
		{
			try{
				Int32.Parse(textBox1.Text);
			}catch(Exception){
				MessageBox.Show("Введите число.");
			}
		}

		private void textBox2_TextChanged(object sender, EventArgs e)
		{
			try{
				Int32.Parse(textBox2.Text);
			}catch(Exception){
				MessageBox.Show("Введите число.");
			}
		}

		private void pictureSizeDialog_Load(object sender, EventArgs e)
		{

		}
	}
}
