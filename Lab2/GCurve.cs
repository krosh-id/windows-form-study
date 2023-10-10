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
	}
}
