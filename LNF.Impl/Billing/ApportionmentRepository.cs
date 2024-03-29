﻿using LNF.Billing;
using LNF.Cache;
using LNF.CommonTools;
using LNF.Data;
using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using LNF.Impl.Repository.Billing;
using LNF.Impl.Repository.Data;
using LNF.PhysicalAccess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Impl.Billing
{
    public class ApportionmentRepository : RepositoryBase, IApportionmentRepository
    {
        protected IPhysicalAccessService PhysicalAccess { get; }

        public ApportionmentRepository(ISessionManager mgr, IPhysicalAccessService physicalAccess) : base(mgr)
        {
            PhysicalAccess = physicalAccess;
        }

        public void CheckClientIssues()
        {
            throw new Exception("Use IReportRepository.SendUserApportionmentReport");
            //DateTime period = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            //var options = new UserApportionmentReportOptions { Period = period };
            //Report.SendUserApportionmentReport(options);
        }

        public int PopulateRoomApportionData(DateTime period)
        {
            return Session.GetNamedQuery("PopulateRoomApportionData")
                .SetParameter("Period", period)
                .UniqueResult<int>();
        }

        public IEnumerable<IApportionmentClient> SelectApportionmentClients(DateTime startDate, DateTime endDate)
        {
            return Session.GetNamedQuery("SelectApportionmentClients")
                .SetParameter("StartDate", startDate)
                .SetParameter("EndDate", endDate)
                .List<ApportionmentClient>();
        }

        public CheckPassbackViolationsProcessResult CheckPassbackViolations(DateTime sd, DateTime ed)
        {
            var startedAt = DateTime.Now;
            var data = new List<string>();
            
            int[] clientIds = PhysicalAccess.GetPassbackViolations(sd, ed).ToArray();
            var totalPassbackViolations = clientIds.Length;

            foreach (int id in clientIds)
            {
                var client = Require<ClientInfo>(id);
                string recip = client.Email;
                string subj = "Lab access data anomaly - please check!";
                string body = client.DisplayName + ":<br /><br />"
                 + "There appears to have been an error with your record of entrances/exists from "
                 + $"one or more of the {GlobalSettings.Current.CompanyName} laboratories. Please check the system to ensure that the time "
                 + "recorded is correct.<br /><br />"
                 + "This matter must be resolved by the close of the third business day of this month.";

                if (recip.Trim().Length > 0)
                    SendEmail.SendSystemEmail("LNF.Billing.ApportionmentUtility.CheckPassbackViolations", subj, body, new[] { recip });

                data.Add($"Has passback violation: {recip}");
            }

            var result = new CheckPassbackViolationsProcessResult(startedAt, data)
            {
                TotalPassbackViolations = totalPassbackViolations
            };

            return result;
        }

        public decimal GetApportionment(IClientAccount ca, IRoom room, DateTime period)
        {
            if (ca == null) return 0;
            if (room == null) return 0;
            RoomApportionment appor = Session.Query<RoomApportionment>().FirstOrDefault(x => x.Client.ClientID == ca.ClientID && x.Account.AccountID == ca.AccountID && x.Room == room && x.Period == period);
            if (appor == null) return 0;
            return appor.ChargeDays;
        }

        public int GetPhysicalDays(DateTime period, int clientId, int roomId)
        {
            var roomData = Session.Query<RoomData>().Where(x => x.Period == period && x.ClientID == clientId && (x.RoomID == roomId || x.ParentID == roomId));
            return roomData.Select(x => x.EvtDate).Distinct().Count();
        }

        public int GetMinimumDays(DateTime period, int clientId, int roomId, int orgId)
        {
            var query = Session.Query<ToolData>().Where(x => x.Period == period && x.ClientID == clientId && x.IsActive);
            var toolData = query.CreateModels<IToolData>();
            var actDates = toolData.Where(x => IsInRoom(x, roomId) && IsInOrg(x, orgId)).Select(x => x.ActDate.ToString("yyyy-MM-dd")).ToList();
            var result = actDates.Distinct().Count();
            return result;
        }

        public bool IsInOrg(IToolData td, int orgId)
        {
            // check if the orgId is a.OrgID
            return orgId == td.OrgID;
        }

        public bool IsInRoom(IToolData td, int roomId)
        {
            // It is possible that td.RoomID is null, however this hasn't happened since Dec 2007 so it
            // probably won't happen again, but the column does allow nulls.
            if (!td.RoomID.HasValue) return false;

            // get the room for this td
            var r = CacheManager.Current.GetRoom(td.RoomID.Value);

            // Sometimes there is a room (e.g. Conference Room) with a non-null RoomID that does not 
            // have an entry in the Room table. In this case td.RoomID != roomId.
            if (r == null)
                return false;

            // check if roomId is either r.RoomID or r.ParentID
            return roomId == r.RoomID || roomId == r.ParentID;
        }

        public decimal GetTotalEntries(DateTime period, int clientId, int roomId)
        {
            var roomData = Session.Query<RoomData>().Where(x => x.Period == period && x.ClientID == clientId && (x.RoomID == roomId || x.ParentID == roomId));
            if (roomData != null && roomData.Count() > 0)
                return Convert.ToDecimal(roomData.Sum(x => x.Entries));
            else
                return 0;
        }

        public decimal GetAccountEntries(DateTime period, int clientId, int roomId, int accountId)
        {
            var roomBilling = Session.Query<RoomBilling>().Where(x => x.Period == period && x.ClientID == clientId && x.RoomID == roomId && x.AccountID == accountId).ToArray();
            if (roomBilling != null && roomBilling.Length > 0)
                return roomBilling.Sum(x => x.Entries);
            else
                return 0;
        }

        public void UpdateRoomBillingEntries(DateTime period, int clientId, int roomId, int accountId, decimal entries)
        {
            RoomBilling rb = Session.Query<RoomBilling>().FirstOrDefault(x => x.Period == period && x.ClientID == clientId && x.RoomID == roomId && x.AccountID == accountId);

            if (rb == null)
                throw new Exception($"Cannot find RoomBilling record for Period = #{period:yyyy-MM-dd}#, ClientID = {clientId}, RoomID = {roomId}, AccountID = {accountId}");

            rb.Entries = entries;

            RoomBillingUserApportionData appData = Session.Query<RoomBillingUserApportionData>().FirstOrDefault(x => x.Period == period && x.Client.ClientID == clientId && x.Room.RoomID == roomId && x.Account.AccountID == accountId);

            if (appData == null)
            {
                appData = new RoomBillingUserApportionData()
                {
                    Client = Require<Client>(rb.ClientID),
                    Account = Require<Account>(rb.AccountID),
                    Room = Require<Room>(rb.RoomID),
                    Period = period,
                    ChargeDays = 0,
                    Entries = 0
                };

                Session.Save(appData);
            }
            else
            {
                appData.Entries = entries;
                Session.Update(appData);
            }
        }

        public decimal GetDefaultApportionmentPercentage(int clientId, int roomId, int accountId)
        {
            ApportionmentDefault item = Session.Query<ApportionmentDefault>()
                .FirstOrDefault(x => x.Client.ClientID == clientId && x.Room.RoomID == roomId && x.Account.AccountID == accountId);

            if (item == null)
                return 0;
            else
                return item.Percentage;
        }

        public int UpdateChildRoomEntryApportionment(DateTime period, int clientId, int parentRoomId)
        {
            int result = 0;

            Room parentRoom = Session.Get<Room>(parentRoomId);

            //make sure this really is a parent room
            if (parentRoom.ParentID == null)
            {
                //need to know the child rooms
                Room[] children = Session.Query<Room>().Where(x => x.ParentID == parentRoomId).ToArray();

                //continue only if there are children rooms
                if (children.Length > 0)
                {
                    //entries and physical days for each child room
                    var entries = Session.Query<RoomData>()
                        .Where(x => x.Period == period && x.ClientID == clientId && x.ParentID == parentRoomId)
                        .GroupBy(x => x.RoomID)
                        .Select(g => new { RoomID = g.Key, TotalEntries = g.Sum(n => n.Entries), PhysicalDays = (decimal)g.Select(n => n.EvtDate).Distinct().Count() })
                        .ToArray();

                    //child room apportionment records, this is what we'll be updating
                    RoomApportionment[] childAppor = Session.Query<RoomApportionment>().Where(x => x.Room.ParentID == parentRoomId && x.Period == period && x.Client.ClientID == clientId).ToArray();

                    //also update the child user apportionment data
                    RoomBillingUserApportionData[] childUserAppor = Session.Query<RoomBillingUserApportionData>().Where(x => x.Room.ParentID == parentRoomId && x.Period == period && x.Client.ClientID == clientId).ToArray();

                    //parent room apportionment records
                    RoomApportionment[] parentAppor = Session.Query<RoomApportionment>().Where(x => x.Period == period && x.Client.ClientID == clientId && x.Room == parentRoom).ToArray();

                    //get the pct for each acct based on parent room day apporiontment
                    foreach (RoomApportionment pa in parentAppor)
                    {
                        decimal pct = Convert.ToDecimal(Math.Round(pa.ChargeDays / pa.PhysicalDays, 3, MidpointRounding.AwayFromZero));

                        //get the total for each child room
                        foreach (Room child in children)
                        {
                            var e = entries.FirstOrDefault(x => x.RoomID == child.RoomID);

                            //e will be null if they didn't go into a room (e.g. wet chem)
                            if (e != null)
                            {
                                //the pct for this acct
                                decimal chargeDays = e.PhysicalDays * pct;
                                decimal chargeEntries = Convert.ToDecimal(e.TotalEntries) * pct;

                                //get the child RoomApportionment record
                                RoomApportionment ca = childAppor.FirstOrDefault(x => x.Room == child && x.Account == pa.Account);

                                if (ca != null)
                                {
                                    //update the record
                                    ca.Entries = chargeEntries;
                                    ca.ChargeDays = chargeDays;
                                    result++;
                                }

                                //update/insert the user apportionment data
                                RoomBillingUserApportionData uad = childUserAppor.FirstOrDefault(x => x.Room == child && x.Account == pa.Account);
                                if (uad != null)
                                {
                                    uad.Entries = chargeEntries;
                                    uad.ChargeDays = chargeDays;
                                }
                                else
                                {
                                    //create the record if it does not exist
                                    uad = new RoomBillingUserApportionData()
                                    {
                                        Client = pa.Client,
                                        Account = pa.Account,
                                        Room = child,
                                        Period = period,
                                        ChargeDays = chargeDays,
                                        Entries = chargeEntries
                                    };
                                    Session.Save(uad);
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}