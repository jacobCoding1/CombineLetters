using System;
using System.IO;

public interface ILetterService
{
	///<summary>
	/// Combine two letter files into one file.
	///</summary>
	///<param name = "inputFile1">File path for the first letter.</param>
	///<param name = "inputFile2">File path for the second letter.</param>
	///<param name = "resultFile">File path for the combined letter.</param>
	void CombineTwoLetters(string inputFile1, string inputFile2, string resultFile)
}

public class LetterService : ILetterService
{
	public void CombineTwoLetters(string inputFile1, string inputFile2, string resultFile)
	{
        if (inputFile1 != null)
        {
			System.IO.File.
        }
	}
}