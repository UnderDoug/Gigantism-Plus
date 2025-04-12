namespace XRL.World.ZoneBuilders
{
	public class ReachabilityNorthEdge
	{
		public bool ClearFirst = true;


        public bool BuildZone(Zone Z)
        {
            if (ClearFirst)
            {
                Z.ClearReachableMap();
            }
            for (int i = 0; i < Z.Height; i++)
            {
                Z.BuildReachableMap(Z.Width - 1, 0);
            }
            return true;
        }
	}
}
