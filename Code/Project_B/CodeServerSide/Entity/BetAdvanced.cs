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
    [DBTable("BetAdvanced")]
    [TargetDb(TargetDB.MASTER)]
    public sealed class BetAdvanced : AbstractEntityTemplateKey<BetAdvanced, int>, IBetAdvanced<int> {

        public enum Fields {
        /// <summary>
        /// 
        /// </summary>
            [DBField(DbType.Int32)] ID,

            /// <summary>
            /// 
            /// </summary>
            [Nullable]
            [DBField(DbType.Single)] Draw,

            /// <summary>
            /// 
            /// </summary>
            [Nullable]
            [DBField(DbType.Single)] Win1draw,

            /// <summary>
            /// 
            /// </summary>
            [Nullable]
            [DBField(DbType.Single)] Win1win2,

            /// <summary>
            /// 
            /// </summary>
            [Nullable]
            [DBField(DbType.Single)] Drawwin2,

        }

        /// <summary>
        /// 
        /// </summary>
        public BetAdvanced() {
        }

        /// <summary>
        /// 
        /// </summary>
        public BetAdvanced(Hashtable ht) : base(ht) {}
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

        /// <summary>
        /// 
        /// </summary>
        public float? Draw {
            get { return (float?) this[Fields.Draw]; }
            set { ForceSetData(Fields.Draw, value); }
        }

        public override Enum[] KeyFields {
            get { return new[] { (Enum) Fields.ID }; }
        }
    }
}
