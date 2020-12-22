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

namespace TestingApp
{
    public partial class MainForm : Form
    {
        GameEngine m_game;

        bool m_paused = false;

        string m_selected_tower;

        int m_nMouseXCoord;
        int m_nMouseYCoord;

        private System.Windows.Forms.ToolStripButton[] m_vToolStripButtons;

        public MainForm()
        {
            InitializeComponent();

            CreateNewTowerButtons();

            InitGame();

            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.pictureBox1.BackColor = System.Drawing.Color.WhiteSmoke;

            ResizeForm();

            timer1.Interval = 10;
            timer1.Start();

            this.toolStripButton1.Text = String.Format("{0}", (m_paused ? "Resume" : "Pause"));
        }

        void InitGame()
        {
            m_game = new GameEngine(new Position(600, 400));

            m_game.Board.Towers.Add(new GunTower(new Position(200, 100)));
            m_game.Board.Towers.Add(new GunTower(new Position(100, 200)));
            m_game.Board.Towers.Add(new GunTower(new Position(200, 200)));

            m_game.Board.Agents.Add(new SlowWalkerAgent(new Position(500, 400)) );

        }
        void CreateNewTowerButtons()
        {
            List<string> tower_types = GameTowerFactory.TowerTypes.Keys.ToList();

            m_vToolStripButtons = new ToolStripButton[GameTowerFactory.TowerTypes.Count + 1];

            for (int i = 0; i < m_vToolStripButtons.Length; i++)
                m_vToolStripButtons[i] = new System.Windows.Forms.ToolStripButton();            

            this.toolStrip1.Items.AddRange(m_vToolStripButtons);

            for (int i = 0; i < m_vToolStripButtons.Length; i++)
            {
                string tower_name = i == 0 ? "remove" : tower_types[i - 1].Split(new string[] { ".", "Tower"}, StringSplitOptions.RemoveEmptyEntries)[2];
                string tower_tag = i == 0 ? "remove" : tower_types[i - 1];

                m_vToolStripButtons[i].CheckOnClick = true;
                m_vToolStripButtons[i].DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
                m_vToolStripButtons[i].ImageTransparentColor = System.Drawing.Color.Magenta;
                m_vToolStripButtons[i].Name = "toolStripButton" + (i).ToString();
                m_vToolStripButtons[i].Size = new System.Drawing.Size(23, 22);
                m_vToolStripButtons[i].Text = tower_name;
                m_vToolStripButtons[i].Tag = tower_tag;
                m_vToolStripButtons[i].Click += new System.EventHandler(this.toolStripButton_Click);
            }            
            m_vToolStripButtons[0].Checked = true;
            m_selected_tower = "Remove";
        }

        public void ResizeForm()
        {
            Size size1 = this.Size;
            Size size2 = pictureBox1.Size;

            pictureBox1.Size = new Size((int)m_game.Board.Size.X + 1, (int)m_game.Board.Size.Y + 1);
            this.Size = pictureBox1.Size + (size1 - size2);
            pictureBox1.Refresh();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (m_game != null && !m_paused)
                m_game.Play();

            pictureBox1.Refresh();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            int camp_size = 10;
            int tower_size = 6;

            //Show board
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            if (m_game.Status == enGameStatus.Over)
                this.pictureBox1.BackColor = System.Drawing.Color.Yellow;
            else
                this.pictureBox1.BackColor = System.Drawing.Color.WhiteSmoke;

            Pen bold_pen = new Pen(Color.Black, 3);

            Rectangle rect = pictureBox1.DisplayRectangle;
            //rect.Height--;
            //rect.Width--;           

            //show game board 
            GameBoard board = m_game.Board;

            //draw camps            
            foreach (GameCamp camp in board.Camps)
            {                
                g.DrawRectangle(Pens.Blue, new Rectangle((int)camp.Position.X - camp_size / 2, (int)camp.Position.Y - camp_size / 2, camp_size, camp_size));
            }

            //draw towers            
            foreach (GameTower tower in board.Towers)
            {
                g.DrawEllipse(Pens.Green, new Rectangle((int)tower.Position.X - tower_size / 2, (int)tower.Position.Y - tower_size / 2, tower_size, tower_size));
            }

            //draw agents            
            foreach (GameAgent agent in board.Agents)
            {
                g.DrawEllipse(Pens.Red, new Rectangle((int)agent.Position.X - tower_size / 2, (int)agent.Position.Y - tower_size / 2, tower_size, tower_size));
            }

            //draw hex = grid
            for (int i =0; i<board.Grid.Rows; i++)
            {
                for (int j = 0; j < board.Grid.Columns; j++)
                {
                    double x, y;
                    board.Grid.GetCellCenter(i, j, out x, out y);
                    g.FillEllipse(Brushes.Gray, (float)x - 1, (float)y - 1, 2, 2);

                    List<Position> vertexes = new List<Position>();
                    for (int k = 0; k < 6; k++)
                    {
                        double x1, y1;
                        double x2, y2;
                        board.Grid.GetVertex(i, j, k, out x1, out y1);
                        board.Grid.GetVertex(i, j, (k + 1) % 6, out x2, out y2);
                        g.DrawLine(Pens.LightGray, (float)x1, (float)y1, (float)x2, (float)y2);
                    }
                }
            }

            //draw selected 
            int row, col;
            board.Grid.FindCell(m_nMouseXCoord, m_nMouseYCoord, out row, out col);

            if (board.Grid.IsOnGrid(row, col))
            {
                double x, y;
                board.Grid.GetCellCenter(row, col, out x, out y);
                g.FillEllipse(Brushes.Gray, (float)x - 1, (float)y - 1, 2, 2);                
                for (int k = 0; k < 6; k++)
                {
                    double x1, y1;
                    double x2, y2;
                    board.Grid.GetVertex(row, col, k, out x1, out y1);
                    board.Grid.GetVertex(row, col, (k + 1) % 6, out x2, out y2);
                    g.DrawLine(Pens.Red, (float)x1, (float)y1, (float)x2, (float)y2);
                }

                //draw neighbours 
                for (int d = 0; d < HexGrid.NEIGHBORS_COUNT; d++)
                {
                    int n_row, n_col;
                    board.Grid.GetNeighbor(row, col, d, out n_row, out n_col);

                    if (board.Grid.IsOnGrid(n_row, n_col))
                    {
                        for (int k = 0; k < 6; k++)
                        {
                            double x1, y1;
                            double x2, y2;
                            board.Grid.GetVertex(n_row, n_col, k, out x1, out y1);
                            board.Grid.GetVertex(n_row, n_col, (k + 1) % 6, out x2, out y2);
                            g.DrawLine(Pens.Yellow, (float)x1, (float)y1, (float)x2, (float)y2);
                        }
                    }
                }
            }

            //g.DrawRectangle(bold_pen, rect);
        }

        private void toolStripButton_Click(object sender, EventArgs e)
        {
            m_selected_tower = (String)((ToolStripButton)sender).Tag;


            for (int i = 0; i < m_vToolStripButtons.Length; i++)
            {
                if ((String)m_vToolStripButtons[i].Tag != m_selected_tower)
                    m_vToolStripButtons[i].Checked = false;
            }
        }
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            m_paused = !m_paused;

            this.toolStripButton1.Text = String.Format("{0}", (m_paused ? "Resume" : "Pause"));
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {

            pictureBox1.Refresh();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            m_nMouseXCoord = e.X;
            m_nMouseYCoord = e.Y;

            pictureBox1.Refresh();
        }
    }
}
