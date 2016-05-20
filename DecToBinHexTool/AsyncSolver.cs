using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace DecToBinHexTool
{
    class AsyncSolver : Solver
    {
        public delegate void UpdateResultDelegate(int idx, string result);
        public event UpdateResultDelegate UpdateResult;

        private enum TaskState
        {
            Waiting,
            Starting,
            Finished
        }

        private class TaskDescription
        {
            public int Idx;
            public int Number;
            public int SecondsRemaining;
            public double TimeRemaining;
            public string Result;
            public TaskState State;
        }

        private List<TaskDescription> _tasks;
        private BackgroundWorker _bw;

        private delegate string ComputeDelegate(int number);
        private class ComputeInfo
        {
            public TaskDescription Task;
            public ComputeDelegate Delegate;
            public IAsyncResult Status;
        }
        private readonly int _maxComputingThreads = Environment.ProcessorCount;
        private int _waitingTasksCount = 0;

        private List<ComputeInfo> _computingTasks;

        public AsyncSolver()
        {
            _tasks = new List<TaskDescription>();
            _computingTasks = new List<ComputeInfo>();

            _bw = new BackgroundWorker() {WorkerReportsProgress = true};
            _bw.DoWork += _bw_DoWork;
            _bw.ProgressChanged += _bw_ProgressChanged;
            _bw.RunWorkerCompleted += _bw_RunWorkerCompleted;
        }
        
        public void AddTask(int idx, int number)
        {
            var rnd = new Random();
            var seconds = rnd.Next(5, 30);

            UpdateResult?.Invoke(idx, Presenter.ProgressMessage(number, seconds));

            var task = new TaskDescription
            {
                Idx = idx,
                Number = number,
                SecondsRemaining = seconds,
                TimeRemaining = seconds,
                Result = string.Empty,
                State = TaskState.Waiting
            };

            var s = (ICollection)_tasks;
            lock (s.SyncRoot)
            {
                _tasks.Add(task);
                _tasks.Sort((t1, t2) => t1.SecondsRemaining.CompareTo(t2.SecondsRemaining));
                _waitingTasksCount++;
            }

            if (!_bw.IsBusy)
            {
                _bw.RunWorkerAsync();
            }
        }

        private void _bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            UpdateResult?.Invoke(e.ProgressPercentage, (string)e.UserState);
        }

        private void _bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                throw new Exception(e.Error.Message);
            }
        }
        
        #region BackgroundWorker Thread

        private void Computing()
        {
            for (var i = _computingTasks.Count - 1; i >= 0; i--)
            {
                var info = _computingTasks[i];
                if (info.Status.IsCompleted)
                {   
                    info.Task.State = TaskState.Finished;
                    info.Task.Result = info.Delegate.EndInvoke(info.Status);
                    _computingTasks.RemoveAt(i);
                }
            }
            while (_waitingTasksCount > 0 && _computingTasks.Count < _maxComputingThreads)
            {
                var task = _tasks.Find(t => t.State == TaskState.Waiting);
                if (task != null)
                {
                    var info = new ComputeInfo
                    {
                        Task = task,
                        Delegate = new ComputeDelegate(Compute)
                    };
                    info.Status = info.Delegate.BeginInvoke(task.Number, null, null);
                    _computingTasks.Add(info);

                    task.State = TaskState.Starting;
                    _waitingTasksCount--;
                }
            }
        }

        private bool UpdateTasks(double elapsedTime)
        {
            var s = (ICollection)_tasks;
            lock (s.SyncRoot)
            {
                Computing();

                for (var i = _tasks.Count - 1; i >= 0; i--)
                {
                    var task = _tasks[i];
                    task.TimeRemaining -= elapsedTime;

                    var secRemaining = (int)task.TimeRemaining + 1;
                    if (secRemaining < task.SecondsRemaining)
                    {
                        task.SecondsRemaining = secRemaining;
                        var message = Presenter.ProgressMessage(task.Number, secRemaining);
                        if (secRemaining <= 0)
                        {
                            message = Presenter.WaitingMessage(task.Number);
                        }
                        _bw.ReportProgress(task.Idx, message);
                    }

                    if (task.TimeRemaining <= 0 && task.State == TaskState.Finished)
                    {
                        _bw.ReportProgress(task.Idx, Presenter.ResultMessage(task.Number, task.Result));
                        _tasks.RemoveAt(i);
                    }
                }

                return _tasks.Count == 0;
            }
        }
        
        private void _bw_DoWork(object sender, DoWorkEventArgs e)
        {
            var currentTime = DateTime.Now;
            bool workComplete = false;

            while (!workComplete)
            {
                var elapsedTime = (DateTime.Now - currentTime).TotalSeconds;
                currentTime = DateTime.Now;

                workComplete = UpdateTasks(elapsedTime);

                elapsedTime = (DateTime.Now - currentTime).TotalSeconds;
                if (elapsedTime < 0.1)
                {
                    Thread.Sleep(100);
                }
            }
        }

        #endregion
    }
}
