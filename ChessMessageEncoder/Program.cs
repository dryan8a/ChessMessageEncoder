using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Numerics;
using System.Windows;

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
            Console.WriteLine();
            BigInteger correspondingNum = BigInteger.Pow(256, bytes.Length) + new BigInteger(bytes);
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
                var moves = board.GetAllPossibleMoves().Moves;
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
                    encodedGame = encodedGame.Remove(0, encodedGame.IndexOf(' ')).Trim();
                }
                var moves = board.GetAllPossibleMoves().Moves;
                string correspondingStringToMoves = "";
                for (int i = 0; i < moves.Count; i++)
                {
                    correspondingStringToMoves += (moves[i] + "|");
                }
                amountsOfPossibleMoves.Add(moves.Count);
                int amountOfCharsToRemove = 0;
                if (encodedGame.Contains(' '))
                {
                    amountOfCharsToRemove = encodedGame.IndexOf(' ') + 1;  
                }
                else
                {
                    amountOfCharsToRemove = encodedGame.Length + 1;
                }
                              
                correspondingStringToMoves = correspondingStringToMoves.Remove(correspondingStringToMoves.IndexOf("~" + encodedGame.Substring(0, amountOfCharsToRemove - 1)) + amountOfCharsToRemove);
                correspondingStringToMoves = correspondingStringToMoves.Substring(correspondingStringToMoves.LastIndexOf('|') + 1);
                indeces.Add(moves.IndexOf(correspondingStringToMoves));

                if (encodedGame.Length < amountOfCharsToRemove)
                {
                    break;
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

            Span<byte> span = correspondingNum.ToByteArray();
            span = span.Slice(0, span.Length - 1);
            return Encoding.UTF8.GetString(span);            
        }

        public static BigInteger UnlimitiedPOWER(BigInteger baseNum, int powerUNLIMITEDPOWER)
        {
            BigInteger output = 1;
            for (int i = 0; i < powerUNLIMITEDPOWER; i++)
            {
                output *= baseNum;
            }
            return output;
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
                else if (action == "2" || action == "Decode" || action == "decode" || action == "d" || action == "D")
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
        public Dictionary<string, ChessPiece> chessPieces { get; private set; }
        public bool isWhitesTurn;
        static readonly List<char> abMoves = "abcdefgh".ToCharArray().ToList();

        //static char[] pieceLetters = "RNBQKBNR".ToCharArray();

        public ChessBoard()
        {
            isWhitesTurn = true;
            chessPieces = new Dictionary<string, ChessPiece>();
            string pawnNumPos = "2";
            string otherNumPos = "1";
            bool isWhite = true;
            for (int side = 0; side < 2; side++)
            {
                for (int pawns = 0; pawns < abMoves.Count; pawns++)
                {
                    chessPieces.Add(abMoves[pawns] + pawnNumPos, new ChessPiece(' ', abMoves[pawns] + pawnNumPos, isWhite, this));
                }
                //char abPosition = 'a';
                //for(int i = 0; i < 8; i++)
                //{
                //    chessPieces.Add($"{(abPosition + i)}{otherNumPos.ToString()}", new ChessPiece(pieceLetters[i], $"{(abPosition + i)}{otherNumPos.ToString()}", isWhite, this));
                //}                
                chessPieces.Add("a" + otherNumPos, new ChessPiece('R', "a" + otherNumPos, isWhite,this));
                chessPieces.Add("b" + otherNumPos, new ChessPiece('N', "b" + otherNumPos, isWhite,this));
                chessPieces.Add("c" + otherNumPos, new ChessPiece('B', "c" + otherNumPos, isWhite,this));
                chessPieces.Add("d" + otherNumPos, new ChessPiece('Q', "d" + otherNumPos, isWhite,this));
                chessPieces.Add("e" + otherNumPos, new ChessPiece('K', "e" + otherNumPos, isWhite,this));
                chessPieces.Add("f" + otherNumPos, new ChessPiece('B', "f" + otherNumPos, isWhite,this));
                chessPieces.Add("g" + otherNumPos, new ChessPiece('N', "g" + otherNumPos, isWhite,this));
                chessPieces.Add("h" + otherNumPos, new ChessPiece('R', "h" + otherNumPos, isWhite,this));
                
                pawnNumPos = "7";
                otherNumPos = "8";
                isWhite = false;
            }
        }
        public ChessBoard(ChessBoard OriginalBoard)
        {
            isWhitesTurn = OriginalBoard.isWhitesTurn;
            chessPieces = new Dictionary<string, ChessPiece>();
            foreach(ChessPiece chessPiece in OriginalBoard.chessPieces.Values)
            {
                chessPieces.Add(chessPiece.CurrentPos,new ChessPiece(chessPiece));
            }
        }

        public (List<string> Moves, bool CanKillTheKing) GetAllPossibleMoves(bool isRegicideCheck = false)
        {
            var possibleMoves = new List<string>();
            var previousPieceNotation = new List<string>();
            bool regicide = false;
            foreach (ChessPiece piece in chessPieces.Values)
            {
                if (piece.IsWhite == isWhitesTurn)
                {
                    var returnTuple = piece.GenerateMoves(chessPieces, isRegicideCheck);
                    possibleMoves.AddRange(returnTuple.Moves);
                    if (returnTuple.CanKillTheKing)
                    {
                        regicide = true;
                    }
                }
            }
            ChessBoard boardForForcedCheckCheck = new ChessBoard(this);
            string murdererPosition = "";
            var RecievedPossibleMoves = boardForForcedCheckCheck.GetAllPossibleMoves(!isWhitesTurn, ref murdererPosition, true);
            if (RecievedPossibleMoves.CanKillTheKing)
            {
                for(int i = 0;i<possibleMoves.Count;i++)
                {
                    string move = possibleMoves[i].Substring(possibleMoves[i].IndexOf('~') + 1);
                    if (!((move.Contains('x') && move.Substring(2) == murdererPosition) || move[0] == 'K'))
                    {
                        possibleMoves.RemoveAt(i);
                        i--;
                    }                    
                }
            }
            return (possibleMoves, regicide);
        }
        public (List<string> Moves, bool CanKillTheKing) GetAllPossibleMoves(bool isWhitesTurn, ref string positionOfPieceWhichPutsTheKingInCheck, bool isRegicideCheck = false)
        {
            var possibleMoves = new List<string>();
            var previousPieceNotation = new List<string>();
            bool regicide = false;           
            foreach (ChessPiece piece in chessPieces.Values)
            {
                if (piece.IsWhite == isWhitesTurn)
                {
                    var returnTuple = piece.GenerateMoves(chessPieces, isRegicideCheck);
                    possibleMoves.AddRange(returnTuple.Moves);
                    if (returnTuple.CanKillTheKing)
                    {
                        positionOfPieceWhichPutsTheKingInCheck = piece.CurrentPos;
                        regicide = true;
                    }
                }
            }
            return (possibleMoves, regicide);
        }

        public void ExecuteMoves(string PreviousNotation, string ExecutedMove)
        {
            if(ExecutedMove.Contains('x'))
            {
                ExecutedMove = ExecutedMove.Remove(ExecutedMove.IndexOf('x'),1);
            }
            string ExecutedPos = ExecutedMove.Substring(ExecutedMove.Length - 2);
            string PreviousPos = PreviousNotation.Substring(PreviousNotation.Length - 2);
            if (chessPieces.ContainsKey(ExecutedPos))
            {
                chessPieces.Remove(ExecutedPos);
            }
            chessPieces.Add(ExecutedPos, new ChessPiece(chessPieces[PreviousPos].PieceType, ExecutedPos, chessPieces[PreviousPos].IsWhite,this));
            chessPieces.Remove(PreviousPos);
            isWhitesTurn = !isWhitesTurn;
        }
    }
    class ChessPiece
    {
        public char PieceType;
        public string CurrentPos;
        ChessBoard ParentBoard;
        public string CurrentNotation => (PieceType.ToString() + CurrentPos).TrimStart(' ');
        public bool IsWhite;
        readonly List<char> abMoves = "abcdefgh".ToCharArray().ToList();

        public ChessPiece(char pieceType, string currentPos, bool isWhite, ChessBoard parentBoard)
        {
            PieceType = pieceType;
            CurrentPos = currentPos;
            IsWhite = isWhite;
            ParentBoard = parentBoard;
        }
        public ChessPiece(ChessPiece originalPiece)
        {
            PieceType = originalPiece.PieceType;
            CurrentPos = originalPiece.CurrentPos;
            IsWhite = originalPiece.IsWhite;           
        }

        public (List<string> Moves,bool CanKillTheKing) GenerateMoves(Dictionary<string, ChessPiece> chessPieces, bool isRegicideCheck)
        {
            List<string> possibleMoves = new List<string>();
            string addition = "";
            int abMoveIndex;
            bool doesPutTheKingIntoCheck = false;
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
                    if (!chessPieces.ContainsKey(addition) && !possibleMoves.Contains(CurrentNotation + "~" + addition))
                    {                           
                        possibleMoves.Add(CurrentNotation + "~" + addition);
                    }
                    if (CurrentPos[1] == '7' || CurrentPos[1] == '2')
                    {
                        string inMyWay = "";
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
                        if (!chessPieces.ContainsKey(addition) && !chessPieces.ContainsKey(inMyWay) && !possibleMoves.Contains(CurrentNotation + "~" + addition))
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
                            if (chessPieces.ContainsKey(addition) && chessPieces[addition].IsWhite != chessPieces[addition].IsWhite && !possibleMoves.Contains(CurrentNotation + "~" + addition))
                            {
                                if(chessPieces[addition].PieceType == 'K')
                                {
                                    doesPutTheKingIntoCheck = true;
                                }
                                else
                                {
                                    possibleMoves.Add(CurrentNotation + "~" + addition);
                                }
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
                            if (!chessPieces.ContainsKey(addition.Substring(1)) && !possibleMoves.Contains(CurrentNotation + "~" + addition))
                            {
                                possibleMoves.Add(CurrentNotation + "~" + addition);
                            }
                            if (chessPieces.ContainsKey(addition.Substring(1)) && chessPieces[addition.Substring(1)].IsWhite != IsWhite && !possibleMoves.Contains(CurrentNotation + "~" + addition.Insert(1,"x")))
                            {
                                if(chessPieces[addition.Substring(1)].PieceType == 'K')
                                {
                                    doesPutTheKingIntoCheck = true;
                                }
                                else
                                {
                                    possibleMoves.Add(CurrentNotation + "~" + addition.Insert(1, "x"));
                                }
                            }
                        }
                    }
                    break;
                case 'B':
                    BishopMoveGeneration(possibleMoves, chessPieces, ref doesPutTheKingIntoCheck);
                    break;
                case 'R':
                    RookMoveGeneration(possibleMoves, chessPieces, ref doesPutTheKingIntoCheck);
                    break;
                case 'Q':
                    BishopMoveGeneration(possibleMoves, chessPieces, ref doesPutTheKingIntoCheck);
                    RookMoveGeneration(possibleMoves, chessPieces, ref doesPutTheKingIntoCheck);
                    break;
                case 'K':
                    int abMoveChange = 1;
                    int numMoveChange = 1;
                    for (int i = 0; i < 8; i++)
                    {
                        switch (i)
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
                        if (abMoves.IndexOf(CurrentPos[0]) + abMoveChange < abMoves.Count && abMoves.IndexOf(CurrentPos[0]) + abMoveChange > 0 && int.Parse(CurrentPos[1].ToString()) + numMoveChange < 9 && int.Parse(CurrentPos[1].ToString()) + numMoveChange > 0)
                        {
                            addition = "" + abMoves[abMoves.IndexOf(CurrentPos[0]) + abMoveChange] + (int.Parse(CurrentPos[1].ToString()) + numMoveChange).ToString();
                            if (!isRegicideCheck && (!chessPieces.ContainsKey(addition) || (chessPieces.ContainsKey(addition) && IsWhite != chessPieces[addition].IsWhite)))
                            {
                                ChessBoard boardForCheckCheck = new ChessBoard(ParentBoard);
                                boardForCheckCheck.ExecuteMoves(CurrentNotation, "" + PieceType + addition);
                                if (boardForCheckCheck.GetAllPossibleMoves(true).CanKillTheKing)
                                {
                                    continue;
                                }
                            }
                            if (!chessPieces.ContainsKey(addition) && !possibleMoves.Contains(CurrentNotation + "~" + PieceType + addition))
                            {
                                possibleMoves.Add(CurrentNotation + "~" + PieceType + addition);
                            }
                            if (chessPieces.ContainsKey(addition) && IsWhite != chessPieces[addition].IsWhite && !possibleMoves.Contains(CurrentNotation + "~" + PieceType + addition.Insert(1,"x")))
                            {
                                if (chessPieces[addition].PieceType == 'K')
                                {
                                    doesPutTheKingIntoCheck = true;
                                }
                                else
                                {
                                    possibleMoves.Add(CurrentNotation + "~" + PieceType + addition.Insert(0, "x"));
                                }
                            }
                        }
                    }
                    break;
            }
            for(int i = 0;i<possibleMoves.Count;i++)
            {
                if(possibleMoves[i].Substring(possibleMoves[i].IndexOf('~') + 1).Length < 2)
                {
                    possibleMoves.RemoveAt(i);
                    i--;
                }
            }
            return (possibleMoves,doesPutTheKingIntoCheck);
        }

        private void RookMoveGeneration(List<string> possibleMoves, Dictionary<string, ChessPiece> chessPieces, ref bool doesPutTheKingInCheck)
        {
            string currentPos = CurrentPos;
            int abMoveChange = 1;
            int numMoveChange = 0;
            for (int i = 0; i < 4; i++)
            {
                switch (i)
                {
                    case 1:
                        abMoveChange = -1;
                        break;
                    case 2:
                        abMoveChange = 0;
                        numMoveChange = 1;
                        break;
                    case 3:
                        numMoveChange = -1;
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
                    if (chessPieces.ContainsKey(currentPos) && !possibleMoves.Contains(CurrentNotation + "~" + PieceType + "x" + currentPos))
                    {
                        if (IsWhite != chessPieces[currentPos].IsWhite)
                        {
                            if (chessPieces[currentPos].PieceType == 'K')
                            {
                                doesPutTheKingInCheck = true;
                            }
                            else
                            {
                                possibleMoves.Add(CurrentNotation + "~" + PieceType + "x" + currentPos);
                            }
                        }
                        break;
                    }
                    else if (!possibleMoves.Contains(CurrentNotation + "~" + PieceType + currentPos))
                    {
                        possibleMoves.Add(CurrentNotation + "~" + PieceType + currentPos);
                    }
                }
            }
            //int moveChange = 1;
            //string currentPos = CurrentPos;
            //for (int i = 0; i < 2; i++)
            //{
            //    while (true)
            //    {
            //        if (abMoves.IndexOf(currentPos[0]) + moveChange < abMoves.Count && abMoves.IndexOf(currentPos[0]) + moveChange > 0)
            //        {
            //            currentPos = "" + abMoves[abMoves.IndexOf(currentPos[0]) + moveChange].ToString() + currentPos[1];
            //        }
            //        else
            //        {
            //            break;
            //        }
            //        if (chessPieces.ContainsKey(currentPos) && !possibleMoves.Contains(CurrentNotation + "~" + PieceType + "x" + currentPos))
            //        {
            //            if (IsWhite != chessPieces[currentPos].IsWhite)
            //            {
            //                if (chessPieces[currentPos].PieceType == 'K')
            //                {
            //                    doesPutTheKingInCheck = true;
            //                }
            //                else
            //                {
            //                    possibleMoves.Add(CurrentNotation + "~" + PieceType + "x" + currentPos);
            //                }
            //            }
            //            break;
            //        }
            //        else if (!possibleMoves.Contains(CurrentNotation + "~" + PieceType + currentPos))
            //        {
            //            possibleMoves.Add(CurrentNotation + "~" + PieceType + currentPos);
            //        }                    
            //    }
            //    moveChange = -1;
            //}
            //moveChange = 1;
            //currentPos = CurrentPos;
            //for (int i = 0; i < 2; i++)
            //{
            //    while (true)
            //    {
            //        if (int.Parse(currentPos[1].ToString()) + moveChange < 9 && int.Parse(currentPos[1].ToString()) + moveChange > 0)
            //        {
            //            currentPos = "" + currentPos[0] + (int.Parse(currentPos[1].ToString()) + moveChange).ToString();
            //        }
            //        else
            //        {
            //            break;
            //        }
            //        if (chessPieces.ContainsKey(currentPos) && !possibleMoves.Contains(CurrentNotation + "~" + PieceType + "x" + currentPos))
            //        {
            //            if (IsWhite != chessPieces[currentPos].IsWhite)
            //            {
            //                if (chessPieces[currentPos].PieceType == 'K')
            //                {
            //                    doesPutTheKingInCheck = true;
            //                }
            //                else
            //                {
            //                    possibleMoves.Add(CurrentNotation + "~" + PieceType + "x" + currentPos);
            //                }
            //            }
            //            break;
            //        }
            //        else if (!possibleMoves.Contains(CurrentNotation + "~" + PieceType + currentPos))
            //        {
            //            possibleMoves.Add(CurrentNotation + "~" + PieceType + currentPos);
            //        }
            //    }
            //    moveChange = -1;
            //}
        }

        private void BishopMoveGeneration(List<string> possibleMoves, Dictionary<string, ChessPiece> chessPieces, ref bool doesPutTheKingInCheck)
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
                    if (chessPieces.ContainsKey(currentPos) && !possibleMoves.Contains(CurrentNotation + "~" + PieceType + "x" + currentPos))
                    {
                        if (IsWhite != chessPieces[currentPos].IsWhite)
                        {
                            if(chessPieces[currentPos].PieceType == 'K')
                            {
                                doesPutTheKingInCheck = true;
                            }
                            else
                            {
                                possibleMoves.Add(CurrentNotation + "~" + PieceType + "x" + currentPos);
                            }
                        }
                        break;
                    }
                    else if (!possibleMoves.Contains(CurrentNotation + "~" + PieceType + currentPos))
                    {
                        possibleMoves.Add(CurrentNotation + "~" + PieceType + currentPos);
                    }
                }
            }
        }
    }
}