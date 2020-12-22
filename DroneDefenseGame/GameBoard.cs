using System;
using System.Collections.Generic;
using System.Text;

namespace ACQ.DroneDefenceGame
{
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
        void UpdatePosition(GameBoard board);
    }

    public interface IDestroyable
    { 
        double HitPoints { get; }
    }

    public class GridPosition
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
    public class Position
    {
        private double m_x;
        private double m_y;

        public Position(double x, double y) 
        {
            m_x = x;
            m_y = y;

        }

        public double X
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

        public double Y
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
    }

    public class GameBoard
    {
        private Position m_size;
        private HexGrid m_grid;

        List<GameCamp> m_camps = new List<GameCamp>();
        List<GameTower> m_towers = new List<GameTower>();
        List<GameAgent> m_agents = new List<GameAgent>();

        public GameBoard(int rows, int cols, Position size)
        {
            m_size = size;
            
            m_grid = new HexGrid(rows, cols, size);
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
    }
}
