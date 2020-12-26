using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using ACQ.DroneDefenceGame;

namespace HexMinesweeper
{
    public partial class MainForm : Form
    {
        HexMinesweeper m_game;

        int m_nMouseXCoord;
        int m_nMouseYCoord;

        DateTime m_game_start_time = DateTime.Now;

        enDifficultyLevel m_game_level = enDifficultyLevel.Beginner;

        public MainForm()
        {
            InitializeComponent();
          
            InitGame();

            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            this.timer1.Start();
        }
        void InitGame()
        {
            double cell_size = 15.0;

            switch (m_game_level)
            {
                case enDifficultyLevel.Beginner: m_game = new HexMinesweeper(9, 9, 10, cell_size); break;
                case enDifficultyLevel.Indermediate: m_game = new HexMinesweeper(16, 16, 40, cell_size); break;
                case enDifficultyLevel.Expert: m_game = new HexMinesweeper(16, 30, 99, cell_size); break;
            }
            m_game_start_time = DateTime.Now;

            ResizeForm();
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
            int mine_size = 10;
            int flag_size = 10;

            bool drawHexGrid = true;
            bool showMines = m_game.Status == enGameStatus.Lost;

#if DEBUG
            showMines = true;
#endif

            //Show board
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Rectangle rect = pictureBox1.DisplayRectangle;

            Font hint_font = new Font(FontFamily.GenericSansSerif, 0.7f * (float)m_game.Grid.Size, FontStyle.Bold);
            StringFormat hint_format = new StringFormat();
            hint_format.Alignment = StringAlignment.Center;
            hint_format.LineAlignment = StringAlignment.Center;

            Brush[] hint_brush = new Brush[] { Brushes.Blue, Brushes.Green, Brushes.Red, Brushes.Navy, Brushes.Maroon, Brushes.Teal };

            SolidBrush cell_brush1 = new SolidBrush(Color.FromArgb(64, Color.SeaGreen.R, Color.SeaGreen.G, Color.SeaGreen.B));
            SolidBrush cell_brush2 = new SolidBrush(Color.FromArgb(64, Color.Coral.R, Color.Coral.G, Color.Coral.B));

            double[] flag_offset_x = new double[] {0, 0.86602540378443864676372317075294, -0.86602540378443864676372317075294 };
            double[] flag_offset_y = new double[] { -1, 0.5, 0.5 };


            //draw hex = grid
            if (drawHexGrid)
            {
                Point[] vertexes = new Point[HexGrid.NEIGHBORS_COUNT];

                for (int i = 0; i < m_game.Grid.Rows; i++)
                {
                    for (int j = 0; j < m_game.Grid.Columns; j++)
                    {
                        double x, y;
                        m_game.Grid.GetCellCenter(i, j, out x, out y);

                        for (int k = 0; k < HexGrid.NEIGHBORS_COUNT; k++)
                        {
                            double x1, y1;
                            m_game.Grid.GetVertex(i, j, k, out x1, out y1);
                            vertexes[k] = new Point((int)x1, (int)y1);
                        }

                        if (m_game.isOpen(i, j))
                            g.FillPolygon(Brushes.Silver, vertexes);
                        else
                            g.FillPolygon(Brushes.Gray, vertexes);

                        if (m_game.isMine(i, j) && showMines) 
                        {                            
                            g.FillEllipse(Brushes.Black, (float)x - mine_size/2, (float)y - mine_size / 2, mine_size, mine_size);
                        }

                        if (m_game.isFlagged(i, j))
                        {
                            g.FillEllipse(Brushes.Red, (float)x - flag_size / 2, (float)y - flag_size / 2, flag_size, flag_size);                            
                        }

                        int hint = 0;
                        if (m_game.GetHint(i, j, ref hint))
                        {
                            if(hint>0 && hint <=6)
                                g.DrawString(hint.ToString(), hint_font, hint_brush[hint], (float)x, (float)y, hint_format);
                        }
                    }
                }

                if (m_game.Status == enGameStatus.Lost)
                {
                    int i = 0, j = 0;
                    if (m_game.TryGetActivatedMine(ref i, ref j))
                    {
                        double x, y;
                        m_game.Grid.GetCellCenter(i, j, out x, out y);

                        for (int k = 0; k < HexGrid.NEIGHBORS_COUNT; k++)
                        {
                            double x1, y1;
                            m_game.Grid.GetVertex(i, j, k, out x1, out y1);
                            vertexes[k] = new Point((int)x1, (int)y1);
                        }
                        g.FillPolygon(Brushes.Red, vertexes);
                        g.FillEllipse(Brushes.Black, (float)x - mine_size / 2, (float)y - mine_size / 2, mine_size, mine_size);
                    }
                }

                int row, col;
                m_game.Grid.FindCell(m_nMouseXCoord, m_nMouseYCoord, out row, out col);

                if (m_game.Grid.IsOnGrid(row, col))
                {
                    double x, y;
                    m_game.Grid.GetCellCenter(row, col, out x, out y);

                    for (int k = 0; k < HexGrid.NEIGHBORS_COUNT; k++)
                    {
                        double x1, y1;
                        m_game.Grid.GetVertex(row, col, k, out x1, out y1);
                        vertexes[k] = new Point((int)x1, (int)y1);
                    }

                    if (!m_game.isOpen(row, col) && !m_game.isFlagged(row, col))
                        g.FillPolygon(Brushes.LightGray, vertexes);

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
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (m_game.TryOpenCell(e.X, e.Y))
                    this.toolStripProgressBar1.Value = (int)(100 * m_game.Completion);
            }
            else
            {
                m_game.TryFlagCell(e.X, e.Y);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            enGameStatus status = m_game.Status;            

            if (status != enGameStatus.InProgress)
            {
                this.toolStripStatusLabel1.Text = "Game Over: " + (m_game.Status == enGameStatus.Lost? "You Lost :(" : "You Won!" );
            }
            else
            {
                this.toolStripStatusLabel1.Text = String.Format("Mines: {0}, Flagged: {1}, Time: {2}", m_game.MineCount, m_game.FlagCount, (DateTime.Now - m_game_start_time).ToString(@"hh\:mm\:ss"));
            }           
        }

        private void toolStripMenuItem_beginner_Click(object sender, EventArgs e)
        {
            m_game_level = enDifficultyLevel.Beginner;
            this.toolStripMenuItem_beginner.Checked = true;            
            this.toolStripMenuItem_intermediate.Checked = false;
            this.toolStripMenuItem_expert.Checked = false;
            InitGame();
        }

        private void toolStripMenuItem_intermediate_Click(object sender, EventArgs e)
        {
            m_game_level = enDifficultyLevel.Indermediate;
            this.toolStripMenuItem_beginner.Checked = false;            
            this.toolStripMenuItem_intermediate.Checked = true;
            this.toolStripMenuItem_expert.Checked = false;
            InitGame();
        }

        private void toolStripMenuItem_expert_Click(object sender, EventArgs e)
        {
            m_game_level = enDifficultyLevel.Expert;
            this.toolStripMenuItem_beginner.Checked = false;            
            this.toolStripMenuItem_intermediate.Checked = false;
            this.toolStripMenuItem_expert.Checked = true;
            InitGame();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InitGame();
        }
    }
}
