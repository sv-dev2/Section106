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
    
    public partial class Correspondence
    {
        public long CorrespondenceId { get; set; }
        public long CorrespondenceTypeId { get; set; }
        public long RequestId { get; set; }
        public string Body { get; set; }
        public System.DateTime Date { get; set; }
    
        public virtual CorrespondenceType CorrespondenceType { get; set; }
        public virtual Request Request { get; set; }
    }
}