namespace WojnaMrowisk
{
    internal class Food
    {
        public static int partsValue = 20;
        private readonly Pos position = new Pos();

        public int foodParts { get; set; }
        
        public void SetPos(int x, int y)
        {
            position.x = x;
            position.y = y;
        }

        public Pos GetPos()
        {
            var pos = new Pos {x = position.x, y = position.y};
            return pos;
        }
    }
}