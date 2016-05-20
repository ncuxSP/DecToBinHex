using System;
using System.ComponentModel;

namespace DecToBinHexTool
{
    class Presenter
    {
        private readonly IView _view;
        private readonly AsyncSolver _solver;
        private readonly BindingList<string> _result;

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
            return string.Format("{0}, will be computed in {1} seconds...", number, seconds);
        }

        public static string ResultMessage(int number, string result)
        {
            return string.Format("{0} = {1}", number, result);
        }

        public static string WaitingMessage(int number)
        {
            return string.Format("{0}, getting result...", number);
        }

        private void _view_ComputeNewNumber(int number)
        {
            _result.Add(string.Empty);
            _solver.AddTask(_result.Count - 1, number);
        }

        private void _solver_UpdateResult(int idx, string result)
        {
            _result[idx] = result;
        }
    }
}
