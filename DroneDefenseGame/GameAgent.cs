using System;
using System.Collections.Generic;
using System.Text;

namespace ACQ.DroneDefenceGame
{
    public abstract class GameAgent : IPositionable, IDestroyable, IMovable
    {
        protected Position m_position;
        double m_hp;
        public GameAgent(Position position, double hp = 100)
        {
            m_position = position;
            m_hp = hp;
        }

        public abstract void UpdatePosition(GameBoard board);

        public double HitPoints
        {
            get 
            {
                return m_hp;
            }
        }
        public Position Position
        {
            get
            {
                return m_position;
            }
        }
    }

    public class SlowWalkerAgent : GameAgent
    {
        double m_speed = 1;
        public SlowWalkerAgent(Position position) : base(position, 100) { }

        public override void UpdatePosition(GameBoard board)
        {
            if (board == null || board.Camps == null || board.Camps.Count == 0)
                return;

            GameCamp camp = board.Camps[0];

            double vx = (camp.Position.X - m_position.X);
            double vy = (camp.Position.Y - m_position.Y);
            double distance = Math.Sqrt(vx * vx + vy * vy);

            if(distance > m_speed) 
            {
                m_position.X = (float)(m_position.X + vx * m_speed / distance);
                m_position.Y = (float)(m_position.Y + vy * m_speed / distance);
            }
        }
    }
}
