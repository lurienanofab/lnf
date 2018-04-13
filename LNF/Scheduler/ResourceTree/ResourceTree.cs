using LNF.Models.Data;
using LNF.Repository;
using LNF.Repository.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Scheduler.ResourceTree
{
    public class ResourceTree
    {
        public ClientItem Client { get; }
        public IEnumerable<ResourceTreeItem> Items { get; }
        public PathInfo Path { get; }
        public ClientSetting Setting { get; }
        public IEnumerable<BuildingNode> Buildings { get; }

        public ResourceTree(ClientItem client, IEnumerable<ResourceTreeItem> items, string path)
        {
            Client = client ?? throw new ArgumentNullException("client");
            Items = items ?? throw new ArgumentNullException("items");
            Path = PathInfo.Parse(path);
            Setting = GetSetting();
            Buildings = GetBuildings();
        }

        private ClientSetting GetSetting()
        {
            var setting = DA.Current.Single<ClientSetting>(Client.ClientID);

            if (setting == null)
            {
                setting = ClientSetting.CreateWithDefaultValues(Client.ClientID);
                DA.Current.Insert(setting);
            }

            return setting;
        }

        private IEnumerable<BuildingNode> GetBuildings()
        {
            int expandedId;

            expandedId = Path.BuildingID == 0 ? Setting.BuildingID.GetValueOrDefault() : Path.BuildingID;

            var buildings = DA.Current.Query<Building>().Where(x => x.IsActive).Select(x => new BuildingNode()
            {
                BuildingID = x.BuildingID,
                BuildingName = x.BuildingName,
                BuildingDescription = x.Description,
                Expanded = x.BuildingID == expandedId
            }).ToList();

            expandedId = Path.LabID == 0 ? Setting.LabID.GetValueOrDefault() : Path.LabID;

            var labs = DA.Current.Query<Lab>().Where(x => x.IsActive).Select(x => new LabNode()
            {
                BuildingID = x.Building.BuildingID,
                LabID = x.LabID,
                LabName = x.DisplayName,
                LabDescription = x.Description,
                Expanded = x.LabID == expandedId
            }).ToList();

           expandedId = Path.ProcessTechID;

            var proctechs = DA.Current.Query<ProcessTech>().Where(x => x.IsActive).Select(x => new ProcessTechNode()
            {
                LabID = x.Lab.LabID,
                ProcessTechID = x.ProcessTechID,
                ProcessTechName = x.ProcessTechName,
                ProcessTypeDescription = x.Description,
                Expanded = x.ProcessTechID == expandedId
            }).ToList();

            expandedId = Path.ResourceID;

            var resources = Items.Select(x => new ResourceNode()
            {
                ProcessTechID = x.ProcessTechID,
                ResourceID = x.ResourceID,
                ResourceName = x.ResourceName,
                Expanded = x.ResourceID == expandedId
            }).ToList();

            foreach (var bldg in buildings)
            {
                bldg.Labs = labs.Where(x => x.BuildingID == bldg.BuildingID).OrderBy(x => x.LabName).ToList();

                foreach (var lab in bldg.Labs)
                {
                    lab.ProcessTechs = proctechs.Where(x => x.LabID == lab.LabID).OrderBy(x => x.ProcessTechName).ToList();

                    foreach (var pt in lab.ProcessTechs)
                    {
                        pt.Resources = resources.Where(x => x.ProcessTechID == pt.ProcessTechID).OrderBy(x => x.ResourceName).ToList();
                    }
                }
            }

            return buildings;
        }
    }

}
