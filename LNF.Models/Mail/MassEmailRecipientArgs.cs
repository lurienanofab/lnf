﻿using LNF.Models.Data;
using System.Collections.Generic;

namespace LNF.Models.Mail
{
    public class MassEmailRecipientArgs
    {
        public int Privs { get; set; }
        public int Communities { get; set; }
        public int Manager { get; set; }
        public IEnumerable<int> Tools { get; set; }
        public IEnumerable<int> InLab { get; set; }
    }
}
