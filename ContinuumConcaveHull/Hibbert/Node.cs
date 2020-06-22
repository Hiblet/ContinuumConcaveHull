namespace ContinuumConcaveHull
{
    public class Node
    {
        public int id;
        public double x;
        public double y;
        //public double cos; // Used for middlepoint calculations
        public double angle = 0;

        public Node(double x, double y, int id) {
            this.x = x;
            this.y = y;
            this.id = id;
        }
    }
}