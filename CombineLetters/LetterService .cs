using System.IO; 
namespace CombineLetters
{
	public interface ILetterService
	{
		///<summary>
		/// Combine two letter files into one file.
		///</summary>
		///<param name = "inputFile1">File path for the first letter.</param>
		///<param name = "inputFile2">File path for the second letter.</param>
		///<param name = "resultFile">File path for the combined letter.</param>
		void CombineTwoLetters(string inputFile1, string inputFile2, string resultFile);
	}
	public class LetterService : ILetterService
	{
		public void CombineTwoLetters(string inputFile1, string inputFile2, string resultFile)
		{
			if (inputFile1 != null)
			{
				File.Copy(inputFile1, resultFile, true);
			}
			if (inputFile2 != null)
            {
				string[] inputFile2Lines = File.ReadAllLines(inputFile2);
                using (StreamWriter sw = File.AppendText(resultFile))
                {
					sw.WriteLine(); 
                    foreach (var line in inputFile2Lines)
                    {						
						sw.WriteLine(line);
                    }
                }
            }
		}
	}
}
