using System;
using System.Collections.Generic;

namespace WojnaMrowisk
{
    class Simulation
    {
        private bool paused = false;
        private int i = 0;
        public Map map = new Map();
        private List<Colony> colonies = new List<Colony>();
        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Simulation sim = new Simulation();
            sim.Start();
            while (!sim.paused)
            {
                sim.Update();
            }
        }
        private void Start() {
            //is called once in the beginning
            map.gameBoard = new int[90, 28];
        }
        private void Update() {
            //is called as fast as possible
            Console.SetCursorPosition(0, 0);
            for (int i = 0; i < 90 + 4; i++) {
                Console.Write("=");
            }
            Console.Write("\n");
            Console.SetCursorPosition(0, 1);
            for (int y = 0; y < 28; y++) {
                Console.Write("||");
                for (int x = 0; x < 90; x++) {
                    //Console.Write(map.gameBoard[x,y]);
                    if (map.gameBoard[x, y] == 0) {
                        Console.Write(" ");
                    }
                    if (map.gameBoard[x, y] == 1) {
                        Console.Write("*");
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

                    default:
                        Console.Write("\n");
                        break;
                }
            }
            for (int p = 0; p < 90 + 4; p++) {
                Console.Write("=");
            }
            //Console.Clear();
        }
        //public void UpdateGraphics()
    }

    class Map
    {
        private float foodSpawnRate;
        private float foodRespawnDelay;
        private int numBases;
        private int numColonies;
        public int[,] gameBoard;
        // = new int[Console.WindowWidth, Console.WindowHeight];
        private Food food;

        void respawnFood() { 
        
        }
    }
    class Obstacle 
    {
        private char[,] footprint;
        private Pos position;
    }
    class Food
    {
        private int partsValue;
        private int numParts;
        private Pos position;

        void destroy() { 
        
        }
    }
    class Pos 
    {
        private int posX;
        private int posY;
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
