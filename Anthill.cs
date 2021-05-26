using System;
using System.Collections.Generic;
using System.Text;

namespace WojnaMrowisk
{
    class Anthill : Colony
    {
        //private int[,] footprint;
        public int[,,] sizes = new int[,,]{
            { {7,7,7,7,7},
            {7,7,7,7,7},
            {7,3,4,3,7} },
            { {7,7,7,7,7},
            {7,7,3,7,7},
            {3,3,4,3,3} },
            { {7,7,3,7,7},
            {7,3,3,3,7},
            {3,3,4,3,3}}
        };
        private int hunger = 0;// 0 - ok, 100 - bad
        private int numUnits = 0;
        private int size = 0;
        private float reprodRate;
        private Pos position;
        public Pos Pos
        {
            get { return position; }
            set { position = value; }
        }
        public List<Ant> ants = new List<Ant>();
        public int getSize()
        {
            return size;
        }
        void destroy()
        {
        }
        void grow()
        {
            size++;
        }
        public void init(Map map, Anthill a)
        {
            spawnAnt(map, a);
        }
        public void spawnAnt(Map map, Anthill ah)
        {
            Pos posToSpawn = new Pos();
            for (int y = ah.position.y; y < ah.position.y + sizes.GetLength(2); y++)
            {
                for (int x = ah.position.x; x < ah.position.x + sizes.GetLength(1); x++)
                {
                    if (ah.position.x + sizes.GetLength(1) < map.dimensionX && ah.position.y + sizes.GetLength(2) < map.dimensionY)
                    {
                        if (map.gameBoard[x, y] == 4)
                        {
                            posToSpawn.x = x; posToSpawn.y = y;
                        }
                    }
                }
            }
            Ant ant = new Ant();
            ant.setPos(posToSpawn);
            ant.health = Health;
            ant.stOnV = 4;
            map.gameBoard[posToSpawn.x, posToSpawn.y] = 2;
            ants.Add(ant);
        }
    }
}
