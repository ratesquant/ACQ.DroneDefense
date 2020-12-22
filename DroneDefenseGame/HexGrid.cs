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
        readonly double m_size; //x-size of the hex-edge, y-size  is m_size * m_aspect_ratio
        readonly double m_aspect_ratio;

        public const int NEIGHBORS_COUNT = 6;

        private const double SQRT3 = 1.7320508075688772935274463415059; //Math.Sqrt(3);
        private const double SQRT3h = 0.86602540378443864676372317075294;// 0.5*Math.Sqrt(3);

        readonly int[,] m_neighbor_offsets_odd  = new int[,] { { -1, 1 }, { 0, 1 }, { 1, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 } }; //clockwise from 1 o'clock -> (1, 3, 5, 7, 9, 11)
        readonly int[,] m_neighbor_offsets_even = new int[,] { { -1, 0 }, { 0, 1 }, { 1, 0 }, { 1, -1 }, { 0, -1 }, { -1, -1 } }; //clockwise from 1 o'clock -> (1, 3, 5, 7, 9, 11)
        readonly double[,] m_vertex_offsets = new double[,] { { 0, -1 }, { SQRT3h, -0.5 }, { SQRT3h, 0.5 }, { 0, 1 }, { -SQRT3h, 0.5 }, { -SQRT3h, -0.5 } }; //clockwise from 12 o'clock -> (12, 2, 4, 6, 8, 10)


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

        /// <summary>
        /// Number of cells
        /// </summary>
        public int Count
        {
            get 
            {
                return m_rows * m_cols;
            }
        }

        /// <summary>
        /// cell x-size 
        /// </summary>
        public double Size
        {
            get
            {
                return m_size;
            }
        }
        public void GetCellCenter(int row, int col, out double x, out double y)
        {
            x = SQRT3h * (1 + 2 * col + (row % 2)) * m_size;
            y = m_aspect_ratio * (m_size * (1 + row * 1.5));            
        }

        public void GetCellCenter(GridPosition pos, out double x, out double y)
        {
            GetCellCenter(pos.Row, pos.Col, out x, out y);
        }

        public Position GetCellCenter(GridPosition pos)
        {
            double x, y;
            GetCellCenter(pos.Row, pos.Col, out x, out y);
            return new Position(x, y);
        }

        public bool IsOnGrid(int row, int col)
        {
            return row >= 0 && row < m_rows && col >= 0 && col < m_cols;
        }
        /// <summary>
        /// Iterate grid position of the neighbor cells, not all neighbors of boundary cells are on the grid, check IsOnGrid  
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="direction"></param>
        /// <param name="n_row"></param>
        /// <param name="n_col"></param>
        public void GetNeighbor(int row, int col, int direction, out int n_row, out int n_col)
        {            
            System.Diagnostics.Debug.Assert(direction >= 0 && direction < NEIGHBORS_COUNT);

            if (row % 2 == 0)
            {
                n_row = row + m_neighbor_offsets_even[direction, 0];
                n_col = col + m_neighbor_offsets_even[direction, 1];            }
            else
            {
                n_row = row + m_neighbor_offsets_odd[direction, 0];
                n_col = col + m_neighbor_offsets_odd[direction, 1];
            }
        }

        public void GetVertex(int row, int col, int direction, out double x, out double y)
        {
            double x_center, y_center;

            GetCellCenter(row, col, out x_center, out y_center);

            x = x_center + m_size * m_vertex_offsets[direction, 0];
            y = y_center + m_aspect_ratio * m_size * m_vertex_offsets[direction, 1];
        }

        /// <summary>
        /// Convert coordinates to cell
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public void FindCell(double x, double y, out int row, out int col)
        {
            double rx = (x / (SQRT3h * m_size));
            double ry = (y / (0.5 * m_size * m_aspect_ratio));            

            int ci = (int)Math.Floor(ry);
            int cj = (int)Math.Floor(rx);

            row = ci / 3;            

            //handle triangles on top and bottom  
            if (ci % 3 == 0 )
            {
                if ((cj + row % 2) % 2 == 0)
                {
                    if ((ry - ci) < (1 - (rx - cj)))
                        row = row - 1;
                }
                else
                {
                    if ((ry - ci) < (rx - cj))
                        row = row - 1;
                }
            }

            col = (cj + row % 2)/2 - row % 2;
        }
    }
}
