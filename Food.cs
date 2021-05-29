using System;
using System.Collections.Generic;
using System.Text;

namespace WojnaMrowisk
{
    class Food
    {
        public static int partsValue = 20;
        private int numParts;
        public int foodParts {
            get { return numParts; }
            set { numParts = value; }
        }
        private Pos position = new Pos();

        void destroy()
        {

        }

        public void setPos(int x, int y)
        {
            position.x = x;
            position.y = y;
        }
        public Pos getPos()
        {
            Pos pos = new Pos();
            pos.x = position.x;
            pos.y = position.y;
            return pos;
        }
    }
}
