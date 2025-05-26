using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentNHibernateExample.Models
{
    public class ClassRoom
    {
        public virtual int ID { get; set; }
        public virtual string Name { get; set; }
        public virtual string Subject { get; set; }
        public virtual Teacher Teacher { get; set; }
    }
}
