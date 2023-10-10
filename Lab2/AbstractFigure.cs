using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Lab2
{
	[Serializable()]
	public abstract class AbstractFigure //базовый класс для всех фигур
	{
		//Properties
		public virtual Point firstPoint
		{
			set { p1 = value; }
			get { return p1; }
		}
		public virtual Point secondPoint
		{
			set { p2 = value; }
			get { return p2; }
		}
		public int lineWidth
		{
			set
			{
				if (value <= 0)
					lWidth = 1;
				else
					lWidth = value;
			}
			get { return lWidth; }
		}
		//Fileds
		protected Point p1, p2; //position
		protected int lWidth; //drawing line width
		protected Color primaryColor, secondaryColor, frameColor;
		public bool fill;
		//Methods
		public void loadColors(Color pc, Color sc, Color fc)
		{
			primaryColor = pc;
			secondaryColor = sc;
			frameColor = fc;
		}
		public abstract void draw(ref Graphics g);
		public abstract void drawFrame(ref Graphics g);

		public abstract Rectangle getRectangle();
	}
}
