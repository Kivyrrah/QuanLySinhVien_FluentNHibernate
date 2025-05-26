using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentNHibernateExample.Models
{
    public class Student
    {
        public virtual int ID { get; set; }
        public virtual string Name { get; set; }
        public virtual DateTime Birthday { get; set; }
        public virtual string Address { get; set; }
        public virtual ClassRoom ClassRoom { get; set; }
    }
}
