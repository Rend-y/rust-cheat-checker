using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace RCC
{    public class file_information
    {
        public string file_name { get; set; }
        public string create_date { get; set; }
        public string directory { get; set; }
        public string size { get; set; }
        public file_information(string file_name, string create_date, string directory, string size)
        {
            this.create_date = create_date;
            this.directory = directory;
            this.size = size;
            this.file_name = file_name;
        }
    }
    public class SearcherFiles
    {
        private readonly DirectoryInfo directory_info;
        private readonly string pattern_for_search;
        private readonly DateTime min_create_time;
        private readonly DateTime max_create_time;
        private readonly double min_file_size;
        private readonly double max_file_size;
        public SearcherFiles(DirectoryInfo directory_info, string pattern_for_search, DateTime min_create_time, DateTime max_create_time, double min_file_size, double max_file_size)
        {
            this.directory_info = directory_info;
            this.pattern_for_search = pattern_for_search;
            this.min_create_time = min_create_time;
            this.max_create_time = max_create_time;
            this.min_file_size = min_file_size;
            this.max_file_size = max_file_size;
            find_files = new List<file_information>();
        }
        public readonly List<file_information> find_files;
        /// <summary>
        /// use this to find all the files in the directory
        /// </summary>
        /// <param name="dir">directory for finding</param>
        /// <returns>all file in the directory as list</returns>
        public List<file_information> search_files(DirectoryInfo dir)
        {
            List<file_information> result = new List<file_information>();
            dir.GetFiles(this.pattern_for_search, SearchOption.TopDirectoryOnly).ToList().ForEach(file => // get all files at directory
            {
                try
                {
                    if (file.CreationTime.Date >= this.min_create_time && file.CreationTime.Date <= this.max_create_time && (double)file.Length / 1000 <= this.max_file_size && (double)file.Length / 1000 >= this.min_file_size)
                    {
                        try { result.Add(new file_information(file.Name, file.CreationTime.ToString(CultureInfo.InvariantCulture), file.DirectoryName, (file.Length / 1024).ToString())); } // adding to result array new file
                        catch { Thread.Sleep(1); } // if this file gives an error
                    }
                }
                catch { Thread.Sleep(1); } // if this file gives an error.
            });
            return result;
        }

        public void start_search()
        {
            try
            {
                List<DirectoryInfo> info_about_current_directory = directory_info.GetDirectories().ToList();
                if (info_about_current_directory.Count != 0)
                {
                    info_about_current_directory.ForEach(directory => // For each directory, we launch a new search
                    {
                        SearcherFiles second = new SearcherFiles(directory, pattern_for_search, min_create_time, max_create_time, min_file_size, max_file_size);
                        second.start_search(); // Start search in subdirectory
                        find_files.AddRange(second.find_files); // Get list of all files which statisfy all conditions
                    });
                }
                find_files.AddRange(search_files(this.directory_info)); // Find all files in current directory
            }
            catch
            {
                // ignored
            }
        }
    }
}