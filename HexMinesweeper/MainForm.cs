using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HexMinesweeper
{
    public partial class MainForm : Form
    {
        HexMinesweeper m_game;

        int m_nMouseXCoord;
        int m_nMouseYCoord;

        DateTime m_game_start_time = DateTime.Now;

        public MainForm()
        {
            InitializeComponent();

            InitGame();
            ResizeForm();

            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            this.timer1.Start();
        }
        void InitGame()
        {
            m_game = new HexMinesweeper(7, 13, 13, 15.0);
            m_game_start_time = DateTime.Now;
        }

        public void ResizeForm()
        {
            Size size1 = this.Size;
            Size size2 = pictureBox1.Size;

            pictureBox1.Size = new Size((int)m_game.Grid.Width + 2, (int)m_game.Grid.Height + 2);
            this.Size = pictureBox1.Size + (size1 - size2);
            pictureBox1.Refresh();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            bool drawHexGrid = true;
            bool showMines = true;
            //Show board
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Rectangle rect = pictureBox1.DisplayRectangle;

            Font hint_font = new Font(FontFamily.GenericSansSerif, 0.7f * (float)m_game.Grid.Size, FontStyle.Bold);
            StringFormat hint_format = new StringFormat();
            hint_format.Alignment = StringAlignment.Center;
            hint_format.LineAlignment = StringAlignment.Center;

            Brush[] hint_brush = new Brush[] { Brushes.Blue, Brushes.Green, Brushes.Red, Brushes.Navy, Brushes.Maroon, Brushes.Teal }; 
            
            //draw hex = grid
            if (drawHexGrid)
            {
                for (int i = 0; i < m_game.Grid.Rows; i++)
                {
                    for (int j = 0; j < m_game.Grid.Columns; j++)
                    {
                        double x, y;
                        m_game.Grid.GetCellCenter(i, j, out x, out y);
                        
                        for (int k = 0; k < 6; k++)
                        {
                            double x1, y1;
                            double x2, y2;
                            m_game.Grid.GetVertex(i, j, k, out x1, out y1);
                            m_game.Grid.GetVertex(i, j, (k + 1) % 6, out x2, out y2);
                            g.DrawLine(Pens.LightGray, (float)x1, (float)y1, (float)x2, (float)y2);
                        }

                        if (m_game.isMine(i, j)) 
                        {
                            int mine_size = 10;
                            g.FillEllipse(Brushes.Black, (float)x - mine_size/2, (float)y - mine_size / 2, mine_size, mine_size);
                        }

                        if (m_game.isFlagged(i, j))
                        {
                            int flag_size = 5;
                            g.FillRectangle(Brushes.Black, (float)x - flag_size / 2, (float)y - flag_size / 2, flag_size, flag_size);
                        }

                        int hint = 0;
                        if (m_game.GetHint(i, j, ref hint))
                        {
                            if(hint>0 && hint <=6)
                                g.DrawString(hint.ToString(), hint_font, hint_brush[hint], (float)x, (float)y, hint_format);
                        }
                    }
                }
            }

            if (showMines)
            {
                
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            m_nMouseXCoord = e.X;
            m_nMouseYCoord = e.Y;

            pictureBox1.Refresh();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            InitGame();
            ResizeForm();
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            m_game.TryOpenCell(e.X, e.Y);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (m_game.IsGameOver)
            {
                this.toolStripStatusLabel1.Text = "Game Over";
            }
            else
            {
                this.toolStripStatusLabel1.Text = String.Format("Time: {0}", (DateTime.Now - m_game_start_time).ToString());
            }           
        }
    }
}
