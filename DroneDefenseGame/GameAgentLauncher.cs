using System;
using System.Collections.Generic;
using System.Text;

namespace ACQ.DroneDefenceGame
{
    public abstract class GameAgentLauncher : IGridPositionable
    {
        private GridPosition m_position;       

        public GameAgentLauncher(GridPosition position)
        {
            m_position = position;
        }
      
        public abstract void Update(GameBoard board);
        
        public GridPosition Position
        {
            get
            {
                return m_position;
            }
        }
    }

    public class StaticLauncher : GameAgentLauncher
    {
        GameAgent m_agent;
        int m_launch_delay;
        int m_counter;

        List<GridPosition> m_path = new List<GridPosition>();
        Position m_agent_position;
        public StaticLauncher(GridPosition position, GameAgent agent, int launch_delay) : base(position)
        {
            m_agent = agent;
            m_launch_delay = launch_delay;
            m_counter = 0;
        }
        public override void Update(GameBoard board)
        {
            //compute path
            if (m_counter == 0)
            {
                GameCamp camp = board.Camps[0];

                m_agent_position = board.Grid.GetCellCenter(Position);
                m_path = board.GetPath(this.Position, camp.Position);
            }

            if (m_counter % m_launch_delay == 0)
            {
                GameAgent agent = m_agent.Clone() as GameAgent;

                agent.Position = m_agent_position;
                agent.Path = m_path;

                board.Agents.Add(agent);
            }
            m_counter++;
        }
    }

    public class ScheduledLauncher : GameAgentLauncher
    {
        private SortedList<int, GameAgent> m_schedule;
        public ScheduledLauncher(GridPosition position, SortedList<int, GameAgent> launch_schedule) : base(position)
        {
            m_schedule = launch_schedule;
        }
        public override void Update(GameBoard board)
        { }
    }

}
