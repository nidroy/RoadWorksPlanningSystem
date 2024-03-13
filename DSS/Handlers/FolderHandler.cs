namespace DSS.Handlers
{
    public class FolderHandler
    {
        /// <summary>
        /// Получаем названия подпапок в папке
        /// </summary>
        /// <param name="folderPath">Путь к папке</param>
        /// <returns>Список названий подпапок в папке</returns>
        public static List<string>? GetSubfolderNames(string folderPath)
        {
            try
            {
                if (!Directory.Exists(folderPath))
                {
                    return null;
                }

                List<string> subfolderNames = new();

                string[] subfolders = Directory.GetDirectories(folderPath);

                foreach (string subfolder in subfolders)
                {
                    subfolderNames.Add(Path.GetFileName(subfolder));
                }

                return subfolderNames;
            }
            catch
            {
                return null;
            }
        }
    }
}
