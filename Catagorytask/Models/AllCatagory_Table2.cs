//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Catagorytask.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class AllCatagory_Table2
    {
        public int Id { get; set; }
        public Nullable<int> Sno { get; set; }
        public string Catagory { get; set; }
        public string Name { get; set; }
        public Nullable<int> Quantity { get; set; }
        public Nullable<System.DateTime> Created_on { get; set; }
        public string Created_by { get; set; }
        public Nullable<System.DateTime> Updated_on { get; set; }
        public string Updated_by { get; set; }
        public Nullable<int> Price { get; set; }
        public Nullable<bool> Status_report { get; set; }
    }
}