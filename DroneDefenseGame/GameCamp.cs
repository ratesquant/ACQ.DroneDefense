using System;
using System.Collections.Generic;
using System.Text;

namespace ACQ.DroneDefenceGame
{
    public class GameCamp : IDestroyable, IGridPositionable
    {
        GridPosition m_position;
        double m_hp;
        public GameCamp(GridPosition position, double hp = 100)
        {
            m_position = position;
            m_hp = hp;
        }

        public double HitPoints
        {
            get
            {
                return m_hp;
            }
        }

        public GridPosition Position
        {
            get
            {
                return m_position;
            }
        }
    }
}
