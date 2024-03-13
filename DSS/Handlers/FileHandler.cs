using DSS.Models.ViewModels;
using OfficeOpenXml;

namespace DSS.Handlers
{
    public class FileHandler
    {
        /// <summary>
        /// Получаем названия файлов Excel в папке
        /// </summary>
        /// <param name="folderPath">Путь к папке</param>
        /// <returns>Список названий файлов Excel в папке</returns>
        public static List<string>? GetExcelFileNames(string folderPath)
        {
            try
            {
                if (!Directory.Exists(folderPath))
                {
                    return null;
                }

                List<string> excelFileNames = new();

                string[] excelFiles = Directory.GetFiles(folderPath, "*.xlsx");

                foreach (string excelFile in excelFiles)
                {
                    excelFileNames.Add(Path.GetFileNameWithoutExtension(excelFile));
                }

                return excelFileNames;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Записываем программу дорожных работ в файл Excel
        /// </summary>
        /// <param name="filePath">Путь к файлу Excel</param>
        /// <param name="roadWorksProgramsData">Программа дорожных работ</param>
        /// <returns>Успешность записи программы дорожных работ в файл Excel</returns>
        public static bool WriteRoadWorksProgramToExcelFile(string filePath, List<RoadWorksProgramEstimatesViewModel> roadWorksProgramsData)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    File.Create(filePath).Close();
                }

                FileInfo fileInfo = new(filePath);

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (ExcelPackage package = new ExcelPackage())
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet1");

                    int rowIndex = 1;
                    int maxRowIndex = 0;
                    int colIndex = 1;

                    foreach (var roadWorksProgram in roadWorksProgramsData)
                    {
                        worksheet.Cells[rowIndex++, colIndex].Value = roadWorksProgram.Month;

                        foreach (var estimate in roadWorksProgram.Estimates)
                        {
                            worksheet.Cells[rowIndex, colIndex].Value = estimate.Name;
                            worksheet.Cells[rowIndex++, colIndex + 1].Value = estimate.Cost;
                        }

                        maxRowIndex = Math.Max(maxRowIndex, rowIndex);
                        rowIndex = 1;
                        colIndex += 2;
                    }

                    double roadWorksProgramCost = roadWorksProgramsData.Sum(r => r.Cost ?? 0);
                    colIndex = 1;

                    foreach (var roadWorksProgram in roadWorksProgramsData)
                    {
                        worksheet.Cells[maxRowIndex, colIndex + 1].Value = roadWorksProgram.Cost ?? 0;
                        colIndex += 2;
                    }

                    worksheet.Cells[maxRowIndex, colIndex].Value = roadWorksProgramCost;

                    package.SaveAs(fileInfo);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
