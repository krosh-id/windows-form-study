using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Lab2
{
	static class Program
	{
		/// <summary>
		/// Главная точка входа для приложения.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Form1());
		}
		public static void helpMe()
		{
			MessageBox.Show("Hello!\nLab C# by:\nPavlovich VladisLove\n(c) 2023");
		}

		public static void m1(Form2 f2)
		{
			int sqs = 50;
			Random rnd = new Random();
			int x = f2.pictWidth / sqs + (f2.pictWidth % sqs != 0 ? 1 : 0);
			int y = f2.pictHeight / sqs + (f2.pictHeight % sqs != 0 ? 1 : 0);
			for (int i = 0; i < x; i++)
				for (int j = 0; j < y; j++)
				{
					AbstractFigure af = new GRectangle();
					af.fill = true;
					af.loadColors(System.Drawing.Color.Black, System.Drawing.Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256)), System.Drawing.Color.Red);
					af.firstPoint = new System.Drawing.Point(i * sqs, j * sqs);
					af.secondPoint = new System.Drawing.Point(i * sqs + sqs, j * sqs + sqs);
					f2.fstorage.Add(af);
				}
			f2.redrawAll();
		}
	}
}
