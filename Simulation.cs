using System;
using System.Collections.Generic;
using System.IO;

namespace WojnaMrowisk
{
    internal class Simulation
    {
        public static List<Colony> colonies = new List<Colony>();
        public static List<Fight> fights = new List<Fight>();
        public static List<AnthillStatistics> ahStats = new List<AnthillStatistics>();
        public static List<AntStatistics> antStats = new List<AntStatistics>();
        public static List<ConsoleColor> colorsUsed = new List<ConsoleColor>();
        public static int step;
        public int colID;
        public List<ColonyStatistics> colStats = new List<ColonyStatistics>();

        private readonly Map map = new Map();

        //private Colony col1 = new Colony();
        //public Pos antTarget = new Pos();
        public Menu menu = new Menu();
        public List<Obstacle> obstacles = new List<Obstacle>();
        private bool paused;
        private bool running = true;
        
        private static void Main(string[] args)
        {
            Console.CursorVisible = false;
            var sim = new Simulation();
            var rock = new Obstacle {footprint = new[,] {{0, 1, 1, 1, 0}, {1, 1, 1, 1, 1}}};
            sim.obstacles.Add(rock);
            sim.Start();
            while (sim.running) sim.Update();
        }

        private void initMap(List<Obstacle> obst)
        {
            map.spawnObstacles(obst);
        }

        private void Start()
        {
            var (size, amount) = menu.menu();
            map.DimensionX = size;
            map.DimensionY = size;
            map.gameBoard = new int[size, size];
            initMap(obstacles);
            map.spawnFood();
            for (var i = 0; i < amount; i++)
            {
                addColony();
                colID++;
            }
        }

        private void Update()
        {
            step++;
            if (step % 30 == 0) map.spawnFood();
            //is called as fast as possible w. 100ms sleep
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.P) //pause
                {
                    paused = !paused;
                    Console.Clear();
                }

                if (key.Key == ConsoleKey.A) //add a colony with default stats
                {
                    colID++;
                    addColony();
                }

                if (key.Key == ConsoleKey.R && !paused) //respawn food
                    map.spawnFood();
                if (key.Key == ConsoleKey.D && !paused) //delete colony
                    colonies[1].anthills[0].Destroy(map);
            }

            foreach (var f in fights.ToArray()) f.evaluateFight();
            foreach (var col in colonies.ToArray())
            {
                col.evaluateColonyLogic(map);
                foreach (var ah in col.anthills.ToArray())
                {
                    ah.EvaluateAnthillLogic(map);
                    foreach (var a in ah.ants.ToArray())
                        if (!a.dead)
                            a.evaluateLogic(map);
                        // Thread.Sleep(10);
                    //Thread.Sleep(10);
                }
            }

            Console.SetCursorPosition(0, 0);
            for (var i = 0; i < map.DimensionX + 4; i++) Console.Write("=");
            Console.Write("\n");
            Console.SetCursorPosition(0, 1);
            for (var y = 0; y < map.DimensionY; y++)
            {
                Console.Write("||");
                for (var x = 0; x < map.DimensionX; x++)
                {
                    //Console.Write(map.gameBoard[x,y]);
                    if (map.gameBoard[x, y] == 0 || map.gameBoard[x, y] == 7) Console.Write(" ");
                    if (map.gameBoard[x, y] == 10)
                    {
                        //food
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Write("*");
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }

                    if (map.gameBoard[x, y] == 1)
                    {
                        //obstacle
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write("#");
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }

                    if (map.gameBoard[x, y] == 5)
                    {
                        //fight
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write("!");
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }

                    if (map.gameBoard[x, y] == 3 || map.gameBoard[x, y] == 4)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("@");
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }

                    if (map.gameBoard[x, y] == 2)
                    {
                        var c = ConsoleColor.Magenta;
                        var symbol = ' ';
                        foreach (var col in colonies)
                        foreach (var ah in col.anthills)
                        foreach (var a in ah.ants)
                            if (a.GetPos().x == x && a.GetPos().y == y)
                            {
                                c = col.color;
                                if (a.isQueen)
                                    symbol = '&';
                                else if (a.State == "fighting" || a.dead)
                                    symbol = ' ';
                                else
                                    symbol = '$';
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
                switch (y)
                {
                    case 0:
                        Console.Write(" Dane symulacji\n");
                        break;
                    case 1:
                        Console.Write(paused ? " [PAUSED]\n" : "\n");
                        break;
                    case 4:
                        Console.Write(" Ilosc kolonii: " + colonies.Count + "\n");
                        break;
                    /*case 5:
                        if (map.food != null)
                        {
                            Console.Write("Pozycja jedzenia: " + map.food.GetPos().x + "," + map.food.GetPos().y + "\n");
                        }
                        else {
                            Console.Write("\n");
                        }
                        break;*/
                    case 6:
                        Console.Write("Steps: " + step + "\n");
                        break;
                    /*case 7:
                        Console.Write("Anthill1.Hunger = " + colonies[0].anthills[0].Hunger + "\n");
                        break;
                     * ============[Debug stuff]===========
                    uncomment the Console.Clear thingy below to make it work properly

                     * case 7:
                        if (colonies[0].anthills[0].ants[0].GetPos() != null)
                        {
                            Console.Write("Ant1_Pos: x=" + colonies[0].anthills[0].ants[0].GetPos().x + " y=" + colonies[0].anthills[0].ants[0].GetPos().y + " t=" + colonies[0].anthills[0].ants[0].t + "\n");

                        }
                        else {
                            Console.Write("\n");
                        }
                        break;
                    case 8:
                        if (colonies[0].anthills[0].ants[0].GetPos() != null)
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

            for (var p = 0; p < map.DimensionX + 4; p++) Console.Write("=");
            Console.WriteLine("COLONIES COUNT: " + colonies.Count);
            if (colonies.Count == 1)
            {
                running = false;
                foreach (var col in colonies)
                {
                    col.WriteStats();
                    foreach (var ah in col.anthills)
                    {
                        ah.WriteStatistics();
                        foreach (var a in ah.ants) a.WriteAntStats();
                    }
                }

                WriteCSV();
            }
            //Console.Clear();
        }

        private void addColony()
        {
            var col = new Colony();
            colonies.Add(col);
            col.ID = colID;
            col.initialize(map);
            colStats.Add(col.colSt);
        }

        //public void UpdateGraphics()
        private void WriteCSV()
        {
            using (var writer = new StreamWriter("ColoniesData.csv"))
            {
                writer.WriteLine("ColonyID,MaxAnthills,MaxAnts,TimeOfDeath/V");
                foreach (var colStat in colStats)
                    writer.WriteLine(
                        $"{colStat.colID},{colStat.maxAnthillsNum},{colStat.maxAntsNum},{colStat.timeOfDeath}");
            }

            using (var writer = new StreamWriter("AnthillsData.csv"))
            {
                writer.WriteLine(
                    "anthillColID,anthillID,maxAnts,size,timeOfCreation,timeOfDeath/V");
                foreach (var ahStat in ahStats)
                    writer.WriteLine(
                        $"{ahStat.ahColId},{ahStat.ahId},{ahStat.maxAntsNum},{ahStat.size},{ahStat.timeOfCreation},{ahStat.timeOfDeath}");
            }

            using (var writer = new StreamWriter("AntsData.csv"))
            {
                writer.WriteLine(
                    "ColonyID,anthillID,isQueen,MaxHP,Damage,VisRange,FoodRange,speed,creationTime,timeOfDeath/V");
                foreach (var antStat in antStats)
                    writer.WriteLine(
                        $"{antStat.colId},{antStat.ahId},{antStat.isQueen},{antStat.maxHealth},{antStat.maxDamage},{antStat.VisRange},{antStat.FoodRange},{antStat.speed},{antStat.timeCreated},{antStat.timeOfDeath}");
            }
        }
    }
    /*========================Saving to files==========================
     - Each colony: 
        -> max anthills
        -> max ants
        -> max ah.size
        -> time(step) of death/victorious
    - End of simulation time
    - Num of colonies
    - Anthills:

        colID, maxAnthillsNum, maxAntsNum, timeOfDeath/V;
        ahColID, maxAntsNum, size, timeOfCreation, timeOfDeath;
     */
}