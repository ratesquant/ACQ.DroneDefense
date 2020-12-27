using System;
using System.Collections.Generic;
using System.Text;

namespace ACQ.DroneDefenceGame
{
    public enum enCellType
    {
        Free,
        Blocked
    }
    public enum enDamageType
    {
        Physical,
        Heat,
        Optical,
        EM
    }
    public interface IPositionable
    {
        Position Position { get; }
    }

    public interface IGridPositionable
    {
        GridPosition Position { get; }
    }

    public interface IMovable
    {
        void Update(GameBoard board);
    }

    public interface IDestroyable
    { 
        double HitPoints { get; }
        bool isAlive { get;  }

        void DoDamage(double damage, enDamageType damage_type);
    }

    public struct GridPosition
    {
        private int m_row;
        private int m_col;

        public GridPosition(int row, int col)
        {
            m_row = row;
            m_col = col;

        }

        public int Row
        {
            get
            {
                return m_row;
            }
            set
            {
                m_row = value;
            }
        }

        public int Col
        {
            get
            {
                return m_col;
            }
            set
            {
                m_col = value;
            }
        }

    }

    public class MyPathNode : IPathNode<GameBoard>
    {
        int i, j;
        public MyPathNode(int row, int col)
        {
            i = row;
            j = col;
        }

        public int x { get { return j; }}
        public int y { get { return i; }}
     
        public bool IsWalkable(GameBoard board)
        {
            return board[i,j] == enCellType.Free;
        }
    }

    public struct Position
    {
        private float m_x;
        private float m_y;

        public Position(float x, float y) 
        {
            m_x = x;
            m_y = y;

        }

        public float X
        {
            get
            {
                return m_x;
            }
            set
            {
                m_x = value;
            }
        }

        public float Y
        {
            get
            {
                return m_y;
            }
            set
            {
                m_y = value;
            }
        }

        public static Position operator +(Position a, Position b)
        {
            return new Position(a.X + b.X, a.Y + b.Y);
        }
        public static Position operator -(Position a, Position b)
        {
            return new Position(a.X - b.X, a.Y - b.Y);
        }

        public override string ToString()
        {
            return String.Format("({0},{1})", X, Y);
        }

        public double Length2
        {
            get
            {
                return m_x * m_x + m_y * m_y;
            }
        }
    }

    public class GameBoard
    {
        private Position m_size;
        private HexGrid m_grid;
        private enCellType[,] m_cell_types;

        List<GameCamp> m_camps = new List<GameCamp>();
        List<GameTower> m_towers = new List<GameTower>();
        List<GameAgent> m_agents = new List<GameAgent>();
        List<GameAgentLauncher> m_launchers = new List<GameAgentLauncher>();

        public GameBoard(int rows, int cols, Position size)
        {
            m_size = size;
            
            m_grid = new HexGrid(rows, cols, size);

            m_cell_types = new enCellType[rows, cols];
        }

        public enCellType this[int i, int j]
        {
            get
            {
                return m_cell_types[i, j];
            }
            set
            {
                m_cell_types[i, j] = value;
            }
        }

        public HexGrid Grid
        {
            get
            {
                return m_grid;
            }
        }

        public Position Size
        {
            get
            {
                return m_size;
            }
        }

        public List<GameTower> Towers
        {
            get 
            {
                return m_towers;
            }
        }

        public List<GameAgent> Agents
        {
            get
            {
                return m_agents;
            }
        }

        public List<GameCamp> Camps
        {
            get
            {
                return m_camps;
            }
        }

        public List<GameAgentLauncher> Launchers
        {
            get
            {
                return m_launchers;
            }
        }        

        public List<GridPosition> GetPath(GridPosition pos_from, GridPosition pos_to)
        {
            MyPathNode[,] grid = new MyPathNode[m_grid.Columns, m_grid.Rows];

            for (int i = 0; i < m_grid.Rows; i++)
            {
                for (int j = 0; j < m_grid.Columns; j++)
                {
                    grid[j, i] = new MyPathNode(i, j);
                }
            }

            PathSolver<MyPathNode, GameBoard> aStar = new PathSolver<MyPathNode, GameBoard>(grid, m_grid);

            IEnumerable<MyPathNode> path = aStar.Search(new Node(pos_from.Col, pos_from.Row), new Node(pos_to.Col, pos_to.Row), this);

            List<GridPosition> res = new List<GridPosition>();

            if (path != null)
            {
                foreach (MyPathNode p in path)
                {
                    res.Add(new GridPosition(p.y, p.x));
                }
            }

            return res;

        }
    }
}
