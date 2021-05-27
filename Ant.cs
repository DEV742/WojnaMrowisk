using System;
using System.Collections.Generic;
using System.Text;

namespace WojnaMrowisk
{
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
        void goToAnthill(Anthill ah)
        {

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
                        if (newPos.x >= map.DimensionX) { newPos.x = map.DimensionX - 1; }
                        if (newPos.y < 0) { newPos.y = 0; }
                        if (newPos.y >= map.DimensionY) { newPos.y = map.DimensionY - 1; }
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
