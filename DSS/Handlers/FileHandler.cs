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
    }
}
