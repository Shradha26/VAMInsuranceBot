using System;
using System.ComponentModel.DataAnnotations;

namespace VAMInsuranceBot.Models
{
    [MetadataType(typeof(clientMetadata))]
    public partial class client
    {
        
    }

    [MetadataType(typeof(policyMetadata))]
    public partial class policy
    {

    }

    [MetadataType(typeof(verificationMetadata))]
    public partial class verification
    {

    }

    [Serializable]
    [MetadataType(typeof(renewMetadata))]
    public partial class renew
    {
        public renew()
        {
            Renewal_Number = string.Empty;
        }
    }

    [Serializable]
    [MetadataType(typeof(claimMetadata))]
    public partial class claim
    {
        public claim()
        {
            Claim_Number = string.Empty;
        }
    }

    [Serializable]
    [MetadataType(typeof(driverMetadata))]
    public partial class driver
    {
        public driver()
        {
            Name = string.Empty;
            Relation = string.Empty;
            License_Number = string.Empty; ;
        }
    }

    [Serializable]
    [MetadataType(typeof(accidentMetadata))]
    public partial class accident
    {
        public accident()
        {
            Location = string.Empty;
            Description = string.Empty;
        }
    }

    [Serializable]
    [MetadataType(typeof(vehicleMetadata))]
    public partial class vehicle
    {
        public vehicle()
        {
            Registration_Number = string.Empty;
            Make = string.Empty;
            Model = string.Empty;
            Class = string.Empty;
            Type = string.Empty;
        }
    }

    [Serializable]
    [MetadataType(typeof(third_partyMetadata))]
    public partial class third_party
    {
        public third_party()
        {
            Registration_Number = string.Empty;
            License_Number = string.Empty;
        }
    }

    [Serializable]
    [MetadataType(typeof(theftMetadata))]
    public partial class theft
    {
        public theft()
        {
            Type = string.Empty;
            Location = string.Empty;
            Description = string.Empty;
            
        }
    }
}