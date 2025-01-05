using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Linq;

class Program{
    static async Task<string> GetRandomWord(){
        try{
            string response=await client.GetStringAsync("https://api.datamuse.com/words?ml=common&max=1000");
            var words=JsonSerializer.Deserialize<List<WordInfo>>(response);
            var filteredWords=words
                .Select(w=>w.word)
                .Where(w=>w.All(char.IsLetter) && w.Length>=4 && w.Length<=7)
                .ToList();
            
            return filteredWords.Count>0 
                ?filteredWords[new Random().Next(filteredWords.Count)] 
                :"csharp";
        }
        catch(Exception){
            Console.WriteLine("Error: Unable to fetch a random word.");
            Environment.Exit(1);
            return null;
        }
    }
    
    static string HangmanFigures(int tries){
        string[] figures={
            @"
        -----
        |    |   
        |    O   
        |   /|\  
        |   / \  
        |       
        --------
        ",
            @"
        -----
        |    |   
        |    O   
        |   /|\  
        |   /    
        |       
        --------
        ",
            @"
        -----
        |    |   
        |    O   
        |   /|\  
        |       
        |       
        --------
        ",
            @"
        -----
        |    |   
        |    O   
        |   /|   
        |       
        |       
        --------
        ",
            @"
        -----
        |    |   
        |    O   
        |    |   
        |       
        |       
        --------
        ",
            @"
        -----
        |    |   
        |    O   
        |       
        |       
        |       
        --------
        ",
            @"
        -----
        |    |   
        |       
        |      
        |       
        |       
        --------
        "
        };
        return figures[tries];
    }

    static void ClearConsole(){
        Console.Clear();
    }
    
    static async Task Game(string word){
        char[] guessedWord=new char[word.Length];
        Array.Fill(guessedWord, '_');
        HashSet<char>guessedLetters=new HashSet<char>();
        int tries=6;
        
        while(tries>0 && guessedWord.Contains('_')){
            ClearConsole();
            Console.WriteLine(HangmanFigures(tries));
            Console.WriteLine(string.Join(" ", guessedWord));
            Console.WriteLine($"Guessed letters: {string.Join(", ", guessedLetters.OrderBy(x=>x))}");
            
            Console.Write("Guess a letter: ");
            string input=Console.ReadLine().ToLower();
            
            if(input.Length!=1 || !char.IsLetter(input[0])){
                Console.WriteLine("Error: Invalid input. Please enter a single valid letter.");
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
                continue;
            }
            char guess=input[0];
            if(guessedLetters.Contains(guess)){
                Console.WriteLine("You've already guessed that letter.");
                Console.WriteLine("Press Enter to continue...");
                Console.ReadLine();
                continue;
            }
            
            guessedLetters.Add(guess);
            if(word.Contains(guess)){
                for(int i=0; i<word.Length; i++){
                    if (word[i]==guess)
                        guessedWord[i]=guess;
                }
            }else{
                tries--;
            }
        }
        
        ClearConsole();
        Console.WriteLine(HangmanFigures(tries));
        Console.WriteLine(string.Join(" ", guessedWord));
        
        if(!guessedWord.Contains('_')){
            Console.WriteLine($"Congrats! You guessed the word: {word}");
        }else{
            Console.WriteLine($"Game over! The word was: {word}");
        }
    }
    
    private class WordInfo{
        public string word{get;set;}
    }

    private static readonly HttpClient client=new HttpClient();
    static async Task Main(string[] args){
        Console.WriteLine(" ------------------------------");
        Console.WriteLine("| Welcome to our Hangman Game! |");
        Console.WriteLine(" ------------------------------");
        Console.WriteLine("The Hangman game challenges players to guess a hidden word, letter by letter,\n" +
                        "before running out of attempts.\n" +
                        "If a player suggests an incorrect letter, part of a hangman figure is drawn,\n" +
                        "progressing toward a complete figure with each wrong guess.\n");
        
        Console.WriteLine("Press ANY key to continue...");
        Console.ReadKey(true);
        
        while(true){
            Console.WriteLine("\nAvailable game modes:");
            Console.WriteLine("     1. Single-Player Mode");
            Console.WriteLine("     2. Two-Player Mode");
            
            int gameMode;
            while(true){
                Console.Write("Please choose one of the listed game modes (1 or 2): ");
                if (int.TryParse(Console.ReadLine(), out gameMode) && (gameMode==1 || gameMode==2))
                    break;
                Console.WriteLine("Invalid input. Please choose Single-Player Mode (1) or Two-Player Mode (2).");
            }
            string word;
            if(gameMode==1){
                word=await GetRandomWord();
            }
            else{
                Console.WriteLine("Which word will other player guess?");
                Console.Write("> ");
                word=Console.ReadLine().ToLower();
            }
            await Game(word);
            while(true){
                Console.Write("\nWould you like to play one more game (yes/no): ");
                string userChoice = Console.ReadLine().ToLower();
                if (userChoice == "yes")
                    break;
                else if (userChoice == "no")
                {
                    Console.WriteLine("Sure! See you next time.");
                    return;
                }
                else
                    Console.WriteLine("Invalid input. Please enter a valid answer.");
            }
        }
    }
}
