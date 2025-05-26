using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate;
using NHibernate;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;

namespace FluentNHibernateExample
{
    public class NHibernateHelper
    {
        private static ISessionFactory _sessionFactory;

        private static ISessionFactory SessionFactory
        {
            get
            {
                if(_sessionFactory == null)
                {
                    string connStr = "Server=NAGAYAMA_PC;Database=Demodb;User Id=kivyrrah;Password=1234;";
                    _sessionFactory = Fluently.Configure()
                        .Database(MsSqlConfiguration.MsSql2012.ConnectionString(connStr))
                        .Mappings(m => m.FluentMappings.AddFromAssemblyOf<Program>())
                        .BuildSessionFactory();
                }

                return _sessionFactory;
            }
        }

        public static ISession GetSession()
        {
            return SessionFactory.OpenSession();
        }
    }
}
