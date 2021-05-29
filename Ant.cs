using System;
using System.Collections.Generic;
using System.Text;

namespace WojnaMrowisk
{
    class Ant : Anthill
    {
        private int hp;
        private string state = "wandering";
        private Pos pos = new Pos();
        public bool carrying;
        public float t;
        public bool reachedDestination = true;
        public float movementTimeStart;
        public Vector movementVector;
        public Vector startMovementVector;
        private Anthill antH;
        private Pos target;

        public Anthill antsAnthill {
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
        public int health
        {
            get { return hp; }
            set { hp = value; }
        }
        public Pos getPos()
        {
            return pos;
        }

        public void evaluateLogic(Map map, Pos antTarget)
        {
  
            if (map.food != null && Vector.CreateVector(getPos(), map.food.getPos()).distance() <= foodRange && !carrying)
            {
                state = "food_going";
                //reachedDestination = false;
                //startMovementVector = Vector.CreateVector(getPos(), map.food.getPos());
                goToFood(map.food, map);

                if (map.food.getPos().x == getPos().x && map.food.getPos().y == getPos().y && !carrying) {
                    if (map.food.foodParts > 0) {
                        carrying = true;
                        map.food.foodParts--;
                        if (map.food.foodParts <= 0) {
                            map.destroyFood();
                        }
                    }
                }
            }//all of the other states should go with else if here...
            else { //wandering/returning state

                if (Vector.CreateVector(getPos(), antsAnthill.getAhPos()).distance() >= antsAnthill.distFromAnthill || carrying)
                {
                    //make the ant go back to the anthill
                    state = "returning";
                }
                else {
                    if (Vector.CreateVector(getPos(), antsAnthill.getAhPos()).distance() < 2f || map.food == null)
                    {
                        state = "wandering";
                    }
                }

                if (state == "returning")
                {
                    if (getPos().x != antsAnthill.getAhPos().x|| getPos().y != antsAnthill.getAhPos().y)
                    {
                        goToAnthill(map);

                    }
                    else {
                        if (carrying) {
                            antsAnthill.Hunger += Food.partsValue;
                            carrying = false;
                        }
                        state = "wandering";
                    }
                }
                else if (state == "wandering") {
                    if (target == null || reachedDestination) {
                        target = map.pickRandomPoint();

                        while (Vector.CreateVector(antsAnthill.getAhPos(), target).distance() > antsAnthill.distFromAnthill) {
                            target = map.pickRandomPoint();
                        }
                    }
                    move(target, map);

                }
            }
            //else {
            //state = "wander";
            //move(antTarget, map);
            //}
        }
        public void setPos(Pos position)
        {
            pos = position;
        }
        void die()
        {

        }
        Food checkForFood(float radius)
        {
            return null;
        }
        void attack()
        {

        }
        void collectFood()
        {

        }
        void goToFood(Food food, Map map)
        {
            move(map.food.getPos(), map);
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

                float distCovered = MathF.Abs(((Simulation.step - movementTimeStart) * Speed));
                if (startMovementVector != null)
                {
                    t = distCovered / startMovementVector.distance();
                }
                else
                {
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
                        if (map.gameBoard[newPos.x, newPos.y] == 2)
                        {
                            foreach (Colony col in Simulation.colonies)
                            {
                                foreach (Anthill ah in col.anthills)
                                {
                                    foreach (Ant ant in ah.ants)
                                    {
                                        if (ant.getPos().x == newPos.x && ant.getPos().y == newPos.y && ant.standingOnValue != 2)
                                        {
                                            standingOnValue = ant.standingOnValue;
                                        }
                                    }
                                }
                            }
                            if (standingOnValue != 10)
                            {
                                map.gameBoard[pos.x, pos.y] = standingOnValue;
                            }
                            else {
                                map.gameBoard[pos.x, pos.y] = 0;
                            }
                            //standingOnValue = map.gameBoard[newPos.x, newPos.y];
                        }
                        else {
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
                        if (newPos.x >= map.DimensionX) { newPos.x = map.DimensionX - 1; }
                        if (newPos.y < 0) { newPos.y = 0; }
                        if (newPos.y >= map.DimensionY) { newPos.y = map.DimensionY - 1; }
                        if (map.gameBoard[newPos.x, newPos.y] == 2)
                        {
                            foreach (Colony col in Simulation.colonies)
                            {
                                foreach (Anthill ah in col.anthills)
                                {
                                    foreach (Ant ant in ah.ants)
                                    {
                                        if (ant.getPos().x == newPos.x && ant.getPos().y == newPos.y && ant.standingOnValue != 2)
                                        {
                                            standingOnValue = ant.standingOnValue;
                                        }
                                    }
                                }
                            }
                            map.gameBoard[pos.x, pos.y] = standingOnValue;
                            //standingOnValue = map.gameBoard[newPos.x, newPos.y];
                        }
                        else
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
