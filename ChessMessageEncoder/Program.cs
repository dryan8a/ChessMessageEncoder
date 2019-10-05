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
                string tempExecutedMove = moves[(int)(correspondingNum % moves.Count)];
                if (tempExecutedMove[tempExecutedMove.IndexOf('~') + 1] < 91)
                {
                    tempExecutedMove = tempExecutedMove.Remove(tempExecutedMove.IndexOf('~') + 1, 1);
                }
                if (tempExecutedMove.Contains("O-O"))
                {
                    Output += tempExecutedMove.Substring(tempExecutedMove.IndexOf('~') + 1) + " ";
                }
                else
                {
                    Output += tempExecutedMove.Remove(tempExecutedMove.IndexOf('~'), 1) + " ";
                }
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
                int amountOfCharsToRemove;
                if (!encodedGame.Substring(0, 3).Contains("O-O"))
                {
                    if (encodedGame[0] < 91)
                    {
                        encodedGame = encodedGame.Remove(1, 2);
                    }
                    else
                    {
                        encodedGame = encodedGame.Remove(0, 2);
                    }
                }

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
}