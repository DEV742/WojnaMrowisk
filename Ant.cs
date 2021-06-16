using System;
using System.Collections.Generic;

namespace WojnaMrowisk
{
    internal class Ant : Anthill
    {
        public List<Ant> antsAround = new List<Ant>();
        public AntStatistics antStats;
        public bool carrying;
        public float closestEnemyAntDist;

        public float closestEnemyQueenDist;
        public new bool dead;
        public List<Ant> enemyAntsAround = new List<Ant>();
        public bool isQueen;
        public float movementTimeStart;
        public Vector movementVector;
        public Pos newPos = new Pos();
        private Pos pos = new Pos();
        public List<Ant> queens = new List<Ant>();
        public bool reachedDestination = true;
        public Vector startMovementVector;
        public float t;
        private Pos target;

        public float Speed { get; set; } = 0.6f;

        public float FoodRange { get; set; } = 15f;

        public float VisRange { get; set; } = 4f;

        public int Health { get; set; } = 100;

        public int Damage { get; set; } = 25;

        public string State { get; set; } = "wandering";

        public Anthill AntsAnthill { get; set; }

        public int CurrentlyStandingOn { get; set; }

        public Pos GetPos()
        {
            return pos;
        }

        //Util method to gather a list of ants around, in range of VisRange, to perform further decision making on that data
        public List<Ant> checkForAntsAround()
        {
            var antsAround = new List<Ant>();

            foreach (var col in Simulation.colonies)
            foreach (var ah in col.anthills)
            foreach (var a in ah.ants)
                if (Vector.CreateVector(GetPos(), a.GetPos()).distance() <= VisRange && a != this)
                    antsAround.Add(a);
            return antsAround;
        }

        //Stops the ant from going to its current destination, resets it.
        public void stopMovement()
        {
            reachedDestination = true;
            movementVector = null;
            startMovementVector = null;
            target = null;
            t = 0;
        }

        //Saves ant data to AntStatistics object
        public void WriteAntStats()
        {
            antStats.timeOfDeath = dead ? Simulation.step.ToString() : "victorious";
        }

        /*Evaluates ants logic based on the following factors:
         * - Ants around
         * - Food in range
         * - Is queen?
         * - Ants state
         * - Ants' queens around
         * - Ants carrying food around
         * */
        public void evaluateLogic(Map map)
        {
            if (Simulation.step > 3)
            {
                var index = 0;
                var minimum = new float[2]; //[0] - index [1] - distance
                foreach (var item in Map.foods)// checks for the nearest food
                {
                    var distance = Vector.CreateVector(GetPos(), Map.foods[index].GetPos()).distance();
                    if (index == 0 || minimum[1] > distance)
                    {
                        minimum[0] = index;
                        minimum[1] = distance;
                    }

                    index++;
                }

                if (!isQueen)
                {
                    antsAround = checkForAntsAround();
                    enemyAntsAround = new List<Ant>();
                    queens = new List<Ant>();
                    foreach (var ant in antsAround)//separates ants from antsAround list into sublists
                    {
                        if (ant.AntsAnthill.GetColonyID() != AntsAnthill.GetColonyID() && !ant.isQueen)
                            enemyAntsAround.Add(ant);
                        if (ant.isQueen && ant.AntsAnthill.GetColonyID() != AntsAnthill.GetColonyID()) queens.Add(ant);
                    }

                    closestEnemyQueenDist = int.MaxValue;
                    closestEnemyAntDist = int.MaxValue;
                    if (queens.Count != 0)
                    {
                        float minDist = 0;
                        for (var i = 0; i < queens.Count; i++)
                        {
                            var vec = Vector.CreateVector(GetPos(), queens[i].GetPos());
                            if (i == 0)
                            {
                                minDist = vec.distance();
                                closestEnemyQueenDist = minDist;
                            }

                            if (minDist > vec.distance())
                            {
                                minDist = vec.distance();
                                closestEnemyQueenDist = minDist;
                            }
                        }
                    }

                    if (enemyAntsAround.Count != 0)
                    {
                        float minDist = 0;
                        for (var i = 0; i < enemyAntsAround.Count; i++)
                        {
                            var vec = Vector.CreateVector(GetPos(), enemyAntsAround[i].GetPos());
                            if (i == 0)
                            {
                                minDist = vec.distance();
                                closestEnemyAntDist = minDist;
                            }

                            if (minDist > vec.distance())
                            {
                                minDist = vec.distance();
                                closestEnemyAntDist = minDist;
                            }
                        }
                    }

                    foreach (var a in antsAround)
                        if (a.GetPos().x == GetPos().x && a.GetPos().y == GetPos().y &&
                            a.AntsAnthill.GetColonyID() != AntsAnthill.GetColonyID() && State != "fighting" &&
                            a.State != "fighting")
                            if (State != "fighting" && a.State != "fighting")
                            {
                                var fight = new Fight();
                                fight.StartFight(this, a, map);
                                stopMovement();
                                a.stopMovement();

                                if (CurrentlyStandingOn != 2) fight.standingOnValue = CurrentlyStandingOn;

                                State = "fighting";
                                a.State = "fighting";
                            }

                    if (Map.foods.Count != 0 && minimum[1] <= FoodRange && !carrying &&
                        checkForObstacle(map, Map.foods[(int) minimum[0]].GetPos()) && State != "fighting" &&
                        State != "attacking_queen" && minimum[1] <= closestEnemyAntDist &&
                        minimum[1] <= closestEnemyQueenDist)
                    {
                        Console.WriteLine(minimum[0]);
                        State = "food_going";

                        goToFood(map, (int) minimum[0]);

                        if (Map.foods[(int) minimum[0]].GetPos().x == GetPos().x &&
                            Map.foods[(int) minimum[0]].GetPos().y == GetPos().y && !carrying)
                            if (Map.foods[(int) minimum[0]].foodParts > 0)
                            {
                                carrying = true;
                                Map.foods[(int) minimum[0]].foodParts--;
                                if (Map.foods[(int) minimum[0]].foodParts <= 0) map.destroyFood((int) minimum[0]);
                            }
                    }
                    else if (queens.Count > 0 && VisRange >= closestEnemyQueenDist && State != "fighting" &&
                             !carrying && closestEnemyQueenDist < minimum[1] &&
                             closestEnemyQueenDist < closestEnemyAntDist)
                    {
                        //attacking the closest queen 
                        float minDist = 0;
                        var closestEnemyQueen = queens[0];
                        for (var i = 0; i < queens.Count; i++)
                        {
                            var vec = Vector.CreateVector(GetPos(), queens[i].GetPos());
                            if (i == 0)
                            {
                                minDist = vec.distance();
                                closestEnemyQueen = queens[i];
                            }

                            if (minDist > vec.distance())
                            {
                                minDist = vec.distance();
                                closestEnemyQueen = queens[i];
                            }
                        }

                        if (State != "attacking_queen" && closestEnemyQueen.GetPos().x != GetPos().x &&
                            closestEnemyQueen.GetPos().y != GetPos().y) State = "attacking_queen";
                        move(closestEnemyQueen.GetPos(), map);
                        if (closestEnemyQueen.GetPos().x == GetPos().x && closestEnemyQueen.GetPos().y == GetPos().y)
                            if (closestEnemyQueen.State != "fighting")
                            {
                                var fight = new Fight();

                                fight.StartFight(this, closestEnemyQueen, map);
                                stopMovement();
                                closestEnemyQueen.stopMovement();
                                State = "fighting";
                                closestEnemyQueen.State = "fighting";
                            }
                    }
                    else if (enemyAntsAround.Count > 0 && VisRange >= closestEnemyAntDist && State != "fighting" &&
                             State != "attacking_queen" && !carrying && closestEnemyAntDist < minimum[1] &&
                             closestEnemyAntDist < closestEnemyQueenDist)
                    {
                        //attacking all ants around
                        float minDist = 0;
                        var a = enemyAntsAround[0];
                        for (var i = 0; i < enemyAntsAround.Count; i++)
                        {
                            var vec = Vector.CreateVector(GetPos(), enemyAntsAround[i].GetPos());
                            if (i == 0)
                            {
                                minDist = vec.distance();
                                a = enemyAntsAround[i];
                            }

                            if (minDist > vec.distance())
                            {
                                minDist = vec.distance();
                                a = enemyAntsAround[i];
                            }
                        }

                        move(a.GetPos(), map);
                        if (a.GetPos().x == GetPos().x && a.GetPos().y == GetPos().y)
                            if (a.State != "fighting")
                            {
                                var fight = new Fight();

                                fight.StartFight(this, a, map);
                                stopMovement();
                                a.stopMovement();
                                State = "fighting";
                                a.State = "fighting";
                            }
                    }
                    else
                    {
                        //wandering/returning state
                        if (State != "fighting")
                        {
                            if (Vector.CreateVector(GetPos(), AntsAnthill.GetAhPos()).distance() >=
                                AntsAnthill.distFromAnthill || carrying)
                            {
                                //make the ant go back to the anthill
                                State = "returning";
                            }
                            else
                            {
                                if (Vector.CreateVector(GetPos(), AntsAnthill.GetAhPos()).distance() < 2f ||
                                    map.food == null) State = "wandering";
                            }

                            if (State == "returning")
                            {
                                if (GetPos().x != AntsAnthill.GetAhPos().x || GetPos().y != AntsAnthill.GetAhPos().y)
                                {
                                    goToAnthill(map);
                                }
                                else
                                {
                                    if (carrying)
                                    {
                                        AntsAnthill.Hunger += Food.partsValue;
                                        carrying = false;
                                    }

                                    State = "wandering";
                                }
                            }
                            else if (State == "wandering")
                            {
                                if (target == null || reachedDestination)
                                {
                                    target = map.pickRandomPoint();

                                    while (Vector.CreateVector(AntsAnthill.GetAhPos(), target).distance() >
                                           AntsAnthill.distFromAnthill ||
                                           !checkForObstacle(map, target) && CurrentlyStandingOn != 1) target = map.pickRandomPoint();
                                }

                                if (target != null) move(target, map);
                            }
                        }
                    }
                }
                else
                {
                    if (target == null || reachedDestination)
                    {
                        target = map.pickRandomPoint();

                        while (Vector.CreateVector(AntsAnthill.GetAhPos(), target).distance() > 2f)
                            target = map.pickRandomPoint();
                    }

                    if (target != null) move(target, map);
                }
            }
        }

        public void SetPos(Pos position)
        {
            pos = position;
        }

        //Destroys the ant
        public void DieAnt(Map map)
        {
            if (!dead)
            {
                dead = true;
                if (isQueen) AntsAnthill.queen = null;
                map.gameBoard[GetPos().x, GetPos().y] = 0;
                antStats.timeOfDeath = Simulation.step.ToString();
                AntsAnthill.ants.Remove(this);
            }
        }

        //Checks for obstacles along the path from the current position to the given position using Linear Interpolation
        public bool checkForObstacle(Map map, Pos tpos)
        {
            float frac;
            var checkingVector = Vector.CreateVector(GetPos(), tpos);
            var checkerPos = new Pos();
            for (frac = 0; frac < 1; frac += 0.02f)
            {
                checkerPos.x =
                    (int) Math.Round(checkingVector.startX + frac * (checkingVector.endX - checkingVector.startX));
                checkerPos.y =
                    (int) Math.Round(checkingVector.startY + frac * (checkingVector.endY - checkingVector.startY));

                if (map.gameBoard[checkerPos.x, checkerPos.y] == 1) return false;
            }

            return true;
        }

        private void goToFood(Map map, int foodIndex)
        {
            move(Map.foods[foodIndex].GetPos(), map);
        }

        private void goToAnthill(Map map)
        {
            move(AntsAnthill.GetAhPos(), map);
        }

        //A method for ants movement. Based on Linear Interpolation w. formula x = startPoint.x + (t*startMovementVector);
        //Every ant objects stores a value of what it(the ant) stands on, and when leaving the current position, it places the StandingOnValue back
        private void move(Pos tpos, Map map)
        {
            if (!reachedDestination)
            {
                if (movementVector != null)
                    if (tpos.x != movementVector.endX || tpos.y != movementVector.endY)
                    {
                        t = 0;
                        movementVector = movementVector = Vector.CreateVector(GetPos(), tpos);
                        startMovementVector = movementVector;
                        movementTimeStart = Simulation.step;
                    }

                movementVector = Vector.CreateVector(GetPos(), tpos);
                var distCovered = MathF.Abs((Simulation.step - movementTimeStart) * Speed);
                if (startMovementVector != null)
                {
                    t = distCovered / startMovementVector.distance();
                }
                else
                {
                    movementVector ??= Vector.CreateVector(GetPos(), tpos);
                    startMovementVector = movementVector;
                    t = distCovered / startMovementVector.distance();
                }
                
                if (pos.x != tpos.x || pos.y != tpos.y)
                {
                    newPos.x = (int) Math.Round(startMovementVector.startX +
                                                t * (startMovementVector.endX - startMovementVector.startX));
                    newPos.y = (int) Math.Round(startMovementVector.startY +
                                                t * (startMovementVector.endY - startMovementVector.startY));

                    if (map.DimensionX > newPos.x && newPos.x >= 0 && map.DimensionY > newPos.y && newPos.y >= 0)
                    {
                        foreach (var col in Simulation.colonies)
                        foreach (var ah in col.anthills)
                        foreach (var ant in ah.ants)
                            if (ant.GetPos().x == GetPos().x && ant.GetPos().y == GetPos().y && ant.CurrentlyStandingOn != 2 &&
                                ant.CurrentlyStandingOn != 5)
                                CurrentlyStandingOn = ant.CurrentlyStandingOn;
                        if (CurrentlyStandingOn != 2 && CurrentlyStandingOn != 5)
                        {
                            if (!dead)
                                map.gameBoard[pos.x, pos.y] = CurrentlyStandingOn;
                            else
                                map.gameBoard[pos.x, pos.y] = 0;
                        }
                        CurrentlyStandingOn = map.gameBoard[newPos.x, newPos.y];
                        map.gameBoard[newPos.x, newPos.y] = 2;
                        pos.x = newPos.x;
                        pos.y = newPos.y;
                    }
                    else
                    {
                        if (newPos.x < 0) newPos.x = 0;
                        if (newPos.x >= map.DimensionX) newPos.x = map.DimensionX - 1;
                        if (newPos.y < 0) newPos.y = 0;
                        if (newPos.y >= map.DimensionY) newPos.y = map.DimensionY - 1;
                        foreach (var col in Simulation.colonies)
                        foreach (var ah in col.anthills)
                        foreach (var ant in ah.ants)
                            if (ant.GetPos().x == GetPos().x && ant.GetPos().y == GetPos().y && ant.CurrentlyStandingOn != 2 &&
                                ant.CurrentlyStandingOn != 5)
                                CurrentlyStandingOn = ant.CurrentlyStandingOn;
                        if (!dead)
                            map.gameBoard[pos.x, pos.y] = CurrentlyStandingOn;
                        else
                            map.gameBoard[pos.x, pos.y] = 0;
                        CurrentlyStandingOn = map.gameBoard[newPos.x, newPos.y];
                        map.gameBoard[newPos.x, newPos.y] = 2;
                        pos.x = newPos.x;
                        pos.y = newPos.y;
                    }
                }

                if (pos.x == tpos.x && pos.y == tpos.y)
                    reachedDestination = true;
            }

            if (reachedDestination && (tpos.x != pos.x || tpos.y != pos.y))
            {
                movementTimeStart = Simulation.step;
                startMovementVector = Vector.CreateVector(GetPos(), tpos);
                reachedDestination = false;
            }
        }
    }
}