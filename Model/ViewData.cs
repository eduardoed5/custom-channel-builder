using System;

namespace CreatorChannelsXrmToolbox.Model
{
    /// <summary>
    ///  Class that handles information from view
    /// </summary>
    public class ViewData
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
}
