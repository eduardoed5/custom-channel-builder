using System;

namespace CreatorChannelsXrmToolbox.Model
{
    /// <summary>
    /// Class that handles information from form
    /// </summary>
    public class FormData
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
}
