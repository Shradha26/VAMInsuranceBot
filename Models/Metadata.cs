using System;

namespace VAMInsuranceBot.Models
{
    [Serializable]
    public class clientMetadata
    {
        public int ClientId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Pin { get; set; }
        public string Father_s_Name { get; set; }
        public DateTime Date_of_Birth { get; set; }
        public string Email { get; set; }
        public string Phone_Number { get; set; }
        public string PAN_Number { get; set; }
    }

    [Serializable]
    public class policyMetadata
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string Policy_Number { get; set; }
        public string Policy_Type { get; set; }
        public DateTime Starting_Date { get; set; }
        public DateTime Date_of_Expiration { get; set; }
        public bool Claim_Filed { get; set; }
        public bool Renew_Filed { get; set; }
    }

    [Serializable]
    public class verificationMetadata
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string otp { get; set; }
        public bool ver_status { get; set; }
        public DateTime time_stamp { get; set; }
    }

    [Serializable]
    public class renewMetadata
    {
        public int Id { get; set; }
        public int PId { get; set; }
        public DateTime Date { get; set; }
        public string Renewal_Number { get; set; }
        public string Status { get; set; }
    }

    [Serializable]
    public class claimMetadata
    {
        public int Id { get; set; }
        public int PId { get; set; }
        public string Claim_Number { get; set; }
        public DateTime Claim_Date { get; set; }
        public string Status { get; set; }
    }

    [Serializable]
    public class vehicleMetadata
    {
        public int Id { get; set; }
        public string Registration_Number { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string Class { get; set; }
        public string Type { get; set; }
    }

    [Serializable]
    public class accidentMetadata
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public bool Third_Party { get; set; }
    }

    [Serializable]
    public class driverMetadata
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Relation { get; set; }
        public string License_Number { get; set; }
        public bool Injured { get; set; }
    }

    [Serializable]
    public class third_partyMetadata
    {
        public int Id { get; set; }
        public string Registration_Number { get; set; }
        public string License_Number { get; set; }
        public bool Injured { get; set; }
        public bool FIR_Filed { get; set; }
    }

    [Serializable]
    public class theftMetadata
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
    }
}