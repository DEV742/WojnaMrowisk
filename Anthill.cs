﻿using System;
using System.Collections.Generic;
using System.Text;

namespace WojnaMrowisk
{
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
        private int hunger = 100;// 100 - ok, 0 - bad
        public int Hunger
        {
            get { return hunger; }
            set { hunger = value; }
        }
        public bool dead = false;
        private int numUnits = 0;
        private int size = 0;
        public int colId;
        public int Size {
            get { return size; }
            set { size = value; }
        }
        private float reprodRate;
        public Ant queen;
        private Pos position;
        public Pos Pos
        {
            get { return position; }
            set { position = value; }
        }
        public List<Ant> ants = new List<Ant>();
        public int getSize()
        {
            return size;
        }
        public Pos getAhPos()
        {
            Pos ps = new Pos();
            ps.x = position.x + 2;
            ps.y = position.y + 2;
            return ps;
        }
        public void destroy(Map map)
        {
            if (!dead)
            {
                dead = true;
                map.destroyAnthill(this);
                foreach (Ant ant in ants.ToArray())
                {
                    ant.die(map);
                }
                anthills.Remove(this);
                die(map);
            }
        }
        void grow()
        {
            size++;
        }

        public void evaluateAnthillLogic(Map map)
        {
            if (hunger > 100) {
                hunger = 100;
            }
            if (queen != null)
            {
                if (Simulation.step % 2 == 0 && hunger > 0) {
                    hunger -= (int)MathF.Round((ants.Count)/(2.3f * (size + 1)));
                }
                if (hunger > 50 && ants.Count > size+1 * 4 && Simulation.step % 40 == 0 && size < sizes.GetLength(0)-1)//upgrading an anthill
                {
                    size++;
                    for (int y = 0; y < sizes.GetLength(2); y++)
                    {
                        for (int x = 0; x < sizes.GetLength(1); x++)
                        {
                            if (map.DimensionX > position.x + y && map.DimensionY > position.y + x)
                            {
                                map.gameBoard[position.x + y, position.y + x] = sizes[getSize(), x, y];
                                if (map.gameBoard[Pos.x + y, Pos.y + x] == 2)
                                {
                                    foreach (Colony col in Simulation.colonies)
                                    {
                                        foreach (Anthill ah in col.anthills)
                                        {
                                            foreach (Ant a in ah.ants)
                                            {
                                                if (a.getPos().x == Pos.x + y && a.getPos().y == Pos.y + x)
                                                {
                                                    a.stOnV = sizes[getSize(), x, y];
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if(hunger > 25 && Simulation.step % 50 == 0)
                {
                    spawnAnt(map, this, false);
                }
            }
            else {
                if (!dead)
                {
                    destroy(map);
                }
            }
            if (ants.Count == 1 && !dead && queen != null && hunger < 25) {
                destroy(map);
            }
            if (ants.Count == 0 && !dead) {
                destroy(map);
            }
            if (Simulation.step>20 && ants.Count == 1)
            {
                destroy(map);
            }
        }
        public void init(Map map, Anthill a)
        {
            spawnAnt(map, a, false);
            spawnAnt(map, a, true);
        }
        public void spawnAnt(Map map, Anthill ah, bool isQueen)
        {
            Pos posToSpawn = new Pos();
            posToSpawn.x = getAhPos().x; posToSpawn.y = getAhPos().y;
            Random random = new Random();
            int antClass = random.Next(1,100);
            Console.Write(antClass);
            Ant ant = new Ant();
            ant.setPos(posToSpawn);
            ant.Health = 100;
            ant.stOnV = 4;
            ant.antsAnthill = this;
            if (antClass<=10)
            {
                //fighter
                int newHP = 100, newDamage = 25;
                int antPoints = random.Next(1, 5);
                for(int i=0;i<antPoints;i++)
                {
                    int antUpgrade = random.Next(1, 2);
                    if (antUpgrade == 1) newHP += 5;
                    if (antUpgrade == 2) newDamage += 3;
                }
                //5% per ant to get this, only once
                int rareUpgrade = random.Next(1, 20);
                if (rareUpgrade == 3) newDamage += 7;
                ant.Health = newHP;
                ant.Damage = newDamage;
                ant.visRange = 7f;
                Console.WriteLine("New Fighter. HP: " + newHP +" Damage: "+ newDamage);
            }
            if (antClass > 10 && antClass <= 20)
            {
                //better Gatherer
                int newHP = 100, newDamage = 25;
                int antPoints = random.Next(1, 5);
                for (int i = 0; i < antPoints; i++)
                {
                    int antUpgrade = random.Next(1, 2);
                    if (antUpgrade == 1) newHP += 9;
                    if (antUpgrade == 2) newDamage += 1;
                }
                //5% per ant to get this, only once
                int rareUpgrade = random.Next(1, 20);
                if (rareUpgrade == 3) newHP += 30;
                ant.Health = newHP;
                ant.Damage = newDamage;
                ant.Speed = 1;
                ant.foodRange = 15f;
                Console.WriteLine("New Gatherer. HP: " + newHP + " Damage: " + newDamage);
            }
            map.gameBoard[posToSpawn.x, posToSpawn.y] = 2;
            ant.isQueen = isQueen;
            if (isQueen) {
                ant.Damage = 0;
                queen = ant;
            }
            ants.Add(ant);
        }
        public int getColonyID() { 
            return colId;
        }
    }
}
