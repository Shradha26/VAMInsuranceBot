using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAMInsuranceBot.Models;
using VAMInsuranceBot.Process;

namespace VAMInsuranceBot.Access
{
    public class DBAccess
    {
        private static DateTime time;

        public static bool CheckPanNumber(string panNo)
        {
            using (var db = new ModelEntities())
            {
                var query = from c in db.clients
                            where c.PAN_Number == panNo
                            select c;

                if (query.Count() > 0)
                    return true;
                else
                    return false;
            }
        }

        public static bool CheckDOB(string panNo, string DOB)
        {
            DateTime dob = Convert.ToDateTime(DOB);

            using (var db = new ModelEntities())
            {
                var query = from c in db.clients
                            where c.PAN_Number == panNo && c.Date_of_Birth == dob
                            select c;

                if (query.Count() > 0)
                    return true;
                else
                    return false;
            }
        }

        public static bool CheckPolicyNumber(string polNo)
        {
            using (var db = new ModelEntities())
            {
                var query = from p in db.policies
                            where p.Policy_Number == polNo
                            select p;

                if (query.Count() > 0)
                    return true;
                else
                    return false;
            }
        }

        private static int? GetPolicyID(string polNo)
        {
            int? PId = null;

            using (var db = new ModelEntities())
            {
                var query = from p in db.policies
                            where p.Policy_Number == polNo
                            select p;

                foreach (var item in query)
                {
                    PId = item.Id;
                }
            }

            return PId;
        }

        private static int? GetClientID(string polNo)
        {
            int? CId = null;

            using (var db = new ModelEntities())
            {
                var query = from p in db.policies
                            where p.Policy_Number == polNo
                            select p;

                foreach (var item in query)
                {
                    CId = item.ClientId;
                }
            }

            return CId;
        }

        public static string GetClientName(string panNo)
        {
            string name = null;

            using (var db = new ModelEntities())
            {
                var query = from c in db.clients
                            where c.PAN_Number == panNo
                            select c;

                foreach(var item in query)
                {
                    name = item.Name;
                }
            }

            return name.Substring(0, name.IndexOf(' '));
        }

        public static string GetPolicies(string panNo)
        {
            StringBuilder data = new StringBuilder();
            ICollection<policy> policies = null;

            using (var db = new ModelEntities())
            {
                var query = from c in db.clients
                            where c.PAN_Number == panNo
                            select c;

                foreach(var item in query)
                {
                    policies = item.policies;
                }

                int counter = 1;

                foreach(var item in policies)
                {
                    data.Append(counter++ + ". " + item.Policy_Number);
                    data.Append("\n\n");
                }
            }

            return data.ToString();
        }

        private static string ViewPolicyData(int? PId)
        {
            StringBuilder data = new StringBuilder();
            ICollection<claim> claims = null;
            ICollection<renew> renews = null;

            using (var db = new ModelEntities())
            {
                var query = from p in db.policies
                            where p.Id == PId
                            select p;

                foreach (var item in query)
                {
                    data.Append("Policy Number: ");
                    data.Append(item.Policy_Number);
                    data.Append("\n\n");
                    data.Append("Policy Type: ");
                    data.Append(item.Policy_Type);
                    data.Append("\n\n");
                    data.Append("Starting Date: ");
                    data.Append(item.Starting_Date.ToShortDateString());
                    data.Append("\n\n");
                    data.Append("Date of Expiration: ");
                    data.Append(item.Date_of_Expiration.ToShortDateString());
                    data.Append("\n\n");

                    renews = item.renews;
                    data.Append("Renewals:");
                    data.Append("\n\n");

                    if (renews.Count > 0)
                    {
                        foreach (var r in renews)
                        {
                            data.Append("Renewal Number: ");
                            data.Append(r.Renewal_Number);
                            data.Append("\n\n");
                            data.Append("File date: ");
                            data.Append(r.Date.ToShortDateString());
                        }
                    }
                    else
                    {
                        data.Append("No renewal history found for policy number " + item.Policy_Number);
                    }

                    claims = item.claims;
                    data.Append("\n\n");
                    data.Append("Claims:-");
                    data.Append("\n\n");

                    if (claims.Count > 0)
                    {
                        foreach (var c in claims)
                        {
                            data.Append("Claim Number: ");
                            data.Append(c.Claim_Number);
                            data.Append("\n\n");
                            data.Append("File date: ");
                            data.Append(c.Claim_Date.ToShortDateString());
                        } 
                    }
                    else
                    {
                        data.Append("No claims found for policy number " + item.Policy_Number);
                    }
                    
                }
            }

            return data.ToString();
        }

        private static string ViewClientData(int? CId)
        {
            StringBuilder data = new StringBuilder();

            using (var db = new ModelEntities())
            {
                var query = from c in db.clients
                            where c.ClientId == CId
                            select c;

                foreach (var item in query)
                {
                    data.Append("Name: ");
                    data.Append(item.Name);
                    data.Append("\n\n");
                    data.Append("Address: ");
                    data.Append(item.Address);
                    data.Append(", ");
                    data.Append(item.City);
                    data.Append(", ");
                    data.Append(item.State);
                    data.Append(" - ");
                    data.Append(item.Pin);
                    data.Append("\n\n");
                    data.Append("Father\'s Name: ");
                    data.Append(item.Father_s_Name);
                    data.Append("\n\n");
                    data.Append("Date of Birth: ");
                    data.Append(item.Date_of_Birth.ToShortDateString());
                    data.Append("\n\n");
                    data.Append("Email: ");
                    data.Append(item.Email);
                    data.Append("\n\n");
                    data.Append("Phone Number: ");
                    data.Append(item.Phone_Number);
                }
            }

            return data.ToString();
        }

        public static string ViewData(string polNo)
        {
            StringBuilder data = new StringBuilder();

            data.Append(ViewClientData(GetClientID(polNo)));
            data.Append("\n\n");
            data.Append(ViewPolicyData(GetPolicyID(polNo)));

            return data.ToString();
        }

        public static DateTime GetRenewDate(string polNo)
        {
            DateTime date = DateTime.MinValue;

            using (var db = new ModelEntities())
            {
                var query = from p in db.policies
                            where p.Policy_Number == polNo
                            select p;

                foreach (var item in query)
                {
                    date = item.Date_of_Expiration;
                }
            }

            return date;
        }

        public static bool ValidatePolicy(string polNo)
        {
            if ((GetRenewDate(polNo) - DateTime.UtcNow).TotalMinutes > 0)
                return true;
            else
                return false;
        }

        public static bool DuplicateClaim(string polNo, out DateTime? claimDate, out string claimNumber)
        {
            bool duplicate = false;
            int? PId = null;
            claimDate = null;
            claimNumber = string.Empty;

            using (var db = new ModelEntities())
            {
                var query_p = from p in db.policies
                            where p.Policy_Number == polNo
                            select p;

                foreach (var item in query_p)
                {
                    duplicate = item.Claim_Filed;
                    PId = item.Id;
                }

                var query_cl = from cl in db.claims
                               where cl.PId == PId && cl.Status == "OPEN"
                               select cl;

                foreach(var item in query_cl)
                {
                    claimDate = item.Claim_Date;
                    claimNumber = item.Claim_Number;
                }
            }

            return duplicate;
        }

        public static void GenerateOtpAndSendEmail(string panNo)
        {
            Otp.GenerateOtp();

            using (var db = new ModelEntities())
            {
                int? CId = null;

                var query_c = from c in db.clients
                               where c.PAN_Number == panNo
                               select c;

                foreach (var item in query_c)
                {
                    Email.recipient = item.Email;
                    CId = item.ClientId;
                }

                var query_v = from v in db.verifications
                               where v.ClientId == CId.Value
                               select v;

                if (query_v != null && query_v.Count() > 0)
                {
                    foreach (var item in query_v)
                    {
                        item.otp = Otp.otp;
                        item.ver_status = false;
                        item.time_stamp = DateTime.UtcNow;
                    }
                }
                else
                {
                    var v = new verification { ClientId = CId.Value, otp = Otp.otp, ver_status = false, time_stamp = DateTime.UtcNow };
                    db.verifications.Add(v);
                }
                db.SaveChanges();
            }

            Email.message = string.Format("Your One Time Password is {0}. This is only valid for 10 mins.", Otp.otp);
            //Email.SendEmail();
        }

        public static bool AuthenticateOTP(string otp)
        {
            bool auth = false;

            using (var db = new ModelEntities())
            {
                var query = from v in db.verifications
                            where v.otp == otp
                            select v;

                time = DateTime.UtcNow;

                foreach (var item in query)
                {
                    if ((time - item.time_stamp).TotalMinutes <= 10)
                    {
                    if (query != null && query.Count() > 0)
                        {
                            auth = true;
                            item.ver_status = true;

                        }
                    }
                }
                db.SaveChanges();
            }
            return auth;
        }

        public static void FileRenew(string polNo)
        {
            using (var db = new ModelEntities())
            {
                var query = from p in db.policies
                            where p.Policy_Number == polNo
                            select p;

                foreach (var item in query)
                {
                    item.Renew_Filed = true;
                }
                db.SaveChanges();
            }
        }

        public static void FileClaim(string polNo)
        {
            using (var db = new ModelEntities())
            {
                var query = from p in db.policies
                            where p.Policy_Number == polNo
                            select p;

                foreach(var item in query)
                {
                    item.Claim_Filed = true;
                }
                db.SaveChanges();
            }
        }

        public static bool CheckExpiration(string polNo, out double days)
        {
            DateTime? end_date = null;

            using (var db = new ModelEntities())
            {
                var query = from p in db.policies
                            where p.Policy_Number == polNo
                            select p;

                foreach(var item in query)
                {
                    end_date = item.Date_of_Expiration;
                }
            }

            days = (end_date.Value - DateTime.UtcNow).TotalDays;

            if ((end_date.Value - DateTime.UtcNow).TotalMinutes <= 131400)
                return true;
            else
                return false;
        }
    }
}