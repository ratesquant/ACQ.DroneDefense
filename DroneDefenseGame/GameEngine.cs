using System;

namespace ACQ.DroneDefenceGame
{
    public enum enGameStatus 
    {
        Live,
        Over
    }

    public class GameEngine
    {
        private GameBoard m_board;
        private enGameStatus m_status;
        public GameEngine(Position size) 
        {
            m_board = new GameBoard(11, 12, size);
            m_status = enGameStatus.Live;
        }

        public GameBoard Board 
        {
            get 
            {
                return m_board;
            }
        }

        public void Play()
        {
            foreach (GameAgent agent in m_board.Agents)
            {
                if (agent.isAlive)
                {
                    agent.Update(m_board);
                }
            }

            foreach (GameTower tower in m_board.Towers)
            {
                tower.Update(m_board);
            }

            foreach (GameAgentLauncher launcher in m_board.Launchers)
            {
                launcher.Update(m_board);
            }
        }

        public enGameStatus Status 
        {
            get 
            {
                return m_status;
            }
        }        
    }
}
