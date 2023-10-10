using System;
using System.Drawing;


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
		[NonSerialized()] public bool selected;
		//Methods
		public void loadColors(Color pc, Color sc, Color fc)
		{
			primaryColor = pc;
			secondaryColor = sc;
			frameColor = fc;
		}
		public void drawSelection(ref Graphics g)
		{
			Pen p = new Pen(frameColor);
			p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
			g.DrawRectangle(p, getRectangle());
			p.Dispose();
		}
		public void drawDragged(ref Graphics g, Point from, Point to)
		{
			move(from, to);
			drawFrame(ref g);
			move(to, from);
		}
		public virtual void move(Point from, Point to)
		{
			int dx = to.X - from.X;
			int dy = to.Y - from.Y;
			p1.X += dx;
			p1.Y += dy;
			p2.X += dx;
			p2.Y += dy;
		}

		public abstract void draw(ref Graphics g);
		public abstract void drawFrame(ref Graphics g);
		public abstract Rectangle getRectangle();
	}
}
