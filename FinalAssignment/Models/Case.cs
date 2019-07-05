using System.Collections.Generic;

namespace FinalAssignment.Models
{
    public class Case
    {
        public int CaseNumber { get; set; }
        public int FireCorner { get; set; }
        public Dictionary<int, List<int>> StreetList { get; set; }
    }
}
