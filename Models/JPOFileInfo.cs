using System;
using System.Collections.Generic;
using System.Text;

namespace Funcs_DataMovement.Models {
    public class JPOFileInfo {
        public string source { get; set; }  //Source Container Name
        public string destination { get; set; } //Destination Container name
        public string tags { get; set; }
        public string origin { get; set; }
        public string fileName { get; set; }
        public DateTime date { get; set; }
        public string description { get; set; }
        public string customerID { get; set; }
    }
}
