namespace XRL.World.ZoneBuilders
{
    public class SolidMaterial
    {
        public string Material;

        public SolidMaterial() 
        {
            Material = "Shale";
        }
        public SolidMaterial(string Material) 
            : this()
        {
            this.Material = Material;
        }

        public bool BuildZone(Zone Z)
        {
            return BuildZone(Z, Material);
        }
        public bool BuildZone(Zone Z, string Material)
        {

            foreach (Cell cell in Z.GetCells())
            {
                cell.Clear();
                cell.AddObject(GameObjectFactory.Factory.CreateObject(Material));
            }
            return true;
        }
    }
}