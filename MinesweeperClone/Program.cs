using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

public class MainForm : System.Windows.Forms.Form
{
	private System.ComponentModel.IContainer components;
	private List<GridSquare> grid;
	private int increment = 0;
	private Point lastClick = new Point ();
	private static int dimension = 15;
	private Random rndm = new Random ();
	private static int squareSpacing = 30;
	private static int squareSize = squareSpacing;
	private bool gameOver = false;

	private static int fieldCount = dimension * dimension;

	private int mineCount;
	private int unrevealedCount;
	//private static int squareSize = ((squareSpacing *19) /20);

	public MainForm()
	{
		InitializeComponent();
		ResetGame ();
	}


	private void PopulateGrid(List<GridSquare> gridList)
	{
		for (int i = 0; i < dimension; i++)
		{
			for (int o = 0; o < dimension; o++)
			{
				bool newMinedState;
				if (rndm.Next (10) == 1) {
					newMinedState = true;
					mineCount++;
				} 
				else 
				{
					newMinedState = false;
				}
				GridSquare newPoint = new GridSquare (i,o,newMinedState);
				gridList.Add (newPoint);

				


			}
		}

		for (int i = 0; i < grid.Count; i++) {
			grid [i].DetermineAdjacency(grid);
		}
	}

	private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container ();
		this.MouseDown += new System.Windows.Forms.MouseEventHandler (this.Form_MouseDown);

		this.AutoScaleBaseSize = new System.Drawing.Size (6, 15);

		this.Name = "MainForm";
		this.Text = "Minesweeper Clone";
		AddMenu ();
		this.Load += new System.EventHandler(this.MainForm_Load);
		this.Paint += new System.Windows.Forms.PaintEventHandler (this.MainForm_Paint);
	}

	public void AddMenu()
	{
		MainMenu menu = new MainMenu ();

		MenuItem menuFile = new MenuItem ("&File");
		MenuItem menuReset = new MenuItem ("&Reset");
		MenuItem menuQuit = new MenuItem ("&Quit");

		menuReset.Click += new System.EventHandler (this.MenuResetClick);
		menuQuit.Click += new System.EventHandler (this.MenuQuitClick);


		MenuItem menuOptions = new MenuItem ("&Options");
		MenuItem menuConfigure = new MenuItem ("&Configure");

		menuConfigure.Click += new System.EventHandler (this.MenuConfigClick);

		menu.MenuItems.Add (menuFile);
		menuFile.MenuItems.Add (menuReset);
		menuFile.MenuItems.Add (menuQuit);

		menu.MenuItems.Add (menuOptions);
		menuOptions.MenuItems.Add (menuConfigure);

		this.Menu = menu;

	}

	private void MenuConfigClick(object sender, System.EventArgs e)
	{
		Console.Write ("click");
		ConfigWindow cfg = new ConfigWindow (this);
		cfg.Show ();
	}

	private void MenuResetClick(object sender, System.EventArgs e)
	{
		ResetGame ();
	}

	private void MenuQuitClick(object sender, System.EventArgs e)
	{
		Application.Exit ();
	}

	static void Main(){
		Application.Run(new MainForm());
	}

	private void MainForm_Load(object sender, System.EventArgs e)
	{
		this.BackColor = Color.FromArgb (255, 240, 240, 210);
		this.ClientSize = new System.Drawing.Size(squareSpacing * dimension,(squareSpacing * dimension));
	}

	private void ResetGame()
	{
		grid = new List<GridSquare>();
		mineCount = 0;
		unrevealedCount = fieldCount;
		PopulateGrid (grid);
		GraphicsUpdate ();
	}

	private void Form_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
	{
		//int targetIndex = -1;
		for (int i = 0; i < grid.Count; i++)
		{
			if (ClickInSquare (e.X, grid [i].X) && ClickInSquare (e.Y, grid [i].Y)) 
			{
				if (e.Button == MouseButtons.Left) 
				{
					if (grid [i].viewState == ViewState.Hide) 
					{
						ActivateSquare (grid [i]);
					}
				}

				if (e.Button == MouseButtons.Right) 
				{
					FlagToggle (grid [i]);
				}
				
			}
		}
		lastClick.X = e.X;
		lastClick.Y = e.Y;
		CheckWinState ();
		GraphicsUpdate ();
	}

	private bool CheckWinState()
	{
		if (mineCount == unrevealedCount) {
			Console.Write ("Winner");
			return true;
		}
		return false;
	}

	private void GameOver()
	{
		gameOver = true;
		RevealAll ();
		GraphicsUpdate ();
	}

	private void ActivateSquare(GridSquare tarGS)
	{

		if (tarGS.minedState == false) 
		{
			unrevealedCount--;
			tarGS.viewState = ViewState.Show;
			AutoReveal (tarGS);
		} 
		else 
		{
			GameOver ();
		}

	}



	private void AutoReveal(GridSquare tarGS)
	{
		if (tarGS.adjacencyCount == 0) {
			foreach(GridSquare element in grid)
			{
				if (tarGS.IsAdjacent(element) && element.viewState == ViewState.Hide) 
				{
					ActivateSquare (element);
				}
			}
		}
	}

	private void DrawBevelRectangle (int margin, Rectangle rectangle, Color colour1, Color colour2, Color colour3, Graphics graphics)
	{

		SolidBrush brush1 = new SolidBrush (colour1);
		SolidBrush brush2 = new SolidBrush (colour2);
		SolidBrush brush3 = new SolidBrush (colour3);

		int[] rectangleArray = new int[4] {
			rectangle.X,
			rectangle.Y,
			rectangle.X+rectangle.Width,
			rectangle.Y+rectangle.Height};

		Point p1;
		Point p2;
		Point p3 = new Point ((rectangleArray [0] + rectangleArray [2]) / 2, (rectangleArray [1] + rectangleArray [3]) / 2); 







		for (int i = 0; i < 4; i++) 
		{

			SolidBrush triangleBrush= brush2;

			if (i % 2 == 0) {
				p1 = new Point (rectangleArray [i], rectangleArray [(i + 1) % 4]);
				p2 = new Point (rectangleArray [i], rectangleArray [(i + 3) % 4]); 
			} else {
				p1 = new Point (rectangleArray [(i + 1) % 4], rectangleArray [i]);
				p2 = new Point (rectangleArray [(i + 3) % 4], rectangleArray [i]);

			}

			if (i < 2) {
				triangleBrush = brush3;
			}


			Point[] triangleArray = new Point[3]{ p1, p2, p3 };

			graphics.FillPolygon (triangleBrush, triangleArray);
		}
		graphics.FillRectangle (brush1, AddPixelsToRectangle(rectangle,-margin,-margin));
	}

	private void RevealAll()
	{
		foreach (GridSquare element in grid) 
		{
			element.viewState = ViewState.Show;
		}
	}

	private void FlagToggle(GridSquare tarGS)
	{
		if (tarGS.viewState == ViewState.Flag) {
			tarGS.viewState = ViewState.Hide;
		} 
		else if (tarGS.viewState == ViewState.Hide)
		{
			tarGS.viewState = ViewState.Flag;
		}
	}

	private bool ClickInSquare(int clickCoord, int squareCoord)
	{
		return ((squareCoord * squareSpacing) < clickCoord) && ((squareCoord * squareSpacing) + squareSize > clickCoord);
	}

	private void CycleIncrement ()
	{
		if (increment < dimension*2) {
			increment = increment + 1;
		} 
		else 
		{
			increment = 0;
		}
	}

	private Rectangle AddPixelsToRectangle(Rectangle inputRectangle, int xPixels, int yPixels)
	{
		Rectangle returnRect;

		int newX = inputRectangle.X - 
			(xPixels);
		int newY = inputRectangle.Y -
			(yPixels);

		int newWidth = inputRectangle.Width + xPixels * 2;
		int newHeight = inputRectangle.Height + yPixels * 2;

		returnRect = new Rectangle (newX, newY, newWidth, newHeight);

		return returnRect;	
	}

	private Rectangle DecimateRectangle(Rectangle inputRectangle)
	{
		Rectangle returnRect;

		int newX = inputRectangle.X + 
			(inputRectangle.Width/20);
		int newY = inputRectangle.Y +
			(inputRectangle.Height/20);

		int newWidth = (inputRectangle.Width*9)/10;
		int newHeight = (inputRectangle.Height*9)/10;

		returnRect = new Rectangle (newX, newY, newWidth, newHeight);

		return returnRect;
	}

	private void DrawMine(Rectangle drawArea, Graphics graphics)
	{
		SolidBrush blackBrush = new SolidBrush (Color.Black);
		Rectangle ellipseRectangle = new Rectangle (
			                             drawArea.X + drawArea.Width / 4, 
			                             drawArea.Y + drawArea.Height / 4,
			                             drawArea.Width / 2,
			                             drawArea.Height / 2
		                             );

		Pen minePen = new Pen (blackBrush);
		minePen.Width = drawArea.Width / 10;

		if (minePen.Width < 1) 
		{
			minePen.Width = 1;
		}

		Point l1;
		Point l2;
		Point ellipseCentre = new Point (ellipseRectangle.X + ellipseRectangle.Width/2, ellipseRectangle.Y + ellipseRectangle.Height/2); 

		int prongLength = (ellipseRectangle.Width * 8) / 10;

		for (int i = 0; i < 4; i++) {
			double prongAngle = (((double)i) / 4) * Math.PI;

			double sinVal = Math.Sin (prongAngle);
			double cosVal = Math.Cos (prongAngle);

			int l1X = ellipseCentre.X + (int)(sinVal * prongLength);
			int l1Y = ellipseCentre.Y + (int)(cosVal * prongLength);

			int l2X = ellipseCentre.X - (int)(sinVal * prongLength);
			int l2Y = ellipseCentre.Y - (int)(cosVal * prongLength);

			l1 = new Point (l1X, l1Y);
			l2 = new Point (l2X, l2Y);
			graphics.DrawLine (minePen, l1, l2);
		}

		graphics.FillEllipse (blackBrush, ellipseRectangle);
	}

	private void DrawFlag(Rectangle drawArea, Graphics graphics)
	{
		SolidBrush flagBrush = new SolidBrush (Color.Red);
		SolidBrush blackBrush = new SolidBrush (Color.Black);
		Pen flagPolePen = new Pen (blackBrush);

		flagPolePen.Width = drawArea.Width / 20;
		if (flagPolePen.Width < 1) 
		{
			flagPolePen.Width = 1;
		}
		Point flagTopLeft = new Point (drawArea.X + drawArea.Width / 3, drawArea.Y + drawArea.Height / 3);
		Rectangle flagBody = new Rectangle (flagTopLeft.X, flagTopLeft.Y, drawArea.Width / 3, drawArea.Height / 4); 
		graphics.FillRectangle (flagBrush, flagBody);
		graphics.DrawLine(flagPolePen, flagTopLeft, new Point(flagTopLeft.X, flagTopLeft.Y + drawArea.Height / 2));
	}

	private void DrawGrid(Graphics graphics)
	{
		foreach (GridSquare element in grid)
		{
			bool columnClicked = (((element.X * squareSpacing) < lastClick.X) && ((element.X * squareSpacing) + squareSize > lastClick.X));
			bool rowClicked = (((element.Y * squareSpacing) < lastClick.Y) && ((element.Y * squareSpacing) + squareSize > lastClick.Y));
			Rectangle rect = new Rectangle (Convert.ToInt32 (element.X * squareSpacing), Convert.ToInt32 (element.Y * squareSpacing), squareSize, squareSize);
			if (element.viewState != ViewState.Show) 
			{


				DrawBevelRectangle ((squareSize*2)/10, rect, Color.DarkGray, Color.Gray, Color.LightGray, graphics);

				if (element.viewState == ViewState.Flag) 
				{
					DrawFlag (rect, graphics);
				}


			} 
			else 
			{
				if (element.minedState != true) {
					DrawBevelRectangle (squareSize / 10, rect, Color.LightBlue, Color.DarkOliveGreen, Color.DarkSeaGreen, graphics);
				} else {
					DrawBevelRectangle (squareSize / 10, rect, Color.DarkRed, Color.LightYellow, Color.LightGoldenrodYellow, graphics);
				}




				Pen tPen = new Pen (Color.Black);

				if (element.minedState == true) 
				{
					DrawMine (rect, graphics);
				} 
				else if (element.adjacencyCount > 0)
				{
					Font drawFont = new Font ("Verdana", squareSize/2);
					StringFormat sForm = new StringFormat ();
					sForm.LineAlignment = StringAlignment.Center;
					sForm.Alignment = StringAlignment.Center;
					SolidBrush textBrush = new SolidBrush (Color.Black);
					graphics.DrawString (element.adjacencyCount.ToString (), drawFont, textBrush, rect, sForm);
				}
			}
		}
	}

	private void MainForm_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
	{

		DrawGrid(e.Graphics);
	}


	private void GraphicsUpdate()
	{
		CycleIncrement ();
		DrawGrid (CreateGraphics());
	}

	private void timer_Tick(object sender, System.EventArgs e)
	{
		DrawGrid(CreateGraphics());
		CycleIncrement ();
	}

}

public class ConfigWindow: System.Windows.Forms.Form
{
	private System.ComponentModel.IContainer components;
	private int nextPanel = 15;
	Panel setSquareSizePanel;
	Panel setDimPanel;
	Panel buttonPanel;
	MainForm parent;
	public ConfigWindow(MainForm parent)
	{
		InitializeComponent();
		this.parent = parent;
		DimSetInit (nextPanel);
		nextPanel = this.setDimPanel.Bottom + 15;
		SquareSizeInit (nextPanel);
		nextPanel = this.setSquareSizePanel.Bottom + 15;
	}

	private void ButtonInit(int y)
	{
		buttonPanel = new Panel ();
		buttonPanel.Top = y;
		buttonPanel.MinimumSize = new Size (10, 10);
		buttonPanel.AutoSize = true;
		buttonPanel.BorderStyle = BorderStyle.FixedSingle;
		this.Controls.Add (buttonPanel);

		Button applyButton = new Button ();
		applyButton.Text = "Apply";
	}

	private void SquareSizeInit(int y)
	{
		setSquareSizePanel = new Panel ();
		setSquareSizePanel.Top = y;
		setSquareSizePanel.MinimumSize = new Size(10,10);
		setSquareSizePanel.AutoSize = true;
		setSquareSizePanel.BorderStyle = BorderStyle.FixedSingle;
		this.Controls.Add (setSquareSizePanel);

		Label setSquareSizeLabel = new Label ();
		int setSquareSizeLabelX = 0;
		int setSquareSizeLabelY = 0;
		setSquareSizeLabel.Location = new Point (setSquareSizeLabelX, setSquareSizeLabelY);
		setSquareSizeLabel.Text = "Set square size (px):";
		setSquareSizeLabel.AutoSize = true;
		setSquareSizeLabel.BorderStyle = BorderStyle.FixedSingle;
		setSquareSizePanel.Controls.Add (setSquareSizeLabel);

		NumericUpDown setSquareSizeField = new NumericUpDown ();
		int setSquareSizeFieldX = setSquareSizeLabel.Right + 5;
		setSquareSizeField.Location = new Point (setSquareSizeFieldX, setSquareSizeLabelY);
		setSquareSizeField.BorderStyle = BorderStyle.FixedSingle;
		setSquareSizePanel.Controls.Add (setSquareSizeField);
	}

	private void DimSetInit(int y)
	{
		setDimPanel = new Panel ();
		setDimPanel.Top = y;
		setDimPanel.AutoSize = true;
		setDimPanel.BorderStyle = BorderStyle.FixedSingle;
		setDimPanel.MinimumSize = new Size(10,10);
		this.Controls.Add (setDimPanel);

		Label setDimLabel = new Label ();
		setDimLabel.Text = "Set grid dimensions:";

		int setDimLabelX = 0;
		int setDimLabelY = 0;
		setDimLabel.Location = new Point (setDimLabelX, setDimLabelY);
		setDimLabel.AutoSize = true;
		setDimPanel.Controls.Add (setDimLabel);

		setDimLabel.BorderStyle = BorderStyle.FixedSingle;

		NumericUpDown setDimField = new NumericUpDown ();
		int setDimFieldX = setDimLabel.Right + 5;

		setDimField.Location = new Point (setDimFieldX, setDimLabelY);
		setDimField.BorderStyle = BorderStyle.FixedSingle;
		setDimPanel.Controls.Add (setDimField);
	}

	private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container ();

		this.MouseDown += new System.Windows.Forms.MouseEventHandler (this.Form_MouseDown);

		this.AutoScaleBaseSize = new System.Drawing.Size (6, 15);

		this.Name = "Configure";

		this.Load += new System.EventHandler(this.MainForm_Load);
	}
	public void MainForm_Load(object sender, System.EventArgs e)
	{
	}
	private void Form_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
	{
	}
}

public class GridSquare
{
	public int X;
	public int Y;
	public bool minedState;
	public ViewState viewState;
	public int adjacencyCount;

	public GridSquare(int xVal, int yVal, bool mState)
	{
		X = xVal;
		Y = yVal;
		minedState = mState;
		viewState = ViewState.Hide;
		adjacencyCount = 0;
	}

	public bool IsAdjacent(GridSquare tarGS)
	{
		return EqualsPlusOrMinus(1, tarGS.X, this.X) && EqualsPlusOrMinus(1, tarGS.Y, this.Y);
	}

	public bool EqualsPlusOrMinus(int pomVal, int cVal1, int cVal2)
	{
		bool retVal = false;
		if (cVal1 == cVal2)
		{
			retVal = true;
		}
		else if (cVal1 > cVal2 && (cVal1 - pomVal) <= cVal2)
		{
			retVal = true;
		}
		else if (cVal2 > cVal1 && (cVal2 - pomVal) <= cVal1)
		{
			retVal = true;
		}
		return retVal;
	}

	public void DetermineAdjacency(List<GridSquare> tarGrid)
	{
		int adjCount = 0;
		foreach (GridSquare element in tarGrid) {
			if (element != this) 
			{
				if (EqualsPlusOrMinus(1, element.X, this.X) && EqualsPlusOrMinus(1, element.Y, this.Y))
				{

					if (element.minedState == true) {
						adjCount++;
					}
				}
			}
		}
		adjacencyCount = adjCount;
	}
}



public enum ViewState
{
	Show, Hide, Flag
}