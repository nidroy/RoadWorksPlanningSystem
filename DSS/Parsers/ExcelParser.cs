using DSS.Handlers;
using DSS.Models.ViewModels;
using OfficeOpenXml;

namespace DSS.Parsers
{
    public class ExcelParser
    {
        /// <summary>
        /// Парсим технические состояния дорог
        /// </summary>
        /// <param name="folderPath">Путь к папке с техническими состояниями дорог</param>
        /// <returns>Модели технических состояний дорог</returns>
        public static List<TechnicalConditionOfRoadViewModel>? ParseTechnicalConditionsOfRoads(string folderPath)
        {
            try
            {
                List<string>? fileNames = FileHandler.GetExcelFileNames(folderPath);

                if (fileNames == null)
                {
                    return null;
                }

                List<TechnicalConditionOfRoadViewModel> technicalConditionsOfRoads = new();

                foreach (var fileName in fileNames)
                {
                    string filePath = Path.Combine(folderPath, $"{fileName}.xlsx");

                    var fileInfo = new FileInfo(filePath);

                    if (!fileInfo.Exists)
                    {
                        return null;
                    }

                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                    using (var package = new ExcelPackage(fileInfo))
                    {
                        var worksheet = package.Workbook.Worksheets[0];

                        TechnicalConditionOfRoadViewModel technicalConditionOfRoad = new();

                        for (int r = 2; r <= worksheet.Dimension.Rows; r++)
                        {
                            for (int c = 2; c <= worksheet.Dimension.Columns; c++)
                            {
                                technicalConditionOfRoad = new()
                                {
                                    Year = int.Parse(fileName),
                                    Month = worksheet.Cells[1, c].Text,
                                    TechnicalCondition = double.Parse(worksheet.Cells[r, c].Text),
                                    RoadId = int.Parse(worksheet.Cells[r, 1].Text)
                                };

                                technicalConditionsOfRoads.Add(technicalConditionOfRoad);
                            }
                        }
                    }
                }

                return technicalConditionsOfRoads;
            }
            catch
            {
                return null;
            }
        }
    }
}
