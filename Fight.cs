using System;
using System.Collections.Generic;
using System.Text;

namespace WojnaMrowisk
{
    class Fight
    {
        public Ant antA;
        public Ant antB;

       public  int standingOnValue;
        Pos position;

        Ant victorious;
        Map gMap;

        public void StartFight(Ant antOne, Ant antTwo, Map map) {
            antA = antOne;
            antB = antTwo;
            position = antOne.getPos();
            gMap = map;
            Simulation.fights.Add(this);
            //standingOnValue = map.gameBoard[position.x, position.y];
            map.gameBoard[position.x, position.y] = 5;
            
        }
       public void evaluateFight() {
            if (antA.stOnV != 2) {
                standingOnValue = antA.stOnV;
            }
            if (antB.stOnV != 2) {
                standingOnValue = antB.stOnV;
            }
            if (antA.Health > 0)
            {
                antB.Health -= antA.Damage;
            }
            if(antA.Health <= 0) {
                //antA dead
                if (antA.carrying) {
                    antA.carrying = false;
                    antB.carrying = true;
                    //destroy antA
                }
                antB.State = "wandering";
                victorious = antB;
                antA.dieAnt(gMap);
                endFight(gMap);
            }
            if (antB.Health > 0)
            {
                antA.Health -= antB.Damage;

            }
            if (antB.Health <= 0) {
                //antB dead
                if (antB.carrying)
                {
                    antB.carrying = false;
                    antA.carrying = true;
                    //destroy antB
                }
                antA.State = "wandering";
                antB.dieAnt(gMap);
                victorious = antA;
                endFight(gMap);
            }
        }
        void endFight(Map map) {
            if (standingOnValue != 2)
            {
                map.gameBoard[position.x, position.y] = standingOnValue;
                victorious.stOnV = standingOnValue;
            }
            Simulation.fights.Remove(this);
            
        }
    }
}
