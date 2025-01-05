import sys
import random
import requests
import os
from time import sleep

def pressAnyKey():
    print("Press ANY key to continue...")
    if os.name=='nt':
        import msvcrt
        msvcrt.getch()
    else:
        import tty
        import termios 
        fd=sys.stdin.fileno()
        old_settings=termios.tcgetattr(fd)
        try:
            tty.setraw(fd)
            sys.stdin.read(1)
        finally:
            termios.tcsetattr(fd, termios.TCSADRAIN, old_settings)

def getRandomWord():
    try:
        response=requests.get("https://api.datamuse.com/words?ml=common&max=1000")
        response.raise_for_status()
        words=[word['word'] for word in response.json() if word['word'].isalpha()]
        filteredWords = [word for word in words if 4<= len(word) <=7]
        return random.choice(filteredWords) if filteredWords else "python"
    except requests.RequestException:
        print("Error: Unable to fetch a random word.")
        exit()

def hangmanFigures(tries):
    figures=[
        """
        -----
        |    |   
        |    O   
        |   /|\\  
        |   / \\  
        |       
        --------
        """,
        """
        -----
        |    |   
        |    O   
        |   /|\\  
        |   /    
        |       
        --------
        """,
        """
        -----
        |    |   
        |    O   
        |   /|\\  
        |       
        |       
        --------
        """,
        """
        -----
        |    |   
        |    O   
        |   /|   
        |       
        |       
        --------
        """,
        """
        -----
        |    |   
        |    O   
        |    |   
        |       
        |       
        --------
        """,
        """
        -----
        |    |   
        |    O   
        |       
        |       
        |       
        --------
        """,
        """
        -----
        |    |   
        |       
        |      
        |       
        |       
        --------
        """
    ]
    return figures[tries]

def clearConsole():
    os.system('cls' if os.name == 'nt' else 'clear')

def game():
    guessedWord=["_"]*len(word)
    guessedLetters=set()
    tries=6

    while tries>0 and "_" in guessedWord:
        clearConsole()
        print(hangmanFigures(tries))
        print(" ".join(guessedWord))
        print(f"Guessed letters: {', '.join(sorted(guessedLetters))}")

        guess=input("Guess a letter: ").lower()
        if len(guess)!=1 or not guess.isalpha():
            print("Error: Invalid input. Please enter a single valid letter.")
            input("Press Enter to continue...")
            continue

        if guess in guessedLetters:
            print("You've already guessed that letter.")
            input("Press Enter to continue...")
            continue
        guessedLetters.add(guess)
        if guess in word:
            for i, letter in enumerate(word):
                if letter==guess:
                    guessedWord[i]=guess
        else:
            tries-=1

    clearConsole()
    if "_" not in guessedWord:
        print(hangmanFigures(tries))
        print(" ".join(guessedWord))
        print("Congrats! You guessed the word:", word)
    else:
        print(hangmanFigures(tries))
        print("Game over! The word was:", word)

print(" ------------------------------")
print("| Welcome to our Hangman Game! |")
print(" ------------------------------")
print("The Hangman game challenges players to guess a hidden word, letter by letter,\n"
      "before running out of attempts.\n"
      "If a player suggests an incorrect letter, part of a hangman figure is drawn,\n"
      "progressing toward a complete figure with each wrong guess.\n")
pressAnyKey()
print("\nAvailable game modes:")
print("     1. Single-Player Mode")
print("     2. Two-Player Mode")
while(True):
    while(True):
        try:
            gameMode=int(input("Please choose one of the listed game modes (1 or 2): "))
            if(gameMode==1 or gameMode==2):
                break
            else:
                print("Invalid input. Please choose Single-Player Mode (1) or Two-Player Mode (2).")
        except Exception:
            print("Invalid input. Please choose Single-Player Mode (1) or Two-Player Mode (2).")
    
    if gameMode==1:
        word=getRandomWord()
        if __name__=="__main__":
            game()

    else:
        print("Which word will other player guess?")
        word=input("> ")
        if __name__=="__main__":
            game()

    while(True):
        userChoice=input("\nWould you like to play one more game (yes/no): ")
        if userChoice.lower()=="yes":
            break
        elif userChoice.lower()=="no":
            print("Sure! See you next time.")
            exit()
        elif userChoice!="yes" and userChoice!="no":
            print("Invalid input. Please enter a valid answer.")

