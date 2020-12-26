using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ACQ.DroneDefenceGame;

namespace HexMinesweeper
{
    enum enDifficultyLevel
    {
        Beginner,
        Indermediate,
        Expert,
        Custom
    }

    enum enCellStatus
    {
        Closed, Flagged, Open
    };

    enum enGameStatus
    {
        Won, Lost, InProgress
    }
    class HexMinesweeper
    {
        HexGrid m_grid;
        int[,] m_board; // mine:-1, empty:0, digits:1-6  
        enCellStatus[,] m_board_status; //closed:0, open:1, flagged:2         
        int m_activated_mine = -1; //game over if mine gets activated
        int m_total_mines;

        public HexMinesweeper(int rows, int cols, int mines, double cell_size)
        {
            m_grid = new HexGrid(rows, cols, cell_size);
            m_board = new int[rows, cols];
            m_board_status = new enCellStatus[rows, cols];

            m_total_mines = Math.Min(mines, rows * cols);

            //generate mines
            System.Random rnd = new System.Random();
            int[] vm = new int[rows * cols];
            for (int i = 0; i < m_total_mines; i++)
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

        public int MineCount
        {
            get
            {
                return m_total_mines;
            }
        }

        public int FlagCount
        {
            get
            {
                int flag_count = 0;
                for (int i = 0; i < m_grid.Rows; i++)
                {
                    for (int j = 0; j < m_grid.Columns; j++)
                    {
                        if (m_board_status[i, j] == enCellStatus.Flagged)
                        {
                            flag_count++;
                        }
                    }
                }
                return flag_count;
            }
        }

        public bool TryGetActivatedMine(ref int i, ref int j) 
        {
            if (m_activated_mine > 0)
            {
                i = m_activated_mine / m_grid.Columns;
                j = m_activated_mine % m_grid.Columns;
                return true;
            }
            else
            {
                return false;
            }
        }

        protected bool TryOpenCell(int i, int j)
        {
            bool status = false;
            
            if (m_board_status[i, j] == enCellStatus.Closed) //check that cell is closed
            {
                if (m_board[i, j] == -1) // check if there is a mine
                {
                    m_activated_mine = j + m_grid.Columns * i;
                    status = true;
                }

                m_board_status[i, j] = enCellStatus.Open;
                status = true;

                //also open neigbors for empty cells
                if (m_board[i, j] == 0)
                {
                    SortedSet<int> opened_cells = new SortedSet<int>();

                    opened_cells.Add(j + m_grid.Columns * i);

                    bool check_again;
                    do
                    {
                        check_again = false;
                        foreach (int index in opened_cells.ToList())
                        {
                            int ki = index / m_grid.Columns;
                            int kj = index % m_grid.Columns;

                            for (int k = 0; k < HexGrid.NEIGHBORS_COUNT; k++)
                            {
                                int ni, nj;
                                if (m_grid.GetNeighbor(ki, kj, k, out ni, out nj) && m_board[ni, nj] >=0 && m_board_status[ni, nj] == enCellStatus.Closed)
                                {
                                    m_board_status[ni, nj] = enCellStatus.Open;

                                    if (m_board[ni, nj] == 0)
                                    {
                                        opened_cells.Add(nj + m_grid.Columns * ni);
                                        check_again = true;
                                    }
                                }
                            }
                        }
                    } while (check_again);
                }
            }

            return status;
        }

        public bool TryOpenCell(double x, double y)
        {
            if (IsGameOver)
                return false;

            int i, j;

            m_grid.FindCell(x, y, out i, out j);

            bool status = false;

            if (isOnGrid(i, j))
            {
                status = TryOpenCell(i, j);
            }
            return status;
        }

        protected bool TryFlagCell(int i, int j)
        {
            bool flagged = false;
            
            enCellStatus status = m_board_status[i, j];
            if (status == enCellStatus.Closed)
            {
                m_board_status[i, j] = enCellStatus.Flagged;
                flagged = true;
            }
            else if (status == enCellStatus.Flagged)
            {
                m_board_status[i, j] = enCellStatus.Closed;
                flagged = true;
            }
            return flagged;
        }

        public bool TryFlagCell(double x, double y)
        {
            bool status = false;
            int i, j;

            m_grid.FindCell(x, y, out i, out j);

            if (isOnGrid(i, j))
            {
                status =  TryFlagCell(i, j);             
            }
            return status;
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

        public double Completion
        {
            get
            {
                int closed_cells = 0;

                for (int i = 0; i < m_grid.Rows; i++)
                {
                    for (int j = 0; j < m_grid.Columns; j++)
                    {
                        if (m_board_status[i, j] == enCellStatus.Closed && m_board[i, j] >= 0)
                            closed_cells++;
                    }                    
                }
                int empty_cells = m_grid.Rows * m_grid.Columns - m_total_mines;

                return (double)(empty_cells - closed_cells)/ empty_cells;
            }
        }

        public bool IsGameOver
        {
            get 
            {
                return Status != enGameStatus.InProgress;
            }
        }

        public enGameStatus Status
        {
            get 
            {
                if (m_activated_mine >= 0)
                    return enGameStatus.Lost;

                for (int i = 0; i < m_grid.Rows; i++)
                {
                    for (int j = 0; j < m_grid.Columns; j++)
                    {
                        if (m_board_status[i, j] == enCellStatus.Closed && m_board[i, j] >= 0)
                            return enGameStatus.InProgress;
                    }
                }
                return enGameStatus.Won;
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

