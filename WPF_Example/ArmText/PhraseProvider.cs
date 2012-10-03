using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ArmText
{

  public class PhraseProvider
  {

    private String phrasesFile = null;
    private String[] allPhrases = null;
    private Random generator = null;

    public PhraseProvider(String filePath)
    {
      generator = new Random((int)DateTime.Now.Ticks);
      if (File.Exists(filePath))
      {
        phrasesFile = filePath;
        return;
      }

      if (File.Exists(Environment.CurrentDirectory + @"\" + filePath))
      {
        phrasesFile = Environment.CurrentDirectory + @"\" + filePath;
        return;
      }

      throw new ArgumentException("The phrases file does not exist.");
    }

    public String GetPhrase()
    {
      if(allPhrases == null)
        LoadPhrases();

      int index = generator.Next() % allPhrases.Length;
      String phrase = allPhrases[index];
      return phrase;
    }

    private void LoadPhrases()
    {
      allPhrases = File.ReadAllLines(phrasesFile);
    }
  }

}
