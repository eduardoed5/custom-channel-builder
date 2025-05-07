using System;
using System.Collections.Generic;

namespace CreatorChannelsXrmToolbox.Model
{
    /// <summary>
    /// Class that handles information from response of a read operation
    /// </summary>
    public class ResponseOperationsRead
    {
        public bool State { get; set; }
        public string Message { get; set; }
        public List<FormData> ConfigurationForms { get; set; }
        public List<FormData> EditorForms { get; set; }
        public List<SolutionData> Solutions { get; set; }
        public ChannelData Channel { get; set; }
        public Exception Exception { get; set; }
    }
}
