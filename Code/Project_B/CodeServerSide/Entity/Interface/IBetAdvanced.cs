namespace Project_B.CodeServerSide.Entity.Interface {
    public interface IBetAdvanced<T> {
        /// <summary>
        /// 
        /// </summary>
        T BetID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        float? Draw { get; set; }

        /// <summary>
        /// 
        /// </summary>
        float? Win1draw { get; set; }

        /// <summary>
        /// 
        /// </summary>
        float? Win1win2 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        float? Drawwin2 { get; set; }

        bool Save();
    }
}