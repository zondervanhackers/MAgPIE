namespace ZondervanLibrary.Harvester.Entities
{
    public partial class HarvesterDataContext : IHarvesterDataContext
    {
        partial void OnCreated()
        {
            Connection.Open();
        }
    }
}
