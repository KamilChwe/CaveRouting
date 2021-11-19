using System.Collections.Generic;

namespace CaveRouting
{
    public class Cavern
    {
        //X and Y of this Cavern
        int posX, posY;
        //Navigation Relationship between this and every other Cavern
        List<int> pathRelations = new List<int>();
        List<Cavern> closeCaverns = new List<Cavern>();

        int id;
        int x;
        int y;

        public Cavern(int x, int y, int id)
        {
            this.x = x;
            this.y = y;
            this.id = id;
        }

        //Modifier for setting and retrieving X
        public int PosX
        {
            get { return posX; }
            set { posX = value; }
        }
        //Modifier for setting and retrieving Y
        public int PosY
        {
            get { return posY; }
            set { posY = value; }
        }

        //Modifier for setting and retrieving path relations list
        public List<int> PathRelations
        {
            get { return pathRelations; }
            set { pathRelations = value; }
        }

        public List<Cavern> closestCavern()
        {
            return closeCaverns;
        }

        public int Id
        {
            get { return id; }
            set { id = value; }
        }
    }   
}
