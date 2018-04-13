using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Scheduler.ResourceTree
{
    public class PathInfo
    {
        public int BuildingID { get; }
        public int LabID { get; }
        public int ProcessTechID { get; }
        public int ResourceID { get; }

        private PathInfo(int buildingId, int labId, int proctechId, int resourceId)
        {
            BuildingID = buildingId;
            LabID = labId;
            ProcessTechID = proctechId;
            ResourceID = resourceId;
        }

        public static PathInfo Parse(string path)
        {
            int[] parts = string.IsNullOrEmpty(path)
                ? new[] { 0, 0, 0, 0 }
                : path.Split('/').Select(x =>
                {
                    if (int.TryParse(x, out int result))
                        return result;
                    else
                        throw new Exception("Invalid character in path. Only integers are allowed, e.g. 1/2/3");
                }).ToArray();

            return new PathInfo(
                buildingId: parts.Length > 0 ? parts[0] : 0,
                labId: parts.Length > 1 ? parts[1] : 0,
                proctechId: parts.Length > 2 ? parts[2] : 0,
                resourceId: parts.Length > 3 ? parts[3] : 0
            );
        }

        public IEnumerable<int> Items()
        {
            var items = new List<int>();

            if (BuildingID > 0)
            {
                items.Add(BuildingID);
                if (LabID > 0)
                {
                    items.Add(LabID);
                    if (ProcessTechID > 0)
                    {
                        items.Add(ProcessTechID);
                        if (ResourceID > 0)
                        {
                            items.Add(ResourceID);
                        }
                    }
                }
            }

            return items;
        }

        public override string ToString()
        {
            return string.Join("/", Items());
        }
    }
}
