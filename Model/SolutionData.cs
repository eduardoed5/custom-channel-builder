using System;

namespace CreatorChannelsXrmToolbox.Model
{
    /// <summary>
    ///  Class that handles information from solution
    /// </summary>
    public class SolutionData
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public override string ToString()
        {
            return DisplayName;
        }
    }
}
