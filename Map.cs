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
        public int dimensionX = 60;
        public int dimensionY = 25;
        public Pos currentWaypoint = new Pos();
        // = new int[Console.WindowWidth, Console.WindowHeight];
        public Food food;

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
            food = sfood;
            //return sfood;
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
            //gameBoard[food.getPos().x, food.getPos().y] = 0;
            Food foodResp = new Food();
            Pos positionToSpawn = pickRandomPoint();
            while (gameBoard[positionToSpawn.x, positionToSpawn.y] != 0)
            {
                positionToSpawn = pickRandomPoint();
            }
            foodResp.setPos(positionToSpawn.x, positionToSpawn.y);
            gameBoard[positionToSpawn.x, positionToSpawn.y] = 10;
            food = foodResp;
            //return foodResp;
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
