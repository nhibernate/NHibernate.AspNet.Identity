using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpArch.Domain.DomainModel;

namespace NHibernate.AspNet.Web.Models
{
    public class Foo :  Entity
    {
        public ApplicationUser User { get; set; }
    }
}