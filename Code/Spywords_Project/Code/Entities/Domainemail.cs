﻿using System;
using System.Collections;
using System.Data;
using CommonUtils;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.Attributes;
using MainLogic.Consts;

namespace Spywords_Project.Code.Entities {
        /// <summary>
        /// 
        /// </summary>
    [Serializable]
    [DBTable("Domainemail")]
    [TargetDb(TargetDB.MASTER)]
    public sealed class Domainemail : AbstractEntityTemplateKey<Domainemail, int> {

        public enum Fields {
        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] ID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] DomainID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.String)] Email,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.DateTime)] Datecreated,

        }

        /// <summary>
        /// 
        /// </summary>
        public Domainemail() {
        }

        /// <summary>
        /// 
        /// </summary>
        public Domainemail(Hashtable ht) : base(ht) {}
        /// <summary>
        /// 
        /// </summary>
        public int ID {
            get { return (int) this[Fields.ID]; }
            set { ForceSetData(Fields.ID, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int DomainID {
            get { return (int) this[Fields.DomainID]; }
            set { ForceSetData(Fields.DomainID, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Email {
            get { return (string) this[Fields.Email]; }
            set { ForceSetData(Fields.Email, CropString(value, DbRestrictions.STRING_FIELD_LENGTH_256)); }
        }

        /// <summary>
        /// 
        /// </summary>
        public DateTime Datecreated {
            get { return (DateTime) this[Fields.Datecreated]; }
            set { ForceSetData(Fields.Datecreated, value); }
        }

        public override Enum[] KeyFields {
            get { return new[] { (Enum) Fields.ID }; }
        }
    }
}