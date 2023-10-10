using System;
using System.Drawing;


namespace Lab2
{
	[Serializable()]
	public class GRectangle : AbstractFigure //rectangle
	{
		public override void drawFrame(ref Graphics g)
		{
			Pen p = new Pen(frameColor);
			p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
			g.DrawRectangle(p, getRectangle());
			p.Dispose();
		}
		public override void draw(ref Graphics g)
		{
			Pen p = new Pen(primaryColor, lWidth);
			
			if (fill)
				g.FillRectangle(new SolidBrush(secondaryColor), getRectangle());
			g.DrawRectangle(p, getRectangle());
			p.Dispose();
		}

		public override Rectangle getRectangle()
		{
			int x, y, w, h;
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
			return new Rectangle(x, y, w, h);
		}
	}
}
