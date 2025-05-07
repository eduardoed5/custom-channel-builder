
namespace CreatorChannelsXrmToolbox.Model
{
    /// <summary>
    /// Class that handles information from entity
    /// </summary>
    public class EntityData
    {
        public string LogicalName { get; set; }
        public string DisplayName { get; set; }
        public int Code { get; set; }
        public bool IsActivity { get; set; }
        public bool IsCustomizable { get; set; }
        public override string ToString()
        {
            return DisplayName;
        }
    }
}
