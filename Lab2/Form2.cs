﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Drawing.Imaging;

namespace Lab2
{
	public partial class Form2 : Form
	{
		//ПЕРЕМЕННЫЕ
		//======================================================================================

		public string fileName; //имя файла где сохранена картина
		public bool fromFile = false;  // открыт из файла или нет
		bool paintAction = false; //происходит рисование фигур или нет
		bool selectAction = false;    //выбор какой-нибудь фигуры
		bool dragAction = false;      //перемещение выбранной фигуры в новое место
		bool changedCanvas = false; //служит для определения внесённых изменений ы
		Point start,finish;
		public List<AbstractFigure> fstorage = new List<AbstractFigure>();
		AbstractFigure toPaint;
		//painting stuff дефолтные значения при рисовании
		Bitmap canvas = new Bitmap(10,10); //новый экземпляр класса Bitmap с заданным размером.
		public Color backColor = Color.Black;		//color of background
		public Color frameColor = Color.Black;		//color of temporary figure
		public Color primaryColor = Color.Black;    //lines color
		public Color secondaryColor = Color.Black;    //fill color
		public int lineWidth = 2;
		public bool solidFill = false; //флаг заливки
		public int figureID = 0; //линия по дефолту 
		public int pictWidth=1,pictHeight=1;
		public Font textFont; //шрифт текста
		public bool selection = false; //флаг выбора

		//ПРОЦЕСС РИСОВАНИЯ
		//======================================================================================

		//отрисовка с битмапом
		public void drawCanvas()
		{
			canvas.Dispose();						   //уничтожение старого bitmap...
			canvas = new Bitmap(pictWidth,pictHeight); //...и создание нового
			Graphics g = Graphics.FromImage(canvas);
			g.Clear(backColor);
			foreach(AbstractFigure go in fstorage) //отрисовывает все фигуры из массива
			{
				go.draw(ref g);
				if (go.selected)
				{
					go.drawSelection(ref g); //нарисовать обведенный прямоугольник (в виде рамки)
					if (dragAction)
						go.drawDragged(ref g, start, finish); //построить кадр в вероятной точке назначения
				}
			}
			if (paintAction || selectAction)
				toPaint.drawFrame(ref g); //рисование новой фигуры ( временной)
			g.Dispose();
		}

		private void initPainter()
		{   
			//устанавливает тип объекта рисования
			switch(figureID)
			{
				case 0:	toPaint = new GLine(); break;
				case 1: toPaint = new GCurve(); break;
				case 2: toPaint = new GRectangle(); break;
				case 3: toPaint = new GEllipse(); break;
				case 4: //новая фигура в айди
					toPaint = new GTextLabel();
					((GTextLabel)toPaint).tFont = textFont;
					//((GTextLabel)toPaint).tbParent = this;
					break;
				default: toPaint = new GLine(); break; //if figure is unknown - set as line
			}
		
			//устанавливает свойства для новой фигуры
			toPaint.loadColors(primaryColor,secondaryColor,frameColor);
			toPaint.firstPoint = start;
			toPaint.secondPoint = start;
			toPaint.lineWidth = lineWidth;
			toPaint.fill = solidFill;
		}

		public void deleteSelected()
		{
			for (int i = 0; i < fstorage.Count; i++)
				if (fstorage[i].selected)
					fstorage.RemoveAt(i--); //удаляет выбранную фигуру
			((Form1)this.MdiParent).clipboardToolsMenu(false); //все выделенное было удалено - нечего вырезать или копировать
			redrawAll();
		}

		private void selectFigures()
		{
			dropSelection(); //отмена предыдущего выбора
			Rectangle trect = toPaint.getRectangle(); //выбранная область
			for (int i = 0; i < fstorage.Count; i++)
				if (trect.IntersectsWith(fstorage[i].getRectangle()))
					fstorage[i].selected = true; //помечает фигуру как выделенную, если ее прямоугольник пересекается с областью выделения
			parentClipboardInterface(); //обновление интерфейса родительской формы
			redrawAll();

        }

		public void dropSelection()
		{
			for (int i = 0; i < fstorage.Count; i++)
				fstorage[i].selected = false; //помечает все фигура как не выбранные
			((Form1)this.MdiParent).clipboardToolsMenu(false); //отключение
			redrawAll();
		}

		public bool isInsideOfRectangle(Rectangle rect, Point p)
		{ //проверить, что точка p лежит внутри прямоугольника rect
			return ((p.X >= rect.Left) && (p.X <= rect.Left + rect.Width) && (p.Y >= rect.Top) && (p.Y <= rect.Top + rect.Height));
		}

		public bool isInside(Rectangle sm, Rectangle lg)
		{ //если прямоугольник sm лежит внутри прямоугольника lg
			return (isInsideOfRectangle(lg, new Point(sm.Left, sm.Top)) && isInsideOfRectangle(lg, new Point(sm.Left + sm.Width, sm.Top + sm.Height)));
		}

		public void selectSingleFigure(Point p)
		{
			bool inside = false;
			for (int i = fstorage.Count - 1; i >= 0; i--)
				if (isInsideOfRectangle(fstorage[i].getRectangle(), p))
				{ //найти самую верхнюю фигуру, содержащую точку p
					fstorage[i].selected = true;
					inside = true;
					break;
				}
			if (!inside)
				dropSelection(); //точка p не принадлежит ни одной фигуре
			parentClipboardInterface();
		}

		//перемещение выбранной фигуры
		public void moveSelectedFigures(Point f, Point s)
		{
			for (int i = 0; i < fstorage.Count; i++)
				if (fstorage[i].selected)
					fstorage[i].move(f, s);
			redrawAll();
		}

		public void redrawAll()
		{
			drawCanvas();
			Refresh();
		}
		//I/O
		//======================================================================================

		public void SaveFile(string name)
		{
			BinaryFormatter formatter = new BinaryFormatter();
			Stream stream = new FileStream(name,FileMode.Create,FileAccess.Write,FileShare.None);
			formatter.Serialize(stream,pictWidth);
			formatter.Serialize(stream,pictHeight);
			formatter.Serialize(stream,backColor); //save background color
			formatter.Serialize(stream,fstorage);  //save figure storage
			stream.Close();
			changedCanvas = false;
		}

		//загрузка файла и отрисовка в битмап
		public void LoadFile(string name)
		{
			BinaryFormatter formatter = new BinaryFormatter();
			Stream stream = new FileStream(name, FileMode.Open, FileAccess.Read, FileShare.Read);
			pictWidth = (int)formatter.Deserialize(stream);
			pictHeight = (int)formatter.Deserialize(stream);
			backColor = (Color)formatter.Deserialize(stream);
			fstorage = (List<AbstractFigure>)formatter.Deserialize(stream);
			stream.Close();
			//repaint
			redrawAll();
		}

		//CLIPBOARD
		//======================================================================================

		public void selectAll()
		{
			selection = true;
			for (int i = 0; i < fstorage.Count; i++)
				fstorage[i].selected = true;
			parentClipboardInterface();
			redrawAll();
		}

		public void parentClipboardInterface()
		{
			bool anySelected = false;
			foreach (AbstractFigure af in fstorage)
				if (af.selected)
				{
					anySelected = true;
					break;
				}
			((Form1)this.MdiParent).clipboardToolsMenu(anySelected);
		}

		public void copySelected()
		{ //any figures is already selected
			List<AbstractFigure> toc = new List<AbstractFigure>();
			foreach (AbstractFigure af in fstorage)
				if (af.selected)
					toc.Add(af); //add selected figure to list
								 //save serialized list
			BinaryFormatter formatter = new BinaryFormatter();
			MemoryStream ms = new MemoryStream();
			formatter.Serialize(ms, toc);
			Clipboard.SetDataObject(ms, true);
			ms.Close();
			checkClipboard();
		}

		public void copyMetafile()
		{
			List<AbstractFigure> toc = new List<AbstractFigure>();
			int minx = Int32.MaxValue, miny = Int32.MaxValue;
			int maxx = 0, maxy = 0;
			foreach (AbstractFigure af in fstorage)
				if (af.selected) //copy selected figures to new List
				{
					toc.Add(af);
					minx = Math.Min(minx, af.getRectangle().Left);
					miny = Math.Min(miny, af.getRectangle().Top);
					maxx = Math.Max(maxx, af.getRectangle().Right);
					maxy = Math.Max(maxy, af.getRectangle().Bottom);
				}
			//create new metafile
			Graphics g = Graphics.FromImage(new Bitmap(maxx, maxy));
			IntPtr dc = g.GetHdc();
			Metafile mf = new Metafile(dc, EmfType.EmfOnly);
			g.ReleaseHdc(dc);
			g.Dispose();
			//draw selected figures on metafile
			Graphics gr = Graphics.FromImage(mf);
			foreach (AbstractFigure af in toc)
			{
				Rectangle rect = af.getRectangle();
				Point from = new Point(rect.Left, rect.Top);
				Point to = new Point(rect.Left - minx, rect.Top - miny);
				af.move(from, to); //paint selected figures at graphics
				af.draw(ref gr);
				af.move(to, from);
			}
			gr.Dispose();
			ClipboardMetafileHelper.PutEnhMetafileOnClipboard(this.Handle, mf); //copy metafile to clipboard	;
		}

		public void cutSelected()
		{
			copySelected();
			deleteSelected();
		}

		public void pasteData()
		{
			dropSelection();
			//get data from clipboard
			MemoryStream ms = (MemoryStream)Clipboard.GetDataObject().GetData(typeof(MemoryStream));
			BinaryFormatter formatter = new BinaryFormatter();
			List<AbstractFigure> toc = (List<AbstractFigure>)formatter.Deserialize(ms);
			ms.Close();
			//check data to insert to fit in picture size
			int minx = Int32.MaxValue, miny = Int32.MaxValue;
			foreach (AbstractFigure af in toc)
			{
				Rectangle rect = af.getRectangle();
				if (rect.Width > pictWidth || rect.Height > pictHeight)
				{
					MessageBox.Show("Изображение слишком большое!");
					return;
				}
				minx = Math.Min(minx, rect.Left);
				miny = Math.Min(miny, rect.Top);
			}
			//paste from clipboard
			foreach (AbstractFigure af in toc)
			{
				Rectangle rect = af.getRectangle();
				af.move(new Point(rect.Left, rect.Top), new Point(rect.Left - minx, rect.Top - miny)); //convert coordinates
				af.selected = true;
				fstorage.Add(af);
			}
			//switch selection mode on
			((Form1)MdiParent).setSelectionMode(true);
			parentClipboardInterface();
			redrawAll();
		}

		public void checkClipboard()
		{
			IDataObject data = Clipboard.GetDataObject();
			if (data.GetDataPresent(typeof(MemoryStream)))
				((Form1)MdiParent).pasteMenu(true); //clipboard is containing something that program can use
			else
				((Form1)MdiParent).pasteMenu(false);
		}
		//FORM APPEARANCE
		//======================================================================================

		public Form2()
		{
			InitializeComponent();
		}

		private void Form2_Shown(object sender, EventArgs e)
		{
			canvas = new Bitmap(pictWidth,pictHeight);
			Graphics.FromImage(canvas).Clear(backColor);
		}

		//вывод размера окна для активной формы
		private void Form2_Activated(object sender, EventArgs e)
		{
			((Form1)this.ParentForm).setWindowSizeCaption(pictWidth,pictHeight); //изменение в статус баре
			parentClipboardInterface();
			redrawAll();
		}

		//СОБЫТИЯ МЫШИ
		//======================================================================================

		private void Form2_MouseDown(object sender, MouseEventArgs e)
		{ //начало рисования
			int eX = e.X - AutoScrollPosition.X;
			int eY = e.Y - AutoScrollPosition.Y;
			if (eX <= pictWidth && eY <= pictHeight) //если внутри канваса
			{
				if (e.Button == MouseButtons.Left && !selection)
				{ //рисование
					dropSelection();
					start.X = eX;
					start.Y = eY;
					finish = start;
					initPainter();
					paintAction = true;
				}
				else
				if (e.Button == MouseButtons.Left && selection)
				{
					start.X = eX;
					start.Y = eY;
					//check start point for belonging to any selected fugure
					foreach (AbstractFigure af in fstorage)
						if (af.selected && isInsideOfRectangle(af.getRectangle(), start))
						{
							dragAction = true; //подготовка к перетаскиванию
							break;
						}
					if (!dragAction)
					{
						////подготовка к выбору
						toPaint = new GRectangle();
						toPaint.loadColors(primaryColor, secondaryColor, frameColor);
						toPaint.firstPoint = new Point(eX, eY);
						toPaint.secondPoint = new Point(eX, eY);
						selectAction = true;
					}
				}
			}
		}

		private void Form2_MouseMove(object sender, MouseEventArgs e)
		{ //процесс рисования
			int eX = e.X - AutoScrollPosition.X;
			int eY = e.Y - AutoScrollPosition.Y;
			if(paintAction || selectAction)
			{
				finish.X = eX;
				finish.Y = eY;
				toPaint.secondPoint = finish; //сохранение текущей конечной точки
				drawCanvas(); //перерисовка битмапа
				Refresh();
			}
			if (selectAction)
				selectFigures();
			if (dragAction)
			{
				finish.X = eX;
				finish.Y = eY;
				drawCanvas();
				Refresh();
			}
			((Form1)this.ParentForm).setMousePositionCaption(eX,eY);
		}

		private void Form2_MouseUp(object sender, MouseEventArgs e)
		{ //рисование закончено
			int eX = e.X - AutoScrollPosition.X;
			int eY = e.Y - AutoScrollPosition.Y;
			finish.X = eX;
			finish.Y = eY;
			if (paintAction)
			{
				paintAction = false;
				toPaint.secondPoint = finish;
				changedCanvas = true; //если что то нарисовано
				if (isInside(toPaint.getRectangle(), new Rectangle(0, 0, pictWidth, pictHeight))) //проверяет чтобы фигура не выходила из битмапа canvas
					fstorage.Add(toPaint); //добавляет новую фигуру к массиву
			}//no "else" - only one action is allowed in one time
			if (selectAction)
			{
				selectAction = false;
				start = toPaint.firstPoint;
				if (Math.Abs(start.X - finish.X) < 3 && Math.Abs(start.Y - finish.Y) < 3)
					selectSingleFigure(toPaint.firstPoint);
			}
			if (dragAction)
			{
				//проверяет новую позицию
				bool OK = true;
				int dx = finish.X - start.X;
				int dy = finish.Y - start.Y;
				foreach (AbstractFigure af in fstorage)
				{
					Rectangle rect = af.getRectangle();
					if (af.selected)
						if (!isInside(new Rectangle(rect.Left + dx, rect.Top + dy, rect.Width, rect.Height), new Rectangle(0, 0, pictWidth, pictHeight)))
						{
							OK = false;
							break;
						}
				}
				if (OK)
					moveSelectedFigures(start, finish);
				changedCanvas = true;
				dragAction = false;
			}
			redrawAll();
		}

		//когда курсор выходит за границу формы 
		private void Form2_MouseLeave(object sender, EventArgs e)
		{
			((Form1)this.ParentForm).setMousePositionCaption(-1,-1);
		}

		//Перерисовка формы
		//======================================================================================

		//	Происходит при перерисовке элемента 
		private void Form2_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.Clear(Color.LightGray);
			e.Graphics.DrawImage(canvas,AutoScrollPosition.X,AutoScrollPosition.Y); //только рисует изображение
			//Рисует заданный объект Image в заданном месте, используя указанный размер.
		}

		//Происходит при изменении размеров 
		private void Form2_Resize(object sender, EventArgs e)
		{
			redrawAll();
		}

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        //ЗАКРЫТИЕ ФОРМЫЫ
        //======================================================================================

        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
		{
			if(this.ParentForm.MdiChildren.Length==1)
			{
				((Form1)this.ParentForm).fileOperationsMenu(false); //при закрытии последнее дочернего элемента 
				((Form1)this.ParentForm).setMousePositionCaption(-1,-1);
				((Form1)this.ParentForm).setWindowSizeCaption(-1,-1);
				((Form1)this.ParentForm).clipboardToolsMenu(false); //nothing to copy
				((Form1)this.ParentForm).pasteMenu(false); //nowhere to paste
			}
		}

		private void Form2_FormClosing(object sender, FormClosingEventArgs e)
		{
			if(changedCanvas) //спрашивает только тогда, если что-то было изменено
				switch(MessageBox.Show("Сохранить изменения в \""+this.Text+"\"?","Вопрос",MessageBoxButtons.YesNoCancel))
				{
					case DialogResult.Yes:
						((Form1)this.ParentForm).saveFile();
					break;
					case DialogResult.Cancel:
						e.Cancel = true;
					break;
				}
				
		}
		private void timer1_Tick(object sender, EventArgs e)
		{
			checkClipboard();
		}
	}
}
