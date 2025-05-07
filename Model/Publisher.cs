using System;

namespace CreatorChannelsXrmToolbox.Model
{
    /// <summary>
    /// Class that handles information from publisher
    /// </summary>
    public class Publisher
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string UniqueName { get; set; }
        public string Prefix { get; set; }
        public int Numeration { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
