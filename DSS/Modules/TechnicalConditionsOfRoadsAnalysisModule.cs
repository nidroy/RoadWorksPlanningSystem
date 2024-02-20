namespace DSS.Modules
{
    public class TechnicalConditionsOfRoadsAnalysisModule
    {
        private double CalculateTechnicalConditionOfRoad(double initialTechnicalConditionOfRoad, int t, double k)
        {
            double exp = Math.Exp(-(t * k));
            double technicalConditionOfRoad = initialTechnicalConditionOfRoad * exp;

            if (technicalConditionOfRoad > 5)
                technicalConditionOfRoad = 5;
            if (technicalConditionOfRoad < 0.5)
                technicalConditionOfRoad = 0.5;

            return Math.Round(technicalConditionOfRoad, 1);
        }
    }
}
