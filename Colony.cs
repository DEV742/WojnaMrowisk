using System;
using System.Collections.Generic;
using System.Text;

namespace WojnaMrowisk
{
    class Colony
    {
        private float speed = 1;
        public float Speed
        {
            get { return speed; }
            set { speed = value; }
        }
        private float foodDetectionRange = 15f;
        public float foodRange
        {
            get { return foodDetectionRange; }
            set { foodDetectionRange = value; }
        }
        private float alertRange = 2f;
        private int health = 100;
        private float maxDistFromAnthill = 4f;
        public float distFromAnthill {
            get { return maxDistFromAnthill; }
            set { maxDistFromAnthill = value; }
        }
        public int Health
        {
            get { return health; }
            set { health = value; }
        }
        private int damage = 25;
        private float reprodRate = 10f;
        public List<Anthill> anthills = new List<Anthill>();
        void die()
        {

        }

        public void initialize(Map map)
        { //automatically create an anthill at a random spot and place a queen
            Anthill a = new Anthill();
            Pos pos = map.pickRandomPoint();
            bool overlaps = true;
            bool test = false;
            while (overlaps)
            {
                test = false;
                for (int y = 0; y < a.sizes.GetLength(2); y++)
                {
                    for (int x = 0; x < a.sizes.GetLength(1); x++)
                    {
                        if (map.DimensionX > pos.x + y && map.DimensionY > pos.y + x)
                        {
                            if (map.gameBoard[pos.x + y, pos.y + x] != 0)
                            {
                                test = true;
                            }
                        }
                    }
                }
                if (test == false)
                {
                    overlaps = false;
                }
                else
                {
                    pos = map.pickRandomPoint();
                }
            }

            for (int y = 0; y < a.sizes.GetLength(2); y++)
            {
                for (int x = 0; x < a.sizes.GetLength(1); x++)
                {
                    if (map.DimensionX > pos.x + y && map.DimensionY > pos.y + x)
                    {
                        map.gameBoard[pos.x + y, pos.y + x] = a.sizes[a.getSize(), x, y];
                    }
                }
            }
            anthills.Add(a);
            a.Pos = pos;
            a.init(map, a);
        }
    }
}
