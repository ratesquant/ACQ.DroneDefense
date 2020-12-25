using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ACQ.DroneDefenceGame;

namespace HexMinesweeper
{
    enum enCellStatus
    {
        Closed, Flagged, Open
    };
    class HexMinesweeper
    {
        HexGrid m_grid;
        int[,] m_board; // mine:-1, empty:0, digits:1-6  
        enCellStatus[,] m_board_status; //closed:0, open:1, flagged:2 
        bool m_IsGameOver = false;

        public HexMinesweeper(int rows, int cols, int mines, double cell_size)
        {
            m_grid = new HexGrid(rows, cols, cell_size);
            m_board = new int[rows, cols];
            m_board_status = new enCellStatus[rows, cols];

            //generate mines
            System.Random rnd = new System.Random();
            int[] vm = new int[rows * cols];
            for (int i = 0; i < Math.Min(mines, rows * cols); i++)
            {
                vm[i] = 1;
            }
            ShuffleArray(vm, rnd);

            for (int i = 0; i < vm.Length; i++)
            {
                if (vm[i] == 1)
                {
                    m_board[i / cols, i % cols] = -1;
                }
            }

            //generate hint digits
            for (int i = 0; i < m_grid.Rows; i++)
            {
                for (int j = 0; j < m_grid.Columns; j++)
                {
                    if (m_board[i, j] == -1)
                    {
                        for (int k = 0; k < HexGrid.NEIGHBORS_COUNT; k++)
                        {
                            int ni, nj;
                            m_grid.GetNeighbor(i, j, k, out ni, out nj);

                            if (m_grid.IsOnGrid(ni, nj))
                            {
                                if (m_board[ni, nj] >= 0)
                                    m_board[ni, nj] += 1;
                            }
                        }
                    }
                }
            }
        }

        protected bool TryOpenCell(int i, int j)
        {
            if (m_board[i, j] == -1)
            {
                m_IsGameOver = true;
            }
            if (m_board_status[i, j] == enCellStatus.Closed)
            {
                m_board_status[i, j] = enCellStatus.Open;
            }

            return !m_IsGameOver;
        }

        public bool TryOpenCell(double x, double y)
        {
            if (m_IsGameOver)
                return false;

            int row, col;

            m_grid.FindCell(x, y, out row, out col);

            bool status = false;

            if (isOnGrid(row, col))
            {
                status = TryOpenCell(row, col);
            }
            return status;
        }

        public void TryFlagCell(int i, int j)
        {
            if (isOnGrid(i, j))
            {
                enCellStatus status = m_board_status[i, j];
                if (status == enCellStatus.Closed)
                {
                    m_board_status[i, j] = enCellStatus.Flagged;
                }
                else if (status == enCellStatus.Flagged)
                    m_board_status[i, j] = enCellStatus.Closed;
            }
        }

        public bool isOnGrid(int i, int j)
        {
            return i >= 0 && i < m_grid.Rows && j >= 0 && j < m_grid.Columns;
        }

        /// <summary>
        /// Fisher-Yates shuffle, also known as the Knuth shuffle
        /// </summary>
        /// <param name="array"></param>
        public static void ShuffleArray(int[] array, System.Random rnd)
        {
            int n = array.Length;
            while (--n > 0)
            {
                int k = rnd.Next(n + 1);  // 0 <= k <= n (!)
                int temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }
        public HexGrid Grid
        {
            get
            {
                return m_grid;
            }
        }

        public int this[int i, int j]
        {
            get
            {
                return m_board[i, j];
            }
        }

        public bool isMine(int i, int j)
        {
            return m_board[i, j] == -1;
        }

        public bool GetHint(int i, int j, ref int hint)
        {
            if (isOnGrid(i, j) && isOpen(i, j))
            {
                hint = m_board[i, j];
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsGameOver
        {
            get 
            {
                return m_IsGameOver;
            }
        }

        public bool isFlagged(int i, int j)
        {
            return m_board_status[i, j] == enCellStatus.Flagged;
        }

        public bool isOpen(int i, int j)
        {
            return m_board_status[i, j] == enCellStatus.Open;
        }

        public bool isOpen(double x, double y) 
        {
            int row, col;

            m_grid.FindCell(x, y, out row, out col);

            bool status = false;

            if (isOnGrid(row, col))
            {
                status = isOpen(row, col);
            }
            return status;
        }
    }
}
