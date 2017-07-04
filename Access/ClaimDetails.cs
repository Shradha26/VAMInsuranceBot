using System;
using System.Linq;
using System.Text;
using VAMInsuranceBot.Models;

#pragma warning disable CS0168

namespace VAMInsuranceBot.Access
{
    public class ClaimDetails
    {
        private static int? GetPId(string polNo)
        {
            int? PId = null;

            using (var db = new ModelEntities())
            {
                var query = from p in db.policies
                            where p.Policy_Number == polNo
                            select p;

                foreach (var items in query)
                {
                    PId = items.Id;
                }
            }

            return PId;
        }

        public static string GenerateClaim(string polNo, out string cn)
        {
            cn = Process.SerialNumber.GenerateClaimNumber();
            int pi = GetPId(polNo).Value;

            using (var db = new ModelEntities())
            {
                var c = new claim { PId = pi, Claim_Number = cn, Claim_Date = DateTime.UtcNow, Status = "OPEN" };
                db.claims.Add(c);
                db.SaveChanges();
            }

            DBAccess.FileClaim(polNo);

            return cn;
        }

        public static string GenerateRenew(string polNo, out string rn)
        {
            rn = Process.SerialNumber.GenerateRenewalNumber();
            int pi = GetPId(polNo).Value;

            using (var db = new ModelEntities())
            {
                var r = new renew { PId = pi, Renewal_Number = rn, Date = DateTime.UtcNow, Status = "OPEN" };
                db.renews.Add(r);
                db.SaveChanges();
            }

            DBAccess.FileRenew(polNo);

            return rn;
        }

        private static int? GetClaimID(int PId)
        {
            int? CId = null;

            using (var db = new ModelEntities())
            {
                var query = from c in db.claims
                            where c.PId == PId
                            select c;

                foreach (var item in query)
                {
                    CId = item.Id;
                }
            }

            return CId;
        }

        private static void SaveVehicle(vehicle veh, int Id)
        {
            veh.Id = Id;

            using (var db = new ModelEntities())
            {
                var v = veh;
                db.vehicles.Add(v);
                db.SaveChanges();
            }
        }

        private static void SaveDriver(driver dri, int Id)
        {
            dri.Id = Id;

            using (var db = new ModelEntities())
            {
                var d = dri;
                db.drivers.Add(d);
                db.SaveChanges();
            }
        }

        private static void SaveAccident(accident acc, int Id)
        {
            acc.Id = Id;

            using (var db = new ModelEntities())
            {
                var a = acc;
                db.accidents.Add(a);
                db.SaveChanges();
            }
        }

        private static void SaveTheft(theft thef, int Id)
        {
            thef.Id = Id;

            using (var db = new ModelEntities())
            {
                var t = thef;
                db.thefts.Add(t);
                db.SaveChanges();
            }
        }

        private static void SaveThirdParty(third_party tp, int Id)
        {
            tp.Id = Id;

            using (var db = new ModelEntities())
            {
                var t = tp;
                db.third_party.Add(t);
                db.SaveChanges();
            }
        }

        public static void SaveDamageDetails(string polNo, vehicle veh, driver dri, accident acc, third_party tp = null)
        {
            int Id = GetClaimID(GetPId(polNo).Value).Value;

            SaveVehicle(veh, Id);
            SaveDriver(dri, Id);
            SaveAccident(acc, Id);
            if(tp != null)
            {
                SaveThirdParty(tp, Id);
            }
        }

        public static void SaveTheftDetails(vehicle veh, theft thef, string polNo)
        {
            int Id = GetClaimID(GetPId(polNo).Value).Value;

            SaveVehicle(veh, Id);
            SaveTheft(thef, Id);
        }

        public static string ViewClaimDetails(vehicle veh, theft thef = null, accident acc = null, driver dri = null, third_party tp = null)
        {
            StringBuilder data = new StringBuilder();

            if (acc != null)
            {
                data.Append("ACCIDENT:");
                data.Append("\n\n");
                data.Append("Date: ");
                data.Append(acc.Date);
                data.Append("\n\n");
                data.Append("Location: ");
                data.Append(acc.Location);
                data.Append("\n\n");
                data.Append("Third party involvement: ");
                data.Append(acc.Third_Party);
                data.Append("\n\n");
                data.Append("Description: ");
                data.Append(acc.Description);
                data.Append("\n\n");
                data.Append("\n\n");
            }

            if (dri != null)
            {
                data.Append("DRIVER:");
                data.Append("\n\n");
                data.Append("Name: ");
                data.Append(dri.Name);
                data.Append("\n\n");
                data.Append("Relation: ");
                data.Append(dri.Relation);
                data.Append("\n\n");
                data.Append("License Number: ");
                data.Append(dri.License_Number);
                data.Append("\n\n");
                data.Append("Injured: ");
                data.Append((dri.Injured ? "Yes" : "No"));
                data.Append("\n\n");
                data.Append("\n\n");
            }

            if(tp != null)
            {
                data.Append("THIRD PARTY:");
                data.Append("\n\n");
                data.Append("Registration Number: ");
                data.Append(veh.Registration_Number);
                data.Append("\n\n");
                data.Append("License Number: ");
                data.Append(dri.License_Number);
                data.Append("\n\n");
                data.Append("Injured: ");
                data.Append((tp.Injured ? "Yes" : "No"));
                data.Append("\n\n");
                data.Append("FIR Filed: ");
                data.Append((tp.FIR_Filed ? "Yes" : "No"));
            }

            if (thef != null)
            {
                data.Append("THEFT:");
                data.Append("\n\n");
                data.Append("Type: ");
                data.Append(thef.Type);
                data.Append("\n\n");
                data.Append("Date: ");
                data.Append(thef.Date);
                data.Append("\n\n");
                data.Append("Location: ");
                data.Append(thef.Location);
                data.Append("\n\n");
                data.Append("Description:");
                data.Append(thef.Description);
                data.Append("\n\n");
                data.Append("\n\n");
            }

            data.Append("VEHICLE:");
            data.Append("\n\n");
            data.Append("Registration Number: ");
            data.Append(veh.Registration_Number);
            data.Append("\n\n");
            data.Append("Make: ");
            data.Append(veh.Make);
            data.Append("\n\n");
            data.Append("Model: ");
            data.Append(veh.Model);
            data.Append("\n\n");
            data.Append("Class: ");
            data.Append(veh.Class);
            data.Append("\n\n");
            data.Append("Type: ");
            data.Append(veh.Type);
            data.Append("\n\n");
            data.Append("\n\n");

            return data.ToString();
        }

        public static bool ValidateDate(DateTime dateTime, string polNo)
        {
            DateTime? start_date = null;
            DateTime? end_date = null;

            using (var db = new ModelEntities())
            {
                var query = from p in db.policies
                            where p.Policy_Number == polNo
                            select p;

                foreach(var item in query)
                {
                    start_date = item.Starting_Date;
                    end_date = item.Date_of_Expiration;
                }
            }

            if ((dateTime - start_date.Value).TotalMinutes > 0 && (end_date.Value - dateTime).TotalMinutes > 0 && (DateTime.UtcNow - dateTime).TotalMinutes > 0)
                return true;
            else
                return false;
        }

        public static bool CheckDateFormat(string date)
        {
            DateTime dateTime;
            try
            {
                dateTime = Convert.ToDateTime(date);
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }
    }
}