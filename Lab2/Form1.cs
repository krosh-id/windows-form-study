using System;
using System.Drawing;
using System.Windows.Forms;

namespace Lab2
{
	public partial class Form1 : Form
	{
		public int wCount = 0; //количество созданных дочерних форм
		public int lineWidth = 1;   //дефолтная ширина pen
		public int pictHeight=600,pictWidth=800; //дефолтные размеры новосозданного окна
		public bool solidFill = false;
		public bool textMode = false;
		public bool selection = false; //флаг выбора
		public int figureID = 0;
		public Font textFont = new Font("Times New Roman", 12); //дефолтный текст 
        

        //создание нового окна(формы)
        public void newWindow()
		{
			Form2 f2 = new Form2(); //создание нового окна(формы)
			f2.MdiParent = this;
			//инициализация параметров рисования 
			f2.lineWidth = lineWidth;
			f2.primaryColor = primColorDialog.Color; 
			f2.secondaryColor = secondColorDialog.Color;
			
			f2.backColor = backColorDialog.Color; //изменить для холста 
			f2.solidFill = solidFill;
			f2.selection = selection;
			f2.figureID = figureID;
			f2.textFont = textFont; 
			//параметры формы
			f2.pictHeight = pictHeight;
			f2.pictWidth = pictWidth;
			f2.AutoScrollMinSize = new Size(pictWidth,pictHeight); //скроллинг
			f2.AutoScroll = true;
			f2.Text = "Рисунок "+(++wCount);
			f2.Show();
		}



		//Интерфейс
		//======================================================================================

		//включение элементов формы
		public void activateMenu()
		{
			saveToolStripMenuItem.Enabled = true;  //сохранить
			saveAsToolStripMenuItem.Enabled = true; // сохранить как
			toolStripButton3.Enabled = true; // кнопка сохранить в кнопочной панели
		}

		//отключение элементов формы
		public void deactivateMenu()
		{
			saveToolStripMenuItem.Enabled = false;
			saveAsToolStripMenuItem.Enabled = false;
			toolStripButton3.Enabled = false;
		}
		
		//отображение размера окна в статус баре
		public void setWindowSizeCaption(int w,int h)
		{
			if(w>=0 && h>=0)
				statusStrip1.Items[7].Text = "Размер рисунка:(" + w + "х" + h + ")";
			else
				statusStrip1.Items[7].Text = "Размер рисунка:(800x600)";
		}
        
		//отображение положения мыши в статус баре 
		public void setMousePositionCaption(int x,int y)
		{
			if(x>=0 && y>=0)
				
				statusStrip1.Items[8].Text = x + "," + y;
			else
				statusStrip1.Items[8].Text = " ";
		}

		public void statusMessage() //устанавливает шрифт в статус бар
		{
			if (textMode)
			{
				statusStrip1.Items[9].Text = textFont.Name + " " + textFont.Size + "pt";
			}
			else
				statusStrip1.Items[9].Text = " ";
		}

		//FILE I/O
		//======================================================================================

		//открытие нового файла
		public void openFile()
		{
			if(openFileDialog1.ShowDialog()==DialogResult.OK)
			{
				newWindow(); //создание нового окна
				((Form2)this.ActiveMdiChild).LoadFile(openFileDialog1.FileName);
				((Form2)this.ActiveMdiChild).fromFile = true;
				((Form2)this.ActiveMdiChild).fileName = openFileDialog1.FileName;
				this.ActiveMdiChild.Text = openFileDialog1.FileName; //установка текста в заголовке документа
			}
			activateMenu(); //активация кнопок 
		}

		//сохранение нового файла
		public void saveAsFile()
		{
			saveFileDialog1.InitialDirectory = Environment.CurrentDirectory;
			if(saveFileDialog1.ShowDialog()==DialogResult.OK)
			{
				((Form2)this.ActiveMdiChild).SaveFile(saveFileDialog1.FileName);
				((Form2)this.ActiveMdiChild).fileName = saveFileDialog1.FileName;
				((Form2)this.ActiveMdiChild).fromFile = true;
				this.ActiveMdiChild.Text = saveFileDialog1.FileName; //set text
			}
		}

		//сохранение существуещго файла
		public void saveFile()
		{
			if(((Form2)this.ActiveMdiChild).fromFile)
				((Form2)this.ActiveMdiChild).SaveFile(((Form2)this.ActiveMdiChild).fileName); //сохранение при вохде из файла
			else
				saveAsFile(); //сохранение нового файла
		}


		//Настройка картинки 
		//======================================================================================

		// установка ширины линии из формы penWidthDialog
		public void setPenWidth()
		{
			penWidthDialog pwd = new penWidthDialog(); //класс формы для выбора ширины пера
			pwd.Val = lineWidth; //установка старой ширины pen( по дефолту)
			if(pwd.ShowDialog()==DialogResult.OK)
			{
				foreach(Form f2 in this.MdiChildren) 
					((Form2)f2).lineWidth = pwd.Val; //применение ко всем дочерним формам
				lineWidth = pwd.Val;
				statusStrip1.Items[0].Text = "Толщина линий: " + lineWidth; // отображение толщины в статус баре
			}
		}

		// установка значений размера нового окна из формы pictureSizeDialog
		public void setPictureSize()
		{
			pictureSizeDialog psd = new pictureSizeDialog();
			if(psd.ShowDialog()==DialogResult.OK)
			{
				pictHeight = psd.newHeight;
				pictWidth = psd.newWidth;
			}
		}

		// установка выбранного цвета пера из формы выборы цветов ColorDialog
		public void setPrimaryColor()
		{
			if(primColorDialog.ShowDialog()==DialogResult.OK)
			{
				foreach(Form f2 in this.MdiChildren)
					((Form2)f2).primaryColor = primColorDialog.Color;
				statusStrip1.Items[2].BackColor = primColorDialog.Color;
				
			}

		}

		// установка выбранного цвета заливки из формы выборы цветов ColorDialog
		public void setSecondaryColor()
		{
			if(secondColorDialog.ShowDialog()==DialogResult.OK)
			{
				foreach(Form f2 in this.MdiChildren)
					((Form2)f2).secondaryColor = secondColorDialog.Color;
				statusStrip1.Items[4].BackColor = secondColorDialog.Color;
				
			}
		}

		public void setBackColor()
		{
			if (backColorDialog.ShowDialog() == DialogResult.OK)
			{
				
				foreach (Form f2 in this.MdiChildren) {
					

					((Form2)f2).backColor = backColorDialog.Color;
				}
				statusStrip1.Items[6].BackColor = backColorDialog.Color;
				
			}
		}

		//установка шрифта из диалогового окна 
		public void setFont()
		{
			if (fontDialog1.ShowDialog() == DialogResult.OK)
			{
				textFont = fontDialog1.Font;
				statusMessage();
				foreach (Form f2 in this.MdiChildren)
					((Form2)f2).textFont = textFont;

			}
		}

		//Выделение\залика рисунка
		//======================================================================================

		public void allDown()
		{ //set all figure type controls "checked" to false
		  //устанавливает для всех фигур тип выбора false, как меню, так и панели кнопок
			lieToolStripMenuItem.Checked = false;
			rectangleToolStripMenuItem.Checked = false;
			ellipseToolStripMenuItem.Checked = false;
			curveToolStripMenuItem.Checked = false;
			textToolStripMenuItem.Checked = false; //новое поле 
			toolStripButton8.Checked = false;
			toolStripButton9.Checked = false;
			toolStripButton10.Checked = false;
			toolStripButton11.Checked = false;
			toolStripButton14.Checked = false;
			
			textMode = false; 
			statusMessage(); //refresh status message
		}

		public void setFigureType()
		{
			foreach(Form f2 in this.MdiChildren) //apply figure type to all active windows
				((Form2)f2).figureID = figureID;
		}

		
		//активирует флаг заливки в кнопке и в меню
		public void setFill()
		{

			if(!fillToolStripMenuItem.Checked) 
			{
				solidFill = true;
				fillToolStripMenuItem.Checked = true;
				toolStripButton12.Checked = true;
			}else{
				solidFill = false;
				fillToolStripMenuItem.Checked = false;
				toolStripButton12.Checked = false; //кнопка заливки в кнопочной панели
			}
			foreach(Form f2 in this.MdiChildren)
				((Form2)f2).solidFill = solidFill;
				
		}

		//в зависимости от выбора оставляет нужный элемент активным
		public void setLine()
		{
			allDown();
			lieToolStripMenuItem.Checked = true;
			toolStripButton8.Checked = true;
			figureID = 0;
			setFigureType();
		}

		public void setCurve()
		{
			allDown();
			curveToolStripMenuItem.Checked = true;
			toolStripButton9.Checked = true;
			figureID = 1;
			setFigureType();
		}

		public void setRectangle()
		{
			allDown();
			rectangleToolStripMenuItem.Checked = true;
			toolStripButton10.Checked = true;
			figureID = 2;
			setFigureType();
		}

		public void setEllipse()
		{
			allDown();
			ellipseToolStripMenuItem.Checked = true;
			toolStripButton11.Checked = true;
			figureID = 3;
			setFigureType();
		}

		public void setTextLabel()
		{
			allDown();
			textToolStripMenuItem.Checked = true; //надпись в toolstrip Figure
			toolStripButton14.Checked = true;
			textMode = true;
			statusMessage();
			figureID = 4;
			setFigureType();
		}

		// УПРАВЛЕНИЕ ФИГУРАМИ
		//======================================================================================
		public void setSelection()
		{
			if (selectToolStripMenuItem.Checked)
			{
				selectToolStripMenuItem.Checked = false;
				toolStripButton15.Checked = false;
				selection = false;
			}
			else
			{
				selectToolStripMenuItem.Checked = true;
				toolStripButton15.Checked = true;
				selection = true;
			}
			foreach (Form f2 in this.MdiChildren)
				((Form2)f2).selection = selection;
		}

		public void deleteFigure()
		{
			if (this.ActiveMdiChild != null)
				((Form2)this.ActiveMdiChild).deleteSelected();
		}

		//======================================================================================

		public Form1()
		{
			InitializeComponent();
		}

		//создать
		private void newToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			newWindow();
			activateMenu();
		}

		//открыть
		private void oPenToolStripMenuItem_Click(object sender, EventArgs e)
		{
			openFile();
		}

		//сохранить
		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			saveFile();
		}

		//сохранить как
		private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			saveAsFile();
		}

		//толщина линии
		private void lineWidthToolStripMenuItem_Click(object sender, EventArgs e)
		{
			setPenWidth();
		}

		//цвет заливки
		private void backgroundColorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			setSecondaryColor();
		}

		//цвет линии
		private void lineColorToolStripMenuItem_Click(object sender, EventArgs e)
		{
			setPrimaryColor();
		}

		//размер новых рисунков
		private void newPicturesSizeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			setPictureSize();
		}

		//активация заливки из меню
		private void fillToolStripMenuItem_Click(object sender, EventArgs e)
		{
			setFill();
		}

		//выбор объектов для рисования
		private void lieToolStripMenuItem_Click(object sender, EventArgs e)
		{
			setLine();
		}

		private void curveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			setCurve();
		}

		private void rectangleToolStripMenuItem_Click(object sender, EventArgs e)
		{
			setRectangle();
		}

		private void ellipseToolStripMenuItem_Click(object sender, EventArgs e)
		{
			setEllipse();
		}

		//задаваемый шрифт
		private void textToolStripMenuItem_Click(object sender, EventArgs e)
		{
			setTextLabel();
		}

		private void fontToolStripMenuItem_Click(object sender, EventArgs e)
		{
			setFont();
		}

		//                             ПАНЕЛЬ КНОПОК

		//новый
		private void toolStripButton1_Click(object sender, EventArgs e)
		{
			newWindow();
			activateMenu();
		}

		//открыть
		private void toolStripButton2_Click(object sender, EventArgs e)
		{
			openFile();
		}

		//сохранить
		private void toolStripButton3_Click(object sender, EventArgs e)
		{
			saveFile();
		}

		//толщина линии
		private void toolStripButton4_Click(object sender, EventArgs e)
		{
			setPenWidth();
		}

		//цвет линии
		private void toolStripButton5_Click(object sender, EventArgs e)
		{
			setPrimaryColor();
		}

		//цвет заливки
		private void toolStripButton6_Click(object sender, EventArgs e)
		{
			setSecondaryColor();
		}

		//размер новых рисунков 
		private void toolStripButton7_Click(object sender, EventArgs e)
		{
			setPictureSize();
		}

		//линия
		private void toolStripButton8_Click(object sender, EventArgs e)
		{
			setLine();
		}

		//кривая
		private void toolStripButton9_Click(object sender, EventArgs e)
		{
			setCurve();
		}

		//прямоугольник
		private void toolStripButton10_Click(object sender, EventArgs e)
		{
			setRectangle();
		}

		//элипс
		private void toolStripButton11_Click(object sender, EventArgs e)
		{
			setEllipse();
		}

        //заливка
        private void toolStripButton12_Click(object sender, EventArgs e)
		{
			setFill();
		}

        //цвет нового холста
        private void toolStripButton13_Click(object sender, EventArgs e)
		{
			setBackColor();
		}

        //задаваемый шрифт 
        private void toolStripButton14_Click(object sender, EventArgs e)
		{
			setFont();
		}

		
        private void toolStripButton15_Click(object sender, EventArgs e)
		{
			setTextLabel();
		}

		//выделить
        private void toolStripButton16_Click(object sender, EventArgs e)
        {
			setSelection();
		}

		private void selectToolStripMenuItem_Click(object sender, EventArgs e)
		{
			setSelection();
		}

		//удалить
		private void toolStripButton17_Click(object sender, EventArgs e)
        {
			deleteFigure();
		}

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
			deleteFigure();
		}
	}
}
