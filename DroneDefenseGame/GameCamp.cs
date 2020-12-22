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
        public bool isAlive
        {
            get
            {
                return m_hp > 0;
            }
        }

        public GridPosition Position
        {
            get
            {
                return m_position;
            }
        }

        public void DoDamage(double damage, enDamageType damage_type)
        {
            if (damage_type == enDamageType.Physical)
            {
                m_hp -= damage;
            }
        }
    }
}
