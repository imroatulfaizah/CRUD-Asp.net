using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TheStartUpBuddy.Models
{
    public class DefaultModel
    {

    }

    public class Users
    {
        public int Admin_ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Profile_Photo { get; set; }
        public string Gender { get; set; }
        public string Age { get; set; }
        public string Address { get; set; }
        public string Position { get; set; }

    }

    public class Position
    {
        public int ID { get; set; }
        public string PositionName { get; set; }
    }
}