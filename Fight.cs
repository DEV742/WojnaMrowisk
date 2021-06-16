namespace WojnaMrowisk
{
    internal class Fight
    {
        public Ant antA;
        public Ant antB;
        private Map gMap;
        private Pos position;

        public int standingOnValue;

        private Ant victorious;

        //Resembles a Start() method in the Simulation class
        //Used to initialise and set up the fight
        public void StartFight(Ant antOne, Ant antTwo, Map map)
        {
            antA = antOne;
            antB = antTwo;
            position = antOne.GetPos();
            gMap = map;
            Simulation.fights.Add(this);
            map.gameBoard[position.x, position.y] = 5;
        }

        //A method used to evaluate logic of the ongoing fight
        public void evaluateFight()
        {
            if (antA.CurrentlyStandingOn != 2) standingOnValue = antA.CurrentlyStandingOn;
            if (antB.CurrentlyStandingOn != 2) standingOnValue = antB.CurrentlyStandingOn;
            if (antA.Health > 0) antB.Health -= antA.Damage;
            if (antA.Health <= 0)
            {
                //antA dead
                if (antA.carrying)
                {
                    antA.carrying = false;
                    antB.carrying = true;
                    //Destroy antA
                }

                antB.State = "wandering";
                victorious = antB;
                antA.DieAnt(gMap);
                endFight(gMap);
            }

            if (antB.Health > 0) antA.Health -= antB.Damage;
            if (antB.Health <= 0)
            {
                //antB dead
                if (antB.carrying)
                {
                    antB.carrying = false;
                    antA.carrying = true;
                    //Destroy antB
                }

                antA.State = "wandering";
                antB.DieAnt(gMap);
                victorious = antA;
                endFight(gMap);
            }
        }

        //A method that finalises the fight, puts it off the map and from simulation
        private void endFight(Map map)
        {
            map.gameBoard[position.x, position.y] = 0;
            victorious.CurrentlyStandingOn = standingOnValue;
            Simulation.fights.Remove(this);
        }
    }
}