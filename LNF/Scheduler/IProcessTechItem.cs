﻿namespace LNF.Scheduler
{
    public interface IProcessTechItem : ILabItem
    {
        int ProcessTechID { get; set; }
        string ProcessTechName { get; set; }
    }
}
