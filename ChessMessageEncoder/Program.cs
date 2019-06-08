using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Numerics;

namespace ChessMessageEncoder
{
    class Program
    {

        public static string Encode(string message)
        {
            var board = new ChessBoard();
            string Output = "";
            string Input = message;
            byte[] bytes = Encoding.UTF8.GetBytes(Input);
            for (int i = 0; i < bytes.Length; i++)
            {
                Console.Write(bytes[i] + " ");
            }
            Array.Reverse(bytes);
            BigInteger correspondingNum = (BigInteger)Math.Pow(256, bytes.Length);
            for (int i = 1; i <= bytes.Length; i++)
            {
                correspondingNum += bytes[i - 1] * (int)Math.Pow(256, bytes.Length - i);
            }
            Console.WriteLine("");
            Console.WriteLine(correspondingNum);
            Console.WriteLine("");
            int round = 0;
            while (correspondingNum != 0)
            {
                if (board.isWhitesTurn)
                {
                    round++;
                    Output += round.ToString() + ". ";
                }
                var moves = board.GetAllPossibleMoves();
                string tempExecutedMove = moves[(int)(correspondingNum % moves.Count)].Substring(moves[(int)(correspondingNum % moves.Count)].IndexOf("~") + 1);
                Output += tempExecutedMove + " ";
                board.ExecuteMoves(moves[(int)(correspondingNum % moves.Count)].Substring(0, moves[(int)(correspondingNum % moves.Count)].IndexOf("~")), tempExecutedMove);
                correspondingNum /= moves.Count;
            }
            if (board.isWhitesTurn)
            {
                Output += "{ White resigns. } 0-1";
            }
            else
            {
                Output += "{ Black resigns. } 1-0";
            }
            return Output;
        }

        public static string Decode(string encodedGame)
        {
            /* take leftover number (0), multiply by what you divided by
                     * add the modded number, multiply by by what you diveded by to get that number
                     * repeat until you have no more numbers to multiply by
                     * check this with the JS example online, you'll get it then
                    */
            var board = new ChessBoard();
            encodedGame = (encodedGame.Remove(encodedGame.Length - 22)).Trim();
            List<int> indeces = new List<int>();
            List<int> amountsOfPossibleMoves = new List<int>();
            while (encodedGame.Length != 0)
            {
                if (board.isWhitesTurn && encodedGame.Length > 3)
                {
                    encodedGame = encodedGame.Remove(0, 3).Trim();
                }
                var moves = board.GetAllPossibleMoves();
                string correspondingStringToMoves = "";
                for (int i = 0; i < moves.Count; i++)
                {
                    correspondingStringToMoves += (moves[i] + "|");
                }
                amountsOfPossibleMoves.Add(moves.Count);

                int amountOfCharsToRemove = 3;
                if (encodedGame[0] == 'K' || encodedGame[0] == 'Q' || encodedGame[0] == 'N' || encodedGame[0] == 'B' || encodedGame[0] == 'R')
                {
                    amountOfCharsToRemove++;
                }
                if (encodedGame[1] == 'x')
                {
                    amountOfCharsToRemove++;
                }
                correspondingStringToMoves = correspondingStringToMoves.Remove(correspondingStringToMoves.IndexOf(encodedGame.Substring(0, amountOfCharsToRemove - 1)) + amountOfCharsToRemove - 1);
                correspondingStringToMoves = correspondingStringToMoves.Substring(correspondingStringToMoves.LastIndexOf('|') + 1);
                indeces.Add(moves.IndexOf(correspondingStringToMoves));
                if (encodedGame.Length < amountOfCharsToRemove)
                {
                    encodedGame = "";
                }
                else
                {
                    encodedGame = encodedGame.Remove(0, amountOfCharsToRemove).Trim();                    
                }
                board.ExecuteMoves(correspondingStringToMoves.Substring(0, correspondingStringToMoves.IndexOf('~')), correspondingStringToMoves.Substring(correspondingStringToMoves.IndexOf('~') + 1));

            }
            BigInteger correspondingNum = 0;
            for (int i = indeces.Count - 1; i > 0; i--)
            {
                correspondingNum += indeces[i];
                correspondingNum *= amountsOfPossibleMoves[i - 1];
            }
            correspondingNum += indeces[0];
            Console.WriteLine(correspondingNum);
            bool foundPower = false;
            int power = 0;
            while (!foundPower)
            {
                if (correspondingNum - (BigInteger)Math.Pow(256, power) > 0)
                {
                    power++;
                }
                else
                {
                    power--;
                    foundPower = true;
                }
            }
            byte[] bytes = new byte[power];
            correspondingNum -= (BigInteger)Math.Pow(256, power);
            for (int i = power - 1; i > -1; i--)
            {
                int temp = (int)(correspondingNum / (BigInteger)Math.Pow(256, i));
                correspondingNum -= (BigInteger)(Math.Pow(256, i) * temp);
                bytes[i] = (byte)temp;
                Console.Write(bytes[i] + " ");
            }
            return Encoding.UTF8.GetString(bytes);
        }

        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("1. Encode");
                Console.WriteLine("2. Decode");
                string action = Console.ReadLine();
                action = action.Trim();
                if (action == "1" || action == "encode" || action == "Encode" || action == "e" || action == "E")
                {
                    Console.WriteLine(Encode(Console.ReadLine()));
                    Console.ReadLine();
                    Console.Clear();
                }
                else if(action == "2" || action == "Decode" || action == "decode" || action == "d" || action == "D")
                {
                    var a = Decode(Console.ReadLine());
                    Console.WriteLine();
                    Console.WriteLine(a);
                    Console.ReadLine();
                    Console.Clear();
                }
            }
        }

    }
    class ChessBoard
    {
        public Dictionary<string, ChessPiece> chessPieces;
        public bool isWhitesTurn;
        readonly List<char> abMoves = "abcdefgh".ToCharArray().ToList();

        public ChessBoard()
        {
            isWhitesTurn = true;
            chessPieces = new Dictionary<string, ChessPiece>();
            string pawnNumPos = "2";
            string otherNumPos = "1";
            bool isWhite = true;            
            for (int side = 0; side < 2; side++)
            {
                for(int pawns = 0;pawns < abMoves.Count;pawns++)
                {
                    chessPieces.Add(abMoves[pawns] + pawnNumPos, new ChessPiece(' ', abMoves[pawns] + pawnNumPos, isWhite));
                }
                chessPieces.Add("a" + otherNumPos, new ChessPiece('R', "a" + otherNumPos, isWhite));
                chessPieces.Add("b" + otherNumPos, new ChessPiece('N', "b" + otherNumPos, isWhite));
                chessPieces.Add("c" + otherNumPos, new ChessPiece('B', "c" + otherNumPos, isWhite));
                chessPieces.Add("d" + otherNumPos, new ChessPiece('Q', "d" + otherNumPos, isWhite));
                chessPieces.Add("e" + otherNumPos, new ChessPiece('K', "e" + otherNumPos, isWhite));
                chessPieces.Add("f" + otherNumPos, new ChessPiece('B', "f" + otherNumPos, isWhite));
                chessPieces.Add("g" + otherNumPos, new ChessPiece('N', "g" + otherNumPos, isWhite));
                chessPieces.Add("h" + otherNumPos, new ChessPiece('R', "h" + otherNumPos, isWhite));
                pawnNumPos = "7";
                otherNumPos = "8";
                isWhite = false;
            }
        }

        public List<string> GetAllPossibleMoves()
        {
            var possibleMoves = new List<string>();
            var previousPieceNotation = new List<string>();
            foreach(ChessPiece piece in chessPieces.Values)
            {
                if(piece.IsWhite == isWhitesTurn)
                {
                    possibleMoves.AddRange(piece.GenerateMoves(chessPieces));
                }
            }
            return possibleMoves;
        }

        public void ExecuteMoves(string PreviousNotation, string ExecutedMove)
        {
            string ExecutedPos = ExecutedMove.Substring(ExecutedMove.Length - 2);
            string PreviousPos = PreviousNotation.Substring(PreviousNotation.Length - 2);
            if (chessPieces.ContainsKey(ExecutedPos))
            {
                chessPieces.Remove(ExecutedPos);
            }
            chessPieces.Add(ExecutedPos, new ChessPiece(chessPieces[PreviousPos].PieceType, ExecutedPos, chessPieces[PreviousPos].IsWhite));
            chessPieces.Remove(PreviousPos);
            isWhitesTurn = !isWhitesTurn;
        }
    }
    class ChessPiece
    {
        public char PieceType;
        public string CurrentPos;
        public string CurrentNotation => (PieceType.ToString() + CurrentPos).TrimStart(' ');
        public bool IsWhite;
        readonly List<char> abMoves = "abcdefgh".ToCharArray().ToList();

        public ChessPiece(char pieceType, string currentPos, bool isWhite)
        {
            PieceType = pieceType;
            CurrentPos = currentPos;
            IsWhite = isWhite;
        }

        public List<string> GenerateMoves(Dictionary<string, ChessPiece> chessPieces)
        {
            List<string> possibleMoves = new List<string>();
            string addition = "";
            int abMoveIndex = 0;
            switch (PieceType)
            {
                case ' ':
                    if (IsWhite && int.Parse(CurrentPos[1].ToString()) + 1 < 8)
                    {
                        addition = "" + CurrentPos[0] + (int.Parse(CurrentPos[1].ToString()) + 1).ToString();
                    }
                    else if (!IsWhite && int.Parse(CurrentPos[1].ToString()) - 1 > 1)
                    {
                        addition = "" + CurrentPos[0] + (int.Parse(CurrentPos[1].ToString()) - 1).ToString();
                    }
                    if (!chessPieces.ContainsKey(addition))
                    {
                        possibleMoves.Add(CurrentNotation + "~" + addition);
                    }
                    if (CurrentPos[1] == '7' || CurrentPos[1] == '2')
                    {
                        string inMyWay =  "";
                        if (IsWhite)
                        {
                            addition = "" + CurrentPos[0] + "4";
                            inMyWay = "" + CurrentPos[0] + "3";
                        }
                        else
                        {
                            addition = "" + CurrentPos[0] + "5";
                            inMyWay = "" + CurrentPos[0] + "6";
                        }
                        if (!chessPieces.ContainsKey(addition) && !chessPieces.ContainsKey(inMyWay)) 
                        { 
                            possibleMoves.Add(CurrentNotation + "~" + addition);
                        }
                    }
                    abMoveIndex = abMoves.IndexOf(CurrentPos[0]) + 1;
                    for (int i = 0; i < 2; i++)
                    {
                        if (abMoves.IndexOf(CurrentPos[0]) + 1 < abMoves.Count && abMoves.IndexOf(CurrentPos[0]) - 1 > -1)
                        {
                            if (IsWhite)
                            {
                                addition = CurrentPos[0] + "x" + abMoves[abMoveIndex] + (int.Parse(CurrentPos[1].ToString()) + 1).ToString();
                            }
                            else
                            {
                                addition = CurrentPos[0] + "x" + abMoves[abMoveIndex] + (int.Parse(CurrentPos[1].ToString()) - 1).ToString();
                            }
                            if (chessPieces.ContainsKey(addition) && chessPieces[addition].IsWhite != chessPieces[addition].IsWhite && chessPieces[addition].PieceType != 'K')
                            {
                                possibleMoves.Add(CurrentNotation + "~" + addition);
                            }
                        }
                        abMoveIndex -= 2;
                    }
                    break;
                case 'N':
                    for (int i = 0; i < 8; i++)
                    {
                        addition = "";
                        switch (i)
                        {
                            case 0:
                                abMoveIndex = abMoves.IndexOf(CurrentPos[0]) + 1;
                                if (abMoves.IndexOf(CurrentPos[0]) + 1 < abMoves.Count)
                                {
                                    addition = "" + PieceType + abMoves[abMoveIndex] + (int.Parse(CurrentPos[1].ToString()) + 2).ToString();
                                }
                                break;
                            case 1:
                                abMoveIndex = abMoves.IndexOf(CurrentPos[0]) - 1;
                                if (abMoves.IndexOf(CurrentPos[0]) - 1 > -1)
                                {
                                    addition = "" + PieceType + abMoves[abMoveIndex] + (int.Parse(CurrentPos[1].ToString()) + 2).ToString();
                                }
                                break;
                            case 2:
                                abMoveIndex = abMoves.IndexOf(CurrentPos[0]) + 1;
                                if (abMoves.IndexOf(CurrentPos[0]) + 1 < abMoves.Count)
                                {
                                    addition = "" + PieceType + abMoves[abMoveIndex] + (int.Parse(CurrentPos[1].ToString()) - 2).ToString();
                                }
                                break;
                            case 3:
                                abMoveIndex = abMoves.IndexOf(CurrentPos[0]) - 1;
                                if (abMoves.IndexOf(CurrentPos[0]) - 1 > -1)
                                {
                                    addition = "" + PieceType + abMoves[abMoveIndex] + (int.Parse(CurrentPos[1].ToString()) - 2).ToString();
                                }
                                break;
                            case 4:
                                abMoveIndex = abMoves.IndexOf(CurrentPos[0]) + 2;
                                if (abMoves.IndexOf(CurrentPos[0]) + 2 < abMoves.Count)
                                {
                                    addition = "" + PieceType + abMoves[abMoveIndex] + (int.Parse(CurrentPos[1].ToString()) + 1).ToString();
                                }
                                break;
                            case 5:
                                abMoveIndex = abMoves.IndexOf(CurrentPos[0]) - 2;
                                if (abMoves.IndexOf(CurrentPos[0]) - 2 > -1)
                                {
                                    addition = "" + PieceType + abMoves[abMoveIndex] + (int.Parse(CurrentPos[1].ToString()) + 1).ToString();
                                }
                                break;
                            case 6:
                                abMoveIndex = abMoves.IndexOf(CurrentPos[0]) + 2;
                                if (abMoves.IndexOf(CurrentPos[0]) + 2 < abMoves.Count)
                                {
                                    addition = "" + PieceType + abMoves[abMoveIndex] + (int.Parse(CurrentPos[1].ToString()) - 1).ToString();
                                }
                                break;
                            case 7:
                                abMoveIndex = abMoves.IndexOf(CurrentPos[0]) - 2;
                                if (abMoves.IndexOf(CurrentPos[0]) - 2 > -1)
                                {
                                    addition = "" + PieceType + abMoves[abMoveIndex] + (int.Parse(CurrentPos[1].ToString()) - 1).ToString();
                                }
                                break;
                        }
                        if (addition != "" && int.Parse(addition.Substring(2)) < 9 && int.Parse(addition.Substring(2)) > 0)
                        {
                            if (!chessPieces.ContainsKey(addition.Substring(1)))
                            {
                                possibleMoves.Add(CurrentNotation + "~" + addition);
                            }
                            if (chessPieces.ContainsKey(addition.Substring(1)) && chessPieces[addition.Substring(1)].IsWhite != IsWhite && chessPieces[addition.Substring(1)].PieceType != 'K')
                            {
                                possibleMoves.Add(CurrentNotation + "~" + addition.Insert(1,"x"));
                            }
                        }                        
                    }
                    break;
                case 'B':
                    BishopMoveGeneration(possibleMoves,chessPieces);
                    break;
                case 'R':
                    RookMoveGeneration(possibleMoves, chessPieces);
                    break;
                case 'Q':
                    BishopMoveGeneration(possibleMoves, chessPieces);
                    RookMoveGeneration(possibleMoves, chessPieces);
                    break;
                case 'K':
                    int abMoveChange = 1;
                    int numMoveChange = 1;
                    for (int i = 0; i < 8; i++)
                    {
                        switch(i)
                        {
                            case 1:
                                abMoveChange = 0;
                                break;
                            case 2:
                                abMoveChange = -1;
                                break;
                            case 3:
                                numMoveChange = 0;
                                break;
                            case 4:
                                abMoveChange = 1;
                                break;
                            case 5:
                                numMoveChange = -1;
                                break;
                            case 6:
                                abMoveChange = 0;
                                break;
                            case 7:
                                abMoveChange = -1;
                                break;
                        }
                        if(abMoves.IndexOf(CurrentPos[0]) + abMoveChange < abMoves.Count && int.Parse(CurrentPos[1].ToString()) + numMoveChange < 9 && int.Parse(CurrentPos[1].ToString()) + numMoveChange > 0)
                        {
                            addition = "" + abMoves[abMoves.IndexOf(CurrentPos[0]) + abMoveChange] + (int.Parse(CurrentPos[1].ToString()) + numMoveChange).ToString();
                            if(!chessPieces.ContainsKey(addition))
                            {
                                possibleMoves.Add(CurrentNotation + "~" + PieceType + addition);
                            }
                            if (chessPieces.ContainsKey(addition) && IsWhite != chessPieces[addition].IsWhite && chessPieces[addition].PieceType != 'K')
                            {
                                possibleMoves.Add(CurrentNotation + "~" + PieceType + addition.Insert(1,"x"));
                            }                            
                        }
                    }
                    break;
            }            
            return possibleMoves;
        }

        private void RookMoveGeneration(List<string> possibleMoves, Dictionary<string, ChessPiece> chessPieces)
        {
            int moveChange = 1;
            string currentPos = CurrentPos;
            for (int i = 0; i < 2; i++)
            {
                while (true)
                {
                    if (abMoves.IndexOf(currentPos[0]) + moveChange < abMoves.Count && abMoves.IndexOf(currentPos[0]) + moveChange > 0)
                    {
                        currentPos = "" + abMoves[abMoves.IndexOf(currentPos[0]) + moveChange].ToString() + currentPos[1];
                    }
                    else
                    {
                        break;
                    }
                    if (chessPieces.ContainsKey(currentPos))
                    {
                        if (IsWhite != chessPieces[currentPos].IsWhite && chessPieces[currentPos].PieceType != 'K')
                        {
                            possibleMoves.Add(CurrentNotation + "~" + PieceType + "x" + currentPos);
                        }
                        break;
                    }
                    possibleMoves.Add(CurrentNotation + "~" + PieceType + currentPos);
                }
                moveChange = -1;
            }
            moveChange = 1;
            currentPos = CurrentPos;
            for (int i = 0; i < 2; i++)
            {
                while (true)
                {
                    if (int.Parse(currentPos[1].ToString()) + moveChange < 9 && int.Parse(currentPos[1].ToString()) + moveChange > 0)
                    {
                        currentPos = "" + currentPos[0] + (int.Parse(currentPos[1].ToString()) + moveChange).ToString();
                    }
                    else
                    {
                        break;
                    }
                    if (chessPieces.ContainsKey(currentPos))
                    {
                        if (IsWhite != chessPieces[currentPos].IsWhite && chessPieces[currentPos].PieceType != 'K')
                        {
                            possibleMoves.Add(CurrentNotation + "~" + PieceType + "x" + currentPos);
                        }
                        break;
                    }
                    possibleMoves.Add(CurrentNotation + "~" + PieceType + currentPos);
                }
                moveChange = -1;
            }
        }

        private void BishopMoveGeneration(List<string> possibleMoves, Dictionary<string, ChessPiece> chessPieces)
        {
            string currentPos = CurrentPos;
            int abMoveChange = 1;
            int numMoveChange = 1;
            for (int i = 0; i < 4; i++)
            {
                switch (i)
                {
                    case 1:
                        abMoveChange = -1;
                        break;
                    case 2:
                        numMoveChange = -1;
                        break;
                    case 3:
                        abMoveChange = 1;
                        break;
                }
                currentPos = CurrentPos;
                while (true)
                {
                    if (abMoves.IndexOf(currentPos[0]) + abMoveChange < abMoves.Count && abMoves.IndexOf(currentPos[0]) + abMoveChange > -1 && int.Parse(currentPos[1].ToString()) + numMoveChange < 9 && int.Parse(currentPos[1].ToString()) + numMoveChange > 0)
                    {
                        currentPos = "" + abMoves[abMoves.IndexOf(currentPos[0]) + abMoveChange] + (int.Parse(currentPos[1].ToString()) + numMoveChange).ToString();
                    }
                    else
                    {
                        break;
                    }
                    if (chessPieces.ContainsKey(currentPos))
                    {
                        if (IsWhite != chessPieces[currentPos].IsWhite && chessPieces[currentPos].PieceType != 'K')
                        {
                            possibleMoves.Add(CurrentNotation + "~" + PieceType + "x" + currentPos);
                        }
                        break;
                    }
                    possibleMoves.Add(CurrentNotation + "~" + PieceType + currentPos);
                }
            }
        }
    }
}