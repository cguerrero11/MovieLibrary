using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace MovieLibrary
{
    class Program
    {

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {

            logger.Info("Program started");

            string file = "movies.csv";
            if (!File.Exists(file))
            {
                logger.Error("File does not exist: {File}", file);
            }
            else
            {
                
                List<Int64> IDs = new List<Int64>();
                List<string> Titles = new List<string>();
                List<string> Genres = new List<string>();
                
                try
                {
                    // file reader
                    StreamReader sr = new StreamReader(file);

                    sr.ReadLine();
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        int idx = line.IndexOf('"');
                        if (idx == -1)
                        {
                            // organizing strings
                            string[] movieDetails = line.Split(',');
                            IDs.Add(Int64.Parse(movieDetails[0]));
                            Titles.Add(movieDetails[1]);
                            Genres.Add(movieDetails[2].Replace("|", ", "));
                        }
                        else
                        {


                            IDs.Add(Int64.Parse(line.Substring(0, idx - 1)));
                            
                            line = line.Substring(idx + 1);
                            idx = line.IndexOf('"');
                            Titles.Add(line.Substring(0, idx));
                            line = line.Substring(idx + 2);
                            Genres.Add(line.Replace("|", ", "));
                        }
                    }
                    sr.Close();
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message);
                }
                
                string choice;
                do
                {
                    // display
                    Console.WriteLine("1) Add Movie");
                    Console.WriteLine("2) Display All Movies");
                    Console.WriteLine("Enter to quit");

                    
                    choice = Console.ReadLine();
                    logger.Info("User choice: {Choice}", choice);

                    if (choice == "1")
                    {
                        // make new movie
                        Console.WriteLine("Enter the movie title");
                        string movieTitle = Console.ReadLine();
                        List<string> LowerCaseMovieTitles = Titles.ConvertAll(t => t.ToLower());
                        if (LowerCaseMovieTitles.Contains(movieTitle.ToLower()))
                        {
                            logger.Info("Duplicate movie title {Title}", movieTitle);
                        }
                        else
                        {
                            Int64 movieId = IDs.Max() + 1;
                            List<string> genres = new List<string>();
                            string genre;
                            do
                            {
                                Console.WriteLine("Enter genre (or done to quit)");
                                genre = Console.ReadLine();
                                if (genre != "done" && genre.Length > 0)
                                {
                                    genres.Add(genre);
                                }
                            } while (genre != "done");

                            if (genres.Count == 0)
                            {
                                genres.Add("[No Genres Listed]");
                            }
                            string genresString = string.Join("|", genres);
                            movieTitle = movieTitle.IndexOf(',') != -1 ? $"\"{movieTitle}\"" : movieTitle;
                            StreamWriter sw = new StreamWriter(file, true);
                            //writing to file
                            sw.WriteLine($"{movieId},{movieTitle},{genresString}");
                            sw.Close();
                            // add movie details to Lists
                            IDs.Add(movieId);
                            Titles.Add(movieTitle);
                            Genres.Add(genresString);
                        }
                    }
                    else if (choice == "2")
                    {
                        for (int i = 0; i < IDs.Count; i++)
                        {
                            Console.WriteLine($"Id: {IDs[i]}");
                            Console.WriteLine($"Title: {Titles[i]}");
                            Console.WriteLine($"Genre(s): {Genres[i]}");
                            Console.WriteLine();
                        }
                    }
                } while (choice == "1" || choice == "2");
            }

            logger.Info("Program ended");
        }
    }
}
