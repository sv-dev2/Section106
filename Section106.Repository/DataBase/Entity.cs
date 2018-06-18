//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Section106.Repository.DataBase
{
    using System;
    using System.Collections.Generic;
    
    public partial class Entity
    {
        public long EntityId { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public int StateId { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public string Fax { get; set; }
        public string MobilePhone { get; set; }
        public string OfficePhone { get; set; }
        public Nullable<int> ContactedBy { get; set; }
        public Nullable<int> CountyId { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual County County { get; set; }
        public virtual State State { get; set; }
    }
}
