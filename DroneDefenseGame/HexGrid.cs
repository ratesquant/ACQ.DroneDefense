using System;
using System.Collections.Generic;
using System.Text;

namespace ACQ.DroneDefenceGame
{
    public struct GridIndex
    {
        private readonly int m_row;
        private readonly int m_col;

        public GridIndex(int row, int col)
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
        }
        public int Col
        {
            get
            {
                return m_col;
            }
        }

        public override string ToString()
        {
            return String.Format("({0}, {1})", m_row, m_col);
        }
    }
    /// <summary>
    /// Hex Grid Class
    /// https://www.redblobgames.com/grids/hexagons/
    /// </summary>
    public class HexGrid
    {
        // double width row_d = row, col_d = 2*col + row % 2
        //
        readonly int m_rows, m_cols;
        readonly double m_size; //size of the hex-edge
        readonly double m_size_x, m_size_y;
        readonly double m_aspect_ratio;

        public const int NEIGHBORS_COUNT = 6;

        private const double SQRT3 = 1.7320508075688772935274463415059; //Math.Sqrt(3);
        private const double SQRT3h = 0.86602540378443864676372317075294;// 0.5*Math.Sqrt(3);

        readonly int[,] m_neighbor_offsets  = new int[,] { { +2, 0 }, { +1, -1 }, { -1, -1 }, { -2, 0 }, { -1, +1 }, { +1, +1 } };
        readonly double[,] m_vertex_offsets = new double[,] { { -SQRT3h, -0.5 }, { 0, -1 }, { SQRT3h, -0.5 }, { SQRT3h, 0.5 }, { 0, 1 }, { -SQRT3h, 0.5 } }; //clockwise from 10 o'clock -> (10, 12, 2, 4, 6, 8)
        

        // width = sqrt(3) * size
        // heigth = 2 * size

        public HexGrid(int rows, int cols, Position size)
        {
            m_rows = rows;
            m_cols = cols;

            m_size = size.X / HexGrid.TotalWidth(m_cols);

            m_aspect_ratio = size.Y / (m_size * HexGrid.TotalHeight(m_rows));
            //m_aspect_ratio = 1.0;
        }

        public static double TotalWidth(int columns)
        {
            return SQRT3 * (columns + 0.5);
        }
        public static double TotalHeight(int rows)
        {
            return (2 + 1.5 * (rows - 1));
        }

        public static double AspectRatio(int rows, int columns)
        {
            return TotalHeight(rows)/ TotalWidth(columns);
        }

        public int Rows
        {
            get 
            {
                return m_rows;
            }
        }

        public int Columns
        {
            get
            {
                return m_cols;
            }
        }

        public int CellCount
        {
            get 
            {
                return m_rows * m_cols;
            }
        }
        public Position GetCellCenter(int row, int col)
        {
            return new Position(SQRT3h * (1 + 2*col + (row % 2) ) * m_size, m_aspect_ratio * (m_size * (1 + row * 1.5  )));
        }

        public bool IsOnGrid(int row, int col)
        {
            return row >= 0 && row < m_rows && col >= 0 && col < m_cols;
        }

        public GridIndex GetNeighbor(GridIndex cell, int direction)
        {            
            System.Diagnostics.Debug.Assert(direction >= 0 && direction < NEIGHBORS_COUNT);            

            return new GridIndex(cell.Row + m_neighbor_offsets[direction, 0], cell.Col + m_neighbor_offsets[direction, 1]);
        }

        public Position GetVertex(int row, int col, int direction)
        {
            Position center = GetCellCenter(row, col);

            return new Position(center.X + m_size * m_vertex_offsets[direction, 0], center.Y + m_aspect_ratio * m_size * m_vertex_offsets[direction, 1]);
        }

        /// <summary>
        /// Convert coordinates to cell
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public GridIndex GetCell(Position position)
        {
            double side = m_size * 3 * 0.5;
            int ci = (int)Math.Floor(position.X / side);
            int cx = (int)(position.X - side * ci);

            double height = SQRT3 * m_size;
            int ty = (int)(position.Y - (ci % 2) * height / 2);
            int cj = (int)Math.Floor((float)ty / (float)height);
            int cy = (int)(position.Y - height * cj);

            if (cx > Math.Abs(m_size / 2 - m_size * cy / height))
            {
                return new GridIndex(cj, ci);
            }
            else
            {
                return new GridIndex(cj + (ci % 2) - ((cy < height / 2) ? 1 : 0), ci - 1);
            }
        }
    }
}
