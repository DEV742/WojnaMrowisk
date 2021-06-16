using System;
using System.Collections.Generic;

namespace WojnaMrowisk
{
    internal class Anthill : Colony
    {
        public AnthillStatistics ahStats;
        public List<Ant> ants = new List<Ant>();
        public int antsMax;
        public int colId;
        public bool dead;
        public Ant queen;

        //private int[,] footprint;
        public int[,,] sizes =
        {
            {
                {7, 7, 7, 7, 7},
                {7, 7, 7, 7, 7},
                {7, 3, 4, 3, 7}
            },
            {
                {7, 7, 7, 7, 7},
                {7, 7, 3, 7, 7},
                {3, 3, 4, 3, 3}
            },
            {
                {7, 7, 3, 7, 7},
                {7, 3, 3, 3, 7},
                {3, 3, 4, 3, 3}
            }
        };

        public int timeCreated;

        public int Hunger { get; set; } = 100;

        public int Size { get; set; }

        public Pos Pos { get; set; }

        public int GetSize()
        {
            return Size;
        }

        public Pos GetAhPos()
        {
            var ps = new Pos
            {
                x = Pos.x + 2,
                y = Pos.y + 2
            };
            return ps;
        }

        public void Destroy(Map map)
        {
            if (!dead)
            {
                dead = true;
                WriteStatistics();
                map.DestroyAnthill(this);
                foreach (var ant in ants.ToArray()) ant.DieAnt(map);
                anthills.Remove(this);
                //Die(map);
            }
        }
        public void WriteStatistics()
        {
            ahStats.ahColId = colId;
            ahStats.ahId = GetAnthillID();
            ahStats.maxAntsNum = antsMax;
            ahStats.size = Size;
            ahStats.timeOfCreation = timeCreated;
            ahStats.timeOfDeath = dead ? Simulation.step.ToString() : "victorious";
        }

        public void EvaluateAnthillLogic(Map map)
        {
            if (antsMax < ants.Count) antsMax = ants.Count;
            if (Hunger > 100) Hunger = 100;
            if (queen != null)
            {
                if (Simulation.step % 2 == 0 && Hunger > 0)
                    Hunger -= (int)MathF.Round(ants.Count / (2.3f * (Size + 1)));
                if (Hunger > 50 && ants.Count > Size + 1 * 4 && Simulation.step % 40 == 0 &&
                    Size < sizes.GetLength(0) - 1) //upgrading an anthill
                {
                    Size++;
                    for (var y = 0; y < sizes.GetLength(2); y++)
                        for (var x = 0; x < sizes.GetLength(1); x++)
                            if (map.DimensionX > Pos.x + y && map.DimensionY > Pos.y + x)
                            {
                                map.gameBoard[Pos.x + y, Pos.y + x] = sizes[GetSize(), x, y];
                                if (map.gameBoard[Pos.x + y, Pos.y + x] == 2)
                                    foreach (var col in Simulation.colonies)
                                        foreach (var ah in col.anthills)
                                            foreach (var a in ah.ants)
                                                if (a.GetPos().x == Pos.x + y && a.GetPos().y == Pos.y + x)
                                                    a.CurrentlyStandingOn = sizes[GetSize(), x, y];
                            }
                }

                if (Hunger > 25 && Simulation.step % 50 == 0) SpawnAnt(map, false);
            }
            else
            {
                if (!dead) Destroy(map);
            }

            if (ants.Count == 1 && !dead && queen != null && Hunger < 25) Destroy(map);
            if (ants.Count == 0 && !dead) Destroy(map);
        }

        public void Initialise(Map map)
        {
            SpawnAnt(map, false);
            SpawnAnt(map, true);
        }

        public void SpawnAnt(Map map, bool isQueen)
        {
            var posToSpawn = new Pos
            {
                x = GetAhPos().x,
                y = GetAhPos().y
            };
            var random = new Random();

            var antClass = random.Next(1, 100);
            Console.Write(antClass);
            var ant = new Ant
            {
                antStats = new AntStatistics()
            };
            Simulation.antStats.Add(ant.antStats);
            ant.SetPos(posToSpawn);
            ant.Health = 100;
            ant.CurrentlyStandingOn = 4;
            ant.AntsAnthill = this;
            if (antClass <= 10)
            {
                //fighter
                int newHP = 100, newDamage = 25;
                var antPoints = random.Next(1, 5);
                for (var i = 0; i < antPoints; i++)
                {
                    var antUpgrade = random.Next(1, 2);
                    if (antUpgrade == 1) newHP += 5;
                    if (antUpgrade == 2) newDamage += 3;
                }

                //5% per ant to get this, only once
                var rareUpgrade = random.Next(1, 20);
                if (rareUpgrade == 3) newDamage += 7;
                ant.Health = newHP;
                ant.Damage = newDamage;
                ant.VisRange = 7f;
                Console.WriteLine("New Fighter. HP: " + newHP + " Damage: " + newDamage);
            }

            if (antClass > 10 && antClass <= 20)
            {
                //better Gatherer
                int newHP = 100, newDamage = 25;
                var antPoints = random.Next(1, 5);
                for (var i = 0; i < antPoints; i++)
                {
                    var antUpgrade = random.Next(1, 2);
                    if (antUpgrade == 1) newHP += 9;
                    if (antUpgrade == 2) newDamage += 1;
                }

                //5% per ant to get this, only once
                var rareUpgrade = random.Next(1, 20);
                if (rareUpgrade == 3) newHP += 30;
                ant.Health = newHP;
                ant.Damage = newDamage;
                ant.Speed = 1;
                ant.FoodRange = 15f;
                Console.WriteLine("New Gatherer. HP: " + newHP + " Damage: " + newDamage);
            }

            map.gameBoard[posToSpawn.x, posToSpawn.y] = 2;
            ant.isQueen = isQueen;
            if (isQueen)
            {
                ant.Damage = 0;
                queen = ant;
            }

            ant.antStats.colId = GetColonyID();
            ant.antStats.ahId = GetAnthillID();
            ant.antStats.timeCreated = Simulation.step;
            ant.antStats.speed = ant.Speed;
            ant.antStats.maxHealth = ant.Health;
            ant.antStats.maxDamage = ant.Damage;
            ant.antStats.VisRange = ant.VisRange;
            ant.antStats.FoodRange = ant.FoodRange;
            ant.antStats.isQueen = ant.isQueen;
            ants.Add(ant);
        }

        public int GetColonyID()
        {
            return colId;
        }

        public int GetAnthillID()
        {
            var id = 0;
            var i = 0;
            foreach (var col in Simulation.colonies)
                foreach (var ah in col.anthills)
                {
                    if (ah == this) id = i;
                    i++;
                }

            return id;
        }
    }
}