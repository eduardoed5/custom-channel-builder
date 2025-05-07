
using System;

namespace CreatorChannelsXrmToolbox.Model
{
    /// <summary>
    /// Class that handles information from response of plugin operation
    /// </summary>
    public class ResponseOperations
    {
        public bool State { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public Exception Exception { get; set; }
    }
}
