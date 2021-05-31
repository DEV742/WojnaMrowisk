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
        private int hunger = 100;// 100 - ok, 0 - bad
        public int Hunger
        {
            get { return hunger; }
            set { hunger = value; }
        }
        private int numUnits = 0;
        private int size = 0;
        public int Size {
            get { return size; }
            set { size = value; }
        }
        private float reprodRate;
        private Pos position;
        public Ant queen;
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
        public Pos getAhPos()
        {
            Pos ps = new Pos();
            ps.x = position.x + 2;
            ps.y = position.y + 2;
            return ps;
        }
        void destroy()
        {
        }
        void grow()
        {
            size++;
        }

        public void evaluateAnthillLogic(Map map)
        {
            if (hunger > 100) {
                hunger = 100;
            }
            if (queen != null)
            {
                if (Simulation.step % 2 == 0 && hunger > 0) {
                    hunger -= (int)MathF.Round(ants.Count/2);
                }
                if (hunger > 50 && ants.Count > size+1 * 4 && Simulation.step % 40 == 0 && size < sizes.GetLength(0)-1)//upgrading an anthill
                {
                    size++;
                    for (int y = 0; y < sizes.GetLength(2); y++)
                    {
                        for (int x = 0; x < sizes.GetLength(1); x++)
                        {
                            if (map.DimensionX > position.x + y && map.DimensionY > position.y + x)
                            {
                                map.gameBoard[position.x + y, position.y + x] = sizes[getSize(), x, y];
                            }
                        }
                    }
                }
                if(hunger > 25 && Simulation.step % 50 == 0)
                {
                    spawnAnt(map, this, false);
                }
            }
            else { 
                //destroy();
            }
        }
        public void init(Map map, Anthill a)
        {
            spawnAnt(map, a, false);
            spawnAnt(map, a, true);
        }
        public void spawnAnt(Map map, Anthill ah, bool isQueen)
        {
            Pos posToSpawn = new Pos();
            posToSpawn.x = getAhPos().x; posToSpawn.y = getAhPos().y;
            Ant ant = new Ant();
            ant.setPos(posToSpawn);
            ant.health = Health;
            ant.stOnV = 4;
            ant.antsAnthill = this;
            map.gameBoard[posToSpawn.x, posToSpawn.y] = 2;
            ant.isQueen = isQueen;
            if (isQueen) {
                queen = ant;
            }
            ants.Add(ant);
        }
        public Colony getColony() { 
            return this;
        }
    }
}
