using System.Data;

namespace DSS.Modules
{
    public class MainModule
    {
        public static List<(string, DataTable)>? CreatePlans()
        {
            List<(string, DataTable)> plans = new();

            // Таблица 1
            DataTable table1 = new DataTable();
            table1.Columns.Add("Id", typeof(int));
            table1.Columns.Add("Name", typeof(string));
            table1.Columns.Add("Value", typeof(double));

            table1.Rows.Add(1, "Row1", 10.5);
            table1.Rows.Add(2, "Row2", 20.3);
            table1.Rows.Add(3, "Row3", 15.7);

            plans.Add(("t1", table1));

            // Таблица 2
            DataTable table2 = new DataTable();
            table2.Columns.Add("Id", typeof(int));
            table2.Columns.Add("Name", typeof(string));
            table2.Columns.Add("Value", typeof(double));

            table2.Rows.Add(1, "Row1", 10);
            table2.Rows.Add(2, "Row2", 20);
            table2.Rows.Add(3, "Row3", 15);

            plans.Add(("t2", table2));

            return plans;
        }
    }
}
