using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Data;

namespace LNF.Repository.Inventory
{
    public class LabelRoom : IDataItem
    {
        public virtual int LabelRoomID { get; set; }
        public virtual string Slug { get; set; }
        public virtual string RoomName { get; set; }
    }
}
