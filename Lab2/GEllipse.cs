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
	public class GEllipse : AbstractFigure //ellipse
	{
		[NonSerialized()] int x, y, w, h;
		void transform()
		{
			if (p2.X > p1.X) //for X
			{
				x = p1.X;
				w = p2.X - p1.X;
			}
			else
			{
				x = p2.X;
				w = p1.X - p2.X;
			}
			if (p2.Y > p1.Y) //for Y
			{
				y = p1.Y;
				h = p2.Y - p1.Y;
			}
			else
			{
				y = p2.Y;
				h = p1.Y - p2.Y;
			}
		}
		public override void drawFrame(ref Graphics g)
		{
			Pen p = new Pen(frameColor);
			p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
			transform();
			g.DrawEllipse(p, x, y, w, h);
			p.Dispose();
		}
		public override void draw(ref Graphics g)
		{
			Pen p = new Pen(primaryColor, lWidth);
			transform();
			if (fill)
				g.FillEllipse(new SolidBrush(secondaryColor), x, y, w, h);
			g.DrawEllipse(p, x, y, w, h);
			p.Dispose();
		}
	}
}
