using System;

namespace CreatorChannelsXrmToolbox.Model
{
    /// <summary>
    /// Class that handles information from custom API
    /// </summary>
    public class CustomAPI
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string UniqueName { get; set; }
        public string DisplayName { get; set; }
        public override string ToString()
        {
            return DisplayName;
        }
    }
}
