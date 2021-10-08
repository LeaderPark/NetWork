using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    class UserData
    {
        public int uid;

        public int hp;
        public int level;

        public float x;
        public float y;
        public float speed;
        public float attackPower;
        public float defencePower;

        UserData()
        {

        }

        public void SetPosition(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public bool IsCollide(float x, float y, float r)
        {
            float dist = ((x - this.x) * (x - this.x)) + ((y - this.y) * (y - this.y));
            return r * r <= dist;
        }
    }
}
