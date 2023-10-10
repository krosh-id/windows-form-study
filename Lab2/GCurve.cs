using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;


namespace Lab2
{
	[Serializable()]
	public class GCurve : AbstractFigure //curve line
	{
		List<Point> curve = new List<Point>();
		public override Point firstPoint
		{
			get { return base.firstPoint; }
			set
			{
				base.firstPoint = value;
				curve.Add(value);
			}
		}
		public override Point secondPoint
		{
			get { return base.secondPoint; }
			set
			{
				base.secondPoint = value;
				curve.Add(value);
			}
		}
		public override void drawFrame(ref Graphics g)
		{
			Pen p = new Pen(frameColor);
			p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
			g.DrawCurve(p, curve.ToArray<Point>());
			p.Dispose();
		}
		public override void draw(ref Graphics g)
		{
			Pen p = new Pen(primaryColor, lWidth);
			g.DrawCurve(p, curve.ToArray<Point>());
			p.Dispose();
		}

		public override Rectangle getRectangle()
		{
			int l = 1000000000, t = 1000000000, r = -1, b = -1;
			foreach (Point p in curve)
			{
				l = Math.Min(l, p.X);
				t = Math.Min(t, p.Y);
				r = Math.Max(r, p.X);
				b = Math.Max(b, p.Y);
			}
			return Rectangle.FromLTRB(l, t, r, b);
		}

		public override void move(Point from, Point to)
		{
			int dx = to.X - from.X;
			int dy = to.Y - from.Y;
			for (int i = 0; i < curve.Count; i++)
			{
				Point p = curve[i];
				p.X += dx;
				p.Y += dy;
				curve[i] = p;
			}
		}
	}
}
