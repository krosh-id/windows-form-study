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
	public class GLine : AbstractFigure //line
	{
		public override void drawFrame(ref Graphics g)
		{
			Pen p = new Pen(frameColor);
			p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
			g.DrawLine(p, p1, p2);
			p.Dispose();
		}
		public override void draw(ref Graphics g)
		{
			Pen p = new Pen(primaryColor, lWidth);
			g.DrawLine(p, p1, p2);
			p.Dispose();
		}
	}
}
