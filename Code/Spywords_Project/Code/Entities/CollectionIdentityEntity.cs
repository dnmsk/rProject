using System.Collections;
using IDEV.Hydra.DAO;
using Spywords_Project.Code.Statuses;

namespace Spywords_Project.Code.Entities {
    public abstract class CollectionIdentityEntity<T> : AbstractEntityTemplateKey<T, int>
        where T : AbstractEntityTemplateKey<T, int> {

        public abstract CollectionIdentity CollectionIdentity { get; set; }
        public CollectionIdentityEntity(Hashtable ht) : base(ht) { }
        public CollectionIdentityEntity() { } 
    }
}