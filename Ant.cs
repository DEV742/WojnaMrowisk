using System;
using System.Collections.Generic;
using System.Text;

namespace WojnaMrowisk
{
    class Ant : Anthill
    {
        private float speed = 0.7f;
        public float Speed
        {
            get { return speed; }
            set { speed = value; }
        }
        public float foodRange
        {
            get { return foodDetectionRange; }
            set { foodDetectionRange = value; }
        }
        private float antsVisionRange = 10f;//distance of aggression
        public float visRange
        {
            get { return antsVisionRange; }
            set { antsVisionRange = value; }
        }
        private int health = 100;
        public int Health
        {
            get { return health; }
            set { health = value; }
        }
        private int damage = 25;
        public int Damage
        {
            get { return damage; }
            set { damage = value; }
        }
        private float foodDetectionRange = 15f;
        private string state = "wandering";
        public string State {
            get { return state; }
            set { state = value; }
        }
        private Pos pos = new Pos();
        public bool carrying;
        public float t;
        public float movementSpeedDebuff = 3f;
        public bool reachedDestination = true;
        public float movementTimeStart;
        public Vector movementVector;
        public Vector startMovementVector;
        private Anthill antH;
        private Pos target;
        public bool isQueen;
        public bool dead = false;

        public List<Ant> antsAround = new List<Ant>();
        public List<Ant> enemyAntsAround = new List<Ant>();
        public List<Ant> queens = new List<Ant>();

        public float closestEnemyQueenDist = 0;
        public float closestEnemyAntDist = 0;

        public Anthill antsAnthill
        {
            get { return antH; }
            set { antH = value; }
        }

        public Pos newPos = new Pos();
        private int standingOnValue;
        public int stOnV
        {
            get { return standingOnValue; }
            set { standingOnValue = value; }
        }
        public Pos getPos()
        {
            return pos;
        }
        public List<Ant> checkForAntsAround() {
            List<Ant> antsAround = new List<Ant>();

            foreach (Colony col in Simulation.colonies)
            {
                foreach (Anthill ah in col.anthills)
                {
                    foreach (Ant a in ah.ants)
                    {
                        if (Vector.CreateVector(getPos(), a.getPos()).distance() <= visRange && a != this) {
                            antsAround.Add(a);
                        }
                    }
                }
            }
            return antsAround;
        }
        public void stopMovement() {
            reachedDestination = true;
            movementVector = null;
            startMovementVector = null;
            target = null;
            t = 0;
        }
        public void evaluateLogic(Map map)
        {
            if (Simulation.step > 3)
            {
                int index = 0;
                float[] minimum = new float[2]; //[0] - index [1] - distance
                foreach (var item in Map.foods)
                {

                    var distance = Vector.CreateVector(getPos(), Map.foods[index].getPos()).distance();
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
                    foreach (Ant ant in antsAround) {
                        if (ant.antsAnthill.getColonyID() != this.antsAnthill.getColonyID() && !ant.isQueen) {
                            enemyAntsAround.Add(ant);
                        }
                        if (ant.isQueen && ant.antsAnthill.getColonyID() != this.antsAnthill.getColonyID()) {
                            queens.Add(ant);
                        }
                    }

                    closestEnemyQueenDist = int.MaxValue;
                    closestEnemyAntDist = int.MaxValue;
                    if (queens.Count != 0) {
                        float minDist = 0;
                        Vector vec;
                        for (int i = 0; i < queens.Count; i++)
                        {
                            vec = Vector.CreateVector(getPos(), queens[i].getPos());
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
                        Vector vec;
                        for (int i = 0; i < enemyAntsAround.Count; i++)
                        {
                            vec = Vector.CreateVector(getPos(), enemyAntsAround[i].getPos());
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
                    foreach (Ant a in antsAround) {
                        if (a.getPos().x == getPos().x && a.getPos().y == getPos().y && a.antsAnthill.getColonyID() != antsAnthill.getColonyID() && state != "fighting" && a.State != "fighting") {
                            if (state != "fighting" && a.state != "fighting") {
                                Fight fight = new Fight();
                                fight.StartFight(this, a, map);
                                stopMovement();
                                a.stopMovement();

                                if (standingOnValue != 2) {
                                    fight.standingOnValue = standingOnValue;
                                }

                                state = "fighting";
                                a.State = "fighting";
                            }
                        }
                    }

                    if (Map.foods.Count != 0 && minimum[1] <= foodRange && !carrying && checkForObstacle(map, Map.foods[(int)minimum[0]].getPos()) && state != "fighting" && state != "attacking_queen" && (minimum[1] <= closestEnemyAntDist && minimum[1] <= closestEnemyQueenDist))
                    {
                        Console.WriteLine(minimum[0]);
                        state = "food_going";
                        //reachedDestination = false;
                        //startMovementVector = Vector.CreateVector(getPos(), map.food.getPos());

                        goToFood(map, (int)minimum[0]);

                        if (Map.foods[(int)minimum[0]].getPos().x == getPos().x && Map.foods[(int)minimum[0]].getPos().y == getPos().y && !carrying)
                        {

                            if (Map.foods[(int)minimum[0]].foodParts > 0)
                            {
                                carrying = true;
                                Map.foods[(int)minimum[0]].foodParts--;
                                if (Map.foods[(int)minimum[0]].foodParts <= 0)
                                {
                                    map.destroyFood((int)minimum[0]);
                                }
                            }
                        }
                    } else if (queens.Count > 0 && visRange >= closestEnemyQueenDist && state != "fighting" && !carrying && (closestEnemyQueenDist < minimum[1] && closestEnemyQueenDist < closestEnemyAntDist)) {//attacking the closest queen 
                        float minDist = 0;
                        Ant closestEnemyQueen = queens[0];
                        Vector vec = new Vector();
                        for (int i = 0; i < queens.Count; i++) {
                            vec = Vector.CreateVector(getPos(), queens[i].getPos());
                            if (i == 0) {
                                minDist = vec.distance();
                                closestEnemyQueen = queens[i];
                            }
                            if (minDist > vec.distance()) {
                                minDist = vec.distance();
                                closestEnemyQueen = queens[i];
                            }
                        }
                        if (state != "attacking_queen" && closestEnemyQueen.getPos().x != getPos().x && closestEnemyQueen.getPos().y != getPos().y)
                        {
                            state = "attacking_queen";
                        }
                        move(closestEnemyQueen.getPos(), map);
                        if (closestEnemyQueen.getPos().x == getPos().x && closestEnemyQueen.getPos().y == getPos().y)
                        {
                            if (closestEnemyQueen.State != "fighting")
                            {
                                Fight fight = new Fight();

                                fight.StartFight(this, closestEnemyQueen, map);
                                stopMovement();
                                closestEnemyQueen.stopMovement();
                                state = "fighting";
                                closestEnemyQueen.State = "fighting";
                            }
                        }
                    } else if (enemyAntsAround.Count > 0 && visRange >= closestEnemyAntDist&& state != "fighting" &&state != "attacking_queen"&& !carrying && (closestEnemyAntDist < minimum[1] && closestEnemyAntDist < closestEnemyQueenDist)) {//attacking all ants around
                        float minDist = 0;
                        Ant a = enemyAntsAround[0];
                        Vector vec = new Vector();
                        for (int i = 0; i < enemyAntsAround.Count; i++) {
                            vec = Vector.CreateVector(getPos(), enemyAntsAround[i].getPos());
                            if (i == 0) {
                                minDist = vec.distance();
                                a = enemyAntsAround[i];
                            }
                            if (minDist > vec.distance()) {
                                minDist = vec.distance();
                                a = enemyAntsAround[i];
                            }

                        }
                        move(a.getPos(), map);
                        if (a.getPos().x == getPos().x && a.getPos().y == getPos().y)
                        {
                            if (a.State != "fighting")
                            {
                                Fight fight = new Fight();

                                fight.StartFight(this, a, map);
                                stopMovement();
                                a.stopMovement();
                                state = "fighting";
                                a.State = "fighting";
                            }
                        }
                    }//all of the other states should go with else if here...
                    else
                    { //wandering/returning state
                        if (state != "fighting")
                        {
                            if (Vector.CreateVector(getPos(), antsAnthill.getAhPos()).distance() >= antsAnthill.distFromAnthill || carrying)
                            {
                                //make the ant go back to the anthill
                                state = "returning";
                            }
                            else
                            {
                                if (Vector.CreateVector(getPos(), antsAnthill.getAhPos()).distance() < 2f || map.food == null)
                                {
                                    state = "wandering";
                                }
                            }

                            if (state == "returning")
                            {
                                if (getPos().x != antsAnthill.getAhPos().x || getPos().y != antsAnthill.getAhPos().y)
                                {
                                    goToAnthill(map);

                                }
                                else
                                {
                                    if (carrying)
                                    {
                                        antsAnthill.Hunger += Food.partsValue;
                                        carrying = false;
                                    }
                                    state = "wandering";
                                }
                            }
                            else if (state == "wandering")
                            {
                                if (target == null || reachedDestination)
                                {
                                    target = map.pickRandomPoint();

                                    while (Vector.CreateVector(antsAnthill.getAhPos(), target).distance() > antsAnthill.distFromAnthill || !checkForObstacle(map, target))
                                    {
                                        target = map.pickRandomPoint();
                                    }
                                }
                                if (target != null)
                                {
                                    move(target, map);
                                }

                            }
                        }
                    }
                }
                else
                {
                    if (target == null || reachedDestination)
                    {
                        target = map.pickRandomPoint();

                        while (Vector.CreateVector(antsAnthill.getAhPos(), target).distance() > 2f)
                        {
                            target = map.pickRandomPoint();
                        }
                    }
                    if (target != null)
                    {
                        move(target, map);
                    }
                }
                //else {
                //state = "wander";
                //move(antTarget, map);
                //}
            }
        }
        public void setPos(Pos position)
        {
            pos = position;
        }
        public void die(Map map)
        {
            if (!dead) { 
                dead = true;
                if (isQueen)
                {
                    antsAnthill.queen = null;
                }
                map.gameBoard[getPos().x, getPos().y] = standingOnValue;
                antsAnthill.ants.Remove(this);
            }
        }
        Food checkForFood(float radius)
        {
            return null;
        }
        public bool checkForObstacle(Map map, Pos tpos) {
            float frac = 0;
            Vector checkingVector = Vector.CreateVector(getPos(), tpos);
            Pos checkerPos = new Pos();
            for (frac = 0; frac < 1; frac += 0.02f)
            {
                checkerPos.x = (int)(Math.Round(checkingVector.startX + (frac * (checkingVector.endX - checkingVector.startX))));
                checkerPos.y = (int)(Math.Round(checkingVector.startY + (frac * (checkingVector.endY - checkingVector.startY))));

                if (map.gameBoard[checkerPos.x, checkerPos.y] == 1) {
                    return false;
                }
            }
            return true;
        }
        void goToFood(Map map, int foodIndex)
        {
            move(Map.foods[foodIndex].getPos(), map);
        }
        void goToAnthill(Map map)
        {
            move(antsAnthill.getAhPos(), map);
        }

        void move(Pos tpos, Map map)
        {
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
                float distCovered  = MathF.Abs(((Simulation.step - movementTimeStart) * Speed));
                    if (startMovementVector != null)
                    {
                        t = distCovered / startMovementVector.distance();
                    }
                    else
                    {
                        if (movementVector == null)
                        {
                            movementVector = Vector.CreateVector(getPos(), tpos);
                        }
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

                    if ((map.DimensionX > newPos.x && newPos.x >= 0) && (map.DimensionY > newPos.y && newPos.y >= 0))
                    {
                        foreach (Colony col in Simulation.colonies)
                        {
                            foreach (Anthill ah in col.anthills)
                            {
                                foreach (Ant ant in ah.ants)
                                {
                                    if (ant.getPos().x == getPos().x && ant.getPos().y == getPos().y && ant.standingOnValue != 2 && ant.standingOnValue != 5)
                                    {
                                        standingOnValue = ant.standingOnValue;
                                    }
                                }
                            }
                        }
                        if (standingOnValue != 2 && standingOnValue != 5)
                        {
                            if (!dead)
                            {
                                map.gameBoard[pos.x, pos.y] = standingOnValue;
                            }
                            else {
                                map.gameBoard[pos.x, pos.y] = 0;
                            }
                        }
                        /*if (standingOnValue != 10)
                        {
                            map.gameBoard[pos.x, pos.y] = standingOnValue;
                        }
                        else {
                            map.gameBoard[pos.x, pos.y] = 0;
                        }*/
                        standingOnValue = map.gameBoard[newPos.x, newPos.y];
                        map.gameBoard[newPos.x, newPos.y] = 2;
                        pos.x = newPos.x;
                        pos.y = newPos.y;
                    }
                    else
                    {

                        if (newPos.x < 0) { newPos.x = 0; }
                        if (newPos.x >= map.DimensionX) { newPos.x = map.DimensionX - 1; }
                        if (newPos.y < 0) { newPos.y = 0; }
                        if (newPos.y >= map.DimensionY) { newPos.y = map.DimensionY - 1; }
                        //if (map.gameBoard[newPos.x, newPos.y] == 2)
                        //{
                        foreach (Colony col in Simulation.colonies)
                        {
                            foreach (Anthill ah in col.anthills)
                            {
                                foreach (Ant ant in ah.ants)
                                {
                                    if (ant.getPos().x == getPos().x && ant.getPos().y == getPos().y && ant.standingOnValue != 2 && ant.standingOnValue != 5)
                                    {

                                        standingOnValue = ant.standingOnValue;
                                    }
                                }
                            }
                        }
                        if (!dead)
                        {
                            map.gameBoard[pos.x, pos.y] = standingOnValue;
                        }
                        else {
                            map.gameBoard[pos.x, pos.y] = 0;
                        }
                            standingOnValue = map.gameBoard[newPos.x, newPos.y];
                            //}
                            //else
                            //{
                            //   map.gameBoard[pos.x, pos.y] = standingOnValue;
                            //   standingOnValue = map.gameBoard[newPos.x, newPos.y];
                            //}
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

                }
                else
                {
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

}

