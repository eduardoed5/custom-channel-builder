
namespace CreatorChannelsXrmToolbox.Model
{
    /// <summary>
    /// Class that handles information from message type
    /// </summary>
    public class MessageType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
}
