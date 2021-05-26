using System;
using System.Collections.Generic;
using System.Text;

namespace WojnaMrowisk
{
    class Vector
    {
        private int xPos;
        private int yPos;

        private int startXPos;
        private int startYPos;
        private int endXPos;
        private int endYPos;
        public int x
        {
            get { return xPos; }
            set { xPos = value; }
        }
        public int y
        {
            get { return yPos; }
            set { yPos = value; }
        }
        public int startX
        {
            get { return startXPos; }
            set { startXPos = value; }
        }
        public int startY
        {
            get { return startYPos; }
            set { startYPos = value; }
        }
        public int endX
        {
            get { return endXPos; }
            set { endXPos = value; }
        }
        public int endY
        {
            get { return endYPos; }
            set { endYPos = value; }
        }
        public float distance()
        {
            return MathF.Sqrt(MathF.Pow(xPos, 2) + MathF.Pow(yPos, 2));
            //return MathF.Sqrt((xPos^2) + (yPos^2));
        }
        public static Vector CreateVector(Pos pos1, Pos pos2)
        {
            Vector vect = new Vector();
            vect.xPos = pos2.x - pos1.x;
            vect.yPos = pos2.y - pos1.y;

            vect.startXPos = pos1.x;
            vect.startYPos = pos1.y;

            vect.endYPos = pos2.y;
            vect.endXPos = pos2.x;
            return vect;

        }

    }
}
