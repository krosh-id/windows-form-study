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
    public partial class SpecialForm : Form
    {
		//ПЕРЕМЕННЫЕ
		//======================================================================================

		public string fileName; //имя файла где сохранена картина
		public bool fromFile = false;  // открыт из файла или нет
		bool paintAction = false; //происходит рисование фигур или нет 
		bool changedCanvas = false; //служит для определения внесённых изменений ы
		Point start, finish;
		List<AbstractFigure> fstorage = new List<AbstractFigure>();
		AbstractFigure toPaint;
		//painting stuff дефолтные значения при рисовании
		Bitmap canvas = new Bitmap(10, 10); //новый экземпляр класса Bitmap с заданным размером.
		public Color backColor = Color.White;       //color of background
		public Color frameColor = Color.Black;      //color of temporary figure
		public Color primaryColor = Color.Black;    //lines color
		public Color secondaryColor = Color.Black;    //fill color
		public int lineWidth = 2;
		public bool solidFill = false; //флаг заливки
		public int figureID = 4; 
		public int pictWidth = 1, pictHeight = 1;
		public Font textFont = new Font("Times New Roman", 12); //шрифт текста 

		//ПРОЦЕСС РИСОВАНИЯ
		//======================================================================================

		//отрисовка с битмапом
		public void drawCanvas()
		{
			canvas.Dispose();                          //уничтожение старого bitmap...
			canvas = new Bitmap(pictWidth, pictHeight); //...и создание нового
			Graphics g = Graphics.FromImage(canvas);
			g.Clear(backColor);
			foreach (AbstractFigure go in fstorage) //отрисовывает все фигуры из массива
				go.draw(ref g);
			if (paintAction)
				toPaint.drawFrame(ref g); //рисование новой фигуры ( временной)
			g.Dispose();
		}

		private void initPainter()
		{
			//устанавливает тип объекта рисования
			switch (figureID)
			{
				case 0: toPaint = new GLine(); break;
				case 1: toPaint = new GCurve(); break;
				case 2: toPaint = new GRectangle(); break;
				case 3: toPaint = new GEllipse(); break;
				case 4: //новая фигура в айди
					toPaint = new GTextLabel();
					((GTextLabel)toPaint).tFont = textFont;
					((GTextLabel)toPaint).tbParent = this;
					break;
				default: toPaint = new GLine(); break; //if figure is unknown - set as line
			}

			//устанавливает свойства для новой фигуры
			toPaint.loadColors(primaryColor, secondaryColor, frameColor);
			toPaint.firstPoint = start;
			toPaint.secondPoint = start;
			toPaint.lineWidth = lineWidth;
			toPaint.fill = solidFill;
		}

		//I/O
		//======================================================================================

		public void SaveFile(string name)
		{
			BinaryFormatter formatter = new BinaryFormatter();
			Stream stream = new FileStream(name, FileMode.Create, FileAccess.Write, FileShare.None);
			formatter.Serialize(stream, pictWidth);
			formatter.Serialize(stream, pictHeight);
			formatter.Serialize(stream, backColor); //save background color
			formatter.Serialize(stream, fstorage);  //save figure storage
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
			drawCanvas();
			Refresh();
		}

		//FORM APPEARANCE
		//======================================================================================

		public SpecialForm()
		{
			InitializeComponent();
		}

		private void SpecialForm_Shown(object sender, EventArgs e)
		{
			canvas = new Bitmap(pictWidth, pictHeight);
			Graphics.FromImage(canvas).Clear(backColor);
		}

		//вывод размера окна для активной формы
		private void SpecialForm_Activated(object sender, EventArgs e)
		{
			((Form1)this.ParentForm).setWindowSizeCaption(pictWidth, pictHeight); //изменение в статус баре
			this.drawCanvas();
			this.Refresh();
		}

		//СОБЫТИЯ МЫШИ
		//======================================================================================

		private void SpecialForm_MouseDown(object sender, MouseEventArgs e)
		{ //начало рисования
			int eX = e.X - AutoScrollPosition.X;
			int eY = e.Y - AutoScrollPosition.Y;
			if (e.Button == MouseButtons.Left && eX <= pictWidth && eY <= pictHeight)
			{
				start.X = eX;
				start.Y = eY;
				finish = start;
				initPainter();
				paintAction = true;
			}
		}

		private void SpecialForm_MouseMove(object sender, MouseEventArgs e)
		{ //процесс рисования
			int eX = e.X - AutoScrollPosition.X;
			int eY = e.Y - AutoScrollPosition.Y;
			if (paintAction)
			{
				finish.X = eX;
				finish.Y = eY;
				toPaint.secondPoint = finish; //сохранение текущей конечной точки
				drawCanvas(); //перерисовка битмапа
				Refresh();
			}
			((Form1)this.ParentForm).setMousePositionCaption(eX, eY);
		}

		private void SpecialForm_MouseUp(object sender, MouseEventArgs e)
		{ //paint is finished
			int eX = e.X - AutoScrollPosition.X;
			int eY = e.Y - AutoScrollPosition.Y;
			if (paintAction)
			{
				paintAction = false;
				finish.X = eX;
				finish.Y = eY;
				toPaint.secondPoint = finish;
				changedCanvas = true; //если что то нарисовано
				if (eX <= pictWidth && eY <= pictHeight && eX >= 0 && eY >= 0) //проверяет чтобы фигура не выходила из битмапа canvas
					fstorage.Add(toPaint); //добавляет новую фигуру к массиву
				drawCanvas(); //перерисовка
				Refresh();
			}
		}

		//когда курсор выходит за границу формы 
		private void SpecialForm_MouseLeave(object sender, EventArgs e)
		{
			((Form1)this.ParentForm).setMousePositionCaption(-1, -1);
		}

		//Перерисовка формы
		//======================================================================================

		//	Происходит при перерисовке элемента 
		private void SpecialForm_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.Clear(Color.LightGray);
			e.Graphics.DrawImage(canvas, AutoScrollPosition.X, AutoScrollPosition.Y); //только рисует изображение
																					  //Рисует заданный объект Image в заданном месте, используя указанный размер.
		}

		//Происходит при изменении размеров 
		private void SpecialForm_Resize(object sender, EventArgs e)
		{
			drawCanvas();
			Refresh();
		}



        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void saveButton_Click(object sender, EventArgs e)
        {
			saveFileDialog1.InitialDirectory = Environment.CurrentDirectory;
			if (saveFileDialog1.ShowDialog() == DialogResult.OK)
			{
				SaveFile(saveFileDialog1.FileName);
				fileName = saveFileDialog1.FileName;
				fromFile = true;
				Text = saveFileDialog1.FileName; //set text
			}
		}

        private void openButton_Click(object sender, EventArgs e)
        {
			if (openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				//newWindow(); //создание нового окна
				LoadFile(openFileDialog1.FileName);
				fromFile = true;
				fileName = openFileDialog1.FileName;
				Text = openFileDialog1.FileName; //установка текста в заголовке документа
			}
		}



        //ЗАКРЫТИЕ ФОРМЫЫ
        //======================================================================================

        private void SpecialForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (this.ParentForm.MdiChildren.Length == 1)
			{
				((Form1)this.ParentForm).deactivateMenu(); //при закрытии последнее дочернего элемента 
				((Form1)this.ParentForm).setMousePositionCaption(-1, -1);
				((Form1)this.ParentForm).setWindowSizeCaption(-1, -1);
			}
		}

		private void SpecialForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (changedCanvas) //спрашивает только тогда, если что-то было изменено
				switch (MessageBox.Show("Сохранить изменения в \"" + this.Text + "\"?", "Вопрос", MessageBoxButtons.YesNoCancel))
				{
					case DialogResult.Yes:
						//((Form1)this.ParentForm).saveFile();
						break;
					case DialogResult.Cancel:
						e.Cancel = true;
						break;
				}

		}
	}
}
