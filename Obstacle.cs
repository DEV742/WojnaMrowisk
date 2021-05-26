using System;
using System.Collections.Generic;
using System.Text;

namespace WojnaMrowisk
{
    class Obstacle
    {
        public int[,] footprint;
        private char texture;
        private Pos position;

        public Pos getPos()
        {
            return position;
        }
        public void setTexture(char ch)
        {
            texture = ch;
        }
    }
}
