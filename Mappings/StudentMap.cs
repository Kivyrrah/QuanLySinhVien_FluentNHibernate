using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using FluentNHibernateExample.Models;

namespace FluentNHibernateExample.Mappings
{
    public class StudentMap : ClassMap<Student>
    {
        public StudentMap()
        {
            Table("Student");
            Id(x => x.ID).GeneratedBy.Identity();
            Map(x => x.Name);
            Map(x => x.Birthday);
            Map(x => x.Address);

            References(x => x.ClassRoom)
                .Column("ClassRoomID")
                .Cascade.All();
        }
    }
}
