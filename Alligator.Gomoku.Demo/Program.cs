﻿using Alligator.Solver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Alligator.Gomoku.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Hello gomoku demo!");

            var solverConfiguration = new SolverConfiguration();
            IRules<IPosition, Ply> rules = new Rules();
            SolverFactory<IPosition, Ply> solverFactory = new SolverFactory<IPosition, Ply>(rules, solverConfiguration, SolverLog);
            ISolver<Ply> solver = solverFactory.Create();

            IPosition position = new Position();
            IList<Ply> history = new List<Ply>();
            bool aiStep = true;

            while (rules.LegalMovesAt(position).Any())
            {
                PrintPosition(position);
                Ply next;
                Position copy = new Position(position.History);

                if (aiStep)
                {
                    while (true)
                    {
                        try
                        {
                            next = AiStep(history, solver);
                            copy.Take(next);
                            break;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }
                }
                else
                {
                    while (true)
                    {
                        try
                        {
                            next = HumanStep();
                            copy.Take(next);
                            break;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }
                }
                position.Take(next);
                history.Add(next);
                aiStep = !aiStep;
            }
            if (!rules.IsGoal(position))
            {
                Console.WriteLine("Game over, DRAW!");
            }
            else
            {
                Console.WriteLine(string.Format("Game over, {0} WON!", aiStep ? "human" : "ai"));
            }

            PrintPosition(position);

            Console.ReadKey();
        }

        private static Ply HumanStep()
        {
            Console.Write("Next step [row:column]: ");
            while (true)
            {
                try
                {
                    string[] msg = Console.ReadLine().Split(':');
                    int row = int.Parse(msg[0]);
                    int column = int.Parse(msg[1]);

                    return new Ply(row, column);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        private static Ply AiStep(IList<Ply> history, ISolver<Ply> solver)
        {
            Ply result = solver.OptimizeNextMove(history);

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("AI thinking...");
            //Console.WriteLine(string.Format("Evaluation value: {0} ({1})", evaluationValue, ToString(evaluationValue)));
            Console.WriteLine(string.Format("Optimal next step: {0}", result));
            //Console.WriteLine(string.Format("Forecast: {0}", string.Join(" ## ", forecast)));
            Console.ForegroundColor = ConsoleColor.White;

            return result;
        }

        private static string ToString(int evaluationValue)
        {
            return evaluationValue.ToString();

            if (evaluationValue == 0)
            {
                return "Hm, draw..";
            }
            return evaluationValue > 0 ? "Ho-Ho-Ho!!!" : "Oh, no!";
        }

        private static void PrintPosition(IPosition position)
        {
            Console.WriteLine();
            Console.Write("  ");
            for (int i = 0; i < Position.BoardSize; i++)
            {
                Console.Write(string.Format(" {0}", i % 10));
            }
            Console.WriteLine();
            for (int i = 0; i < Position.BoardSize; i++)
            {
                Console.Write(string.Format(" {0}", i % 10));
                for (int j = 0; j < Position.BoardSize; j++)
                {
                    switch (position.GetStone(i, j))
                    {
                        case Stone.Empty:
                            Console.Write(string.Format(" {0}", "."));
                            break;
                        case Stone.Black:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(string.Format(" {0}", "X"));
                            break;
                        case Stone.White:
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.Write(string.Format(" {0}", "O"));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        private static void SolverLog(string message)
        {
            var prevColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(string.Format("[SolverLog] {0}", message));
            Console.ForegroundColor = prevColor;
        }
    }
}