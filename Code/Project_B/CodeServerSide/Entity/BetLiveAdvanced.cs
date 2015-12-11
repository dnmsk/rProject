using System;
using System.Collections;
using System.Data;
using CommonUtils;
using IDEV.Hydra.DAO;
using IDEV.Hydra.DAO.Attributes;
using Project_B.CodeServerSide.Entity.Interface;

namespace Project_B.CodeServerSide.Entity {
        /// <summary>
        /// 
        /// </summary>
    [Serializable]
    [DBTable("BetLiveAdvanced")]
    [TargetDb(TargetDB.MASTER)]
    public sealed class BetLiveAdvanced : AbstractEntityTemplateKey<BetLiveAdvanced, long>, IBetAdvanced<long> {

        public enum Fields {
        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int64)] ID,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Single)] Draw,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Single)] Win1draw,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Single)] Win1win2,

        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Single)] Drawwin2,

        }

        /// <summary>
        /// 
        /// </summary>
        public BetLiveAdvanced() {
        }

        /// <summary>
        /// 
        /// </summary>
        public BetLiveAdvanced(Hashtable ht) : base(ht) {}
        /// <summary>
        /// 
        /// </summary>
        public long ID {
            get { return (long) this[Fields.ID]; }
            set { ForceSetData(Fields.ID, value); }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public float? Draw {
            get { return (float?) this[Fields.Draw]; }
            set { ForceSetData(Fields.Draw, value); }
        }


        /// <summary>
        /// 
        /// </summary>
        public float? Win1draw {
            get { return (float?) this[Fields.Win1draw]; }
            set { ForceSetData(Fields.Win1draw, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public float? Win1win2 {
            get { return (float?) this[Fields.Win1win2]; }
            set { ForceSetData(Fields.Win1win2, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public float? Drawwin2 {
            get { return (float?) this[Fields.Drawwin2]; }
            set { ForceSetData(Fields.Drawwin2, value); }
        }

        public override Enum[] KeyFields {
            get { return new[] { (Enum) Fields.ID }; }
        }
    }
}
