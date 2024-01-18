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
            // Проверяем существование указанной папки
            if (!Directory.Exists(folderPath))
            {
                return null;
            }

            // Список названий файлов Excel
            List<string> excelFileNames = new();
            // Получаем все файлы Excel из папки
            string[] excelFiles = Directory.GetFiles(folderPath, "*.xlsx");
            // Получаем названия всех файлов Excel
            foreach (string excelFile in excelFiles)
            {
                excelFileNames.Add(Path.GetFileNameWithoutExtension(excelFile));
            }

            return excelFileNames;
        }
    }
}
