using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using FluentNHibernateExample.Models;

namespace FluentNHibernateExample.Mappings
{
    public class TeacherMap : ClassMap<Teacher>
    {
        public TeacherMap()
        {
            Table("Teacher");
            Id(x => x.ID).GeneratedBy.Identity();
            Map(x => x.Name);
            Map(x => x.Birthday);
        }
    }
}
