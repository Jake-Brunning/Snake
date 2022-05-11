using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Snake
{
    public partial class Form1 : Form
    {
        Rectangle[,] grid = new Rectangle[17, 15]; //virtual grid on the form made of rectangles
        List<Keys> KeysPressed = new List<Keys>(); //list of the keys the player has pressed
        
        List<Cordinates> SpacesOccupied = new List<Cordinates>(); //stores what rectangles the snake takes up
        List<Keys> DirectionToGoIn = new List<Keys>();//stores which way each rectangle needs to go in. Type is keys because how the snake moves is with arrow keys
        Cordinates AppleCordinates = new Cordinates(); //look at name

        
        public Form1()
        { 
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) //handles arrow key presses
        {
            if (keyData == Keys.Up)
            {
                KeysPressed.Add(Keys.Up);
            }
           
            if (keyData == Keys.Down)
            {
                KeysPressed.Add(Keys.Down);
            }
           
            if (keyData == Keys.Left)
            {
                KeysPressed.Add(Keys.Left);
            }
            
            if (keyData == Keys.Right)
            {
                KeysPressed.Add(Keys.Right);
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }


        private void StartGame_Click(object sender, EventArgs e)
        {
            StartGame.Hide();
            DrawGrid();
        }
        private void DrawGrid() //Setting up the grid layout
        {        
            //Making the form a perfect grid
            ActiveForm.Width -= ActiveForm.Width % 17;
            ActiveForm.Height -= ActiveForm.Height % 15;

            //Dimensions of rectangles ( in pixels)
            int WidthOfRect = ActiveForm.Width / 17;
            int HeightOfRect = ActiveForm.Height / 15;
            Point rectPoint = new Point(0, 0);
            for(int i = 0; i < 17; i++)
            {
                for(int j = 0; j < 15; j++)
                {
                    grid[i, j] = new Rectangle(rectPoint, new Size(WidthOfRect, HeightOfRect));                  
                    rectPoint.Y += HeightOfRect;
                }
                rectPoint.X += WidthOfRect;
                rectPoint.Y = 0;
            }
            SpacesOccupied.Add(new Cordinates { x = 8, y = 7 });
            DirectionToGoIn.Add(Keys.Right);
            MakeApple();
            GlobalTimer.Start();
        }

        private void MakeApple()
        {
            Random rnd = new Random();

            
            AppleCordinates.x = rnd.Next(16);
            AppleCordinates.y = rnd.Next(14);
        }

        private void GlobalTimer_Tick(object sender, EventArgs e)
        {
         
            //Update Direction to go in        
            for (int i = DirectionToGoIn.Count - 1; i > 0; i--)//Loop through the list and moving everying left one to make space for the newest key press
            {
                DirectionToGoIn[i] = DirectionToGoIn[i - 1];
                
            }
            if (KeysPressed.Any()) //Returns true if there is no elements in keys pressed
            {
                DirectionToGoIn[0] = KeysPressed[0];
                KeysPressed.RemoveAt(0);
            }
           
            
            //Update spaces Occupied
            for (int i = 0; i < SpacesOccupied.Count; i++) //Loop through the list and move Rectangle Co-ords
            {              
                SpacesOccupied[i].moveData(DirectionToGoIn[i]);
            }
            
            //If 2 values in SpacesOcupied are the same the snake is overlaying, therefore fail
            //so this for loop checks if the head of the snake has the same co-ords of any part of te body of the snake.
            for (int i = SpacesOccupied.Count - 1; i > 1; i--) //Length -1 so it doesnt compare the head of the snake with the head of the snake
            {
                if (SpacesOccupied[0].x == SpacesOccupied[i].x & SpacesOccupied[0].y == SpacesOccupied[i].y)
                {
                    MessageBox.Show("Fail!");
                    GlobalTimer.Stop();
                }
            }

            if (SpacesOccupied[0].x == AppleCordinates.x & SpacesOccupied[0].y == AppleCordinates.y)//checking if in apple spaces
            {         
                SpacesOccupied.Add(new Cordinates { x = SpacesOccupied[SpacesOccupied.Count - 1].x, y = SpacesOccupied[SpacesOccupied.Count - 1].y });
                Keys key = DirectionToGoIn[DirectionToGoIn.Count - 1];
                if (key == Keys.Up) //move new snake block one behind the last block
                {
                    SpacesOccupied[SpacesOccupied.Count - 1].y += 1;
                }

                if (key == Keys.Down)
                {
                    SpacesOccupied[SpacesOccupied.Count - 1].y -= 1;
                }

                if (key == Keys.Left)
                {
                    SpacesOccupied[SpacesOccupied.Count - 1].x += 1;
                }

                if (key == Keys.Right)
                {
                    SpacesOccupied[SpacesOccupied.Count - 1].x -= 1;
                }
                DirectionToGoIn.Add(key);
                MakeApple();
            }

            //draw snake
            Graphics graphics = ActiveForm.CreateGraphics();
            SolidBrush fill = new SolidBrush(Color.Blue);
            graphics.Clear(Color.White);
            graphics.FillRectangle(new SolidBrush(Color.Cyan), grid[SpacesOccupied[0].x, SpacesOccupied[0].y]); //draw the head of snake a different colour to make it clear
            for (int i = 1; i < SpacesOccupied.Count; i++)
            {
                try// If snake is about to go off screen, the rectangle trying to be coloured in wiil not be in grid array. So try catch here detects failure
                {
                    
                    graphics.FillRectangle(fill, grid[SpacesOccupied[i].x, SpacesOccupied[i].y]);
                }
                catch
                {
                    MessageBox.Show("You failed!");
                    GlobalTimer.Stop();
                }
            }
            
            graphics.FillRectangle(new SolidBrush(Color.Red), grid[AppleCordinates.x, AppleCordinates.y]);
        }

        public class Cordinates
        {                                                                                                                                                                                                                                                                                                                 
            public int x { get; set; }
            public int y { get; set; }

            public void moveData(Keys key)
            {
                if (key == Keys.Up)
                {
                    y -= 1;
                }

                if (key == Keys.Down)
                {
                    y += 1;
                }

                if (key == Keys.Left)
                {
                    x -= 1;
                }

                if (key == Keys.Right)
                {
                    x += 1;
                }
            }
        }

        
    }
}
