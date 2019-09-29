using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChessMessageEncoder
{
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
                chessPieces.Add("a" + otherNumPos, new ChessPiece('R', "a" + otherNumPos, isWhite, this));
                chessPieces.Add("b" + otherNumPos, new ChessPiece('N', "b" + otherNumPos, isWhite, this));
                chessPieces.Add("c" + otherNumPos, new ChessPiece('B', "c" + otherNumPos, isWhite, this));
                chessPieces.Add("d" + otherNumPos, new ChessPiece('Q', "d" + otherNumPos, isWhite, this));
                chessPieces.Add("e" + otherNumPos, new ChessPiece('K', "e" + otherNumPos, isWhite, this));
                chessPieces.Add("f" + otherNumPos, new ChessPiece('B', "f" + otherNumPos, isWhite, this));
                chessPieces.Add("g" + otherNumPos, new ChessPiece('N', "g" + otherNumPos, isWhite, this));
                chessPieces.Add("h" + otherNumPos, new ChessPiece('R', "h" + otherNumPos, isWhite, this));

                pawnNumPos = "7";
                otherNumPos = "8";
                isWhite = false;
            }
        }
        public ChessBoard(ChessBoard OriginalBoard)
        {
            isWhitesTurn = OriginalBoard.isWhitesTurn;
            chessPieces = new Dictionary<string, ChessPiece>();
            foreach (ChessPiece chessPiece in OriginalBoard.chessPieces.Values)
            {
                chessPieces.Add(chessPiece.CurrentPos, new ChessPiece(chessPiece));
            }
        }

        public (List<string> Moves, bool CanKillTheKing) GetAllPossibleMoves(bool isRegicideCheck = false)
        {
            var possibleMoves = new List<string>();
            bool regicide = false;
            ChessBoard tempBoardOfExecutedMove;
            foreach (ChessPiece piece in chessPieces.Values)
            {
                if (piece.IsWhite == isWhitesTurn)
                {
                    var returnTuple = piece.GenerateMoves(chessPieces, isRegicideCheck);
                    for (int i = 0; i < returnTuple.Moves.Count; i++)
                    {
                        tempBoardOfExecutedMove = new ChessBoard(this);
                        tempBoardOfExecutedMove.ExecuteMoves(returnTuple.Moves[i].Substring(0, returnTuple.Moves[i].IndexOf('~')), returnTuple.Moves[i].Substring(returnTuple.Moves[i].IndexOf('~') + 1));
                        if (tempBoardOfExecutedMove.GetAllPossibleMoves(true, true).CanKillTheKing || tempBoardOfExecutedMove.GetAllPossibleMoves(false, true).CanKillTheKing)
                        {
                            i--;
                            returnTuple.Moves.RemoveAt(i + 1);
                        }
                    }
                    possibleMoves.AddRange(returnTuple.Moves);
                    if (returnTuple.CanKillTheKing)
                    {
                        regicide = true;
                    }
                }
            }
            return (possibleMoves, regicide);
        }
        public (List<string> Moves, bool CanKillTheKing) GetAllPossibleMoves(bool isWhitesTurn, bool isRegicideCheck = false)
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
            return (possibleMoves, regicide);
        }

        public void ExecuteMoves(string PreviousNotation, string ExecutedMove)
        {
            if (ExecutedMove.Contains('x'))
            {
                ExecutedMove = ExecutedMove.Remove(ExecutedMove.IndexOf('x'), 1);
            }
            string ExecutedPos = ExecutedMove.Substring(ExecutedMove.Length - 2);
            string PreviousPos = PreviousNotation.Substring(PreviousNotation.Length - 2);
            if (chessPieces.ContainsKey(ExecutedPos))
            {
                chessPieces.Remove(ExecutedPos);
            }
            chessPieces.Add(ExecutedPos, new ChessPiece(chessPieces[PreviousPos].PieceType, ExecutedPos, chessPieces[PreviousPos].IsWhite, this));
            if (chessPieces[ExecutedPos].PieceType == ' ' && ((ExecutedPos[1] == '1' && !chessPieces[ExecutedPos].IsWhite) || (ExecutedPos[1] == '8' && chessPieces[ExecutedPos].IsWhite)))
            {
                chessPieces[ExecutedPos].PieceType = 'Q';
            }
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
        public bool HasMoved;
        readonly List<char> abMoves = "abcdefgh".ToCharArray().ToList();

        public ChessPiece(char pieceType, string currentPos, bool isWhite, ChessBoard parentBoard, bool hasMoved = false)
        {
            PieceType = pieceType;
            CurrentPos = currentPos;
            IsWhite = isWhite;
            ParentBoard = parentBoard;
            HasMoved = hasMoved;
        }
        public ChessPiece(ChessPiece originalPiece)
        {
            PieceType = originalPiece.PieceType;
            CurrentPos = originalPiece.CurrentPos;
            IsWhite = originalPiece.IsWhite;
            HasMoved = originalPiece.HasMoved;
        }

        public (List<string> Moves, bool CanKillTheKing) GenerateMoves(Dictionary<string, ChessPiece> chessPieces, bool isRegicideCheck)
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
                    if ((CurrentPos[1] == '7' && !IsWhite) || (CurrentPos[1] == '2' && IsWhite))
                    {
                        string inMyWay;
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
                                addition = "x" + abMoves[abMoveIndex] + (int.Parse(CurrentPos[1].ToString()) + 1).ToString();
                            }
                            else
                            {
                                addition = "x" + abMoves[abMoveIndex] + (int.Parse(CurrentPos[1].ToString()) - 1).ToString();
                            }
                            if (chessPieces.ContainsKey(addition.Substring(1)) && chessPieces[addition.Substring(1)].IsWhite != IsWhite)
                            {
                                if (chessPieces[addition.Substring(1)].PieceType == 'K')
                                {
                                    doesPutTheKingIntoCheck = true;
                                }
                                else if (!possibleMoves.Contains(CurrentNotation + "~" + addition))
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
                            if (chessPieces.ContainsKey(addition.Substring(1)) && chessPieces[addition.Substring(1)].IsWhite != IsWhite && !possibleMoves.Contains(CurrentNotation + "~" + addition.Insert(1, "x")))
                            {
                                if (chessPieces[addition.Substring(1)].PieceType == 'K')
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
                            if (chessPieces.ContainsKey(addition) && IsWhite != chessPieces[addition].IsWhite && !possibleMoves.Contains(CurrentNotation + "~" + PieceType + addition.Insert(1, "x")))
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
                    if(!HasMoved)
                    {
                        if(IsWhite)
                        {
                            numMoveChange = 1;
                        }
                        else
                        {
                            numMoveChange = 8;
                        }
                        //do king-side and queen-side castle check here
                    }
                    break;
            }
            for (int i = 0; i < possibleMoves.Count; i++)
            {
                if (possibleMoves[i].Substring(possibleMoves[i].IndexOf('~') + 1).Length < 2)
                {
                    possibleMoves.RemoveAt(i);
                    i--;
                }
            }
            return (possibleMoves, doesPutTheKingIntoCheck);
        }

        private void RookMoveGeneration(List<string> possibleMoves, Dictionary<string, ChessPiece> chessPieces, ref bool doesPutTheKingInCheck)
        {
            string currentPos;
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
                    else if (!chessPieces.ContainsKey(currentPos) && !possibleMoves.Contains(CurrentNotation + "~" + PieceType + currentPos))
                    {
                        possibleMoves.Add(CurrentNotation + "~" + PieceType + currentPos);
                    }
                }
            }
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
                    else if (!chessPieces.ContainsKey(currentPos) && !possibleMoves.Contains(CurrentNotation + "~" + PieceType + currentPos))
                    {
                        possibleMoves.Add(CurrentNotation + "~" + PieceType + currentPos);
                    }
                }
            }
        }
    }
}
