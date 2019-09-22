using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;

public class garfieldKartScript : MonoBehaviour {

    public KMBombInfo Bomb;
    public KMAudio Audio;

    public KMSelectable[] Buttons;
    public GameObject Screen;
    public Material[] Mats;
    public GameObject[] PuzzlePieces; //Make 3 squares
    public Material[] PuzzleMats; //Make this just 1 blue (there) and 0 red (not there)

    //             01234567
    string Venn = "JGLOSNHA";
    int characterNum = 0;
    public List<string> characterNames = new List<string> { "Jon", "Garfield", "Liz", "Odie", "Squeak", "Nermel", "Harry", "Arlene" };
    public List<float> stats = new List<float> {
        0.25f,     2,  0.5f, //Jon
            1,     1,     1, //Garfield
        0.25f, 1.75f, 0.75f, //Liz
         0.5f,  1.5f,  1.5f, //Odie
        1.25f,  0.5f,  1.5f, //Squeak
        1.15f, 0.33f, 0.67f, //Nermel
        0.75f, 1.25f, 1.25f, //Harry
            1, 0.75f, 1.25f  //Arlene
    };
    float spd = 0;
    float acc = 0;
    float han = 0;
    int trackNum = 0;                                  
    public List<string> trackNames = new List<string> {
        "Play Misty for Me", "Sneak-A-Peak", "Blazing Oasis", "Pastacosi Factory", //Hamburger Cup
        "Mysterious Temple", "Prohibited Site", "Caskou Park", "Loopy Lagoon", //Ice cream Cup
        "Catz in the Hood", "Crazy Dunes", "Palerock Lake", "City Slicker", //Lasagna Cup
        "Country Bumpkin", "Spooky Manor", "Mally Market", "Valley of the Kings"  //Pizza Cup
    };
    int screenX = 0;
    int screenY = 0;
    int numInCup = 0;
    int cupNum = 0;
    int puzzleNum = 0;
    public List<string> ordinals = new List<string> { "1st", "2nd", "3rd", "4th", "5th", "6th"};
    public List<string> cupNames = new List<string> { "Hamburger", "Ice Cream", "Lasagna", "Pizza" };
    int calc = 0;
    int fact = 0;
    int dig = 0;
    public List<int> dumbNum = new List<int> { 3, 4, 2, 1 };
    string lastInNames = "NDZEKLYE";
    public List<string> trackLetters = new List<string> {
        "PLAYMISTYFORME", "SNEAKAPEAK", "BLAZINGOASIS", "PASTACOSIFACTORY", 
        "MYSTERIOUSTEMPLE", "PROHIBITEDSITE", "CASKOUPARK", "LOOPYLAGOON", 
        "CATZINTHEHOOD", "CRAZYDUNES", "PALEROCKLAKE", "CITYSLICKER", 
        "COUNTRYBUMPKIN", "SPOOKYMANOR", "MALLYMARKET", "VALLEYOFTHEKINGS"  
    };
    public List<int> factNum = new List<int> { 1, 1, 2, 6, 24, 120, 720, 5040, 40320, 362880, 3628800, 39916800, 479001600 };
    string logString = "";

    //Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    void Awake () {
        moduleId = moduleIdCounter++;
        foreach (KMSelectable button in Buttons) {
            KMSelectable pressedButton = button;
            button.OnInteract += delegate () { buttonPress(pressedButton); return false; };
        }
    }

    // Use this for initialization
    void Start () {
		if (Bomb.GetSerialNumber().Any(ch => "GARFIELD".Contains(ch))) { characterNum += 1; }
        if (Bomb.GetSerialNumber().Any(ch => "KART".Contains(ch))) { characterNum += 2; }
        if (Bomb.GetOnIndicators().Any(x => new[] { "G", "A", "R", "F", "I", "E", "L", "D", "K", "T" }.Any(y => x.Contains(y)))) { characterNum += 4; }

        spd = stats[characterNum * 3];
        acc = stats[(characterNum * 3) + 1];
        han = stats[(characterNum * 3) + 2];

        Debug.LogFormat("[Garfield Kart #{0}] Your character is: {1}", moduleId, characterNames[characterNum]);
        Debug.LogFormat("[Garfield Kart #{0}] Stats: {1} speed, {2} acceleration, {3} handling", moduleId, spd, acc, han);

        trackNum = UnityEngine.Random.Range(0, 16);
        Screen.GetComponent<MeshRenderer>().material = Mats[trackNum];
        Debug.LogFormat("[Garfield Kart #{0}] The track is: {1}", moduleId, trackNames[trackNum]);

        screenX = UnityEngine.Random.Range(5, 52);
        if (0.34f < screenX)
        {
            screenY = UnityEngine.Random.Range(25, 34);
        } else
        {
            screenY = UnityEngine.Random.Range(25, 46);
        }
        Screen.GetComponent<MeshRenderer>().material.mainTextureOffset = new Vector2(screenX * 0.01f, screenY * 0.01f);

        numInCup = (trackNum % 4);
        cupNum = (trackNum - numInCup) / 4;
        Debug.LogFormat("[Garfield Kart #{0}] This track is the {1} in the {2} Cup.", moduleId, ordinals[numInCup], cupNames[cupNum]);

        puzzleNum = UnityEngine.Random.Range(0, 8);
        if (puzzleNum > 3)
        {
            PuzzlePieces[0].GetComponent<MeshRenderer>().material = PuzzleMats[1];
            logString += " 1";
        } else
        {
            PuzzlePieces[0].GetComponent<MeshRenderer>().material = PuzzleMats[0];
        }

        if (puzzleNum % 4 > 1)
        {
            PuzzlePieces[1].GetComponent<MeshRenderer>().material = PuzzleMats[1];
            logString += " 2";
        }
        else
        {
            PuzzlePieces[1].GetComponent<MeshRenderer>().material = PuzzleMats[0];
        }

        if (puzzleNum % 2 == 1)
        {
            PuzzlePieces[2].GetComponent<MeshRenderer>().material = PuzzleMats[1];
            logString += " 3";
        }
        else
        {
            PuzzlePieces[2].GetComponent<MeshRenderer>().material = PuzzleMats[0];
        }
        Debug.LogFormat("[Garfield Kart #{0}] Puzzle pieces present:{1}", moduleId, logString);

        Debug.LogFormat("[Garfield Kart #{0}] ==CALCULATION BEGINS HERE==", moduleId);

        if (characterNum == 1 && cupNum == 2)
        {
            calc = (numInCup + 1) * 522;
            Debug.LogFormat("[Garfield Kart #{0}] {1} * 522 = {2} (Garfield & Lasagna Cup multiplier)", moduleId, numInCup + 1, calc);
        } else
        {
            calc = (numInCup + 1) * dumbNum[cupNum];
            Debug.LogFormat("[Garfield Kart #{0}] {1} * {2} = {3} (Cup the track is on times number of cup)", moduleId, numInCup + 1, dumbNum[cupNum], calc);
        }

        if (puzzleNum == 4)
        {
            calc += 1;
            Debug.LogFormat("[Garfield Kart #{0}] {1} + 1 = {2} (One puzzle piece in 1st position)", moduleId, calc - 1, calc);
        } else if (puzzleNum == 2)
        {
            calc += 2;
            Debug.LogFormat("[Garfield Kart #{0}] {1} + 2 = {2} (One puzzle piece in 2nd position)", moduleId, calc - 2, calc);
        } else if (puzzleNum == 1)
        {
            calc += 3;
            Debug.LogFormat("[Garfield Kart #{0}] {1} + 3 = {2} (One puzzle piece in 3rd position)", moduleId, calc - 3, calc);
        } else if (puzzleNum == 7 || puzzleNum == 0)
        {
            fact = calc % 13;
            calc = factNum[fact];
            Debug.LogFormat("[Garfield Kart #{0}] ({1} % 13)! = {2} (All or no puzzle pieces, mod 13 then factorial G)", moduleId, fact, calc);
        } else
        {
            if (trackNum == 11)
            {
                dig = MultiplicativeDigitalRoot(calc);
                Debug.LogFormat("[Garfield Kart #{0}] MDR({1}) = {2} (Two puzzle pieces & City Slicker, Multiplicitive Digital Root)", moduleId, calc, dig);
            }
            else
            {
                dig = (((calc - 1) % 9) + 1);
                Debug.LogFormat("[Garfield Kart #{0}] DR({1}) = {2} (Two puzzle pieces without City Slicker, Additive Digital Root)", moduleId, calc, dig);
            }
            calc = dig;
        }

        if (trackLetters[trackNum].Contains(lastInNames[characterNum]))
        {
            calc = calc * 23;
            Debug.LogFormat("[Garfield Kart #{0}] {1} * 23 = {2} (Last letter of character is in the track name)", moduleId, calc/23, calc);
        }

        if (calc < 100)
        {
            calc += 200;
            Debug.LogFormat("[Garfield Kart #{0}] {1} + 200 = {2} (G is less than 100)", moduleId, calc - 200, calc);
        }

        if (calc > 10000)
        {
            dig = calc;
            calc %= 10000;
            Debug.LogFormat("[Garfield Kart #{0}] {1} % 10000 = {2} (G is more than 10000)", moduleId, dig, calc);
        }

        dig = calc;
        calc = (int) Math.Floor(calc * spd);
        Debug.LogFormat("[Garfield Kart #{0}] {1} * {2} = {3} (Multiply by speed, remove decimals)", moduleId, dig, spd, calc);

        calc += characterNames[characterNum].Length;
        Debug.LogFormat("[Garfield Kart #{0}] {1} + {2} = {3} (Add number of letters in character's name)", moduleId, calc - characterNames[characterNum].Length, characterNames[characterNum].Length, calc);

        dig = calc;
        calc = (int) Math.Ceiling(calc * acc);
        Debug.LogFormat("[Garfield Kart #{0}] {1} * {2} = {3} (Multiply by acceleration, rounding up)", moduleId, dig, acc, calc);

        if (trackLetters[trackNum].Length % 2 == 0)
        {
            calc *= 5;
            Debug.LogFormat("[Garfield Kart #{0}] {1} * 5 = {2} (Even amount of letters in track name, multiply by 5)", moduleId, calc / 5, calc);
        } else
        {
            calc *= 7;
            Debug.LogFormat("[Garfield Kart #{0}] {1} * 6 = {2} (Odd amount of letters in track name, multiply by 7)", moduleId, calc / 6, calc);
        }

        dig = calc;
        calc = (int) Math.Floor(calc * han);
        Debug.LogFormat("[Garfield Kart #{0}] {1} * {2} = {3} (Multiply by handling, remove decimals)", moduleId, dig, han, calc);

        dig = calc;
        calc %= 6;
        Debug.LogFormat("[Garfield Kart #{0}] ({1} % 6) + 1 = {2} (Modulo 6, then add 1)", moduleId, dig, calc + 1);

        Debug.LogFormat("[Garfield Kart #{0}] FINAL PLACE: {1}", moduleId, ordinals[calc]);
    }
	
	void buttonPress(KMSelectable pressedButton)
    {
        pressedButton.AddInteractionPunch();
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        if (pressedButton == Buttons[calc])
        {
            GetComponent<KMBombModule>().HandlePass();
            moduleSolved = true;
            Debug.LogFormat("[Garfield Kart #{0}] Correct place selected, module solved.", moduleId);
        } else
        {
            GetComponent<KMBombModule>().HandleStrike();
            Debug.LogFormat("[Garfield Kart #{0}] Incorrect place selected, module striked. Get a job.", moduleId);
        }
    }

    int MultiplicativeDigitalRoot(int number)
    {
        while (number >= 10)
        {
            number = number.ToString().Select(digit => digit - '0').Aggregate((a, b) => a * b);
        }
        return number;
    }
}
