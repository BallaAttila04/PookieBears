using System;

namespace PookieBears.Dnn.Dnn_PookieBears_HelloWorld.Models
{
    public class Item
    {
        public int ItemId { get; set; }
        public int ModuleId { get; set; }
        public string ItemName { get; set; }
        public string ItemDescription { get; set; }
        public int AssignedUserId { get; set; }
        public int CreatedByUserId { get; set; }
        public DateTime CreatedOnDate { get; set; }
        public int LastModifiedByUserId { get; set; }
        public DateTime LastModifiedOnDate { get; set; }
    }
}