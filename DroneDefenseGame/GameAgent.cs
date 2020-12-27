using System;
using System.Collections.Generic;
using System.Text;

namespace ACQ.DroneDefenceGame
{
    public abstract class GameAgent : IPositionable, IDestroyable, IMovable, ICloneable
    {
        protected Position m_position;
        protected List<GridPosition> m_path;
        double m_hp;
        public GameAgent(Position position, double hp = 100)
        {
            m_position = position;            
            m_hp = hp;
        }

        public abstract void Update(GameBoard board);

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


        public Position Position
        {
            get
            {
                return m_position;
            }
            set
            {
                m_position = value;
            }
        }

        public List<GridPosition> Path
        {
            get 
            {
                return m_path;
            }
            set 
            {
                m_path = value;
            }
        }


        public void DoDamage(double damage, enDamageType damage_type)
        {
            m_hp -= damage;
        }
        public abstract object Clone();
    }

    public class SlowWalkerAgent : GameAgent
    {
        float m_speed = 1;
        public SlowWalkerAgent(Position position) : base(position, 100) { }

        public override void Update(GameBoard board)
        {
            if (board == null || board.Camps == null || board.Camps.Count == 0)
                return;

            if (Path != null)
            {
                //walk along the path
                int row, col;
                board.Grid.FindCell(m_position.X, m_position.Y, out row, out col);

                int path_index= 0;
                for (int i = 0; i < Path.Count; i++)
                {
                    //we need to find closest - not exact match
                    if (Path[i].Row == row && Path[i].Col == col)
                    {
                        path_index = i;
                        break;
                    }
                }

                int next_path_index = Math.Min(path_index + 1, Path.Count - 1);

                float vx, vy;
                board.Grid.GetCellCenter(Path[next_path_index].Row, Path[next_path_index].Col, out vx, out vy);

                vx -= m_position.X;
                vy -= m_position.Y;
                float distance = (float)Math.Sqrt(vx * vx + vy * vy);

                if (distance > m_speed)
                {
                    m_position.X = (float)(m_position.X + vx * m_speed / distance);
                    m_position.Y = (float)(m_position.Y + vy * m_speed / distance);
                }

            }
            else
            {
                GameCamp camp = board.Camps[0];

                float vx, vy;
                board.Grid.GetCellCenter(camp.Position.Row, camp.Position.Col, out vx, out vy);

                vx -= m_position.X;
                vy -= m_position.Y;
                float distance = (float)Math.Sqrt(vx * vx + vy * vy);

                if (distance > m_speed)
                {
                    m_position.X = (float)(m_position.X + vx * m_speed / distance);
                    m_position.Y = (float)(m_position.Y + vy * m_speed / distance);
                }
            }
        }
        public override object Clone()
        {
            var clone = new SlowWalkerAgent(this.Position);           
            return clone;
        }
    }
}
