using System.Collections.Generic;

namespace WojnaMrowisk
{
    internal class Map
    {
        public static List<Food> foods = new List<Food>();
        public Pos currentWaypoint = new Pos();

        // = new int[Console.WindowWidth, Console.WindowHeight];
        public Food food;


        /*Map idea: every thing has a certain ID, for example, an ant is a 1,
        a piece of food is 2 etc.*/
        public int[,] gameBoard;

        public int DimensionX { get; set; }

        public int DimensionY { get; set; }

        public void spawnFood()
        {
            var sfood = new Food();
            var positionToSpawn = pickRandomPoint();
            while (gameBoard[positionToSpawn.x, positionToSpawn.y] != 0) positionToSpawn = pickRandomPoint();
            sfood.SetPos(positionToSpawn.x, positionToSpawn.y);
            gameBoard[positionToSpawn.x, positionToSpawn.y] = 10;
            sfood.foodParts = rand.generate(1, 5);
            foods.Add(sfood);
            //return sfood;
        }

        public void destroyFood(int index)
        {
            if (gameBoard[foods[index].GetPos().x, foods[index].GetPos().y] != 2)
            {
                gameBoard[foods[index].GetPos().x, foods[index].GetPos().y] = 0;
            }
            else
            {
                foreach (var col in Simulation.colonies)
                foreach (var ah in col.anthills)
                foreach (var ant in ah.ants)
                    if (ant.GetPos().x == foods[index].GetPos().x && ant.GetPos().y == foods[index].GetPos().y)
                        ant.CurrentlyStandingOn = 0;
                foods.RemoveAt(index);
            }
        }

        public void DestroyAnthill(Anthill anthill)
        {
            for (var y = 0; y < anthill.sizes.GetLength(2); y++)
            for (var x = 0; x < anthill.sizes.GetLength(1); x++)
            {
                if (gameBoard[anthill.Pos.x + y, anthill.Pos.y + x] == 2)
                    foreach (var col in Simulation.colonies)
                    foreach (var ah in col.anthills)
                    foreach (var a in ah.ants)
                        if (a.GetPos().x == anthill.Pos.x + y && a.GetPos().y == anthill.Pos.y + x)
                            a.CurrentlyStandingOn = 0;
                gameBoard[anthill.Pos.x + y, anthill.Pos.y + x] = 0;
            }
        }

        public void spawnObstacles(List<Obstacle> obst)
        {
            //Random rand = new Random();
            var numObst = rand.generate(5, 15); //spawning between 5 and 15 obstacles
            for (var i = 0; i < numObst; i++)
            {
                var spawnPos = pickRandomPoint();
                var randChoice = rand.generate(0, obst.Count);
                //obst[randChoice];
                for (var y = 0; y < obst[randChoice].footprint.GetLength(1); y++)
                for (var x = 0; x < obst[randChoice].footprint.GetLength(0); x++)
                    if (DimensionX > spawnPos.x + y && DimensionY > spawnPos.y + x &&
                        obst[randChoice].footprint[x, y] != 0)
                        gameBoard[spawnPos.x + y, spawnPos.y + x] = obst[randChoice].footprint[x, y];
            }
        }

        public Pos pickRandomPoint()
        {
            var position = new Pos {x = rand.generate(0, DimensionX - 1), y = rand.generate(0, DimensionY - 1)};
            //Random randY = new Random();

            return position;
        }

       
    }
}