using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

namespace WojnaMrowisk
{
    class Simulation
    {
        private bool paused = false;
        private bool running = true;
        public static List<Colony> colonies = new List<Colony>();
        
        private Map map = new Map();
        private Colony col1 = new Colony();
        public Pos antTarget = new Pos();
        public Menu menu = new Menu();
        public Food food = new Food();
        public List<Obstacle> obstacles = new List<Obstacle>();
        public Map getMap() {
            return map;
        }
        public static int step;
        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Simulation sim = new Simulation();
            Obstacle rock = new Obstacle();
            rock.footprint = new int[,] {
             {0,1,1,1,0},
             {1,1,1,1,1}
            };
            sim.obstacles.Add(rock);
            sim.Start();
            while (sim.running)
            {
                sim.Update();
            }
        }

        void initMap(List<Obstacle> obst) {
            map.spawnObstacles(obst);
        }
        private void Start() {
            (var size, var amount) = menu.menu();
            map.DimensionX = size;
            map.DimensionY = size;
            map.gameBoard = new int[size, size];
            initMap(obstacles);
            map.spawnFood();
            for (int i=0;i<amount;i++)
            {
                addColony();
            }
            
        }
        private void Update() {
            step++;
            if (step % 30 == 0)
            {
                map.spawnFood();
            }
            //is called as fast as possible w. 100ms sleep
            if (Console.KeyAvailable)
            {
                
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.P)//pause
                {
                    paused = paused ? false : true;
                    Console.Clear();
                }
                if (key.Key == ConsoleKey.A)//add a colony with default stats
                {
                    addColony();
                }
                if (key.Key == ConsoleKey.R && !paused)//respawn food
                {
                    map.spawnFood();
                }
            }
            foreach (Colony col in colonies.ToArray()) {
                col.evaluateColonyLogic(map);
                foreach (Anthill ah in col.anthills.ToArray()) {
                    ah.evaluateAnthillLogic(map);
                    foreach (Ant a in ah.ants.ToArray())
                    {
                        if (antTarget != null) {
                            a.evaluateLogic(map, antTarget);
                        }
                        Thread.Sleep(10);
                    }
                }
            }
            Console.SetCursorPosition(0, 0);
            for (int i = 0; i < map.DimensionX + 4; i++) {
                Console.Write("=");
            }
            Console.Write("\n");
            Console.SetCursorPosition(0, 1);
            for (int y = 0; y < map.DimensionY; y++) {
                Console.Write("||");
                for (int x = 0; x < map.DimensionX; x++) {
                    //Console.Write(map.gameBoard[x,y]);
                    if (map.gameBoard[x, y] == 0 || map.gameBoard[x,y] == 7) {
                        Console.Write(" ");
                    }
                    if (map.gameBoard[x, y] == 10) {//food
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write("*");
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                    if (map.gameBoard[x, y] == 1) { //obstacle
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write("#");
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                    if (map.gameBoard[x, y] == 3 || map.gameBoard[x, y] == 4) {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("@");
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                    if (map.gameBoard[x, y] == 2) {
                        ConsoleColor c = ConsoleColor.Magenta;
                        char symbol = '$';
                        foreach (Colony col in colonies)
                        {
                            foreach (Anthill ah in col.anthills)
                            {
                                foreach (Ant a in ah.ants)
                                {
                                    if (a.getPos().x == x && a.getPos().y == y) {
                                        c = col.color;
                                        if (a.isQueen)
                                        {
                                            symbol = '&';
                                        }
                                    }
                                }
                            }
                        }
                        Console.ForegroundColor = c;
                        Console.Write(symbol);
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                    if (map.gameBoard[x, y] == 11)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.Write("o");
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                }
                Console.Write("||");
                switch (y) {
                    case 0:
                        Console.Write(" Dane symulacji\n");
                        break;
                    case 1:
                        if (paused)
                        {
                            Console.Write(" [PAUSED]\n");
                        }
                        else {
                            Console.Write("\n");
                        }
                        break;
                    case 4:
                        Console.Write(" Ilosc kolonii: " + colonies.Count + "\n");
                        break;
                    case 5:
                        if (map.food != null)
                        {
                            Console.Write("Pozycja jedzenia: " + map.food.getPos().x + "," + map.food.getPos().y + "\n");
                        }
                        else {
                            Console.Write("\n");
                        }
                        break;
                    case 6:
                        Console.Write("Steps: " + step + "\n");
                        break;
                    case 7:
                        Console.Write("Anthill1.Hunger = " + colonies[0].anthills[0].Hunger + "\n");
                        break;
                    /*
                     * ============[Debug stuff]===========
                    uncomment the Console.Clear thingy below to make it work properly

                     * case 7:
                        if (colonies[0].anthills[0].ants[0].getPos() != null)
                        {
                            Console.Write("Ant1_Pos: x=" + colonies[0].anthills[0].ants[0].getPos().x + " y=" + colonies[0].anthills[0].ants[0].getPos().y + " t=" + colonies[0].anthills[0].ants[0].t + "\n");

                        }
                        else {
                            Console.Write("\n");
                        }
                        break;
                    case 8:
                        if (colonies[0].anthills[0].ants[0].getPos() != null)
                        {
                            Console.Write("Ant1_MTS: " + colonies[0].anthills[0].ants[0].movementTimeStart + " rd=" + colonies[0].anthills[0].ants[0].reachedDestination + "\n");

                        }
                        else
                        {
                            Console.Write("\n");
                        }
                        break;
                    case 9:
                        if (colonies[0].anthills[0].ants[0].movementVector != null && colonies[0].anthills[0].ants[0].startMovementVector != null)
                        {
                            Console.Write("Ant1_VectorsD: mV" + colonies[0].anthills[0].ants[0].movementVector.distance() + " svD=" + colonies[0].anthills[0].ants[0].startMovementVector.distance() + "\n");

                        }
                        else
                        {
                            Console.Write("\n");
                        }
                        break;
                    case 10:
                        if (colonies[0].anthills[0].ants[0].movementVector != null && colonies[0].anthills[0].ants[0].startMovementVector != null)
                        {
                            Console.Write("Ant1_distance: " + ((float)(time.stopwatch.Elapsed.TotalSeconds) - colonies[0].anthills[0].ants[0].movementTimeStart) * (colonies[0].anthills[0].ants[0].Speed) +  "\n");

                        }
                        else
                        {
                            Console.Write("\n");
                        }
                        break;
                    */
                    default:
                        Console.Write("\n");
                        break;
                }
            }
            for (int p = 0; p < map.DimensionX + 4; p++) {
                Console.Write("=");
            }
            //Console.Clear();
        }

        void addColony() {
            Colony col = new Colony();
            colonies.Add(col);
            col.initialize(map);
        }
        //public void UpdateGraphics()
    }

}
