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
    
    public partial class vehicle
    {
        public int Id { get; set; }
        public string Registration_Number { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string Class { get; set; }
        public string Type { get; set; }
    
        public virtual claim claim { get; set; }
    }
}
