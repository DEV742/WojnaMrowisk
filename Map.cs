using System;
using System.Collections.Generic;
using System.Text;

namespace WojnaMrowisk
{
    class Map
    {
        /*Map idea: every thing has a certain ID, for example, an ant is a 1,
        a piece of food is 2 etc.*/
        private float foodSpawnRate;
        private float foodRespawnDelay;
        private int numBases;
        private int numColonies;
        public int[,] gameBoard;
        private int dimensionX;
        private int dimensionY;
        public int DimensionX
        {
            get { return dimensionX; }
            set { dimensionX = value; }
        }
        public int DimensionY
        {
            get { return dimensionY; }
            set { dimensionY = value; }
        }
        public Pos currentWaypoint = new Pos();
        // = new int[Console.WindowWidth, Console.WindowHeight];
        public Food food;
        public static List<Food> foods = new List<Food>();

        public void spawnFood()
        {
            Food sfood = new Food();
            Pos positionToSpawn = pickRandomPoint();
            while (gameBoard[positionToSpawn.x, positionToSpawn.y] != 0)
            {
                positionToSpawn = pickRandomPoint();
            }
            sfood.setPos(positionToSpawn.x, positionToSpawn.y);
            gameBoard[positionToSpawn.x, positionToSpawn.y] = 10;
            sfood.foodParts = rand.generate(1, 5);
            foods.Add(sfood);
            //return sfood;
        }
        public void destroyFood(int index)
        {
            if (gameBoard[foods[index].getPos().x, foods[index].getPos().y] != 2)
            {
                gameBoard[foods[index].getPos().x, foods[index].getPos().y] = 0;
            }
            else
            {
                foreach (Colony col in Simulation.colonies)
                {
                    foreach (Anthill ah in col.anthills)
                    {
                        foreach (Ant ant in ah.ants)
                        {
                            if (ant.getPos().x == foods[index].getPos().x && ant.getPos().y == foods[index].getPos().y)
                            {
                                ant.stOnV = 0;
                            }
                        }
                    }
                }
                foods.RemoveAt(index);
            }
        }
        public void destroyAnthill(Anthill anthill)
        {
            for (int y = 0; y < anthill.sizes.GetLength(2); y++)
            {
                for (int x = 0; x < anthill.sizes.GetLength(1); x++)
                {
                    if (gameBoard[anthill.Pos.x + y, anthill.Pos.y + x] == 2) {
                        foreach (Colony col in Simulation.colonies) {
                            foreach (Anthill ah in col.anthills) {
                                foreach (Ant a in ah.ants) {
                                    if (a.getPos().x == anthill.Pos.x + y && a.getPos().y == anthill.Pos.y + x) {
                                        a.stOnV = 0;
                                    }
                                }
                            }
                        }
                    }
                    gameBoard[anthill.Pos.x + y, anthill.Pos.y + x] = 0;
                }
            }
        }
        public void spawnObstacles(List<Obstacle> obst)
        {
            //Random rand = new Random();
            Pos spawnPos = new Pos();
            int numObst = rand.generate(5, 15);//spawning between 5 and 15 obstacles
            int randChoice = 0;
            for (int i = 0; i < numObst; i++)
            {
                spawnPos = pickRandomPoint();
                randChoice = rand.generate(0, obst.Count);
                //obst[randChoice];
                for (int y = 0; y < obst[randChoice].footprint.GetLength(1); y++)
                {
                    for (int x = 0; x < obst[randChoice].footprint.GetLength(0); x++)
                    {
                        if (dimensionX > spawnPos.x + y && dimensionY > spawnPos.y + x && obst[randChoice].footprint[x, y] != 0)
                        {
                            gameBoard[spawnPos.x + y, spawnPos.y + x] = obst[randChoice].footprint[x, y];
                        }
                    }
                }
            }

        }
        public void respawnFood()
        {
            Food sfood = new Food();
            Pos positionToSpawn = pickRandomPoint();
            int i = 0;
            while (gameBoard[positionToSpawn.x, positionToSpawn.y] != 0 && i < 20)
            {
                positionToSpawn = pickRandomPoint();
                i++;
            }
            sfood.setPos(positionToSpawn.x, positionToSpawn.y);
            gameBoard[positionToSpawn.x, positionToSpawn.y] = 10;
            sfood.foodParts = rand.generate(1, 5);
            food = sfood;
            //return sfood;
        }
        public Pos pickRandomPoint()
        {
            Pos position = new Pos();
            //Random randY = new Random();
            position.x = rand.generate(0, dimensionX - 1);
            position.y = rand.generate(0, dimensionY - 1);

            return position;
        }
        public Pos pickRandomWaypoint()
        {
            Pos position = new Pos();

            if (currentWaypoint != null)
            {
                gameBoard[position.x, position.y] = 0;
                currentWaypoint = null;
            }
            //Random randY = new Random();
            position.x = rand.generate(0, dimensionX - 1);
            position.y = rand.generate(0, dimensionY - 1);
            gameBoard[position.x, position.y] = 11;
            currentWaypoint = position;
            return position;
        }
    }
}
