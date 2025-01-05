const readline=require('readline');
const rl=readline.createInterface({
    input: process.stdin,
    output: process.stdout
});

const hangmanFigures=[
    `
        -----
        |    |   
        |    O   
        |   /|\\  
        |   / \\  
        |       
        --------
        `,
    `
        -----
        |    |   
        |    O   
        |   /|\\  
        |   /    
        |       
        --------
        `,
    `
        -----
        |    |   
        |    O   
        |   /|\\  
        |       
        |       
        --------
        `,
    `
        -----
        |    |   
        |    O   
        |   /|   
        |       
        |       
        --------
        `,
    `
        -----
        |    |   
        |    O   
        |    |   
        |       
        |       
        --------
        `,
    `
        -----
        |    |   
        |    O   
        |       
        |       
        |       
        --------
        `,
    `
        -----
        |    |   
        |       
        |      
        |       
        |       
        --------
        `
];

function clearConsole(){
    console.clear();
}
async function getRandomWord(){
    try{
        const response=await fetch("https://api.datamuse.com/words?ml=common&max=1000");
        if(!response.ok) throw new Error('Network Error');
        const words=await response.json();
        const filteredWords=words
          .map(word=>word.word)
          .filter(word=>/^[a-zA-Z]+$/.test(word) && word.length>=4 && word.length<=7);

        return filteredWords.length>0
          ?filteredWords[Math.floor(Math.random()*filteredWords.length)]
          :"script";
    }catch (error){
        console.log("Error: Unable to fetch a random word.");
        process.exit(1);
    }
}
function pressAnyKey(){
    return new Promise(resolve=>{
        process.stdin.setRawMode(true);
        process.stdin.resume();
        process.stdin.once('data', ()=>{
            process.stdin.setRawMode(false);
            resolve();
        });
    });
}

async function game(word){
    const guessedWord=Array(word.length).fill('_');
    const guessedLetters=new Set();
    let tries=6;
    while(tries>0 && guessedWord.includes('_')){
        clearConsole();
        console.log(hangmanFigures[tries]);
        console.log(guessedWord.join(' '));
        console.log(`Guessed letters: ${[...guessedLetters].sort().join(', ')}`);

        const guess=await new Promise(resolve=>{
            rl.question('Guess a letter: ', answer=>resolve(answer.toLowerCase()));
        });

        if(guess.length!==1 || !/[a-z]/.test(guess)){
            console.log("Error: Invalid input. Please enter a single valid letter.");
            await new Promise(resolve=>{
                rl.question('Press Enter to continue...', resolve);
            });
            continue;
        }

        if(guessedLetters.has(guess)){
            console.log("You've already guessed that letter.");
            await new Promise(resolve=>{
                rl.question('Press Enter to continue...', resolve);
            });
            continue;
        }

        guessedLetters.add(guess);
        if(word.includes(guess)){
            for(let i=0; i<word.length; i++){
                if(word[i]===guess){
                    guessedWord[i]=guess;
                }
            }
        }else{
            tries--;
        }
    }

    clearConsole();
    console.log(hangmanFigures[tries]);
    console.log(guessedWord.join(' '));

    if(!guessedWord.includes('_')){
        console.log(`Congrats! You guessed the word: ${word}`);
    }else{
        console.log(`Game over! The word was: ${word}`);
    }
}

async function main() {
    console.log(" ------------------------------");
    console.log("| Welcome to our Hangman Game! |");
    console.log(" ------------------------------");
    console.log("The Hangman game challenges players to guess a hidden word, letter by letter,\n" +
      "before running out of attempts.\n" +
      "If a player suggests an incorrect letter, part of a hangman figure is drawn,\n" +
      "progressing toward a complete figure with each wrong guess.\n");

    console.log("Press ANY key to continue...");
    await pressAnyKey();

    while(true){
        console.log("\nAvailable game modes:");
        console.log("     1. Single-Player Mode");
        console.log("     2. Two-Player Mode");

        let gameMode;
        while(true){
            gameMode=await new Promise(resolve=>{
                rl.question('Please choose one of the listed game modes (1 or 2): ', answer=>resolve(answer));
            });

            if (gameMode==='1' || gameMode==='2') {
                break;
            }
            console.log("Invalid input. Please choose Single-Player Mode (1) or Two-Player Mode (2).");
        }

        let word;
        if(gameMode==='1'){
            word=await getRandomWord();
        }else{
            console.log("Which word will other player guess?");
            word=await new Promise(resolve=>{
                rl.question('> ', answer=>resolve(answer.toLowerCase()));
            });
        }
        await game(word);
        while(true){
            const playAgain=await new Promise(resolve => {
                rl.question('\nWould you like to play one more game (yes/no): ', answer=>resolve(answer.toLowerCase()));
            });

            if(playAgain==='yes'){
                break;
            }else if(playAgain==='no'){
                console.log("Sure! See you next time.");
                rl.close();
                return;
            }else{
                console.log("Invalid input. Please enter a valid answer.");
            }
        }
    }
}
main().catch(console.error);
