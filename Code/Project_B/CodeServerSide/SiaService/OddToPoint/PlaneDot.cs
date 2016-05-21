using System.Linq;

namespace Project_B.CodeServerSide.SiaService.OddToPoint {
    public class PlaneDot {
        public readonly double[] Dot;
        public readonly float[] Odds;

        public PlaneDot(params float[] odds) {
            Odds = odds;
            Dot = odds.Select(odd => 1d / odd).ToArray();
        }
    }
}