using System;
using System.Collections.Generic;
using System.Text;

namespace WojnaMrowisk
{
    class Colony
    {
        public ConsoleColor color;

        public ConsoleColor getRandomColor() {
            var consoleColors = Enum.GetValues(typeof(ConsoleColor));

            ConsoleColor col = (ConsoleColor)consoleColors.GetValue(rand.generate(0, consoleColors.Length));
            int i = 0;
            while (col == ConsoleColor.Black || Simulation.colorsUsed.Contains(col)) {
                i++;
                col = (ConsoleColor)consoleColors.GetValue(rand.generate(0, consoleColors.Length));

                if (i == 10) break;
            }

            Simulation.colorsUsed.Add(col);

            return col;
        }
        private float maxDistFromAnthill = 40f;
        public float distFromAnthill
        {
            get { return maxDistFromAnthill; }
            set { maxDistFromAnthill = value; }
        }
        private float reprodRate = 10f;
        public List<Anthill> anthills = new List<Anthill>();
        public void die()
        {
            if (anthills.Count == 0)
           {
                Simulation.colonies.Remove(this);
                Console.WriteLine("KOLONIE COUNT: " + Simulation.colonies.Count);
           }
        }

        public void evaluateColonyLogic(Map map)
        {
            foreach (Anthill ah in anthills.ToArray()) {
                if (ah.Hunger > 90 && ah.queen != null && ah.ants.Count > 1 && ah.Size == ah.sizes.GetLength(0) - 1 && Simulation.step % 75 == 0) {
                    foreach (Ant a in ah.ants.ToArray()) {
                        if (a.stOnV == 0) {
                            Pos spawnPos = new Pos();
                            spawnPos = a.getPos();
                            break;
                        }

                    }
                    spawnAnthill(map);
                }
            }
        }
        public void spawnAnthill(Map map) {
            Anthill a = new Anthill();
            Pos pos = map.pickRandomPoint();
            bool overlaps = true;
            bool test = false;
            while (overlaps)
            {
                test = false;
                for (int y = 0; y < a.sizes.GetLength(2); y++)
                {
                    for (int x = 0; x < a.sizes.GetLength(1); x++)
                    {
                        if (map.DimensionX > pos.x + y && map.DimensionY > pos.y + x)
                        {
                            if (map.gameBoard[pos.x + y, pos.y + x] != 0)
                            {
                                test = true;
                            }
                        }
                        else
                        {
                            test = true;
                        }
                    }
                }
                if (test == false)
                {
                    overlaps = false;
                }
                else
                {
                    pos = map.pickRandomPoint();
                }
            }

            for (int y = 0; y < a.sizes.GetLength(2); y++)
            {
                for (int x = 0; x < a.sizes.GetLength(1); x++)
                {
                    if (map.DimensionX > pos.x + y && map.DimensionY > pos.y + x)
                    {
                        map.gameBoard[pos.x + y, pos.y + x] = a.sizes[a.getSize(), x, y];
                    }
                }
            }
            anthills.Add(a);
            a.Pos = pos;
        }
        public void initialize(Map map)
        { //automatically create an anthill at a random spot and place a queen
            color = getRandomColor();
            Anthill a = new Anthill();
            Pos pos = map.pickRandomPoint();
            bool overlaps = true;
            bool test = false;
            while (overlaps)
            {
                test = false;
                for (int y = 0; y < a.sizes.GetLength(2); y++)
                {
                    for (int x = 0; x < a.sizes.GetLength(1); x++)
                    {
                        if (map.DimensionX > pos.x + y && map.DimensionY > pos.y + x)
                        {
                            if (map.gameBoard[pos.x + y, pos.y + x] != 0)
                            {
                                test = true;
                            }
                        }
                        else {
                            test = true;
                        }
                    }
                }
                if (test == false)
                {
                    overlaps = false;
                }
                else
                {
                    pos = map.pickRandomPoint();
                }
            }

            for (int y = 0; y < a.sizes.GetLength(2); y++)
            {
                for (int x = 0; x < a.sizes.GetLength(1); x++)
                {
                    if (map.DimensionX > pos.x + y && map.DimensionY > pos.y + x)
                    {
                        map.gameBoard[pos.x + y, pos.y + x] = a.sizes[a.getSize(), x, y];
                    }
                }
            }
            anthills.Add(a);
            a.Pos = pos;
            a.init(map, a);
        }
    }
}
