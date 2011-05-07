using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.ActiveRecord;
using Raven.Client;

namespace Spider.Domain
{
    public class Ground
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string CricketArchiveUrl { get; set; }
        public string Address { get; set; }
        public string HomeTeam { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public static string GenerateId(int groundId)
        {
            return "ground/" + groundId;
        }


        public static Ground Load(IDocumentStore store, string id)
        {
            using (IDocumentSession session = store.OpenSession())
            {
                return session.Load<Ground>(id);
            }
        }

        public static void Save(IDocumentStore store, Ground ground)
        {
            using (IDocumentSession session = store.OpenSession())
            {
                session.Store(ground);
                session.SaveChanges();
            }
        }
    }
}
