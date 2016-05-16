using System;
using System.ComponentModel;

namespace DecToBinHexTool
{
    class Presenter
    {
        private readonly IView _view;
        private AsyncSolver _solver;
        private BindingList<string> _result;

        public Presenter(IView view)
        {
            _result = new BindingList<string>();

            _view = view;
            _view.BindDataSource(_result);
            _view.ComputeNewNumber += _view_ComputeNewNumber;

            _solver = new AsyncSolver();
            _solver.UpdateResult += _solver_UpdateResult;
        }

        public static string ProgressMessage(int number, int seconds)
        {
            return number + ", will be computed in " + seconds + " seconds...";
        }

        public static string ResultMessage(int number, string result)
        {
            return number + " = " + result;
        }

        private void _view_ComputeNewNumber(int number)
        {
            Random rnd = new Random();
            var seconds = rnd.Next(5, 30);

            _solver.AddTask(_result.Count, number, seconds);
            _result.Add(ProgressMessage(number, seconds));
        }

        private void _solver_UpdateResult(int idx, string result)
        {
            _result[idx] = result;
        }
    }
}
