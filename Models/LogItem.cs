using System;
using System.Collections.Generic;
using System.Text;

namespace Funcs_DataMovement.Models {
    public class LogItem {
        public string operation { get; set; }
        public string source { get; set; }
        public string destination { get; set; }
        public string fileName { get; set; }
        public DateTime timestamp { get; set; }
        public string customerID { get; set; }
        public string version { get; set; }
    }
}
