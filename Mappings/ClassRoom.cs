using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using FluentNHibernateExample.Models;

namespace FluentNHibernateExample.Mappings
{
    public class ClassRoomMap : ClassMap<ClassRoom>
    {
        public ClassRoomMap()
        {
            Table("ClassRoom");
            Id(x => x.ID).GeneratedBy.Identity();
            Map(x => x.Name);
            Map(x => x.Subject);

            References(x => x.Teacher)
                .Column("TeacherID")
                .Cascade.All();
        }
    }
}
