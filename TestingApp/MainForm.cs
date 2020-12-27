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
            m_game = new GameEngine(new Position(500, 400));

            m_game.Board.Camps.Add(new GameCamp(new GridPosition(1, 1) ));


            m_game.Board.Towers.Add(new GunTower(new GridPosition(3, 4)));
            m_game.Board.Towers.Add(new GunTower(new GridPosition(4, 3)));
            m_game.Board.Towers.Add(new GunTower(new GridPosition(4, 4)));
            m_game.Board.Towers.Add(new GunTower(new GridPosition(3, 1)));

            m_game.Board.Launchers.Add(new StaticLauncher(new GridPosition(9, 11), new SlowWalkerAgent(new Position(0, 0)), 50 ));

            //m_game.Board.Agents.Add(new SlowWalkerAgent(new Position(400, 300)) );
            //m_game.Board.Agents.Add(new SlowWalkerAgent(new Position(300, 400)) );
            //m_game.Board.Agents.Add(new SlowWalkerAgent(new Position(350, 400)));
            //m_game.Board.Agents.Add(new SlowWalkerAgent(new Position(300, 450)));
            //m_game.Board.Agents.Add(new SlowWalkerAgent(new Position(400, 400)) );

            m_game.Board[3, 4] = enCellType.Blocked;
            m_game.Board[4, 3] = enCellType.Blocked;
            m_game.Board[4, 4] = enCellType.Blocked;
            m_game.Board[3, 1] = enCellType.Blocked;
            m_game.Board[3, 7] = enCellType.Blocked;
            m_game.Board[8, 7] = enCellType.Blocked;
            m_game.Board[9, 7] = enCellType.Blocked;
            //m_game.Board[5, 2] = enCellType.Blocked;

            for (int i = 1; i < m_game.Board.Grid.Rows-1; i++)
            {
                m_game.Board[i-1, 6] = enCellType.Blocked;
                m_game.Board[i+1, 9] = enCellType.Blocked;
            }

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
            float camp_size = 10;
            float tower_size = 6;
            float agent_size = 6;

            bool drawHexGrid = true;

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

            //draw hex = grid
            if (drawHexGrid)
            {
                Point[] vertexes = new Point[HexGrid.NEIGHBORS_COUNT];

                for (int i = 0; i < board.Grid.Rows; i++)
                {
                    for (int j = 0; j < board.Grid.Columns; j++)
                    {
                        float x, y;
                        board.Grid.GetCellCenter(i, j, out x, out y);
                        g.FillEllipse(Brushes.Gray, x - 1, y - 1, 2, 2);

                        for (int k = 0; k < HexGrid.NEIGHBORS_COUNT; k++)
                        {
                            float x1, y1;
                            board.Grid.GetVertex(i, j, k, out x1, out y1);
                            vertexes[k] = new Point((int)x1, (int)y1);
                        }

                        if (m_game.Board[i, j] == enCellType.Free)
                            g.FillPolygon(Brushes.Silver, vertexes);
                        else
                            g.FillPolygon(Brushes.Gray, vertexes);

                        for (int k = 0; k < 6; k++)
                        {
                            float x1, y1;
                            float x2, y2;
                            board.Grid.GetVertex(i, j, k, out x1, out y1);
                            board.Grid.GetVertex(i, j, (k + 1) % 6, out x2, out y2);
                            g.DrawLine(Pens.LightGray, x1, y1, x2, y2);
                        }
                    }
                }
            }

            //draw camps            
            foreach (GameCamp camp in board.Camps)
            {
                float cx, cy;
                board.Grid.GetCellCenter(camp.Position, out cx, out cy);
                g.DrawRectangle(Pens.Blue, cx - camp_size / 2, cy - camp_size / 2, camp_size, camp_size);
            }

            //draw towers            
            foreach (GameTower tower in board.Towers)
            {
                float cx, cy;
                board.Grid.GetCellCenter(tower.Position, out cx, out cy);

                g.DrawEllipse(Pens.Green, cx - tower_size / 2, cy - tower_size / 2, tower_size, tower_size);
            }

            //draw agents            
            foreach (GameAgent agent in board.Agents)
            {
                g.DrawEllipse(agent.isAlive ? Pens.Red : Pens.Gray, agent.Position.X - agent_size / 2, agent.Position.Y - agent_size / 2, agent_size, agent_size);
                //draw health bar
                if (agent.isAlive)
                {
                    g.FillRectangle(Brushes.Green, agent.Position.X - 5, agent.Position.Y - (agent_size + 5), (float)(11 * agent.HitPoints / 100), 4);
                    g.DrawRectangle(Pens.Black, agent.Position.X - 5, agent.Position.Y - (agent_size + 5), 11, 4);
                }
            }


            //draw selected cell
            int row, col;
            board.Grid.FindCell(m_nMouseXCoord, m_nMouseYCoord, out row, out col);

            if (board.Grid.IsOnGrid(row, col))
            {
                SolidBrush cell_brush1 = new SolidBrush(Color.FromArgb(64, Color.SeaGreen.R, Color.SeaGreen.G, Color.SeaGreen.B));
                SolidBrush cell_brush2 = new SolidBrush(Color.FromArgb(64, Color.Coral.R, Color.Coral.G, Color.Coral.B));

                PointF[] vertexes = new PointF[HexGrid.NEIGHBORS_COUNT];

                float x, y;
                board.Grid.GetCellCenter(row, col, out x, out y);
                g.FillEllipse(Brushes.Gray, x - 1, y - 1, 2, 2);
                for (int k = 0; k < 6; k++)
                {
                    float x1, y1;
                    board.Grid.GetVertex(row, col, k, out x1, out y1);
                    vertexes[k] = new PointF(x1, y1);
                }
                g.FillPolygon(cell_brush1, vertexes);

                //draw neighbours                 
                for (int d = 0; d < HexGrid.NEIGHBORS_COUNT; d++)
                {
                    int n_row, n_col;
                    board.Grid.TryGetNeighbor(row, col, d, out n_row, out n_col);

                    if (board.Grid.IsOnGrid(n_row, n_col))
                    {
                        for (int k = 0; k < HexGrid.NEIGHBORS_COUNT; k++)
                        {
                            float x1, y1;
                            board.Grid.GetVertex(n_row, n_col, k, out x1, out y1);
                            vertexes[k] = new PointF(x1, y1);
                        }

                        g.FillPolygon(cell_brush2, vertexes);
                    }
                }

                List<GridPosition> path = board.GetPath(new GridPosition(row, col), new GridPosition(1, 1));

                double total_length = 0;
                for (int k = 0; k < path.Count - 1; k++)
                {
                    float x1, y1, x2, y2;
                    board.Grid.GetCellCenter(path[k].Row, path[k].Col, out x1, out y1);
                    board.Grid.GetCellCenter(path[k + 1].Row, path[k + 1].Col, out x2, out y2);

                    g.DrawLine(Pens.Red, x1, y1, x2, y2);

                    total_length += Math.Sqrt( (x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
                }
                this.toolStripStatusLabel2.Text = String.Format("Length: {0:F1}", total_length);

                //g.DrawRectangle(bold_pen, rect);
            }
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

            int row, col;
            m_game.Board.Grid.FindCell(m_nMouseXCoord, m_nMouseYCoord, out row, out col);

            this.toolStripStatusLabel1.Text = String.Format("Cursor: ({0}, {1})", row, col);

            pictureBox1.Refresh();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InitGame();
        }
    }
}

