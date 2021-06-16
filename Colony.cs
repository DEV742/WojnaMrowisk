using System;
using System.Collections.Generic;

namespace WojnaMrowisk
{
    internal class Colony
    {
        public List<Anthill> anthills = new List<Anthill>();
        public bool coldead;
        public ConsoleColor color;
        public ColonyStatistics colSt;
        public int ID;
        private int maxAnthills;
        private int maxAnts;

        public float distFromAnthill { get; set; } = 40f;

        //Picks a random color from ConsoleColor enumeration if it is not already in use
        public ConsoleColor getRandomColor()
        {
            var consoleColors = Enum.GetValues(typeof(ConsoleColor));

            var col = (ConsoleColor) consoleColors.GetValue(rand.generate(0, consoleColors.Length));
            var i = 0;
            while (col == ConsoleColor.Black || Simulation.colorsUsed.Contains(col))
            {
                i++;
                col = (ConsoleColor) consoleColors.GetValue(rand.generate(0, consoleColors.Length));

                if (i == 10) break;
            }

            Simulation.colorsUsed.Add(col);

            return col;
        }

        //Wipes the entire colony
        public void die(Map map)
        {
            Simulation.colonies.Remove(this);
            coldead = true;
            WriteStats();
            if (anthills.Count > 0)
                foreach (var ah in anthills)
                    ah.Destroy(map);
        }

        //Saves colony stats to its own ColonyStatistics object
        public void WriteStats()
        {
            colSt.maxAnthillsNum = maxAnthills;
            colSt.maxAntsNum = maxAnts;
            colSt.colID = ID;
            colSt.timeOfDeath = coldead ? Simulation.step.ToString() : "victorious";
        }

        //Evaluates colony logic(anthill spawning, death)
        public void evaluateColonyLogic(Map map)
        {
            if (maxAnthills < anthills.Count) maxAnthills = anthills.Count;
            var ma = 0;
            foreach (var ah in anthills) ma += ah.antsMax;
            if (maxAnts < ma) maxAnts = ma;
            var i = 0;
            foreach (var ah in anthills.ToArray())
            {
                if (ah.Hunger > 85 && ah.queen != null && ah.ants.Count > 1 && ah.Size == ah.sizes.GetLength(0) - 1 &&
                    Simulation.step % 75 == 0)
                    spawnAnthill(map);
                if (!ah.dead) i++;
            }

            if (i == 0 && !coldead && Simulation.step > 5) die(map);
        }

        //Spawns an anthill at a random point that is not obstructed by anything else
        public void spawnAnthill(Map map)
        {
            var a = new Anthill {ahStats = new AnthillStatistics()};
            var pos = map.pickRandomPoint();
            var overlaps = true;

            a.timeCreated = Simulation.step;
            Simulation.ahStats.Add(a.ahStats);
            var i = 0;
            while (overlaps && i < 40)
            {
                i++;
                var test = false;
                for (var y = 0; y < a.sizes.GetLength(2); y++)
                for (var x = 0; x < a.sizes.GetLength(1); x++)
                    if (map.DimensionX > pos.x + y && map.DimensionY > pos.y + x)
                    {
                        if (map.gameBoard[pos.x + y, pos.y + x] != 0) test = true;
                    }
                    else
                    {
                        test = true;
                    }

                if (test == false)
                    overlaps = false;
                else
                    pos = map.pickRandomPoint();
            }

            for (var y = 0; y < a.sizes.GetLength(2); y++)
            for (var x = 0; x < a.sizes.GetLength(1); x++)
                if (map.DimensionX > pos.x + y && map.DimensionY > pos.y + x)
                    map.gameBoard[pos.x + y, pos.y + x] = a.sizes[a.GetSize(), x, y];
            anthills.Add(a);
            a.Pos = pos;
            a.colId = ID;
            a.Initialise(map);
        }

        //Initialises the colony, creating one anthill, a queen and an ant
        public void initialize(Map map)
        {
            color = getRandomColor();
            var a = new Anthill {ahStats = new AnthillStatistics(), timeCreated = Simulation.step};
            Simulation.ahStats.Add(a.ahStats);
            colSt = new ColonyStatistics();
            var pos = map.pickRandomPoint();
            var overlaps = true;
            var i = 0;
            while (overlaps && i < 40)
            {
                i++;
                var test = false;
                for (var y = 0; y < a.sizes.GetLength(2); y++)
                for (var x = 0; x < a.sizes.GetLength(1); x++)
                    if (map.DimensionX > pos.x + y && map.DimensionY > pos.y + x)
                    {
                        if (map.gameBoard[pos.x + y, pos.y + x] != 0) test = true;
                    }
                    else
                    {
                        test = true;
                    }

                if (test == false)
                    overlaps = false;
                else
                    pos = map.pickRandomPoint();
            }

            for (var y = 0; y < a.sizes.GetLength(2); y++)
            for (var x = 0; x < a.sizes.GetLength(1); x++)
                if (map.DimensionX > pos.x + y && map.DimensionY > pos.y + x)
                    map.gameBoard[pos.x + y, pos.y + x] = a.sizes[a.GetSize(), x, y];
            anthills.Add(a);
            a.Pos = pos;
            a.colId = ID;
            a.Initialise(map);
            colSt.colID = ID;
        }
    }
}