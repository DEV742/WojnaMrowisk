using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

namespace WojnaMrowisk
{
    class time { 
        //public static float deltaTime;
        public static Stopwatch stopwatch = new Stopwatch();
        public static float getStartTime() {
            return (float)stopwatch.Elapsed.TotalSeconds;
        }
    }
    class Simulation
    {
        private bool paused = false;
        private bool running = true;
        private int i = 0;
        private List<Colony> colonies = new List<Colony>();
        private Map map = new Map();
        private Colony col1 = new Colony();
        public Pos antTarget = new Pos();
        time t = new time();
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
            List<Obstacle> obstacles = new List<Obstacle>();
            obstacles.Add(rock);
            sim.Start();
            sim.initMap(obstacles);
            while (sim.running)
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
            map.spawnFood();
            col1.initialize(map);
            colonies.Add(col1);
            time.stopwatch.Start();
        }
        private void Update() {
            step++;
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
                    map.respawnFood();
                }
            }
            foreach (Colony col in colonies) {
                foreach (Anthill ah in col.anthills) {
                    foreach (Ant a in ah.ants)
                    {
                        if (antTarget != null) {
                            a.evaluateLogic(map, antTarget);
                        }
                        //Thread.Sleep(100);
                    }
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
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.Write("$");
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
                        Console.Write("Pozycja jedzenia: " + map.food.getPos().x + "," + map.food.getPos().y +"\n");
                        break;
                    case 6:
                        Console.Write("Steps: " + step + "\n");
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
            for (int p = 0; p < map.dimensionX + 4; p++) {
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
        public int dimensionX = 25;
        public int dimensionY = 25;
        public Pos currentWaypoint = new Pos();
        // = new int[Console.WindowWidth, Console.WindowHeight];
        public Food food;

        public void spawnFood() {
            Food sfood = new Food();
            Pos positionToSpawn = pickRandomPoint();
            while (gameBoard[positionToSpawn.x, positionToSpawn.y] != 0) {
                positionToSpawn = pickRandomPoint();
            }
            sfood.setPos(positionToSpawn.x, positionToSpawn.y);
            gameBoard[positionToSpawn.x, positionToSpawn.y] = 10;
            food = sfood;
            //return sfood;
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
        public void respawnFood() {
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
            position.x = rand.generate(0,dimensionX-1);
            position.y = rand.generate(0,dimensionY-1);

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
            position.x = rand.generate(0, dimensionX-1);
            position.y = rand.generate(0, dimensionY-1);
            gameBoard[position.x, position.y] = 11;
            currentWaypoint = position;
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
        private float speed = 1;
        public float Speed
        {
            get { return speed; }
            set { speed = value; }
        }
        private float foodDetectionRange = 100f;
        public float foodRange {
            get { return foodDetectionRange; }
            set { foodDetectionRange = value; }
        }
        private float alertRange = 2f;
        private int health = 100;
        public int Health {
            get { return health; }
            set { health = value; }
        }
        private int damage = 25;
        private float reprodRate = 10f;
        public List<Anthill> anthills = new List<Anthill>();
        void die() { 
            
        }

        public void initialize(Map map) { //automatically create an anthill at a random spot and place a queen
            Anthill a = new Anthill();
            Pos pos = map.pickRandomPoint();
            bool overlaps = true;
            bool test = false;
            while (overlaps) {
                test = false;
                for (int y = 0; y < a.sizes.GetLength(2); y++)
                {
                    for (int x = 0; x < a.sizes.GetLength(1); x++)
                    {
                        if (map.dimensionX > pos.x + y && map.dimensionY > pos.y + x)
                        {
                            if (map.gameBoard[pos.x + y, pos.y + x] != 0) {
                                test = true;
                            }
                        }
                    }
                }
                if (test == false)
                {
                    overlaps = false;
                }
                else {
                    pos = map.pickRandomPoint();
                }
            }
            
            for (int y = 0; y < a.sizes.GetLength(2); y++) {
                for (int x = 0; x < a.sizes.GetLength(1); x++) {
                    if (map.dimensionX > pos.x + y && map.dimensionY > pos.y + x) {
                        map.gameBoard[pos.x + y, pos.y + x] = a.sizes[a.getSize(), x, y];
                    }
                }
            }
            anthills.Add(a);
            a.Pos = pos;
            a.init(map, a);
        }
    }
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
        public Pos Pos {
            get { return position; }
            set { position = value; }
        }
        public List<Ant> ants = new List<Ant>();
        public int getSize() {
            return size;
        }
        void destroy() { 
        }
        void grow() {
            size++;
        }
        public void init(Map map, Anthill a) {
            spawnAnt(map, a);
        }
        public void spawnAnt(Map map, Anthill ah) {
            Pos posToSpawn = new Pos();
            for (int y = ah.position.y; y < ah.position.y + sizes.GetLength(2); y++) {
                for (int x = ah.position.x; x < ah.position.x + sizes.GetLength(1); x++) {
                    if (ah.position.x + sizes.GetLength(1) < map.dimensionX && ah.position.y + sizes.GetLength(2) < map.dimensionY) {
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
    class Ant : Anthill
    {
        private int hp;
        private string state;
        private Pos pos = new Pos();
        private bool carrying;
        public float t;
        public bool reachedDestination = true;
        public float movementTimeStart;
        public Vector movementVector;
        public Vector startMovementVector;

        public Pos newPos = new Pos();
        private int standingOnValue;
        public int stOnV {
            get { return standingOnValue; }
            set { standingOnValue = value; }
        } 
        public int health{
            get { return hp; }
            set { hp = value; }
        }
        public Pos getPos() {
            return pos;
        }

        public void evaluateLogic(Map map, Pos antTarget) {
            if (Vector.CreateVector(getPos(), map.food.getPos()).distance() <= foodRange)
            {
                state = "food_going";
                //reachedDestination = false;
                //startMovementVector = Vector.CreateVector(getPos(), map.food.getPos());
                goToFood(map.food, map);
            }
            //else {
                //state = "wander";
            //move(antTarget, map);
            //}
        }
        public void setPos(Pos position) {
            pos = position;
        }
        void die() { 
        
        }
        Food checkForFood(float radius) {
            return null;  
        }
        void attack() { 
            
        }
        void collectFood() { 
            
        }
        void goToFood(Food food, Map map) {
            move(map.food.getPos(), map);
        }
        void goToAnthill(Anthill ah) { 
            
        }

        void move(Pos tpos, Map map) {
            /*Movement ideas
             [Linear Interpolation]

            f(t) = f0 + ((t - t0)/(t1 - t0))(f1 - f0)
            */
            if (!reachedDestination)
            {
                if (movementVector != null)
                {
                    if (tpos.x != movementVector.endX || tpos.y != movementVector.endY)
                    {
                        t = 0;
                        movementVector = movementVector = Vector.CreateVector(getPos(), tpos);
                        startMovementVector = movementVector;
                        movementTimeStart = Simulation.step;
                    }
                }
                movementVector = Vector.CreateVector(getPos(), tpos);

                float distCovered = MathF.Abs(((Simulation.step - movementTimeStart) * Speed));
                if (startMovementVector != null)
                {
                    t = distCovered / startMovementVector.distance();
                }
                else {
                    startMovementVector = movementVector;
                    t = distCovered / startMovementVector.distance();
                }
                //t = movementVector.distance() / (Speed);
                //t = Math.Clamp(t, 0, 1);
                if (pos.x != tpos.x || pos.y != tpos.y)
                {
                    //newPos.x = (int)(MathF.Round((1 - t) * pos.x + t * tpos.x));
                    //newPos.y = (int)(MathF.Round((1 - t) * pos.y + t * tpos.y));
                    newPos.x = (int)(Math.Round(startMovementVector.startX + (t * (startMovementVector.endX - startMovementVector.startX))));
                    newPos.y = (int)(Math.Round(startMovementVector.startY + (t * (startMovementVector.endY - startMovementVector.startY))));

                    if ((map.dimensionX > newPos.x && newPos.x >= 0) && (map.dimensionY > newPos.y && newPos.y >= 0))
                    {
                        if (standingOnValue != 2)
                        {
                            map.gameBoard[pos.x, pos.y] = standingOnValue;
                            standingOnValue = map.gameBoard[newPos.x, newPos.y];
                        }
                        map.gameBoard[newPos.x, newPos.y] = 2;
                        pos.x = newPos.x;
                        pos.y = newPos.y;
                    }
                    else
                    {

                        if (newPos.x < 0) { newPos.x = 0; }
                        if (newPos.x >= map.dimensionX) { newPos.x = map.dimensionX - 1; }
                        if (newPos.y < 0) { newPos.y = 0; }
                        if (newPos.y >= map.dimensionY) { newPos.y = map.dimensionY - 1; }
                        if (standingOnValue != 2)
                        {
                            map.gameBoard[pos.x, pos.y] = standingOnValue;
                            standingOnValue = map.gameBoard[newPos.x, newPos.y];
                        }
                        map.gameBoard[newPos.x, newPos.y] = 2;
                        pos.x = newPos.x;
                        pos.y = newPos.y;

                    }
                }
                 if (pos.x == tpos.x && pos.y == tpos.y)
                {
                    reachedDestination = true;
                    //t = 0;
                    //movementTimeStart = 0;
                    //startMovementVector = null;
                }

            }else {
                //t = 0;
                //movementTimeStart = 0;
                //startMovementVector = null;
            }
            if (reachedDestination && (tpos.x != pos.x || tpos.y != pos.y))
            {
                movementTimeStart = Simulation.step;
                startMovementVector = Vector.CreateVector(getPos(), tpos);
                reachedDestination = false;
                //movementVector = Vector.CreateVector(getPos(), tpos);
                //t = 0;
            }
        }
    }

    class Vector{
        private int xPos;
        private int yPos;

        private int startXPos;
        private int startYPos;
        private int endXPos;
        private int endYPos;
        public int x {
            get { return xPos; }
            set { xPos = value; }
        }
        public int y {
            get { return yPos; }
            set { yPos = value; }
        }
        public int startX {
            get { return startXPos; }
            set { startXPos = value; }
        }
        public int startY {
            get { return startYPos; }
            set { startYPos = value; }
        }
        public int endX {
            get { return endXPos; }
            set { endXPos = value; }
        }
        public int endY {
            get { return endYPos; }
            set { endYPos = value; }
        }
        public float distance() {
            return MathF.Sqrt(MathF.Pow(xPos,2) + MathF.Pow(yPos,2));
            //return MathF.Sqrt((xPos^2) + (yPos^2));
        }
        public static Vector CreateVector(Pos pos1, Pos pos2) {
            Vector vect = new Vector();
            vect.xPos = pos2.x - pos1.x;
            vect.yPos = pos2.y - pos1.y;

            vect.startXPos = pos1.x;
            vect.startYPos = pos1.y;

            vect.endYPos = pos2.y;
            vect.endXPos = pos2.x;
            return vect;

        }

    }
}
