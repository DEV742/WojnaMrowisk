using System;
using System.Collections.Generic;

namespace WojnaMrowisk
{
    class Simulation
    {
        private bool paused = false;
        private int i = 0;
        private List<Colony> colonies = new List<Colony>();
        Map map = new Map();
        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Simulation sim = new Simulation();
            Obstacle rock = new Obstacle();
            rock.footprint = new int[,] {
             {0,1,1,1,0},
             {1,1,1,1,1}
            };
            List<Obstacle> obstacles = new List<Obstacle>();
            obstacles.Add(rock);
            sim.Start();
            sim.initMap(obstacles);
            while (!sim.paused)
            {
                sim.Update();
            }
        }

        void initMap(List<Obstacle> obst) {
            map.spawnObstacles(obst);
        }
        private void Start() {
            //is called once in the beginning
            map.gameBoard = new int[map.dimensionX, map.dimensionY];
            map.food = map.spawnFood();
        }
        private void Update() {
            //is called as fast as possible
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.R)
                {
                    map.food = map.respawnFood();
                }
            }
            Console.SetCursorPosition(0, 0);
            for (int i = 0; i < map.dimensionX + 4; i++) {
                Console.Write("=");
            }
            Console.Write("\n");
            Console.SetCursorPosition(0, 1);
            for (int y = 0; y < map.dimensionY; y++) {
                Console.Write("||");
                for (int x = 0; x < map.dimensionX; x++) {
                    //Console.Write(map.gameBoard[x,y]);
                    if (map.gameBoard[x, y] == 0) {
                        Console.Write(" ");
                    }
                    if (map.gameBoard[x, y] == 10) {
                        Console.Write("*");
                    }
                    if (map.gameBoard[x, y] == 1) { 
                        Console.Write("#");
                    }
                }
                Console.Write("||");
                switch (y) {
                    case 0:
                        Console.Write(" Dane symulacji\n");
                        break;
                    case 4:
                        Console.Write(" Ilosc kolonii: " + colonies.Count + "\n");
                        break;
                    case 5:
                        Console.Write("Pozycja jedzenia: " + map.food.getPos().x + "," + map.food.getPos().y +"\n");
                        break;
                    default:
                        Console.Write("\n");
                        break;
                }
            }
            for (int p = 0; p < map.dimensionX + 4; p++) {
                Console.Write("=");
            }
            //Console.Clear();
        }
        //public void UpdateGraphics()
    }
    static class rand
    {
        static Random genX = new Random();

        public static int generate(int a, int b) {
            int i = genX.Next(a, b);
            return i;
        }
    }

    class Map
    {
        /*Map idea: every thing has a certain ID, for example, an ant is a 1,
        a piece of food is 2 etc.*/
        private float foodSpawnRate;
        private float foodRespawnDelay;
        private int numBases;
        private int numColonies;
        public int[,] gameBoard;
        public int dimensionX = 90;
        public int dimensionY = 25;
        // = new int[Console.WindowWidth, Console.WindowHeight];
        public Food food;

        public Food spawnFood() {
            Food food = new Food();
            Pos positionToSpawn = pickRandomPoint();
            while (gameBoard[positionToSpawn.x, positionToSpawn.y] != 0) {
                positionToSpawn = pickRandomPoint();
            }
            food.setPos(positionToSpawn.x, positionToSpawn.y);
            gameBoard[positionToSpawn.x, positionToSpawn.y] = 10;
            return food;
        }
        public void spawnObstacles(List<Obstacle> obst) {
            //Random rand = new Random();
            Pos spawnPos = new Pos();
            int numObst = rand.generate(5, 15);//spawning between 5 and 15 obstacles
            int randChoice = 0;
            for (int i = 0; i < numObst; i++) {
                spawnPos = pickRandomPoint();
                randChoice = rand.generate(0, obst.Count);
                //obst[randChoice];
                for (int y = 0; y < obst[randChoice].footprint.GetLength(1); y++) {
                    for (int x = 0; x < obst[randChoice].footprint.GetLength(0); x++) {
                        if (dimensionX > spawnPos.x + y && dimensionY > spawnPos.y + x && obst[randChoice].footprint[x, y] != 0) {
                            gameBoard[spawnPos.x+y, spawnPos.y+x] = obst[randChoice].footprint[x, y];
                        }
                    }
                }
            }

        }
        public Food respawnFood() {
            gameBoard[food.getPos().x, food.getPos().y] = 0;
            Food foodResp = new Food();
            Pos positionToSpawn = pickRandomPoint();
            while (gameBoard[positionToSpawn.x, positionToSpawn.y] != 0)
            {
                positionToSpawn = pickRandomPoint();
            }
            foodResp.setPos(positionToSpawn.x, positionToSpawn.y);
            gameBoard[positionToSpawn.x, positionToSpawn.y] = 10;
            return foodResp;
        }
        Pos pickRandomPoint()
        {
            Pos position = new Pos();
            //Random randY = new Random();
            position.x = rand.generate(0,dimensionX);
            position.y = rand.generate(0,dimensionY);

            return position;
        }
    }
    class Obstacle 
    {
        public int[,] footprint;
        private char texture;
        private Pos position;

        public Pos getPos() {
            return position;
        }
        public void setTexture(char ch) {
            texture = ch;
        }
    }
    class Food
    {
        private int partsValue;
        private int numParts;
        private Pos position = new Pos();

        void destroy() { 
        
        }

        public void setPos(int x, int y) {
            position.x = x;
            position.y = y;
        }
        public Pos getPos() {
            Pos pos = new Pos();
            pos.x = position.x;
            pos.y = position.y;
            return pos;
        }
    }
    class Pos 
    {
        private int posX;
        private int posY;
        public int x {
            get { return posX; }
            set { posX = value; }
        }
        public int y
        {
            get { return posY; }
            set { posY = value; }
        }
    }
    class Colony 
    {
        private float speed;
        private float foodDetectionRange;
        private float alertRange;
        private int health;
        private int damage;
        private float reprodRate;
        void die() { 
            
        }
    }
    class Anthill:Colony 
    {
        private int hunger;
        private int numUnits;
        private int size;
        private float reprodRate;
        private Pos position;
        void destroy() { 
        
        }
        void grow() {
            size++;
        }
        void spawnAnt() { 
            
        }
    }
    class Ant:Anthill 
    {
        private char[,] footprint;
        private int health;
        private string state;
        private Pos position;
        private bool carrying;

        void die() { 
        
        }
        Food checkForFood(float radius) {
            return null;  
        }
        void attack() { 
            
        }
        void collectFood() { 
            
        }
        void goToFood(Food food) { 
            
        }
    }
}
