//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VAMInsuranceBot.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class third_party
    {
        public int Id { get; set; }
        public string Registration_Number { get; set; }
        public string License_Number { get; set; }
        public bool Injured { get; set; }
        public bool FIR_Filed { get; set; }
    
        public virtual claim claim { get; set; }
    }
}
