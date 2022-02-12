using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;


namespace CombineLetters
{
    internal class Program
    {
        static void Main(string[] args)
        {                        
            //Change the rootFolder variable to wherever file path the root folder is at. 
            string rootFolder = @"";

            //The instructions stated that input files needed to be archived and it was not clear to me
            //if only those files that matched should be archived, or if everything should be.
            //With the ambiguity in mind I setup code to handle both scenarios. To archive everything, set the below bool value to true.
            //otherwise, if only things that match should be archived, set it to false.
            bool shouldAllFilesAndDirectoriesBeArchivedEvenIfNotMatched = false; 

            string inputFolder = Path.Combine(rootFolder, "Input");
            string outputFolder = Path.Combine(rootFolder, "Output");
            string archiveFolder = Path.Combine(rootFolder, "Archive");

            LetterService letterService = new LetterService();

            string inputFolderAdmissionFolder = Path.Combine(inputFolder, "Admission");
            string inputFolderScholarshipFolder = Path.Combine(inputFolder, "Scholarship");

            string archiveFolderAdmissionFolder = Path.Combine(archiveFolder, "Admission");
            string archiveFolderScholarshipFolder = Path.Combine(archiveFolder, "Scholarship");

            DirectoryInfo[] inputFolderScholarshipSubDirectories = new DirectoryInfo(inputFolderScholarshipFolder).GetDirectories();

            DirectoryInfo[] inputFolderAdmissionSubDirectories = new DirectoryInfo(inputFolderAdmissionFolder).GetDirectories();

            IEnumerable<string> inputFolderSharedSubDirectories = inputFolderScholarshipSubDirectories.Select(x => x.Name)
                                                                                                        .Intersect(inputFolderAdmissionSubDirectories
                                                                                                        .Select(x => x.Name));

            try
            {


                foreach (var sharedFolder in inputFolderSharedSubDirectories)
                {
                    string sharedOutputDirectory = Path.Combine(outputFolder, sharedFolder);
                    string admissionSharedArchiveFolder = Path.Combine(archiveFolderAdmissionFolder, sharedFolder);
                    string scholarshipSharedArchiveFolder = Path.Combine(archiveFolderScholarshipFolder, sharedFolder);

                    FileInfo[] inputSharedFolderAdmissionFolderFiles = new DirectoryInfo(Path.Combine(inputFolderAdmissionFolder, sharedFolder)).GetFiles();
                    FileInfo[] inputSharedFolderScholarshipFolderFiles = new DirectoryInfo(Path.Combine(inputFolderScholarshipFolder, sharedFolder)).GetFiles();


                    List<string> filesWithMatchedID = inputSharedFolderAdmissionFolderFiles
                                                .Select(y => System.Text.RegularExpressions.Regex.Replace(y.Name, "[^0-9]", ""))
                                                .Intersect(inputSharedFolderScholarshipFolderFiles.Select(y => System.Text.RegularExpressions.Regex.Replace(y.Name, "[^0-9]", ""))).ToList();

                    if (filesWithMatchedID.Count > 0)
                    {
                        List<string> combinedFileIDs = new List<string>();

                        foreach (var matchedFileID in filesWithMatchedID)
                        {

                            FileInfo admissonFile = inputSharedFolderAdmissionFolderFiles.Where(x => x.Name.Contains(matchedFileID)).FirstOrDefault();
                            FileInfo scholarshipFile = inputSharedFolderScholarshipFolderFiles.Where(x => x.Name.Contains(matchedFileID)).FirstOrDefault();

                            string combinedFile = $"combined-{matchedFileID}.txt";

                            if (Directory.Exists(sharedOutputDirectory) == false)
                            {
                                Directory.CreateDirectory(sharedOutputDirectory);
                            }

                            letterService.CombineTwoLetters(admissonFile.FullName, scholarshipFile.FullName, Path.Combine(sharedOutputDirectory, combinedFile));

                            ArchiveFiles(admissionSharedArchiveFolder, admissonFile);

                            ArchiveFiles(scholarshipSharedArchiveFolder, scholarshipFile);

                            combinedFileIDs.Add(matchedFileID);

                        }

                        if (File.Exists(Path.Combine(sharedOutputDirectory, "Report.txt")))
                        {
                            File.Delete(Path.Combine(sharedOutputDirectory, "Report.txt"));
                        }
                        using (StreamWriter sw = File.AppendText(Path.Combine(sharedOutputDirectory, "Report.txt")))
                        {
                            sw.WriteLine($"{DateTime.Now.ToShortDateString()} Report");
                            sw.WriteLine("-------------------------------\n");
                            sw.WriteLine($"Number of Combined Letters: {combinedFileIDs.Count}");
                            foreach (var combinedIDs in combinedFileIDs)
                            {
                                sw.WriteLine(combinedIDs);
                            }
                        }
                    }

                    //If shouldAllFilesAndDirectoriesBeArchivedEvenIfNotMatched set to true, will archive the unique files
                    //that are within the matched folders. 
                    if (shouldAllFilesAndDirectoriesBeArchivedEvenIfNotMatched == true)
                    {
                        List<string> uniqueAdmissionFiles = inputSharedFolderAdmissionFolderFiles
                                                    .Select(y => System.Text.RegularExpressions.Regex.Replace(y.Name, "[^0-9]", ""))
                                                    .Except(inputSharedFolderScholarshipFolderFiles.Select(y => System.Text.RegularExpressions.Regex.Replace(y.Name, "[^0-9]", ""))).ToList();

                        if (uniqueAdmissionFiles.Count > 0)
                        {
                            foreach (var uniqueAdmissionFile in uniqueAdmissionFiles)
                            {
                                FileInfo admissonFile = inputSharedFolderAdmissionFolderFiles.Where(x => x.Name.Contains(uniqueAdmissionFile)).FirstOrDefault();
                                ArchiveFiles(admissionSharedArchiveFolder, admissonFile);
                            }
                        }

                        List<string> uniqueScholarshipFiles = inputSharedFolderScholarshipFolderFiles
                                                    .Select(y => System.Text.RegularExpressions.Regex.Replace(y.Name, "[^0-9]", ""))
                                                    .Except(inputSharedFolderAdmissionFolderFiles.Select(y => System.Text.RegularExpressions.Regex.Replace(y.Name, "[^0-9]", ""))).ToList();

                        if (uniqueScholarshipFiles.Count > 0)
                        {
                            foreach (var uniqueSchoarshipFile in uniqueScholarshipFiles)
                            {
                                FileInfo scholarshipFile = inputSharedFolderScholarshipFolderFiles.Where(x => x.Name.Contains(uniqueSchoarshipFile)).FirstOrDefault();
                                ArchiveFiles(scholarshipSharedArchiveFolder, scholarshipFile);
                            }
                        }
                    }

                    //Verify that the shared folders are empty, if so, delete. 
                    inputSharedFolderAdmissionFolderFiles = new DirectoryInfo(Path.Combine(inputFolderAdmissionFolder, sharedFolder)).GetFiles();
                    inputSharedFolderScholarshipFolderFiles = new DirectoryInfo(Path.Combine(inputFolderScholarshipFolder, sharedFolder)).GetFiles();
                    if (inputSharedFolderAdmissionFolderFiles.Count() == 0)
                    {
                        Directory.Delete(Path.Combine(inputFolderAdmissionFolder, sharedFolder));
                    }

                    if (inputSharedFolderScholarshipFolderFiles.Count() == 0)
                    {
                        Directory.Delete(Path.Combine(inputFolderScholarshipFolder, sharedFolder));
                    }
                }

                //If shouldAllFilesAndDirectoriesBeArchivedEvenIfNotMatched set to true, will archive the unique folders 
                if (shouldAllFilesAndDirectoriesBeArchivedEvenIfNotMatched == true)
                {
                    IEnumerable<string> inputFolderScholarshipOnlyFolders = inputFolderScholarshipSubDirectories.Select(x => x.Name)
                                                                                                                .Except(inputFolderAdmissionSubDirectories.Select(x => x.Name));
                    foreach (string uniqueScholarshipFolder in inputFolderScholarshipOnlyFolders)
                    {
                        string unqiueScholarshipArchiveFolder = Path.Combine(archiveFolderScholarshipFolder, uniqueScholarshipFolder);
                        FileInfo[] inputFolderScholarshipFolderFiles = new DirectoryInfo(Path.Combine(inputFolderScholarshipFolder, uniqueScholarshipFolder)).GetFiles();

                        foreach (FileInfo scholarshipFile in inputFolderScholarshipFolderFiles)
                        {
                            ArchiveFiles(unqiueScholarshipArchiveFolder, scholarshipFile);
                        }

                        inputFolderScholarshipFolderFiles = new DirectoryInfo(Path.Combine(inputFolderScholarshipFolder, uniqueScholarshipFolder)).GetFiles();
                        //Verify that the folders are empty, if so, delete. 
                        if (inputFolderScholarshipFolderFiles.Count() == 0)
                        {
                            Directory.Delete(Path.Combine(inputFolderScholarshipFolder, uniqueScholarshipFolder));
                        }
                    }

                    IEnumerable<string> inputFolderAdmissionOnlyFolders = inputFolderAdmissionSubDirectories.Select(x => x.Name)
                                                                                                            .Except(inputFolderScholarshipSubDirectories.Select(x => x.Name));
                    foreach (string uniqueAdmissionFolder in inputFolderAdmissionOnlyFolders)
                    {
                        string uniqueAdmissionArchiveFolder = Path.Combine(archiveFolderAdmissionFolder, uniqueAdmissionFolder);
                        FileInfo[] inputFolderAdmissionFolderFiles = new DirectoryInfo(Path.Combine(inputFolderAdmissionFolder, uniqueAdmissionFolder)).GetFiles();
                        foreach (FileInfo admissionFile in inputFolderAdmissionFolderFiles)
                        {
                            ArchiveFiles(uniqueAdmissionArchiveFolder, admissionFile);
                        }

                        inputFolderAdmissionFolderFiles = new DirectoryInfo(Path.Combine(inputFolderAdmissionFolder, uniqueAdmissionFolder)).GetFiles();
                        //Verify that the folders are empty, if so, delete. 
                        if (inputFolderAdmissionFolderFiles.Count() == 0)
                        {
                            Directory.Delete(Path.Combine(inputFolderAdmissionFolder, uniqueAdmissionFolder));
                        }
                    }
                }
            }
            catch (IOException io)
            {
                Console.WriteLine($"An error occured when handling the files and or directories. Stacktrace: {io.StackTrace}");
                Console.WriteLine("Press Enter");
                Console.ReadLine();
            }            
        }

        public static void ArchiveFiles(string destinationFolder, FileInfo fileToMove)
        {

            if (Directory.Exists(destinationFolder) == false)
            {
                Directory.CreateDirectory(destinationFolder);
            }

            if (File.Exists(Path.Combine(destinationFolder, fileToMove.Name)))
            {
                File.Delete(Path.Combine(destinationFolder, fileToMove.Name));
            }

            File.Move(fileToMove.FullName, Path.Combine(destinationFolder, fileToMove.Name));
        }
    }
}
