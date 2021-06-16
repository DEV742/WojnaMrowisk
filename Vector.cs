using System;

namespace WojnaMrowisk
{
    internal class Vector
    {
        public int x { get; set; }

        public int y { get; set; }

        public int startX { get; set; }

        public int startY { get; set; }

        public int endX { get; set; }

        public int endY { get; set; }

        //A method to calculate distance.
        public float distance()
        {
            return MathF.Sqrt(MathF.Pow(x, 2) + MathF.Pow(y, 2));
        }

        //A static metod to create a vector, storing not only the vector coordinates, but also the start&end coords.
        public static Vector CreateVector(Pos pos1, Pos pos2)
        {
            var vect = new Vector
            {
                x = pos2.x - pos1.x,
                y = pos2.y - pos1.y,
                startX = pos1.x,
                startY = pos1.y,
                endY = pos2.y,
                endX = pos2.x
            };


            return vect;
        }
    }
}