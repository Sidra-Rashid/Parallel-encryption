
using System.IO;
using System.Collections;
using System.Security.Authentication;
using System.Threading;

class Program {
    static void Main(string[] args) {

        Encrypt encryptor_t = new();
        Encrypt encryptor_s = new();

        string filePath = Path.Combine(Directory.GetCurrentDirectory(), "A Non-programming Example.txt");

        encryptor_t.ReadFile(filePath);
        encryptor_s.ReadFile(filePath);
        
        encryptor_t.CreateKey(5000);
        encryptor_s.CreateKey(5000);

        ArrayList key = encryptor_t.GetKey();
        ArrayList key_s = encryptor_s.GetKey();

        int threadCount = 6; // Number of threads to use
        Thread[] threads = new Thread[threadCount];
        int wordsPerThread = encryptor_t.input.Count / threadCount;
        object lockObject = new();

        var watch = System.Diagnostics.Stopwatch.StartNew();

        for (int i = 0; i < threadCount; i++) {
            int start = i * wordsPerThread;
            int end = (i == threadCount - 1) ? encryptor_t.input.Count : (i + 1) * wordsPerThread;

            threads[i] = new Thread(() => EncryptWords(encryptor_t, key, start, end, lockObject));
            threads[i].Start();
        }

        foreach (Thread thread in threads) 
            thread.Join();
        

        watch.Stop();
        Console.WriteLine("Time taken with threads: {0}ms", watch.Elapsed.TotalMilliseconds);

        using (StreamWriter outputFile = new StreamWriter(Path.Combine(Directory.GetCurrentDirectory(), "threaded encryption.txt")))
        {
            for (int i = 0; i < encryptor_t.input.Count; i++)
            {
                string word = (string)encryptor_t.input[i];
                outputFile.WriteLine(word);
            }
        }    

    static void EncryptWords(Encrypt encryptor, ArrayList key, int start, int end, object lockObject)
    {
        for (int i = start; i < end; i++)
        {
            string word = (string)encryptor.input[i];
            string encryptedWord = word;

            for (int j = 0; j < encryptor.keyLength; j++)
            {
                encryptedWord = Encrypt.Cipher(encryptedWord, (int)key[j]);
            }

            lock (lockObject)
            {
                encryptor.input[i] = encryptedWord;
            }
        }
    }
 

        watch = System.Diagnostics.Stopwatch.StartNew();

        using (StreamWriter outputFile = new StreamWriter(Path.Combine(Directory.GetCurrentDirectory(), "serial encryption.txt"))) {
            foreach (string word in encryptor_s.input) {
                string final_encryption = word;
                for(int i = 0; i < encryptor_s.keyLength; i++) {
                    final_encryption = Encrypt.Cipher(final_encryption, (int)key[i]);
                }
                outputFile.WriteLine(final_encryption);
            }          
        }
        watch.Stop();
        Console.WriteLine("Time taken without threads: {0}ms", watch.Elapsed.TotalMilliseconds);

                
        ArrayList back = new();
        back = encryptor_t.ReadFile(Path.Combine(Directory.GetCurrentDirectory(), "threaded encryption.txt"), back);


        using (StreamWriter outputFile = new StreamWriter(Path.Combine(Directory.GetCurrentDirectory(), "threaded decryption.txt"))) {
            foreach (string encryptedLine in back) {
                string decryptedLine = encryptedLine;
                for(int i = encryptor_t.keyLength - 1; i >= 0; i--) {
                    decryptedLine = Encrypt.Cipher(decryptedLine, -(int)key[i]);
                }
                outputFile.WriteLine(decryptedLine);
            }          
        }

        ArrayList back_s = new();
        back_s = encryptor_s.ReadFile(Path.Combine(Directory.GetCurrentDirectory(), "serial encryption.txt"), back_s);


        using (StreamWriter outputFile = new StreamWriter(Path.Combine(Directory.GetCurrentDirectory(), "serial decryption.txt"))) {
            foreach (string encryptedLine in back_s) {
                string decryptedLine = encryptedLine;
                for(int i = encryptor_s.keyLength - 1; i >= 0; i--) {
                    decryptedLine = Encrypt.Cipher(decryptedLine, -(int)key[i]);
                }
                outputFile.WriteLine(decryptedLine);
            }          
        }
    }
}

class Encrypt {
    public ArrayList input = new();
    private ArrayList key = new();
    public int keyLength;

    public void ReadFile(string filePath) {
        String? line;
        try {
            StreamReader sr = new(filePath);
            line = sr.ReadLine();
            while (line != null) {
                for(int i = 0; i < line.Split(" ").Length; i++) {
                    this.input.Add(line.Split(" ")[i]);
                }
                
                line = sr.ReadLine();
            }
            sr.Close();
        }
        catch(Exception e) {
            Console.WriteLine("Exception: " + e.Message);
        }
    }

    public ArrayList ReadFile(string filePath, ArrayList input) {
        String? line;
        try {
            StreamReader sr = new(filePath);
            line = sr.ReadLine();
            while (line != null) {
                for(int i = 0; i < line.Split(" ").Length; i++) {
                    input.Add(line.Split(" ")[i]);
                }
                
                line = sr.ReadLine();
            }
            sr.Close();
        }
        catch(Exception e) {
            Console.WriteLine("Exception: " + e.Message);
        }
        return input;
    }

    public static string Cipher(string text, int shift) {
        string result = "";

        foreach (char ch in text) {
            if (char.IsLetter(ch)) {
                char offset = char.IsUpper(ch) ? 'A' : 'a';
                char shiftedChar = (char)(((ch - offset + shift) % 26 + 26) % 26 + offset);
                result += shiftedChar;
            }
            else if (char.IsDigit(ch)) {
                char shiftedChar = (char)(((ch - '0' + shift) % 10 + 10) % 10 + '0');
                result += shiftedChar;
            }
            else {
                result += ch;
            }
        }

        return result;
    }
    


    public void CreateKey(int iter) {
        var random = new Random();
        int shift = random.Next(256);

        for(int i = 0; i < iter; i++) {
            this.key.Add(random.Next(256));
        }
        this.keyLength = this.key.Count;
    }

    public ArrayList GetKey() {
        return this.key;
    }
}
