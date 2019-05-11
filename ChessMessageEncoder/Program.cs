using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace ChessMessageEncoder
{
    class Program
    {
        static void Main(string[] args)
        {
            //string Input = Console.ReadLine();
            //byte[] bytes = Encoding.UTF8.GetBytes(Input);
            ////for (int i = 0; i < bytes.Length; i++)
            ////{
            ////    Console.Write(bytes[i] + " ");
            ////}
            //Array.Reverse(bytes);
            //int correspondingnum = (int)Math.Pow(256, bytes.Length);
            //for (int i = 1; i <= bytes.Length; i++)
            //{
            //    correspondingnum += bytes[i - 1] * (int)Math.Pow(256, bytes.Length - i);
            //}
            ChessPiece piece = new ChessPiece(' ', "B2", true);
            List<string> list = piece.GenerateMoves(new Dictionary<string, ChessPiece>());
            for (int i = 0; i < list.Count; i++)
            {
                Console.Write(list[i] + ",");
            }
            //Console.WriteLine("");
            //Console.Write(correspondingnum);
            //Console.WriteLine("");

        }

    }
    class ChessBoard
    {
        Dictionary<string, ChessPiece> chessPieces;
        bool isWhitesTurn;
        
        public ChessBoard()
        {
            isWhitesTurn = true;
            chessPieces = new Dictionary<string, ChessPiece>();
            int numPos = 2;
            bool isWhite = true;
            for (int side = 0; side < 2; side++)
            {
                //hey, look here!
                //finish this!
            }
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
                        addition = CurrentPos[0] + (int.Parse(CurrentPos[1].ToString()) + 1).ToString();
                    }
                    else if (!IsWhite && int.Parse(CurrentPos[1].ToString()) - 1 > 1)
                    {
                        addition = CurrentPos[0] + (int.Parse(CurrentPos[1].ToString()) - 1).ToString();
                    }
                    if (!chessPieces.ContainsKey(addition))
                    {
                        possibleMoves.Add(addition);
                    }
                    if (CurrentPos[1] == '7' || CurrentPos[1] == '2')
                    {
                        if (IsWhite)
                        {
                            addition = CurrentPos[0] + "4";
                        }
                        else
                        {
                            addition = CurrentPos[0] + "5";
                        }
                        if (!chessPieces.ContainsKey(addition))
                        {
                            possibleMoves.Add(addition);
                        }
                    }
                    abMoveIndex = abMoves.IndexOf(CurrentPos[0]) + 1;
                    for (int i = 0; i < 2; i++)
                    {
                        if (abMoves.IndexOf(CurrentPos[0]) + 1 < abMoves.Count && abMoves.IndexOf(CurrentPos[0]) - 1 > -1)
                        {
                            if (IsWhite)
                            {
                                addition = abMoves[abMoveIndex] + (int.Parse(CurrentPos[1].ToString()) + 1).ToString();
                            }
                            else
                            {
                                addition = abMoves[abMoveIndex] + (int.Parse(CurrentPos[1].ToString()) - 1).ToString();
                            }
                            if (chessPieces.ContainsKey(addition) && chessPieces[addition].IsWhite != chessPieces[addition].IsWhite && chessPieces[addition].PieceType != 'K')
                            {
                                possibleMoves.Add(addition);
                            }
                        }
                        abMoveIndex -= 2;
                    }
                    break;
                case 'N':
                    for (int i = 0; i < 8; i++)
                    {
                        switch (i)
                        {
                            case 0:
                                abMoveIndex = abMoves.IndexOf(CurrentPos[0]) + 1;
                                if (abMoves.IndexOf(CurrentPos[0]) + 1 < abMoves.Count)
                                {
                                    addition = PieceType + abMoves[abMoveIndex] + (int.Parse(CurrentPos[1].ToString()) + 2).ToString();
                                }
                                break;
                            case 1:
                                abMoveIndex = abMoves.IndexOf(CurrentPos[0]) - 1;
                                if (abMoves.IndexOf(CurrentPos[0]) - 1 > -1)
                                {
                                    addition = PieceType + abMoves[abMoveIndex] + (int.Parse(CurrentPos[1].ToString()) + 2).ToString();
                                }
                                break;
                            case 2:
                                abMoveIndex = abMoves.IndexOf(CurrentPos[0]) + 1;
                                if (abMoves.IndexOf(CurrentPos[0]) + 1 < abMoves.Count)
                                {
                                    addition = PieceType + abMoves[abMoveIndex] + (int.Parse(CurrentPos[1].ToString()) - 2).ToString();
                                }
                                break;
                            case 3:
                                abMoveIndex = abMoves.IndexOf(CurrentPos[0]) - 1;
                                if (abMoves.IndexOf(CurrentPos[0]) - 1 > -1)
                                {
                                    addition = PieceType + abMoves[abMoveIndex] + (int.Parse(CurrentPos[1].ToString()) - 2).ToString();
                                }
                                break;
                            case 4:
                                abMoveIndex = abMoves.IndexOf(CurrentPos[0]) + 2;
                                if (abMoves.IndexOf(CurrentPos[0]) + 2 < abMoves.Count)
                                {
                                    addition = PieceType + abMoves[abMoveIndex] + (int.Parse(CurrentPos[1].ToString()) + 1).ToString();
                                }
                                break;
                            case 5:
                                abMoveIndex = abMoves.IndexOf(CurrentPos[0]) - 2;
                                if (abMoves.IndexOf(CurrentPos[0]) - 2 > -1)
                                {
                                    addition = PieceType + abMoves[abMoveIndex] + (int.Parse(CurrentPos[1].ToString()) + 1).ToString();
                                }
                                break;
                            case 6:
                                abMoveIndex = abMoves.IndexOf(CurrentPos[0]) + 2;
                                if (abMoves.IndexOf(CurrentPos[0]) + 2 < abMoves.Count)
                                {
                                    addition = PieceType + abMoves[abMoveIndex] + (int.Parse(CurrentPos[1].ToString()) - 1).ToString();
                                }
                                break;
                            case 7:
                                abMoveIndex = abMoves.IndexOf(CurrentPos[0]) - 2;
                                if (abMoves.IndexOf(CurrentPos[0]) - 2 > -1)
                                {
                                    addition = PieceType + abMoves[abMoveIndex] + (int.Parse(CurrentPos[1].ToString()) - 1).ToString();
                                }
                                break;
                        }
                        if (int.Parse(addition.Substring(2)) < 9 && int.Parse(addition.Substring(2)) > -1)
                        {
                            if (!chessPieces.ContainsKey(addition.Substring(1)))
                            {
                                possibleMoves.Add(addition);
                            }
                            if (chessPieces.ContainsKey(addition.Substring(1)) && chessPieces[addition.Substring(1)].IsWhite != IsWhite && chessPieces[addition.Substring(1)].PieceType != 'K')
                            {
                                possibleMoves.Add(addition);
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
                            addition = abMoves[abMoves.IndexOf(CurrentPos[0]) + abMoveChange] + (int.Parse(CurrentPos[1].ToString()) + numMoveChange).ToString();
                            if(!chessPieces.ContainsKey(addition))
                            {
                                possibleMoves.Add(PieceType + addition);
                            }
                            if (chessPieces.ContainsKey(addition) && IsWhite != chessPieces[addition].IsWhite && chessPieces[addition].PieceType != 'K')
                            {
                                possibleMoves.Add(PieceType + addition);
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
                    if (abMoves.IndexOf(currentPos[0]) + moveChange < abMoves.Count)
                    {
                        currentPos = abMoves[abMoves.IndexOf(currentPos[0]) + moveChange].ToString() + currentPos[1];
                    }
                    else
                    {
                        break;
                    }
                    if (chessPieces.ContainsKey(currentPos))
                    {
                        if (IsWhite != chessPieces[currentPos].IsWhite && chessPieces[currentPos].PieceType != 'K')
                        {
                            possibleMoves.Add(PieceType + currentPos);
                        }
                        break;
                    }
                    possibleMoves.Add(PieceType + currentPos);
                }
                moveChange = -1;
            }
            for (int i = 0; i < 2; i++)
            {
                while (true)
                {
                    if (int.Parse(currentPos[1].ToString()) + moveChange < 9 && int.Parse(currentPos[1].ToString()) + moveChange > 0)
                    {
                        currentPos = currentPos[0] + (int.Parse(currentPos[1].ToString()) + moveChange).ToString();
                    }
                    else
                    {
                        break;
                    }
                    if (chessPieces.ContainsKey(currentPos))
                    {
                        if (IsWhite != chessPieces[currentPos].IsWhite && chessPieces[currentPos].PieceType != 'K')
                        {
                            possibleMoves.Add(PieceType + currentPos);
                        }
                        break;
                    }
                    possibleMoves.Add(PieceType + currentPos);
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
                while (true)
                {
                    if (abMoves.IndexOf(currentPos[0]) + abMoveChange < abMoves.Count && int.Parse(currentPos[1].ToString()) + numMoveChange < 9 && int.Parse(currentPos[1].ToString()) + numMoveChange > 0)
                    {
                        currentPos = abMoves[abMoves.IndexOf(currentPos[0]) + abMoveChange] + (int.Parse(currentPos[1].ToString()) + numMoveChange).ToString();
                    }
                    else
                    {
                        break;
                    }
                    if (chessPieces.ContainsKey(currentPos))
                    {
                        if (IsWhite != chessPieces[currentPos].IsWhite && chessPieces[currentPos].PieceType != 'K')
                        {
                            possibleMoves.Add(PieceType + currentPos);
                        }
                        break;
                    }
                    possibleMoves.Add(PieceType + currentPos);
                }
            }
        }
    }
}
